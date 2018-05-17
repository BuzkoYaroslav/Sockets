using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using NumericalAnalysisLibrary;
using NumericalAnalysisLibrary.Functions;
using NumericalAnalysisLibrary.Functions.Generic;
using System.Numerics;

namespace FunctionWorker
{
    class Program
    {
        class StateObject
        {
            // Client  socket.  
            public Socket workSocket = null;
            // Size of receive buffer.  
            public int BufferSize { get { return buffer.Length; } }
            // Receive buffer.  
            public byte[] allBytes;
            //
            public byte[] buffer = new byte[1024];
            // containing position for the result
            public int bytesSent = 0;
        }

        private static Mutex mtx = new Mutex(true);

        private static Socket sender = null;
        private static int socketPort = 11000;
        private static string host = "localhost";
        private const string endMessage = "<Close socket>";

        static void Main(string[] args)
        {
            MathFunction<BigFloat> function = null;

            Tuple<int, double> task = null;

            Console.WriteLine("Input host address:");
            host = Console.ReadLine();
            Console.WriteLine("Input port number:");
            socketPort = Convert.ToInt32(Console.ReadLine());

            var r = new BigFloat(double.MaxValue);

            ConnectToSocket(socketPort, host);

            function = ReceiveFunctionFromSocket();
            int curPower = 0;
            try
            {
                while ((task = ReceiveTaskFromSocket()) != null)
                {
                    Console.WriteLine(task);
                    SendMessageFromSocket(task, CalculateFunction(task.Item1, task.Item2, ref function, ref curPower));
                }
            } catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            Console.ReadKey();
            DisconnectFromSocket();
        }

        static void ConnectToSocket(int port, string host)
        {
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, port);

            sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            while (true)
            {
                try
                {
                    Console.WriteLine("Trying to connect to endpoint: {0}", ipEndPoint);
                    sender.Connect(ipEndPoint);
                    break;
                }
                catch (Exception e)
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

        static MathFunction<BigFloat> ReceiveFunctionFromSocket()
        {
            byte[] bytes = new byte[10000];

            int rec = sender.Receive(bytes);

            using (MemoryStream ms = new MemoryStream(bytes, 0, rec))
            {
                BinaryFormatter bf = new BinaryFormatter();

                return (MathFunction<BigFloat>)bf.Deserialize(ms);
            }
        }
        static Tuple<int, double> ReceiveTaskFromSocket()
        {
            byte[] bytes = new byte[1024];

            int bytesRec = sender.Receive(bytes);

            string receivedStr = Encoding.UTF8.GetString(bytes, 0, bytesRec);

            if (receivedStr.IndexOf(endMessage) != -1)
            {
                return null;
            }

            string[] numbers = receivedStr.Split(';');

            return new Tuple<int, double>(Convert.ToInt32(numbers[0]), Convert.ToDouble(numbers[1]));
        }
        static void SendMessageFromSocket(Tuple<int, double> values, MathFunction<BigFloat> result)
        {
            mtx.ReleaseMutex();

            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();

                bf.Serialize(ms, result);

                StateObject state = new StateObject();
                state.workSocket = sender;
                state.allBytes = ms.ToArray();

                byte[] msg = Encoding.UTF8.GetBytes(string.Format("{0};{1};{2};", values.Item1, values.Item2, state.allBytes.Length));
                Console.WriteLine(msg);

                mtx.WaitOne();

                sender.BeginSend(msg, 0, msg.Length, 0, new AsyncCallback(SendCallback), state);

                mtx.WaitOne();
            }
        }
        static void SendCallback(IAsyncResult ar)
        {
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;

            handler.EndSend(ar);

            Console.WriteLine(state.allBytes.Length);

            while (state.allBytes.Length != state.bytesSent)
            {
                state.bytesSent += sender.Send(state.allBytes, state.bytesSent, state.allBytes.Length - state.bytesSent, SocketFlags.Partial);
                Console.WriteLine("{0} bytes sent", state.bytesSent);
            }
        }


        private static MathFunction<BigFloat> CalculateFunction(int power, BigFloat point, ref MathFunction<BigFloat> function, ref int curPower)
        {
            function = function.Derivative(power - curPower);
            curPower = power;
            BigFloat value = function.Calculate(point);

            BigInteger fact = Factorial.Calculate(power);

            value /= fact;

            if (power == 0)
                return value;
            else if (power == 1)
                return value * (new XFunction<BigFloat>(1.0) - point);
            else
                return new PowerFunction<BigFloat>(value, new XFunction<BigFloat>(1.0) - point, (BigFloat)power);
        }
    }
}
