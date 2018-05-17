using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using NumericalAnalysisLibrary.Functions;
using NumericalAnalysisLibrary;
using NumericalAnalysisLibrary.Functions.Generic;

namespace SocketServer_MathFunction
{
    public partial class SocketsServer : Form
    {

        public class StateObject
        {
            // Client  socket.  
            public Socket workSocket = null;
            // Size of receive buffer.  
            public int BufferSize { get { return buffer.Length; } }
            // Buffer for the received bytes  
            public byte[] buffer = new byte[1024];
            // containing position for the result
            public int? functionIndex;
            // function
            public MathFunction<BigFloat> function;
            // All yet received bytes for current iteration
            public List<byte> receivedBytes = new List<byte>();
            // Count of all bytes for current iteration
            public int allBytesCount = 0;
        }

        private static class LogFormats
        {
            public static string TryingToBindSocketToLocalEndPoint(IPEndPoint endPoint)
            {
                return string.Format("Trying to bind socket to local end point. End point : {0}", endPoint);
            }
            public const string bindingWasSuccessfulString = "Binding was successful!";
            public const string tryingToListenString = "Trying to start listen";
            public const string listeningWasStartedString = "Listening was started!";

            public static string ExceptionWasThrownString(Exception e)
            {
                return string.Format("Exception in StartListening! Exception message: {0}", e.ToString());
            }

            public static string AcceptedSuccessfully(string workerName)
            {
                return string.Format("Accepted successfully for {0}", workerName);
            }
            public static string FunctionWasSentToWorker<T>(MathFunction<T> func, string workerName) where T: new()
            {
                return string.Format("Function {0} was sent to {1}", func, workerName);
            }
            public static string AtemptingToSendMessage(string message, string workerName)
            {
                return string.Format("Atempting to send message: \"{0}\" to {1}", message, workerName);
            }
            public static string BytesWereSentSuccessfuly(int bytesSent, string workerName)
            {
                return string.Format("Sent {0} bytes to {1}", bytesSent, workerName);
            }
            public static string ClosingSocket(string workerName)
            {
                return string.Format("{0} is finishing working", workerName);
            }

            public static string WaitingToReceiveResultFromWorker(string workerName)
            {
                return string.Format("Waiting to receive result from {0}", workerName);
            }
            public static string BytesWereSuccessfulyReceived(int bytesLength, string workerName)
            {
                return string.Format("Received {0} bytes from {1}", bytesLength, workerName);
            }

            public static string ResultWasUpdated<T>(int functionIndex, MathFunction<T> function) where T: new()
            {
                return string.Format("Update expression for index {0} function {1}", functionIndex, function);
            }

            public static string ListenerSocketWasClosed()
            {
                return string.Format("Closing listener socker");
            }

            public static string SavingResultsInto(string fileName)
            {
                return string.Format("Saving results into {0}", fileName);
            }
        }
        
        // Server-client collaboration info
        private const int socketPort = 11000;
        private const string host = "localhost";
        private const int numberOfPossibleListeners = 100;
        private const string endMessage = "<Close socket>";

        // delegate for invoking code on main thread
        private delegate void Method();

        // socket for listening
        private Socket listener;
        private int numberToAccept = 0;

        // result of the multiplication
        private List<MathFunction<BigFloat>> proccessedFunctions;

        // function to process
        //private MathFunction<BigFloat> function = (new CosFunction<BigFloat>(2.0, new XFunction<BigFloat>(1.0)) + new SinFunction<BigFloat>(1.0, new XFunction<BigFloat>(1.0))) *
        //    (new XFunction<BigFloat>(1.0) ^ 20) + new SinFunction<BigFloat>(1.0, new XFunction<BigFloat>(1.0));
        private MathFunction<BigFloat> function = new CosFunction<BigFloat>(1.0, new XFunction<BigFloat>(1.0));

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
        private object socketDictObj = new object();

        // dictionary for name - socket mapping
        private Dictionary<string, Socket> socketDict;

        // indicates whether results are saved or not
        private bool resultsSaved = false;

        // user input data
        private double pointOfExpression;
        private int powerOfExpression;

        // time calculation
        private Stopwatch wtch = new Stopwatch();

        public SocketsServer()
        {
            InitializeComponent();
        }

        // Starting of server-client work
        private void startExecutionButton_Click(object sender, EventArgs e)
        {
            pointOfExpression = Convert.ToDouble(pointTextBox.Text);
            powerOfExpression = Convert.ToInt32(powerTextBox.Text);

            proccessedFunctions = new MathFunction<BigFloat>[powerOfExpression + 1].ToList();
            socketDict = new Dictionary<string, Socket>();

            string logFileName = logTextBox.Text == "" ? "log.txt" : logTextBox.Text;
            logFile = new StreamWriter(logFileName);
            logListBox.Items.Clear();
            workersList.Items.Clear();

            progressIdicator.Value = 0;
            progressIdicator.Maximum = powerOfExpression + 1;

            wtch.Start();

            StartListening();
        }

        // Starting to listen to the connections
        private void StartListening()
        {
            IPHostEntry ipHostInfo = Dns.GetHostEntry(host);
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, socketPort);

            listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                WriteLog(LogFormats.TryingToBindSocketToLocalEndPoint(localEndPoint));
                listener.Bind(localEndPoint);
                WriteLog(LogFormats.bindingWasSuccessfulString);
                WriteLog(LogFormats.tryingToListenString);
                listener.Listen(numberOfPossibleListeners);
                WriteLog(LogFormats.listeningWasStartedString);

                Thread thrd = new Thread(() =>
                {
                    for (int i = 0; i < numberToAccept; i++)
                        listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);
                });
                thrd.Start();
            } catch (Exception e)
            {
                WriteLog(LogFormats.ExceptionWasThrownString(e));
            }
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            AddSocket(handler);

            string socketName = GetSocketName(handler);

            WriteLog(LogFormats.AcceptedSuccessfully(socketName));

            lock (workerListObj)
            {
                Invoke(new Method(() => workersList.Items.Add(socketName)));
            }
            
            handler.Send(GetSerializedFunction());
            WriteLog(LogFormats.FunctionWasSentToWorker(function, socketName));

            Send(handler);
        }

        private void Send(Socket handler)
        {
            string msg = GetNewMessage();

            StateObject state = new StateObject();
            state.workSocket = handler;
            state.buffer = Encoding.UTF8.GetBytes(msg);

            WriteLog(LogFormats.AtemptingToSendMessage(msg, GetSocketName(handler)));
            handler.BeginSend(state.buffer, 0, state.buffer.Length, 0, new AsyncCallback(SendCallback), state);
        }

        // callbacks
        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                StateObject state = (StateObject)ar.AsyncState;
                Socket handler = state.workSocket;

                int bytesSent = handler.EndSend(ar);

                string socketName = GetSocketName(handler);
                WriteLog(LogFormats.BytesWereSentSuccessfuly(bytesSent,socketName));

                if (Encoding.UTF8.GetString(state.buffer).IndexOf(endMessage) != -1)
                {
                    WriteLog(LogFormats.ClosingSocket(socketName));
                    handler.Close();
                    return;
                }

                state.buffer = new byte[100];
                state.workSocket = handler;

                handler.BeginReceive(state.buffer, 0, state.BufferSize, SocketFlags.Partial, new AsyncCallback(ReceiveCallback), state);
                WriteLog(LogFormats.WaitingToReceiveResultFromWorker(socketName));
            } catch (Exception e)
            {
                WriteLog(LogFormats.ExceptionWasThrownString(e));
            }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;

            int rec = handler.EndReceive(ar);

            WriteLog(LogFormats.BytesWereSuccessfulyReceived(rec, GetSocketName(handler)));

            string[] arr = Encoding.UTF8.GetString(state.buffer).Split(';');

            state.functionIndex = Convert.ToInt32(arr[0]);
            state.allBytesCount = Convert.ToInt32(arr[2]);

            int length = arr[0].Length + arr[1].Length + arr[2].Length + 3;

            if (rec > length)
            {
                state.receivedBytes = state.buffer.ToList();
                state.receivedBytes.RemoveRange(0, length);

                rec -= length;

                state.receivedBytes.RemoveRange(rec, state.receivedBytes.Count - rec);
            }

            state.buffer = new byte[1024];

            handler.BeginReceive(state.buffer, 0, state.BufferSize, SocketFlags.Partial, new AsyncCallback(ReceiveFunctionCallBack), state);
        }
        private void ReceiveFunctionCallBack(IAsyncResult ar)
        {
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;

            int bytesRec = handler.EndReceive(ar);

            WriteLog(LogFormats.BytesWereSuccessfulyReceived(bytesRec, GetSocketName(handler)));

            List<byte> res = state.buffer.ToList();

            if (bytesRec < state.buffer.Length)
                res.RemoveRange(bytesRec, state.buffer.Length - bytesRec);

            state.receivedBytes.AddRange(res);

            if (state.allBytesCount > state.receivedBytes.Count)
            {
                int length = handler.Available > 1024 ? 1024 : handler.Available;
                state.buffer = new byte[length];

                handler.BeginReceive(state.buffer, 0, length, SocketFlags.Partial, new AsyncCallback(ReceiveFunctionCallBack), state);
            } else
            {
                state.function = GetDeserializedFunction(state.receivedBytes.ToArray());

                UpdateResult(state);

                Send(handler);
            }
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
                proccessedFunctions[state.functionIndex ?? 0] = state.function;

                if (proccessedFunctions.IndexOf(null) == -1)
                    SaveResults();
            }

            WriteLog(LogFormats.ResultWasUpdated(state.functionIndex ?? 0, state.function));
        }

        private void CloseListenerSocket()
        {
            WriteLog(LogFormats.ListenerSocketWasClosed());
            listener.Close();
        }

        private string GetNewMessage()
        {
            lock (getMessageObj)
            {
                int row = currentNotProcessed++;
                if (row >= proccessedFunctions.Count)
                    return endMessage;

                return string.Format("{0};{1}", row, pointOfExpression);
            }
        }

        private string GetSocketName(Socket sock)
        {
            lock (socketDictObj)
            {
                var res = socketDict.Where((pair) => pair.Value == sock);

                foreach (var r in res)
                    return r.Key;

                return null;
            }
        }
        private void AddSocket(Socket sock)
        {
            lock(socketDictObj)
            {
                int count = socketDict.Keys.Count;

                socketDict.Add(SocketNameForIndex(count + 1), sock);
            }
        }
        private string SocketNameForIndex(int index)
        {
            return string.Format("Worker {0}", index);
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
                
                string resultsFileName = resultTextBox.Text != "" ? resultTextBox.Text : "results.txt";
                WriteLog(LogFormats.SavingResultsInto(resultsFileName));

                var result = new MathFunction<BigFloat>(1.0, NumericalAnalysisLibrary.Functions.Generic.MathFunctionType.Sum, proccessedFunctions.ToArray());

                StreamWriter results = new StreamWriter(resultsFileName);
                results.WriteLine("Function to express y = {0}\n"
                    + "Function expression by point x = {1}, power = {2}\n"
                    + "y = {3}",
                    function, pointOfExpression, powerOfExpression, result);
                Invoke(new Method(() => MessageBox.Show(string.Format("Function to express y = {0}\n"
                    + "Function expression by point x = {1}, power = {2}\n"
                    + "y = {3}",
                    function, pointOfExpression, powerOfExpression, result))));

                results.WriteLine("Time spent: {0}", wtch.Elapsed);

                results.Close();

                resultsSaved = true;
            }
        }

        // MathFunction serialization
        private byte[] GetSerializedFunction()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();

                bf.Serialize(ms, function);

                return ms.ToArray();
            }
        }
        private MathFunction<BigFloat> GetDeserializedFunction(byte[] buffer)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();

                ms.Write(buffer, 0, buffer.Length);
                ms.Seek(0, SeekOrigin.Begin);

                return (MathFunction<BigFloat>)bf.Deserialize(ms);
            }
        }

        private void stopExecutionButton_Click(object sender, EventArgs e)
        {
            logFile.Close();
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

        private void SocketsServer_Load(object sender, EventArgs e)
        {
            BigFloat.Precision = 100;

            functionDescriptionRichTextBox.Text = string.Format("f(x) = {0}", function);
        }
    }
}
