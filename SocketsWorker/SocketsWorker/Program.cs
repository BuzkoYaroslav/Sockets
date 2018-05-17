using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Numerics;

namespace SocketsWorker
{
    class Program
    {
        private static Socket sender = null;
        private const int socketPort = 11000;
        private const string host = "localhost";
        private const string endMessage = "<Close socket>";

        static void Main(string[] args)
        {
            
            Tuple<int, string, string> filesToProcess = null;

            ConnectToSocket(socketPort, host);

            while ((filesToProcess = RecieveMessageFromSocket(socketPort, host)) != null)
            {
                SendMessageFromSocket(filesToProcess.Item1, ProcessFiles(filesToProcess.Item2, filesToProcess.Item3));
            }

            DisconnectFromSocket();
        }

        static void ConnectToSocket(int port, string host)
        {
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, port);

            sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            while (true) {
                try
                {
                    Console.WriteLine("Trying to connect to endpoint: {0}", ipEndPoint);
                    sender.Connect(ipEndPoint);
                    break;
                } catch (Exception e)
                {
                    Console.WriteLine("Exception: {0}", e.Message);
                }
            }
            
        }
        static void DisconnectFromSocket()
        {
            if (sender == null)
                return;

            sender.Shutdown(SocketShutdown.Both);
            sender.Close();
        }

        static Tuple<int, string, string> RecieveMessageFromSocket(int port, string host)
        {
            byte[] bytes = new byte[1024];

            int bytesRec = sender.Receive(bytes);

            string receivedStr = Encoding.UTF8.GetString(bytes, 0, bytesRec);

            if (receivedStr.IndexOf(endMessage) != -1)
            {
                return null;
            }

            string[] fileNames = receivedStr.Split(';');

            return new Tuple<int, string, string>(Convert.ToInt32(fileNames[0]), fileNames[1], fileNames[2]);
        }
        static void SendMessageFromSocket(int index, BigInteger result)
        {
            byte[] msg = Encoding.UTF8.GetBytes(string.Format("{0};{1}", index, result));

            sender.Send(msg);
        }

        private static BigInteger ProcessFiles(string vector, string matrixRow)
        {
            StreamReader vectReader = new StreamReader(vector);
            StreamReader matrixReader = new StreamReader(matrixRow);

            int num1 = Convert.ToInt32(vectReader.ReadLine()),
                num2 = Convert.ToInt32(matrixReader.ReadLine());

            // Exception when sequences have not got the same size
            if (num1 != num2)
                throw new InvalidOperationException();

            BigInteger result = 0;

            for (int i = 0; i < num1; i++)
                result += ReadNumberFromStream(vectReader) * ReadNumberFromStream(matrixReader);

            return result;
        }
        private static long ReadNumberFromStream(StreamReader stream)
        {
            string str = "";
            char c;

            while (!stream.EndOfStream && char.IsDigit((c = (char)stream.Read())))
            {
                str += c;
            }
            
            return Convert.ToInt64(str);
        }
    }
}
