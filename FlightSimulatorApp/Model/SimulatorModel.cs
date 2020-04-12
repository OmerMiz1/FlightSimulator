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
using System.Windows;
using System.Windows.Automation;
using FlightSimulatorApp.ViewModel;
using ThreadState = System.Threading.ThreadState;

namespace FlightSimulatorApp.Model {
    public class SimulatorModel : TcpClient, INotifyPropertyChanged {
        /* Server related fields */
        private TcpClient _tcpClient = new TcpClient();
        private NetworkStream _stream;
        private Boolean _running = false;
        public string Ip { get; set; }
        public int Port { get; set; }

        public Boolean Connected {
            get => _tcpClient.Connected;
        }

        private Thread getValuesThread;
        private Thread setValuesThread;
        private static Mutex mtx = new Mutex();

        /* Variables related fields */
        private DictionaryIndexer _variables = new DictionaryIndexer();
        private DictionaryIndexer _setRequestsDic = new DictionaryIndexer();
        private Queue<string> _setRequests = new Queue<string>();
        private VariableNamesManager _varNamesMgr = new VariableNamesManager();

        public event PropertyChangedEventHandler PropertyChanged;
        public event PropertyChangedEventHandler ConnectionChanged;

        public SimulatorModel() {
            InitVariables();
        }

        public void Connect() {
            try { /* Connect to server */
                _tcpClient = new TcpClient(Ip, Port);
                NotifyConnectionChanged("Connected");
                Debug.WriteLine("TCP Client: Connected successfully to server...");
            }
            catch (SocketException se) { /* Usually this error points that server is not on yet */
                NotifyConnectionChanged("Error Connection Failed:\n 1. Try to turn on the server\n 2. Click on 'Connect'");
                return;
            }
            catch (Exception e) { /* Unexpected error */
                NotifyConnectionChanged("Error Connection Failed:\n Unexpected error occured");
                return;
            }

            /** Check connection & Start communicating. NOTE:
               Named the thread for easier debugging */
            if (_tcpClient.Connected) {
                Thread t = new Thread(Start);
                t.Name = "SimulatorModel.Start Thread";
                t.Start();
            }
        }

        public void Disconnect() {
            _tcpClient.Close();
            _setRequests.Clear();
            NotifyConnectionChanged("Disconnected");
            Debug.WriteLine("TCP Client: Disconnected successfully to server...");
        }

        public string Read() {
            /* Double check reading is possible */
            if (_tcpClient.Connected) {
                byte[] readBuffer = new byte[4096];
                int bytesRead = 0;
                StringBuilder strBuilder = new StringBuilder();

                /** Read all data sent from simulator
                 * NOTE: Probably there's no need in the string builder
                 * because the messaged received from sim are short.
                 * Still i think its a good idea just in-case.
                 **/
                do {
                    try {
                        bytesRead = _stream.Read(readBuffer, 0, readBuffer.Length);
                        strBuilder.AppendFormat("{0}", Encoding.ASCII.GetString(readBuffer, 0, bytesRead));
                    }
                    catch (ArgumentNullException e) {
                        Debug.WriteLine("Argument Null Exception thrown at Read()");
                    }
                    catch (ObjectDisposedException e) {
                        Debug.WriteLine("Object Disposed Exception thrown at Read()");
                    }
                    catch (InvalidOperationException e) {
                        Debug.WriteLine("Invalid Operation Exception thrown at Read()");
                    }
                    catch (IOException e) {
                        Debug.WriteLine("I/O Exception thrown at Read()");
                        Debug.WriteLine("Inner Exception:\n" + e.InnerException);
                    }
                    catch (Exception e) { /* Error occured - might be server shut down unexpectedly */
                        if (!_tcpClient.Connected) {
                            NotifyConnectionChanged("Error (Reading): Connection to server lost\\broken");
                        }
                        /**Debug.WriteLine("<<<<<<<<<<<<<< ERROR REASON <<<<<<<<<<<<<<");
                        Debug.WriteLine("Stack Trace: \n"+e.StackTrace);
                        Debug.WriteLine("Message: \n"+e.Message);
                        Debug.WriteLine("Data: \n"+e.Data);
                        Debug.WriteLine("StringBuilder Data: \n"+strBuilder);
                        Debug.WriteLine(">>>>>>>>>>>>>> ERROR REASON >>>>>>>>>>>>>>");
                        Debug.WriteLine("");*/
                        Stop();
                    }
                } while (_stream.DataAvailable);
                return strBuilder.ToString();
            }
            return "";
        }

        public void Write(string msg) {
            /* Clear server buffer and get rid of data not related to this query */
            /*var buffer = new byte[4096];
            while (_stream.DataAvailable) {
                _stream.Read(buffer, 0, buffer.Length);
            }*/
            /*if (_stream.CanRead && _stream.DataAvailable) {
                Read();
            }*/
            if (_tcpClient.Connected) {
                try {
                    byte[] writeBuffer = Encoding.ASCII.GetBytes(msg + "\r\n");
                    _stream.Write(writeBuffer, 0, writeBuffer.Length);
                } catch (Exception e) { /* Error occured - might be server shut down unexpectedly */
                    NotifyConnectionChanged("Error (Writing): Connection to server lost\\broken");
                    Stop();
                }
            }
            
        }

        public void Start() {
            /* Clear any requests sent before starting this server*/
            _running = true;
            _setRequests.Clear();

            /* Create stream & set timeout to 10 seconds */
            _stream = _tcpClient.GetStream();
            _stream.ReadTimeout = 10;

            /* Start getting data from simulator (continuously) */
            getValuesThread = new Thread(ReadValuesFromSim);
            getValuesThread.Name = "Get Values Thread";
            getValuesThread.Start();

            /* Start responding to set requests from user input (GUI actually) */
            setValuesThread = new Thread(WriteValuesToSim);
            setValuesThread.Name = "Set Values Thread";
            setValuesThread.Start();

            getValuesThread.Join();
            setValuesThread.Join();

            Disconnect();
        }

        public void Stop() {
            _running = false;
        }

        public void NotifyPropertyChanged(string propName) {
            if (_varNamesMgr.toName(propName) != "Variable Not Found") {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(_varNamesMgr.toName(propName)));
            }
            else {
                /*TODO WHAT SHOULD WE PRINT IF HAPPENS?*/
                Debug.WriteLine("Some Weird BUG", Thread.CurrentThread.Name);
            }
        }

        public void NotifyConnectionChanged(string connected) {
            ConnectionChanged?.Invoke(this, new PropertyChangedEventArgs(connected));
        }

        /* Personal Implementations */
        private void ReadValuesFromSim() {
            StringBuilder strBuilder = new StringBuilder();
            List<string> paths = new List<string>();
            List<string>.Enumerator pathEnum;
            string requestMsg, valuesFromSimRaw;

            /** Build get requests message (once). More info at:
             *  https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2.system-collections-idictionary-getenumerator?view=netframework-4.8
             */
            var entry = _variables.GetEnumerator();
            while (entry.MoveNext()) {
                /*TODO: Possible problem, possible fix =  use AppendLine*/
                strBuilder.Append("get " + entry.Key + "\r\n");
                paths.Add((string) entry.Key);
            }

            requestMsg = strBuilder.ToString();
            Debug.WriteLine("Request message= " + requestMsg); /*TODO DEBUG*/

            /* Request values from simulator every 100ms */
            while (_running) {
                /* TODO maybe need to show notification to user? */
                if (!_tcpClient.Connected) {
                    Debug.WriteLine("Error #1 SimulatorModel.Start()...");
                }

                /* Send request for updates & read response */
                mtx.WaitOne();
                Write(requestMsg);
                valuesFromSimRaw = Read();
                mtx.ReleaseMutex();

                /* Enumerate each variable manually (iterator)*/
                pathEnum = paths.GetEnumerator();
                pathEnum.MoveNext();

                /* Split received values and update each one */
                List<string> valuesFromSim = valuesFromSimRaw.Split("\n").ToList();
                valuesFromSim.RemoveAll(string.IsNullOrEmpty);
                foreach (var v in valuesFromSim) {
                    Debug.WriteLine(v);
                }

                Debug.WriteLine("---------------");
                /*TODO DEBUG START*/ /**
                Debug.WriteLine("paths.Count = " + paths.Count + " ---  valuesFromSim.Count = " + valuesFromSim.Count);
                var pathsEnum = paths.GetEnumerator();
                var valuesFromSimEnum = valuesFromSim.GetEnumerator();
                int i = 1;
                bool b1 = pathsEnum.MoveNext();
                bool b2 = valuesFromSimEnum.MoveNext();
                while (b1 & b2) {
                    Debug.WriteLine( "#"+i+" path=" + pathsEnum.Current + " --- value="+valuesFromSimEnum.Current);
                    b1 = pathsEnum.MoveNext();
                    b2 = valuesFromSimEnum.MoveNext();
                    ++i;
                }

                while (b1) {
                    Debug.WriteLine("@" + i + " path=" + pathsEnum.Current + " --- value=END");
                    b1 = pathsEnum.MoveNext();
                }

                while (b2) {
                    Debug.WriteLine("@" + i + " path=END" + " --- value="+valuesFromSimEnum.Current);
                    b2 = valuesFromSimEnum.MoveNext();
                }
                Debug.WriteLine("");
                */ /*TODO DEBUG END*/

                if (valuesFromSim.Count == paths.Count) {
                    foreach (string newVal in valuesFromSim) {
                        if (newVal == "ERR") {
                            /*TODO notify user about error for 2 ~ 5 seconds and make it disappear. */
                        }
                        else if (newVal == "") {
                            // DO NOTHING
                        }
                        else if (_variables.ContainsKey(pathEnum.Current)) {
                            _variables[pathEnum.Current] = newVal;
                            NotifyPropertyChanged(pathEnum.Current);
                        }
                        else {
                            /*TODO Make sure we notify user (with GUI text box/smthing..) about an error!!!! */
                            Console.WriteLine("ERR");
                        }

                        if (!pathEnum.MoveNext()) {
                            break;
                        }
                    }
                }

                pathEnum.Dispose();
                Thread.Sleep(100);
            }
        }

        private void WriteValuesToSim() {
            while (_running) {

                if (_setRequests.Count != 0) {
                    /*Send requested value change and read respond afterwards
                     there is no actual use in what is read, so for now just read*/
                    string request = _setRequests.Dequeue();
                    mtx.WaitOne();
                    Write("set " + request);
                    Read();
                    mtx.ReleaseMutex();
                }
                /**try {
                    Thread.Sleep(100);
                }
                catch (ThreadInterruptedException tie) {
                    //DO NOTHING JUST CONTINUE
                }*/
            }
        }

        private void InitVariables() {
            /* Initializes NAMES of variables that model is GETTING FROM simulator */
            _variables = new DictionaryIndexer {
                ["/instrumentation/heading-indicator/indicated-heading-deg"] = "NO_VALUE_YET",
                ["/instrumentation/gps/indicated-vertical-speed"] = "NO_VALUE_YET",
                ["/instrumentation/gps/indicated-ground-speed-kt"] = "NO_VALUE_YET",
                ["/instrumentation/airspeed-indicator/indicated-speed-kt"] = "NO_VALUE_YET",
                ["/instrumentation/gps/indicated-altitude-ft"] = "NO_VALUE_YET",
                ["/instrumentation/attitude-indicator/internal-roll-deg"] = "NO_VALUE_YET",
                ["/instrumentation/attitude-indicator/internal-pitch-deg"] = "NO_VALUE_YET",
                ["/instrumentation/altimeter/indicated-altitude-ft"] = "NO_VALUE_YET",
                ["/position/longitude-deg"] = "NO_VALUE_YET",
                ["/position/latitude-deg"] = "NO_VALUE_YET",
            };
        }

        public string GetVariable(string varName) {
            return _variables[_varNamesMgr.toPath(varName)];
        }

        public void SetVariable(string varName, string varValue) {
            if (_setRequests.Count() > 8) {
                _setRequests.Dequeue();
            }
            string varPath = _varNamesMgr.toPath(varName);
            _setRequests.Enqueue(varPath + " " + varValue);
            /**if (setValuesThread.ThreadState != ThreadState.Running) {
                setValuesThread.Interrupt();
            }*/
        }
    }
}