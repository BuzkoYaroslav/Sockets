using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using System.Diagnostics;

namespace SocketWorker_v2
{
    public partial class Client : Form
    {
        private class StateObject
        {
            public Socket workSocket;
            public byte[] buffer = new byte[1024];
        }

        private static class LogStrings
        {
            public static string DataGeneratedSuccessfully(string vect, string matrix)
            {
                return string.Format("Matrix (path: {0}) and vector (path: {1}) generated!", matrix, vect);
            }
            public static string TryingToConnectToEndPoint(IPEndPoint endPoint)
            {
                return string.Format("Trying to connect to local endpoint: {0}", endPoint);
            }
            public static string ConnectedSuccessfullyToEndPoint(IPEndPoint endPoint)
            {
                return string.Format("Connected successfully to endpoint: {0}", endPoint);
            }

            public static string ExceptionWasThrown(Exception e)
            {
                return string.Format("Exception was thrown! Info: {0}", e.ToString());
            }

            public static string AttemptingToSendMessage(string msg)
            {
                return string.Format("Attempting to send \"{0}\" to server", msg);
            }
            public static string BytesWereSuccessfulySent(int bytesCount)
            {
                return string.Format("{0} bytes were sent to server!", bytesCount);
            }

            public static string WaitingToReceiveMessageFromServer()
            {
                return string.Format("Waiting to reveive response from server!");
            }
            public static string BytesWereReceivedFromServer(int bytesCount)
            {
                return string.Format("{0} bytes were received from server", bytesCount);
            }
            public static string MessageWasReceived(string msg)
            {
                return string.Format("\"{0}\" was received from server", msg);
            }

            public static string CloseConnection()
            {
                return string.Format("Closing connection to server");
            }
            public static string CloseSocket()
            {
                return string.Format("Closing socket!");
            }

            public static string ResultOfMultiplyingMatricies(string matrix, string vect, string path)
            {
                return string.Format("Result of multipling matrix (path: {0})" +
                    "and vector (path: {1}) is vector (path: {2})",
                    matrix, vect, path);
            }
        }

        // delegate for invoking on main thread
        private delegate void Method();

        // data info
        private string vectorFileName = "vector.txt";
        private string matrixFileName = "matrix.txt";
        private int size;

        // generating data
        private const int minValue = 0;
        private const int maxValue = 10000;
        private Random rnd = new Random();

        // server info
        private string host = "localhost";
        private int port = 11000;
        private const string endMessage = "<Close socket>";

        // collaboration socker
        private Socket sender;

        // managging async work
        private object senderObj = new object();
        private SemaphoreSlim senderSema = new SemaphoreSlim(1);

        public Client()
        {
            InitializeComponent();
        }

        #region Generating data

        private void GetFileNames()
        {
            vectorFileName = vectorFileTextBox.Text == "" ? string.Format("vector{0}.txt", GetHashCode()) : vectorFileTextBox.Text;
            matrixFileName = matrixFileTextBox.Text == "" ? string.Format("matrix{0}.txt", GetHashCode()) : matrixFileTextBox.Text;
            
            FileStream v = File.Open(vectorFileName, FileMode.OpenOrCreate);
            FileStream m = File.Open(matrixFileName, FileMode.OpenOrCreate);

            vectorFileName = v.Name;
            matrixFileName = m.Name;

            v.Close();
            m.Close();
        }
        private int GetValue()
        {
            return rnd.Next(minValue, maxValue);
        }

        private void GenerateData()
        {
            GenerateVector();
            GenerateMatrix();

            WriteLog(LogStrings.DataGeneratedSuccessfully(vectorFileName, matrixFileName));
        }

        private void GenerateVector()
        {
            StreamWriter writer = new StreamWriter(vectorFileName);

            writer.WriteLine(size);
            GenerateIntSequence(writer, size);

            writer.Close();
        }
        private void GenerateMatrix()
        {
            StreamWriter writer = new StreamWriter(matrixFileName);

            writer.WriteLine("{0} {0}", size);
            for (int i = 0; i < size; i++)
            {
                GenerateIntSequence(writer, size);
                writer.WriteLine();
            }

            writer.Close();
        }
        private void GenerateIntSequence(StreamWriter writer, int size)
        {
            for (int i = 0; i < size; i++)
            {
                writer.Write(GetValue());
                if (i != size - 1)
                    writer.Write(' ');
            }
        }

        #endregion

        private void ConnectToServer()
        {
            IPHostEntry ipHostInfo = Dns.GetHostEntry(host);
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, port);

            senderSema.Wait();
            sender = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            while (true)
            {
                try
                {
                    WriteLog(LogStrings.TryingToConnectToEndPoint(ipEndPoint));
                    sender.Connect(ipEndPoint);
                    WriteLog(LogStrings.ConnectedSuccessfullyToEndPoint(ipEndPoint));
                    senderSema.Release();
                    break;
                }
                catch (Exception exc)
                {
                    WriteLog(LogStrings.ExceptionWasThrown(exc));
                }
            }
        }
        private void SendTaskToServer()
        {
            if (sender == null)
                return;

            senderSema.Wait();

            string msg = GetMessage();
            byte[] bytes = Encoding.UTF8.GetBytes(msg);

            WriteLog(LogStrings.AttemptingToSendMessage(msg));

            sender.BeginSend(bytes, 0, bytes.Length, 0, new AsyncCallback(SendCallback), sender);
        }
        private void SendCallback(IAsyncResult ar)
        {
            Socket sender = (Socket)ar.AsyncState;
            int bytesSent = 0;

            try
            {
                bytesSent = sender.EndSend(ar);
            } catch (Exception e)
            {
                WriteLog(LogStrings.ExceptionWasThrown(e));
                return;
            }

            WriteLog(LogStrings.BytesWereSuccessfulySent(bytesSent));

            StateObject state = new StateObject();
            state.workSocket = sender;

            WriteLog(LogStrings.WaitingToReceiveMessageFromServer());

            sender.BeginReceive(state.buffer, 0, state.buffer.Length, 0, new AsyncCallback(ReceiveCallback), state);
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;
            int bytesRec = 0;

            try
            {
                bytesRec = handler.EndReceive(ar);
            } catch (Exception e)
            {
                WriteLog(LogStrings.ExceptionWasThrown(e));
                return;
            }

            WriteLog(LogStrings.BytesWereReceivedFromServer(bytesRec));

            senderSema.Release();

            string path = Encoding.UTF8.GetString(state.buffer, 0, bytesRec);

            PrintResult(path);
        }

        private void CloseConnection()
        {
            if (sender == null)
                return;

            senderSema.Wait();

            WriteLog(LogStrings.CloseConnection());
            WriteLog(LogStrings.AttemptingToSendMessage(endMessage));

            byte[] bytes = Encoding.UTF8.GetBytes(endMessage);

            sender.BeginSend(bytes, 0, bytes.Length, 0, new AsyncCallback(CloseConnectionCallback), sender);
        }

        private void CloseConnectionCallback(IAsyncResult ar)
        {
            int sent = 0;

            try
            {
                sent = sender.EndSend(ar);
            } catch (Exception e)
            {
                WriteLog(LogStrings.ExceptionWasThrown(e));
                return;
            }

            WriteLog(LogStrings.BytesWereSuccessfulySent(sent));
            WriteLog(LogStrings.CloseSocket());

            try
            {
                sender.Shutdown(SocketShutdown.Both);
                sender.Close();
            } catch (Exception e)
            {
                WriteLog(LogStrings.ExceptionWasThrown(e));
            }

            senderSema.Release();
        }

        private string GetMessage()
        {
            return string.Format("{0};{1};", vectorFileName, matrixFileName);
        }

        private void PrintResult(string path)
        {
            string msg = LogStrings.ResultOfMultiplyingMatricies(matrixFileName, vectorFileName, path);

            WriteLog(msg);
            Invoke(new Method(() =>
            {
                if (MessageBox.Show(msg, "Result", MessageBoxButtons.OK, MessageBoxIcon.Information) == DialogResult.OK)
                {
                    Process.Start(path);
                }
            }));
        }

        private void WriteLog(string msg)
        {
            Invoke(new Method(() => logListBox.Items.Add(msg)));
        }

        #region Form events

        private void connectButton_Click(object sender, EventArgs e)
        {
            Thread thrd = new Thread(() => ConnectToServer());
            thrd.Start();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            matrixFileTextBox.Text = matrixFileName;
            vectorFileTextBox.Text = vectorFileName;

            GetFileNames();

            hostTextBox.Text = host;
            portTextBox.Text = port.ToString();
        }

        private void generateButton_Click(object sender, EventArgs e)
        {
            try
            {
                size = Convert.ToInt32(sizeTextBox.Text);
                GetFileNames();

                GenerateData();
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.ToString(), "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void sendTaskButton_Click(object sender, EventArgs e)
        {
            SendTaskToServer();
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            CloseConnection();
        }

        #endregion
    }
}
