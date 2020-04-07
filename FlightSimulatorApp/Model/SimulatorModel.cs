using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
        public DictionaryIndexer Variables; /* Should be tested */
        private Queue<string> setRequests = new Queue<string>();

        private TcpClient myTcpClient = new TcpClient();
        private NetworkStream stream;
        private Boolean running;

        public SimulatorModel() {
            InitVariables();
        }

        /* General Implementations */
        public void Connect(string ip, int port) {
            /* Connect to server */
            try {
                myTcpClient = new TcpClient(ip, port);

            } catch (Exception e) {
                Debug.WriteLine("Error #1 SimulatorModel.Connect()..");
            }
            Debug.WriteLine("TCP Client: Connected successfully to server...");
        }
        public void Disconnect() {
            myTcpClient.GetStream().Close();
            myTcpClient.Close();
            Debug.WriteLine("TCP Client: Disconnected successfully to server...");
        }

        public string Read() {
            /* Double check reading is possible */
            if (stream.CanRead) {
                byte[] readBuffer = new byte[4096];
                int bytesRead = 0;
                StringBuilder strBuilder = new StringBuilder();

                /*** Read all data sent from simulator
                 * NOTE: Probably there's no need in the string builder
                 * because the messaged received from sim are short.
                 * Still i think its a good idea just in-case.
                 ***/
                do {
                    bytesRead = stream.Read(readBuffer, 0, readBuffer.Length);
                    strBuilder.AppendFormat("{0}", Encoding.ASCII.GetString(readBuffer, 0, bytesRead));
                } while (stream.DataAvailable);

                return strBuilder.ToString();
            } else {
                Debug.WriteLine("Error #1 at SimulatorModel.Read()...");
            }



            /* Error */
            /*TODO keep the exception or simple console message?*/
            Debug.WriteLine("Error #2 at Simulator.Read()...");
            throw new System.InvalidOperationException("Error #2 at Simulator.Read()...");
        }
        public void Write(string msg) {
            /* Double check writing is possible */
            if (stream.CanWrite) {
                byte[] writeBuffer = Encoding.ASCII.GetBytes(msg + "\r\n");
                stream.Write(writeBuffer, 0, writeBuffer.Length);
            }
            Debug.WriteLine("Error #2 at Simulator.Write()...");
        }

        public void Start() {
            running = true;

            /* Create stream & set timeout to 10 seconds */
            stream = myTcpClient.GetStream();
            stream.ReadTimeout = 10;

            /* Start getting data from simulator (continuously) */
            Thread getValuesThread = new Thread(ReadValuesFromSim);
            getValuesThread.Start();

            /* Start responding to set requests from user input (GUI actually) */
            Thread setValuesThread = new Thread(WriteValuesToSim);
            setValuesThread.Start();

            /* TODO JUST A TEST */
            /*string longiPath = "/position/longitude-deg";
            Thread t = new Thread(() =>
            {
                for (double longi = 0; true; longi++)
                {
                    if (longi > 40)
                    {
                        longi = 0;
                    }

                    Debug.WriteLine("Inside SimulatorModel.Start, longi=" + longi + "now...");
                    Variables[longiPath] = longi.ToString();
                    NotifyPropertyChanged(longiPath);
                    Thread.Sleep(50);
                }
            });
            t.Start();*/

            getValuesThread.Join();
            setValuesThread.Join();

            Disconnect();
        }
        public void Stop() {
            running = false;
        }

        public void NotifyPropertyChanged(string propName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        /* Personal Implementations */
        private void ReadValuesFromSim() {
            /* Build get requests message (once) */
            StringBuilder strBuilder = new StringBuilder();
            foreach (var varName in Variables) {
                /*TODO: Possible problem, possible fix =  use AppendLine*/
                strBuilder.Append("get " + varName);
            }
            string requestMsg = strBuilder.ToString();

            /* Request values from simulator every 100ms */
            while (running) {
                /* TODO maybe need to show notification to user? */
                if (!myTcpClient.Connected) {
                    Console.WriteLine("Error #1 SimulatorModel.Start()...");
                }

                /* Send request for updates & read response */
                Write(requestMsg);
                string valuesFromSim = Read();

                /* Case response is valid */
                if (valuesFromSim != null) {
                    /* Enumerate each variable manually (iterator)*/
                    var varEnum = Variables.GetEnumerator();

                    /* Split received values and update each one */
                    string[] newValsArray = valuesFromSim.Split("\r\n");
                    foreach (string newVal in newValsArray) {
                        Variables[varEnum.Key.ToString()] = newVal;
                        NotifyPropertyChanged(varEnum.Key.ToString());
                    }

                } else {
                    Debug.WriteLine("Error #2 SimulatorModel.Start()...");
                }
                Thread.Sleep(100);
            }
        }
        /**
         *TODO Its not supposed to be like that.
         *TODO Each property update should be triggered from VM and updated as event.
         **/
        private void WriteValuesToSim() {
            while (running) {
                if (setRequests.Count != 0) {
                    /* Send requested value change */
                    string request = setRequests.Dequeue();
                    Write("set " + request);

                    /* Simulator always responds with the new value, check if request was valid.*/
                    string response = Read();
                    if (response != request) {
                        /*TODO debug*/
                        Debug.WriteLine("Err #1 WriteValuesToSim(): Requested set request is invalid...");
                    }
                }
            }
        }

        private void InitVariables() {
            /* Initializes NAMES of variables that model is GETTING FROM simulator */
            Variables = new DictionaryIndexer {
                ["/orientation/heading-deg"] = "NO_VALUE_YET",
                ["/instrumentation/gps/indicated-vertical-speed"] = "NO_VALUE_YET",
                ["/instrumentation/gps/indicated-ground-speed-kt"] = "NO_VALUE_YET",
                ["/instrumentation/airspeed-indicator/indicated-speed-kt"] = "NO_VALUE_YET",
                ["/instrumentation/gps/indicated-altitude-ft"] = "NO_VALUE_YET",
                ["/instrumentation/attitude-indicator/internal-roll-deg"] = "NO_VALUE_YET",
                ["/instrumentation/attitude-indicator/internal-pitch-deg"] = "NO_VALUE_YET",
                ["/instrumentation/altimeter/indicated-altitude-ft"] = "NO_VALUE_YET",

                /* Optional (to test) */
                ["/position/longitude-deg"] = "NO_VALUE_YET",
                ["/position/latitude-deg"] = "NO_VALUE_YET",
                ["/position/altitude-ft"] = "NO_VALUE_YET"
            };
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
