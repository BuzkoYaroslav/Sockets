using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Numerics;

namespace MatrixGenerator
{
    public partial class SocketsServer : Form
    {
        public class StateObject
        {
            // Client  socket.  
            public Socket workSocket = null;
            // Size of receive buffer.  
            public int BufferSize { get { return buffer.Length; } }
            // Receive buffer.  
            public byte[] buffer = new byte[1024];
            // containing result of multiplication
            public BigInteger result;
            // containing position for the result
            public int resultIndex;
        }

        private const int socketPort = 11000;
        private const string host = "localhost";
        private const int numberOfPossibleListeners = 100;
        private const string endMessage = "<Close socket>";

        private delegate void Method();

        // socket for listening
        private Socket listener;
        private int numberToAccept = 0;

        // result of the multiplication
        private List<BigInteger?> proccessedRows;

        // matrix-row file name template
        private string matrixTemplate;

        // vector file name
        private string vectorFileName;

        // stream to write a log
        private StreamWriter logFile;

        // current available info
        private int currentNotProcessed = 0;

        // objects for locking
        private object getMessageObj = new object();
        private object writeLogObj = new object();
        private object updateResultObj = new object();
        private object workerListObj = new object();
        private object saveResultsObj = new object();

        // indicates whether results are saved or not
        private bool resultsSaved = false;

        // time calculation
        private Stopwatch wtch = new Stopwatch();

        public SocketsServer()
        {
            InitializeComponent();
        }

        private void startExecutionButton_Click(object sender, EventArgs e)
        {
            matrixTemplate = matrixTemplateTextBox.Text;
            vectorFileName = VectoFileTextBox.Text;

            StreamReader reader = new StreamReader(vectorFileName);
            int size = Convert.ToInt32(reader.ReadLine());
            reader.Close();

            proccessedRows = new BigInteger?[size].ToList();

            string logFileName = logTextBox.Text == "" ? "log.txt" : logTextBox.Text;
            logFile = new StreamWriter(logFileName);
            logListBox.Items.Clear();
            workersList.Items.Clear();

            progressIdicator.Value = 0;
            progressIdicator.Maximum = size;

            wtch.Start();

            StartListening();
        }

        private void StartListening()
        {
            IPHostEntry ipHostInfo = Dns.GetHostEntry(host);
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, socketPort);

            listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                WriteLog(string.Format("Trying to bind socket to local end point. End point : {0}", localEndPoint));
                listener.Bind(localEndPoint);
                WriteLog("Binding was successful!");
                WriteLog("Trying to start listen");
                listener.Listen(numberOfPossibleListeners);
                WriteLog("Listening was started!");

                for (int i = 0; i < numberToAccept; i++)
                    listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);

            } catch (Exception e)
            {
                WriteLog(string.Format("Exception in StartListening! Exception message: {0}", e.ToString()));
            }
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            WriteLog("Waiting to process accept");

            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);
            WriteLog(string.Format("Accepted successfully. Socket for connection: {0}", handler));

            lock (workerListObj)
            {
                Invoke(new Method(() => workersList.Items.Add(handler)));
            }

            Send(handler);
        }

        private void Send(Socket handler)
        {
            string msg = GetNewMessage();
            
            StateObject state = new StateObject();
            state.workSocket = handler;
            state.buffer = Encoding.UTF8.GetBytes(msg);

            WriteLog(string.Format("Atempting to send message: \"{0}\" by socket: {1}", msg, handler));
            handler.BeginSend(state.buffer, 0, state.buffer.Length, 0, new AsyncCallback(SendCallback), state);
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                StateObject state = (StateObject)ar.AsyncState;
                Socket handler = state.workSocket;

                int bytesSent = handler.EndSend(ar);
                WriteLog(string.Format("Sent {0} bytes to client via socket {1}", bytesSent, handler));

                if (Encoding.UTF8.GetString(state.buffer).IndexOf(endMessage) != -1)
                {
                    WriteLog(string.Format("Closing socket {0}", handler));
                    handler.Close();
                    return;
                }

                state.buffer = new byte[1024];
                state.workSocket = handler;

                handler.BeginReceive(state.buffer, 0, state.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
                WriteLog(string.Format("Waiting to receive result by socket {0}", handler));
            } catch (Exception e)
            {
                WriteLog(string.Format("Exception in SendCallback: {0}", e.ToString()));
            }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;

            handler.EndReceive(ar);

            WriteLog(string.Format("Received {0} bytes via socket {1}", state.buffer.Length, handler));

            string msg = Encoding.UTF8.GetString(state.buffer);
            string[] numbers = msg.Split(';');

            state.resultIndex = Convert.ToInt32(numbers[0]);
            state.result = BigInteger.Parse(numbers[1]);

            UpdateResult(state);

            Send(handler);
        }

        private void UpdateResult(StateObject state)
        {
            Invoke(new Method(() =>
            {
                progressIdicator.Value += 1;
                timeLeftLabel.Text = wtch.Elapsed.ToString();
            }));

            lock (updateResultObj)
            {
                proccessedRows[state.resultIndex - 1] = state.result;

                if (proccessedRows.IndexOf(null) == -1)
                    SaveResults();
            }

            WriteLog(string.Format("Update result vector for index {0} value {1}", state.resultIndex, state.result));
        }

        private string GetNewMessage()
        {
            lock (getMessageObj)
            {
                int row = currentNotProcessed++;
                if (row >= proccessedRows.Count)
                    return endMessage;

                return string.Format("{0};{1};{2}", row + 1, matrixTemplate.Replace("#", (row + 1).ToString()), vectorFileName);
            }
        }

        private void WriteLog(string msg)
        {
            lock (writeLogObj)
            {
                if (logFile != null)
                {
                    logFile.WriteLine(msg);
                    Invoke(new Method(() => logListBox.Items.Add(msg)));
                }
            }
        }

        private void SaveResults()
        {
            lock (saveResultsObj)
            {
                wtch.Stop();

                if (resultsSaved)
                    return;
                
                WriteLog(string.Format("Closing listener socker {0}", listener));
                listener.Close();

                string resultsFileName = resultTextBox.Text != "" ? resultTextBox.Text : "results.txt";
                WriteLog(string.Format("Saving results info {0}", resultsFileName));
                StreamWriter results = new StreamWriter(resultsFileName);

                results.WriteLine(proccessedRows.Count);

                for (int i = 0; i < proccessedRows.Count; i++)
                {
                    results.Write(proccessedRows[i]);

                    if (i != proccessedRows.Count - 1)
                        results.Write(' ');
                }

                results.WriteLine("Time spent: {0}", wtch.Elapsed);
                results.Close();

                resultsSaved = true;
            }
        }

        private void stopExecutionButton_Click(object sender, EventArgs e)
        {
            logFile.Close();
        }

        private void generateNewMatrixButton_Click(object sender, EventArgs e)
        {
            MatrixGenerator mgen = new MatrixGenerator();
            mgen.MatrixGenerated += MatrixFilesGenerated;
            Enabled = false;
            mgen.Show();
        }

        private void MatrixFilesGenerated(object sender, MatrixGeneratorEventArgs e)
        {
            Enabled = true;
            matrixTemplateTextBox.Text = e.MatrixTemplateFileName;
            VectoFileTextBox.Text = e.VectorFileName;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();

            if (op.ShowDialog() == DialogResult.OK)
            {
                workerPathTextBox.Text = op.FileName;
            }

            numberToAccept = 0;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (workerPathTextBox.Text != null)
            {
                if (listener == null)
                    numberToAccept += 1;
                else
                    listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);

                Process.Start(workerPathTextBox.Text);
            }
        }
    }
}
