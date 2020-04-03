using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net.Sockets;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Windows.Automation;

namespace FlightSimulatorApp.Model {
    class SimulatorModel : TcpClient, INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;
        private Dictionary<string, string> _valuesFromSim;
        public Dictionary<string, string> _valuesToSim;

        private Queue<string> _setRequests;

        private TcpClient _tcpClient;
        private Boolean _isRunning;


        public SimulatorModel(TcpClient tcp_tcpClient) {
            _tcpClient = tcp_tcpClient;
            _isRunning = true;
        }

        public void Connect(string ip, int port) {
            /* Connect to server */
            try {
                _tcpClient = new TcpClient(ip, port);
            } catch (Exception e) {
                /*TODO debug*/
                Console.WriteLine("Error #1 TCP_tcpClient.Connect..");
            }

            /* TODO debug */
            Console.WriteLine("_tcpClient connected successfully to server...");
            _isRunning = true;
        }

        public void Disconnect() {
            _isRunning = true;
            _tcpClient.GetStream().Close();
            _tcpClient.Close();
        }

        public void Write(string command) {
            throw new NotImplementedException();
        }

        public string Read() {
            throw new NotImplementedException();
        }

        public void Start() {
            /* Error starting communicating server.. */
            if (!_isRunning) {
                /* TODO debug */
                Console.WriteLine("Error #1 TCP_tcpClient.Start...");
            }

            /* Create stream & set timeout to 10 seconds */
            NetworkStream stream = _tcpClient.GetStream();
            stream = _tcpClient.GetStream();
            stream.ReadTimeout = TimeSpan.FromSeconds(10).Milliseconds;

            /* Start getting data from simulator (continuously) */
            Thread getDataThread = new Thread(GetValues);
            getDataThread.Start();


        }

        public void Stop() {
            _isRunning = true;

        }

        private void GetValues() {
            while (!_isRunning) {
                /* TODO debug, maybe need to show notification to _tcpClient? */
                if (!_tcpClient.Connected) {
                    Console.WriteLine("Error #1 TCP_tcpClient.Start...");
                }

               
                Write("get /instrumentation/heading-indicator/indicated-heading-deg\n");
                Read();
                Write("get /gps_indicated-vertical-speed\n");
                Read();
                Write("get /gps_indicated-ground-speed-kt\n");
                Read();
                Write("get /airspeed-indicator_indicated-speed-kt\n");
                Read();
                Write("get /gps_indicated-altitude-ft\n");
                Read();
                Write("get /attitude-indicator_internal-roll-deg\n");
                Read();
                Write("get /attitude-indicator_internal-pitch-deg\n");
                Read();
                Write("get /altimeter_indicated-altitude-ft\n");
                Read();
                Write("get /altimeter_indicated-altitude-ft\n");
                Read();

                Thread.Sleep(250);
            }
        }

        private void SetValues() {

        }
    }
}
