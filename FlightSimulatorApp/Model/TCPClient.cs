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
        private TcpClient _client;
        private Boolean _running;

        private Dictionary<string, string> _valuesFromSim;
        private Dictionary<string, string> _valuesToSim;

        public String Ip
        {
            get { return _ip;}
            set { _ip = value;}
        }

        /*TCPClient(String ip, Int32 port)
        {
            _ip = ip;
            _port = port;

        }*/

        public Boolean Connect(String ip, Int32 port)
        {
            /* Connect to server */
            try
            {
                _client = new TcpClient(ip, port);
            }
            catch (Exception e)
            {
                /*TODO debug*/
                Console.WriteLine("Error #1 TCPClient.Connect..");
            }

            if (!_client.Connected)
            {
                _running = false;
                return false;
            }

            /* TODO debug */
            Console.WriteLine("Client connected successfully to server...");
            _running = true;
            return true;
        }

        public void Disconnect()
        {
            _running = false;
            _client.GetStream().Close();
            _client.Close();
        }

        public void Start()
        {
            /* Error starting communicating server.. */
            if (!_running)
            {
                /* TODO debug */
                Console.WriteLine("Error #1 TCPClient.Start...");
            }
            
            
            
            using (stream = _client.GetStream())
            {
                /* Timeout after 10 seconds */
                _stream.ReadTimeout = TimeSpan.FromSeconds(10).Milliseconds;

                /* Start working */
                new Thread(() =>
                {
                    /* Analyzer mainly used for parsing read data */
                    SimDataAnalyzer analyzer = new SimDataAnalyzer();
                    String data = null;
                    Byte[] readBuffer = new byte[4096];

                    while (running && _client.Connected)
                    {
                        /* Read data from server */
                        try
                        {
                            _stream.Read(readBuffer);
                        }
                        catch (IOException e)
                        {
                            /*TODO add GUI notification for this error*/
                            if (e.GetBaseException().GetType() == typeof(SocketException))
                            {
                                Console.WriteLine("Err");
                            }
                        }

                        Thread.Sleep(250);
                    }
                }).Start();
            }
        }
    }

        public void Stop()
        {

        }

    }
}
