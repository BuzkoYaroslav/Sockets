using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Numerics;
using System.IO;

namespace SocketServer_v2
{
    public partial class Server : Form
    {
        // object for async sockets
        class StateObject
        {
            public Socket workerSocket;

            public int BufferSize { get { return buffer.Length; } }

            public byte[] buffer = new byte[1024];
        }

        // delegate for invoking on main thread
        private delegate void Method();

        // socket for listening connections
        private Socket listener;

        // server info
        private const string host = "localhost";
        private const int port = 11000;
        private const int maxNumberOfListeners = 100;
        private const string endMessage = "<Close socket>";

        // message when vector and matrix do not have appropriate sizes
        private const string invalidInputException = "InvalidInputException";

        private static class LogStrings
        {
            public static string TryingToBindListenerToEndPoint(IPEndPoint endPoint)
            {
                return string.Format("Trying to bind listener to local endpoint: {0}", endPoint);
            }
            public static string BindingWasSuccessful(IPEndPoint endPoint)
            {
                return string.Format("Binding to {0} was successful", endPoint);
            }

            public static string StartingToListenConnections()
            {
                return string.Format("Starting to listen to incoming connections");
            }

            public static string ExceptionWasThrown(Exception e)
            {
                return string.Format("Exception was thrown! Info: {0}", e.ToString());
            }

            public static string StartingToAcceptConnection()
            {
                return string.Format("Starting to accept incoming connection");
            }
            public static string ClientConnectionWasAccepted(string client)
            {
                return string.Format("{0} connected successfuly to server", client);
            }

            public static string WaitingToReceiveMessageFromClient(string client)
            {
                return string.Format("Waiting to receive message from {0}", client);
            }
            public static string BytesWereReceivedSuccessfuly(int bytesCount, string client)
            {
                return string.Format("Received {0} bytes from {1}", bytesCount, client);
            }
            public static string MessageWasReceivedSuccessfuly(string message, string client)
            {
                return string.Format("Received \"{0}\" from {1}", message, client);
            }

            public static string TryingToMultiplyMatrixAndVector(string matrix, string vect)
            {
                return string.Format("Trying to multiply matrix (path: {0}) and vector (path: {1})", matrix, vect);
            }
            public static string MultiplicationWasSuccessful(string matrix, string vect)
            {
                return string.Format("Multiplication of matrix (path: {0}) and vector (path: {1}) was successful", matrix, vect);
            }

            public static string AttemptingToSendMessageToClient(string message, string client)
            {
                return string.Format("Attempting to send \"{0}\" to {1}", message, client);
            }
            public static string BytesWereSentSuccessfuly(int bytesSent, string client)
            {
                return string.Format("{0} bytes were sent successfuly to {1}", bytesSent, client);
            }

            public static string MatrixWasLoaded(string matrix)
            {
                return string.Format("Matrix (path: {0}) was loaded", matrix);
            }
            public static string VectorWasLoaded(string vector)
            {
                return string.Format("Vector (path: {0}) was loaded", vector);
            }

            public static string ResulWasSaved(string path)
            {
                return string.Format("Result was saved into {0}", path);
            }

            public static string StopListening()
            {
                return string.Format("Closing listener socket");
            }
            public static string ClosingSocket(string client)
            {
                return string.Format("Closing socket between server and {0}", client);
            }
        }

        // managging async work
        private object socketDictObj = new object();

        // socket names
        private Dictionary<string, Socket> socketDict = new Dictionary<string, Socket>();

        public Server()
        {
            InitializeComponent();
        }

        #region Multiplying matrix and vector

        private string MultiplyVectorAndMatrix(string vectorFile, string matrixFile)
        {
            WriteLog(LogStrings.TryingToMultiplyMatrixAndVector(matrixFile, vectorFile));

            int[,] matrix = GetMatrixFromFile(matrixFile);
            int[] vector = GetVectorFromFile(vectorFile);

            if (matrix.GetLength(1) != vector.Length)
                return null;

            string fileName = GetFileName(vectorFile, matrixFile);
            StreamWriter writer = new StreamWriter(fileName);

            int length = matrix.GetLength(0);
            writer.WriteLine(length);

            for (int i = 0; i < length; i++)
            {
                BigInteger number = 0;

                for (int j = 0; j < vector.Length; j++)
                {
                    number += matrix[i, j] * vector[j];
                }

                writer.Write(number);
                if (i != length - 1)
                    writer.Write(' ');
            }

            WriteLog(LogStrings.MultiplicationWasSuccessful(matrixFile, vectorFile));

            writer.Close();

            return fileName;
        }

        private int[,] GetMatrixFromFile(string file)
        {
            StreamReader reader = new StreamReader(file);

            int[] sizes = GetNumbersFromRow(reader);

            int[,] matrix = new int[sizes[0], sizes[1]];

            for (int i = 0; i < sizes[0]; i++)
            {
                int[] nums = GetNumbersFromRow(reader);
                for (int j = 0; j < sizes[1]; j++)
                    matrix[i, j] = nums[j];
            }

            WriteLog(LogStrings.MatrixWasLoaded(file));

            reader.Close();

            return matrix;
        }
        private int[] GetVectorFromFile(string file)
        {
            StreamReader reader = new StreamReader(file);

            reader.ReadLine();

            int[] vector = GetNumbersFromRow(reader);

            reader.Close();

            WriteLog(LogStrings.VectorWasLoaded(file));

            return vector;
        }

        private int[] GetNumbersFromRow(StreamReader reader)
        {
            return reader.ReadLine().Split(' ').Select(x => Convert.ToInt32(x)).ToArray();
        }

        private string GetFileName(string vectorFile, string matrixFile)
        {
            int pos = vectorFile.LastIndexOfAny(new char[] { '/', '\\' });
            string path = vectorFile.Substring(0, pos);

            string vect = vectorFile.Substring(pos + 1);
            string matrix = matrixFile.Substring(matrixFile.LastIndexOfAny(new char[] { '/', '\\' }) + 1);

            vect = vect.Remove(vect.IndexOf('.'));
            matrix = matrix.Remove(matrix.IndexOf('.'));

            return string.Format("{0}\\{1}_x_{2}.txt", path, matrix, vect);
        }

        #endregion

        private void StartListening()
        {
            IPHostEntry ipHostEntry = Dns.GetHostEntry(host);
            IPAddress ipAddress = ipHostEntry.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, port);

            listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                WriteLog(LogStrings.TryingToBindListenerToEndPoint(localEndPoint));
                listener.Bind(localEndPoint);
                WriteLog(LogStrings.BindingWasSuccessful(localEndPoint));
                listener.Listen(maxNumberOfListeners);
                WriteLog(LogStrings.StartingToListenConnections());
            } catch (Exception exc)
            {
                WriteLog(LogStrings.ExceptionWasThrown(exc));
            }
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = null;
            try
            {
                handler = listener.EndAccept(ar);
            } catch (Exception e)
            {
                WriteLog(LogStrings.ExceptionWasThrown(e));
                return;
            }

            AddSocket(handler);
            string name = GetSocketName(handler);

            WriteLog(LogStrings.ClientConnectionWasAccepted(name));

            Invoke(new Method(() => clientsListBox.Items.Add(name)));

            Receive(handler);
        }

        private void Receive(Socket handler)
        {
            StateObject state = new StateObject();
            state.workerSocket = handler;

            WriteLog(LogStrings.WaitingToReceiveMessageFromClient(GetSocketName(handler)));

            handler.BeginReceive(state.buffer, 0, state.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workerSocket;

            string name = GetSocketName(handler);
            int bytesRec = 0;

            try
            {
                bytesRec = handler.EndReceive(ar);
            } catch (Exception e)
            {
                WriteLog(LogStrings.ExceptionWasThrown(e));
                return;
            }

            WriteLog(LogStrings.BytesWereReceivedSuccessfuly(bytesRec, name));

            string msg = Encoding.UTF8.GetString(state.buffer, 0, bytesRec);

            WriteLog(LogStrings.MessageWasReceivedSuccessfuly(msg, name));

            if (msg.IndexOf(endMessage) != -1)
            {
                Invoke(new Method(() => clientsListBox.Items.Remove(clientsListBox.Items[clientsListBox.Items.IndexOf(name)])));

                WriteLog(LogStrings.ClosingSocket(name));

                RemoveClient(name);

                try
                {
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                } catch (Exception e)
                {
                    WriteLog(LogStrings.ExceptionWasThrown(e));
                }

                return;
            }

            string[] paths = msg.Split(';');

            string response = MultiplyVectorAndMatrix(paths[0], paths[1]);

            if (response == null)
                response = invalidInputException;

            byte[] bytes = Encoding.UTF8.GetBytes(response);

            WriteLog(LogStrings.AttemptingToSendMessageToClient(response, name));

            handler.BeginSend(bytes, 0, bytes.Length, 0, new AsyncCallback(SendCallback), handler);
        }

        private void SendCallback(IAsyncResult ar)
        {
            Socket handler = (Socket)ar.AsyncState;

            int bytesSent = 0;

            try
            {
                bytesSent = handler.EndSend(ar);
            } catch (Exception e)
            {
                WriteLog(LogStrings.ExceptionWasThrown(e));
                return;
            }

            WriteLog(LogStrings.BytesWereSentSuccessfuly(bytesSent, GetSocketName(handler)));

            Receive(handler);
        }

        private void WriteLog(string msg)
        {
            Invoke(new Method(() => logListBox.Items.Add(msg)));
        }

        #region Client names

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
            lock (socketDictObj)
            {
                int count = socketDict.Keys.Count;

                socketDict.Add(SocketNameForIndex(count + 1), sock);
            }
        }
        private void RemoveClient(string client)
        {
            lock (socketDictObj)
            {
                socketDict.Remove(client);
            }
        }
        private string SocketNameForIndex(int index)
        {
            return string.Format("Client {0}", index);
        }

        #endregion

        #region Form events

        private void startListeningButton_Click(object sender, EventArgs e)
        {
            StartListening();
        }

        private void beginAcceptButton_Click(object sender, EventArgs e)
        {
            if (listener == null)
                return;

            listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);

            WriteLog(LogStrings.StartingToAcceptConnection());
        }

        private void stopListeningButton_Click(object sender, EventArgs e)
        {
            if (listener == null)
                return;

            WriteLog(LogStrings.StopListening());

            listener.Close();
        }

        #endregion
    }
}
