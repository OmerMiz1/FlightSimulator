using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace FlightSimulatorApp.Model {
    public class SimulatorModel : TcpClient, INotifyPropertyChanged {
        /* Server related fields */
        private TcpClient _tcpClient = new TcpClient();
        private NetworkStream _stream;
        private static readonly Mutex mtx = new Mutex();
        private Thread _getValuesThread;
        private Thread _setValuesThread;
        private bool _running;
        private static string ConnectionError { get; } = "CONNECTION_ERROR";

        /* Variables related fields */
        private Dictionary<string, string> _variables;
        private Queue<string> _setRequests = new Queue<string>();
        private VariableNamesManager _varNamesMgr = new VariableNamesManager();

        public string Ip { get; set; }
        public int Port { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public event PropertyChangedEventHandler StatusChanged;

        public SimulatorModel() {
            InitVariables();
        }

        ~SimulatorModel() {
            try {
                Disconnect();
            }
            catch (Exception) {
                // DO NOTHING
            }
        }

        /*** Connect to server
         * Method connects the model to the server using the ip and port configured
         *from outside (different source, not self!) */
        public void Connect() {
            try {
                /* Connect to server */
                _tcpClient = new TcpClient(Ip, Port);
                NotifyStatusChanged("Connected");
                //Debug.WriteLine("TCP Client: Connected successfully to server...");
            }
            catch (SocketException) {
                /* Usually this error points that server is not on yet */
                NotifyStatusChanged(
                    "Error Connection Failed:\n 1. Try to turn on the server\n 2. Click on 'Connect'");
                return;
            }
            catch (Exception) {
                /* Unexpected error */
                NotifyStatusChanged("Error Connection Failed:\n Unexpected error occured");
                return;
            }

            /* Check connection & Start communicating. NOTE:
               Named the thread for easier debugging */
            if (!_tcpClient.Connected) {
                return;
            }

            var t = new Thread(Start);
            t.Name = "SimulatorModel.Start Thread";
            t.Start();
        }

        /*** Disconnect from server.
         * Disconnects from server and signals threads its time to stop.
         */
        public void Disconnect() {
            /* Signal threads to stop */
            Stop();

            /* Wait for threads to finish before closing connection
               Conditions are to avoid starvation */
            if (Thread.CurrentThread.Name != _getValuesThread.Name) {
                _getValuesThread.Join();
            }
            if (Thread.CurrentThread.Name != _setValuesThread.Name) {
                _setValuesThread.Join();
            }

            try {
                _tcpClient.Close();
                NotifyStatusChanged("Disconnected");
            }
            catch (Exception e) {
                // DO NOTHING - just in-case TCP Client has already been disposed.
            }
            //Debug.WriteLine("TCP Client: Disconnected successfully from server...");
        }

        /*** Reads data from server. IMPORTANT NOTES BELOW!
         * Reads until there is no more data available to read.
         * IMPORTANT NOTE: Method can call Disconnect method, therefor after every use it is
         * important to check for ConnectionError output and if so, abort any actions with
         * server
         */
        public string Read() {
            var readBuffer = new byte[4096];
            var strBuilder = new StringBuilder();

            /** Read all data sent from simulator
                 * NOTE: Probably there's no need in the string builder
                 * because the messaged received from sim are short.
                 * Still i think its a good idea just in-case.
                 **/
            do {
                try {
                    var bytesRead = _stream.Read(readBuffer, 0, readBuffer.Length);
                    strBuilder.AppendFormat("{0}", Encoding.ASCII.GetString(readBuffer, 0, bytesRead));
                    //} catch (ArgumentNullException) {
                    //    Debug.WriteLine("Argument Null Exception thrown at Read()");
                    //} catch (ObjectDisposedException) {
                    //    Debug.WriteLine("Object Disposed Exception thrown at Read()");
                    //} catch (InvalidOperationException) {
                    //    Debug.WriteLine("Invalid Operation Exception thrown at Read()");
                    //} catch (IOException e) {
                    //    Debug.WriteLine("I/O Exception thrown at Read()");
                    //    Debug.WriteLine("Inner Exception:\n" + e.InnerException);
                }
                catch (Exception) {
                    /* Connection error */
                    NotifyStatusChanged("Error Failed to read from server\nConnection to server lost\\broken");
                    Disconnect();
                    return ConnectionError;
                }
            } while (_stream.DataAvailable);

            return strBuilder.ToString();
        }

        /*** Write data to server
         * Method adds \r\n to the end of the message.
         * Data sent to server is supposed to be in the format of:
         * {get\set} + path-to-variable + value (value only if set is sent).
         */
        public void Write(string msg) {
            if (_tcpClient.Connected)
                try {
                    var writeBuffer = Encoding.ASCII.GetBytes(msg + "\r\n");
                    _stream.Write(writeBuffer, 0, writeBuffer.Length);
                }
                catch (Exception) {
                    /* Error occured - might be server shut down unexpectedly */
                    NotifyStatusChanged("Error Failed writing to server\nConnection to server lost\\broken");
                }
        }

        /*** Starts sending\receiving data to\from simulator.
         * Method start up the threads to work in the background.
         * Thread1: Requests data from simulator every 0.1 seconds (100ms).
         * Thread2: Sending set requests from user, like when user is moving the joystick controller.
         */
        public void Start() {
            /* Clear any requests sent before starting this server*/
            _running = true;
            _setRequests.Clear();

            /* Create stream & set timeout to 10 seconds */
            _stream = _tcpClient.GetStream();
            _stream.ReadTimeout = 10;

            /* Start getting data from simulator (sample data continuously) */
            _getValuesThread = new Thread(ReadValuesFromSim);
            _getValuesThread.Name = "Get Values Thread";
            _getValuesThread.Start();

            /* Start responding to set requests from user input (GUI actually) */
            _setValuesThread = new Thread(WriteValuesToSim);
            _setValuesThread.Name = "Set Values Thread";
            _setValuesThread.Start();
        }

        public void Stop() {
            _running = false;
        }

        public void NotifyPropertyChanged(string propName) {
            /* Double check that the variable exists to avoid crashing */
            if (_varNamesMgr.toName(propName) != VariableNamesManager.VariableNotFound)
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(_varNamesMgr.toName(propName)));
        }

        /*** Model notifies about connection's status
         * Such as - Connected, Disconnect or error connecting because this and that.
         */
        public void NotifyStatusChanged(string connected) {
            StatusChanged?.Invoke(this, new PropertyChangedEventArgs(connected));
        }

        /*** Read data from the simulator every 0.1 seconds.
         * 
         */
        private void ReadValuesFromSim() {
            /* Declarations */
            var strBuilder = new StringBuilder();
            var paths = new List<string>();

            /*** Build get requests message (once). More info at:
             *  https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2.system-collections-idictionary-getenumerator?view=netframework-4.8
             */
            IDictionaryEnumerator entry = _variables.GetEnumerator();
            while (entry.MoveNext()) {
                strBuilder.Append("get " + entry.Key + "\r\n");
                paths.Add((string) entry.Key);
            }

            var requestMsg = strBuilder.ToString();

            /* Request values from simulator every 100ms */
            while (_running) {
                if (!_tcpClient.Connected) {
                    break;
                }

                /* Send request for updates & read response */
                mtx.WaitOne();
                Write(requestMsg);
                var valuesFromSimRaw = Read();
                mtx.ReleaseMutex();

                /* No connection, see 'Read' comment for more info */
                if (valuesFromSimRaw == ConnectionError) {
                    break;
                }

                /* Iterate paths list manually */
                var pathEnum = paths.GetEnumerator();
                pathEnum.MoveNext();

                /* Split received values and update each one */
                List<string> valuesFromSim = valuesFromSimRaw.Split("\n").ToList();
                valuesFromSim.RemoveAll(string.IsNullOrEmpty);

                /* Minor message validity test. If condition is not met:
                 data might be corrupted so just skip
                 this sample. */
                if (valuesFromSim.Count == paths.Count)
                    foreach (var newVal in valuesFromSim) {
                        if (_variables.ContainsKey(pathEnum.Current) &&
                            _variables[pathEnum.Current] != VariableNamesManager.VariableNotFound) {
                            _variables[pathEnum.Current] = newVal;
                            NotifyPropertyChanged((pathEnum.Current));
                        }
                        pathEnum.MoveNext();
                    }

                pathEnum.Dispose();
                Thread.Sleep(100);
            }
        }

        private void WriteValuesToSim() {
            while (_running)
                if (_setRequests.Count != 0) {
                    /*Send requested and read respond to clear server's output buffer.
                      there is no actual use in what is read, so for now just read*/
                    var request = _setRequests.Dequeue();
                    mtx.WaitOne();
                    Write("set " + request);
                    //ClearServerOutputBuffer();
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

        private void InitVariables() {
            /* Initializes NAMES of variables that model is GETTING FROM simulator */
            _variables = new Dictionary<string,string> {
                ["/instrumentation/heading-indicator/indicated-heading-deg"] = "NO_VALUE_YET",
                ["/instrumentation/gps/indicated-vertical-speed"] = "NO_VALUE_YET",
                ["/instrumentation/gps/indicated-ground-speed-kt"] = "NO_VALUE_YET",
                ["/instrumentation/airspeed-indicator/indicated-speed-kt"] = "NO_VALUE_YET",
                ["/instrumentation/gps/indicated-altitude-ft"] = "NO_VALUE_YET",
                ["/instrumentation/attitude-indicator/internal-roll-deg"] = "NO_VALUE_YET",
                ["/instrumentation/attitude-indicator/internal-pitch-deg"] = "NO_VALUE_YET",
                ["/instrumentation/altimeter/indicated-altitude-ft"] = "NO_VALUE_YET",
                ["/position/longitude-deg"] = "NO_VALUE_YET",
                ["/position/latitude-deg"] = "NO_VALUE_YET"
            };
        }

        public string GetVariable(string varName) {
            string path = _varNamesMgr.toPath(varName);
            if (_variables.ContainsKey(path)) 
                return _variables[path];
            return "ERR";
        }

        public void SetVariable(string varName, string varValue) {
            if (_setRequests.Count() > 8) _setRequests.Dequeue();
            var varPath = _varNamesMgr.toPath(varName);
            _setRequests.Enqueue(varPath + " " + varValue);
            /*if (setValuesThread.ThreadState != ThreadState.Running) {
                setValuesThread.Interrupt();
            }*/
        }

        private void ClearServerOutputBuffer() {
            /* Clear server buffer and get rid of data not related to this query */
            var buffer = new byte[4096];
            while (_stream.DataAvailable) {
                _stream.Read(buffer, 0, buffer.Length);
            }
        }
    }
}