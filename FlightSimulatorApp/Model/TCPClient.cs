using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Windows.Automation;

namespace FlightSimulatorApp.Model
{
    class TCPClient
    {
        private TcpClient client;
        private Boolean running;

        private Dictionary<string, string> _valuesFromSim;
        private Dictionary<string, string> _valuesToSim;

        public Boolean Connect(String ip, Int32 port)
        {
            /* Connect to server */
            try
            {
                client = new TcpClient(ip, port);
            }
            catch (Exception e)
            {
                /*TODO debug*/
                Console.WriteLine("Error #1 TCPClient.Connect..");
            }

            if (!client.Connected)
            {
                running = false;
                return false;
            }

            /* TODO debug */
            Console.WriteLine("Client connected successfully to server...");
            running = true;
            return true;
        }

        public void Disconnect()
        {
            running = false;
            client.GetStream().Close();
            client.Close();
        }

        public void Start()
        {
            /* Error starting communicating server.. */
            if (!running)
            {
                /* TODO debug */
                Console.WriteLine("Error #1 TCPClient.Start...");
            }

            NetworkStream stream = client.GetStream();
            using (stream = client.GetStream())
            {
                /* Timeout after 10 seconds */
                stream.ReadTimeout = TimeSpan.FromSeconds(10).Milliseconds;

                /* Start working */
                new Thread(() =>
                {
                    /* Analyzer mainly used for parsing read data */
                    SimDataAnalyzer analyzer = new SimDataAnalyzer();
                    Byte[] readBuffer;

                    while (running)
                    {
                        if (!client.Connected)
                        {
                            /* TODO debug */
                            Console.WriteLine("Error #1 TCPClient.Start...");
                        }

                        if (client.ReceiveBufferSize > 0)
                        {
                            readBuffer = new byte[client.ReceiveBufferSize];

                            /* Read data from server */
                            try
                            {
                                stream.Read(readBuffer, 0, client.ReceiveBufferSize);
                            }
                            catch (IOException e)
                            {
                                /*TODO add GUI notification for this error*/
                                if (e.GetBaseException().GetType() == typeof(SocketException))
                                {
                                    Console.WriteLine("Err");
                                }
                            }

                            analyzer.Parse(Encoding.ASCII.GetString(readBuffer));
                        }

                        Thread.Sleep(250);
                    }
                }).Start();
            }
        }


        public void Stop()
        {

        }

    }
}
