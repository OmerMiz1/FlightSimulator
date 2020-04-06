using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net.Sockets;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows.Automation;

namespace FlightSimulatorApp.Model {
    public class SimulatorModel : TcpClient, INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;
        private Dictionary<string, Double> _valuesFromSim;
        public Dictionary<string, Double> _valuesToSim;
        private List<string> _valuesFromSimNames;

        private Queue<string> _setRequests;

        private TcpClient _tcpClient;
        private NetworkStream _stream;
        private Boolean _isRunning;
        private Boolean _debug;

        public SimulatorModel(TcpClient tcpClient) : this(tcpClient, false) { }

        public SimulatorModel(TcpClient tcpClient, Boolean mode) {
            InitializeNames();
            _tcpClient = tcpClient;
            _debug = mode;
        }

        public void Connect(string ip, int port) {
            /* Connect to server */
            try {
                _tcpClient = new TcpClient(ip, port);
            } catch (Exception e) {
                if (_debug) {
                    Console.WriteLine("Error #1 SimulatorModel.Connect()..");
                }
            }

            if (_debug) {
                Console.WriteLine("TCP Client: Connected successfully to server...");
            }
        }

        public void Disconnect() {
            _tcpClient.GetStream().Close();
            _tcpClient.Close();

            if (_debug) {
                Console.WriteLine("TCP Client: Disconnected successfully to server...");
            }
        }

        public void Write(string msg) {
            /* Double check writing is possible */
            if (_stream.CanWrite) {
                byte[] writeBuffer = Encoding.ASCII.GetBytes(msg);
                _stream.Write(writeBuffer, 0, writeBuffer.Length);
            } else if (_debug) {
                Console.WriteLine("Error #2 at Simulator.Write()...");
            }
        }

        public string Read() {
            /* Double check reading is possible */
            if (_stream.CanRead) {
                byte[] readBuffer = new byte[4096];
                int bytesRead = 0;
                StringBuilder strBuilder = new StringBuilder();

                /*** Read all data sent from simulator
                 * NOTE: Probably there's no need in the string builder
                 * because the messaged received from sim are short.
                 * Still i think its a good idea just in-case.
                 ***/
                do {
                    bytesRead = _stream.Read(readBuffer, 0, readBuffer.Length);
                    strBuilder.AppendFormat("{0}", Encoding.ASCII.GetString(readBuffer, 0, bytesRead));
                } while (_stream.DataAvailable);

                return strBuilder.ToString();
            } else if (_debug) {
                Console.WriteLine("Error #1 at SimulatorModel.Read()...");
            }

            /* Error */
            if (_debug) {
                /*TODO keep the exception or simple console message?*/
                throw new System.InvalidOperationException("Error #2 at Simulator.Read()...");
                Console.WriteLine("Error #2 at Simulator.Read()...");
            }
            return null;
        }

        public void Start() {
            _isRunning = true;

            /* Create stream & set timeout to 10 seconds */
            _stream = _tcpClient.GetStream();
            _stream.ReadTimeout = TimeSpan.FromSeconds(10).Milliseconds;

            /* Start getting data from simulator (continuously) */
            Thread getValuesThread = new Thread(ReadValuesFromSim);
            getValuesThread.Start();

            /* Start responding to set requests from user input (GUI actually) */
            Thread setValuesThread = new Thread(WriteValuesToSim);
            setValuesThread.Start();

            getValuesThread.Join();
            setValuesThread.Join();

            Disconnect();
        }

        public void Stop() {
            _isRunning = false;
        }

        public void NotifyPropertyChanged(string propName) {
            if (PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }

        private void ReadValuesFromSim() {
            /* Build get requests message (once) */
            StringBuilder strBuilder = new StringBuilder();
            foreach (string varName in _valuesFromSimNames) {
                strBuilder.Append("get ");
                strBuilder.AppendLine(varName);
            }
            string requestMsg = strBuilder.ToString();

            /* Request values from simulator every 100ms */
            while (_isRunning) {
                /* TODO maybe need to show notification to user? */
                if (!_tcpClient.Connected && _debug) {
                    Console.WriteLine("Error #1 SimulatorModel.Start()...");
                }

                /* Send request for updates & read response */
                Write(requestMsg);
                string valuesFromSim = Read();

                /* Case response is valid */
                if (valuesFromSim != null) {
                    /* Enumerate each variable manually (iterator)*/
                    List<string>.Enumerator varEnum = _valuesFromSimNames.GetEnumerator();

                    /* Split received values and update each one */
                    string[] newValsArray = valuesFromSim.Split('\n');
                    foreach (string newVal in newValsArray) {
                        _valuesFromSim[varEnum.Current] = Convert.ToDouble(newVal);
                        NotifyPropertyChanged(varEnum.Current);
                    }

                    varEnum.Dispose();
                } else if (_debug) {
                    Console.WriteLine("Error #2 SimulatorModel.Start()...");
                }
                Thread.Sleep(100);
            }
        }

        /**
         *TODO Its not supposed to be like that.
         *TODO Each property update should be triggered from VM and updated as event.
         **/
        private void WriteValuesToSim() {
            while (_isRunning) {
                if (_setRequests.Count != 0) {
                    Write(_setRequests.Dequeue());
                }
            }
        }

        private void InitializeNames() {
            /* Initializes NAMES of variables that model is GETTING FROM simulator */
            _valuesFromSimNames.Add("indicated-heading-deg");
            _valuesFromSimNames.Add("gps_indicated-vertical-speed");
            _valuesFromSimNames.Add("gps_indicated-ground-speed-kt");
            _valuesFromSimNames.Add("airspeed-indicator_indicated-speed-kt");
            _valuesFromSimNames.Add("gps_indicated-altitude-ft");
            _valuesFromSimNames.Add("attitude-indicator_internal-roll-deg");
            _valuesFromSimNames.Add("attitude-indicator_internal-pitch-deg");
            _valuesFromSimNames.Add("altimeter_indicated-altitude-ft");
            _valuesFromSimNames.Add("altimeter_indicated-altitude-ft");
        }
    }
}
