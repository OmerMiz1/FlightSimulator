using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows.Automation;
using FlightSimulatorApp.ViewModel;

namespace FlightSimulatorApp.Model {
    public class SimulatorModel : TcpClient, INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;
        public DictionaryIndexer Variables; /* Should be tested */
        private Queue<string> setRequests = new Queue<string>();
        private VariableNamesManager varNamesMgr = new VariableNamesManager();

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
                NotifyPropertyChanged("Connected");
            }
            catch (Exception e) {
                Debug.WriteLine("Error #1 SimulatorModel.Connect()..");
            }

            Debug.WriteLine("TCP Client: Connected successfully to server...");
            new Thread(Start).Start();
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
                //do {
                try {
                    bytesRead = stream.Read(readBuffer, 0, readBuffer.Length);
                    strBuilder.AppendFormat("{0}", Encoding.ASCII.GetString(readBuffer, 0, bytesRead));
                }
                catch (Exception e) { }
                //} while (stream.DataAvailable);

                return strBuilder.ToString();
            }
            else {
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
            else {
                Debug.WriteLine("Error #2 at Simulator.Write()...");
            }
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

            /* TODO JUST A TEST */ /*string longiPath = "/position/longitude-deg";
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
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(varNamesMgr.toName(propName)));
        }

        /* Personal Implementations */
        private void ReadValuesFromSim() {
            /* Build get requests message (once) */
            StringBuilder strBuilder = new StringBuilder();
            List<string> paths = new List<string>();
            string requestMsg, valuesFromSim;

            /*More info at: https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2.system-collections-idictionary-getenumerator?view=netframework-4.8 */
            var entry = Variables.GetEnumerator();
            while (entry.MoveNext()) {
                /*TODO: Possible problem, possible fix =  use AppendLine*/
                strBuilder.Append("get " + entry.Key + "\r\n");
                paths.Add((string) entry.Key);
            }

            requestMsg = strBuilder.ToString();

            /* Request values from simulator every 100ms */
            while (running) {
                /* TODO maybe need to show notification to user? */
                if (!myTcpClient.Connected) {
                    Debug.WriteLine("Error #1 SimulatorModel.Start()...");
                }

                /* Send request for updates & read response */
                Write(requestMsg);
                valuesFromSim = Read();

                /* Case response is valid */
                if (valuesFromSim != null) {
                    /* Enumerate each variable manually (iterator)*/
                    var pathEnum = paths.GetEnumerator();
                    pathEnum.MoveNext();

                    /* Split received values and update each one */
                    List<string> newValsArray = valuesFromSim.Split("\n").ToList();
                    foreach (string newVal in newValsArray) {
                        if (newVal != "ERR" && newVal != "" && Variables.ContainsKey(pathEnum.Current)) {
                            Variables[pathEnum.Current] = newVal;
                            NotifyPropertyChanged(pathEnum.Current);
                        }
                        else {
                            /*TODO Make sure we notify user (with GUI text box/smthing..) about an error!!!! */
                            Console.WriteLine("ERR");
                        }

                        pathEnum.MoveNext();
                    }

                    pathEnum.Dispose();
                }
                else {
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
                    /* Send requested value change and read respond afterwards */
                    string request = setRequests.Dequeue();
                    Write("set " + request);
                    string response = Read();

                    /* Check if request was valid. */
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
                ["/instrumentation/heading-indicator/indicated-heading-deg"] = "NO_VALUE_YET",
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

        public string GetVariable(string varName) {
            return Variables[varNamesMgr.toPath(varName)];
        }

        public void SetVariable(string varName, string varValue) {
            /*TODO Add checks for values out of their range + Exception !!! */
            string varPath = varNamesMgr.toPath(varName);
            double newValue = Convert.ToDouble(varValue);
            double curValue = Convert.ToDouble(Variables[varPath]);

            /*switch (varName) {
                case "Heading": {
                    if (curValue <  )
                }
                case "VerticalSpeed": {

                }
                case "GroundSpeed": {

                }
                case "Speed": {

                }
                case "AltitudeGps": {

                }
                case "Roll": {

                }
                case "Pitch": {

                }
                case "AltitudeAltimeter": {

                }
                case "Longitude": {
                }
                case "Latitude": { 
                }
                case "Altitude": { 
                }
            }*/

            setRequests.Enqueue(varPath + " " + varValue);
        }
    }
}