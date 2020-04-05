using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows.Automation;

namespace FlightSimulatorApp.Model {
    public class SimulatorModel : TcpClient, INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;

        // private Dictionary<string, string> Variables;
        public DictionaryIndexer Variables; /* Should be tested */
        
        public List<string> VariablesOut; /* Variables we send updates TO the simulator*/
        public List<string> VariablesIn; /* Variables we update FROM the simulator*/

        /*/* Properties FROM server #1#
        public String GpsVerticalSpeed { get; set; }
        public String HeadingDegree { get; set; }
        public String GpsGroundSpeed { get; set; }
        public String AirSpeedIndicator { get; set; }
        public String AltitudeGps { get; set; }
        public String AttitudeIndicatoInternalRollDeg { get; set; }
        public String AttitudeIndicatorInternalPitchDeg { get; set; }
        public String AltimeterIndicatedAltitudeFt { get; set; }

        /* Properties TO server#1#
        public String Rudder { get; set; }
        public String Elevators { get; set; }
        public String Ailerons { get; set; }
        public String Throttle { get; set; }*/

        private Queue<string> _setRequests;

        private TcpClient _tcpClient;
        private NetworkStream _stream;
        private Boolean _isRunning;
        private Boolean _debug;

        public SimulatorModel(TcpClient tcpClient) : this(tcpClient, false) { }

        public SimulatorModel(TcpClient tcpClient, Boolean mode) {
            InitVariables();
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
                byte[] writeBuffer = Encoding.ASCII.GetBytes(msg + "\r\n");
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
                Console.WriteLine("Error #2 at Simulator.Read()...");
                throw new System.InvalidOperationException("Error #2 at Simulator.Read()...");
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

        public void NotifyPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        private void ReadValuesFromSim() {
            /* Build get requests message (once) */
            StringBuilder strBuilder = new StringBuilder();
            foreach (string varName in VariablesIn) {
                /*TODO: Possible problem, possible fix =  use AppendLine*/
                strBuilder.Append("get " + varName);
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
                    var varEnum = VariablesIn.GetEnumerator();

                    /* Split received values and update each one */
                    string[] newValsArray = valuesFromSim.Split("\r\n");
                    foreach (string newVal in newValsArray) {
                        Variables[varEnum.Current] = newVal;
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
                    /* Send requested value change */
                    string request = _setRequests.Dequeue();
                    Write("set " + request);

                    /* Simulator always responds with the new value, check if request was valid.*/
                    string response = Read();
                    if (response != request) {
                        /*TODO debug*/
                        Console.WriteLine("Err #1 WriteValuesToSim(): Requested set request is invalid...");
                    }
                }
            }
        }

        private void InitVariables() {
            /* Initializes NAMES of variables that model is GETTING FROM simulator */
            VariablesIn.Add("/orientation/heading-deg");
            VariablesIn.Add("/instrumentation/gps/indicated-vertical-speed");
            VariablesIn.Add("/instrumentation/gps/indicated-ground-speed-kt");
            VariablesIn.Add("/instrumentation/airspeed-indicator/indicated-speed-kt");
            VariablesIn.Add("/instrumentation/gps/indicated-altitude-ft");
            VariablesIn.Add("/instrumentation/attitude-indicator/internal-roll-deg");
            VariablesIn.Add("/instrumentation/attitude-indicator/internal-pitch-deg");
            VariablesIn.Add("/instrumentation/altimeter/indicated-altitude-ft");
            
            /* Optional (to test) */
            VariablesIn.Add("/position/longitude-deg");
            VariablesIn.Add("/position/latitude-deg");

            Variables = new DictionaryIndexer();
            foreach (string var in VariablesIn) {
                Variables[var] = "NO_VALUE_YET";
            }

            
        }
        /*public Dictionary<string, string> NameToPath() {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict["HeadingDegree"] = "indicated-heading-deg";
            dict["GpsVerticalSpeed"] = "gps_indicated-vertical-speed";
            dict["GpsGroundSpeed"] = "gps_indicated-ground-speed-kt";
            dict["AirSpeedIndicator"] = "airspeed-indicator_indicated-speed-kt";
            dict["GpsAltitudeFt"] = "gps_indicated-altitude-ft";
            dict["RollDegree"] = "attitude-indicator_internal-roll-deg";
            dict["PitchDegree"] = "attitude-indicator_internal-pitch-deg";
            dict["AltimeterAltitudeFt"] = "altimeter_indicated-altitude-ft";
            return dict;
        }

        public Dictionary<string, string> PathToName() {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict["indicated-heading-deg"] = "HeadingDegree";
            dict["GpsVerticalSpeed"] = "gps_indicated-vertical-speed";
            dict["GpsGroundSpeed"] = "gps_indicated-ground-speed-kt";
            dict["AirSpeedIndicator"] = "airspeed-indicator_indicated-speed-kt";
            dict["GpsAltitudeFt"] = "gps_indicated-altitude-ft";
            dict["RollDegree"] = "attitude-indicator_internal-roll-deg";
            dict["PitchDegree"] = "attitude-indicator_internal-pitch-deg";
            dict["AltimeterAltitudeFt"] = "altimeter_indicated-altitude-ft";
            return dict;
        }*/
    }
}
