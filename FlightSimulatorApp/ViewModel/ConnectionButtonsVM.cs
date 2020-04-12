using System;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using FlightSimulatorApp.Model;

namespace FlightSimulatorApp.ViewModel {
    public class ConnectionButtonsVM : INotifyPropertyChanged {
        private SimulatorModel _model;
        public event PropertyChangedEventHandler PropertyChanged;
        private string _ip;
        private int _port;
        private string _status = "Status: Disconnected";
        private Brush _statusColor = Brushes.Red;
        private bool _connectButtonEnabled = true;
        private bool _disconnectButtonEnabled = false;
        private bool _settingsButtonEnabled = true;

        public bool ConnectButtonEnabled {
            get { return _connectButtonEnabled; }
            set {
                _connectButtonEnabled = value;
                NotifyPropertyChanged("ConnectButtonEnabled");
            }
        }

        public bool DisconnectButtonEnabled {
            get { return _disconnectButtonEnabled; }
            set {
                _disconnectButtonEnabled = value;
                NotifyPropertyChanged("DisconnectButtonEnabled");
            }
        }

        public bool SettingsButtonEnabled {
            get { return _settingsButtonEnabled; }
            set {
                _settingsButtonEnabled = value;
                NotifyPropertyChanged("SettingsButtonEnabled");
            }
        }

        // private bool _isConnected = false;
        // private bool _connectionFailed = false;
        public string Status {
            get { return _status; }
            set {
                _status = value;
                NotifyPropertyChanged("Status");
            }
        }

        public Brush StatusColor {
            get { return _statusColor; }
            set {
                _statusColor = value;
                NotifyPropertyChanged("StatusColor");
            }
        }

        public string Ip {
            get => _ip;
            set {
                //If you remove this condition make sure it won't cause an infinite loop while initializing
                if (_ip != value) {
                    _ip = value;
                    NotifyPropertyChanged("Ip");
                    _model.Ip = value;
                }
            }
        }

        public int Port {
            get => _port;
            set {
                //If you remove this condition make sure it won't cause an infinite loop while initializing
                if (_port != value) {
                    _port = value;
                    NotifyPropertyChanged("Port");
                    _model.Port = value;
                }
            }
        }

        public ConnectionButtonsVM(SimulatorModel model) {
            _model = model;
            model.ConnectionChanged += Model_ConnectionChanged;
            Ip = model.Ip;
            Port = model.Port;
        }

        private void NotifyPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Connect() {
            _model.Connect();
        }

        public void Disconnect() {
            _model.Disconnect();
        }

        private void Model_ConnectionChanged(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == "Connected") {
                Status = "Status: Connected";
                StatusColor = Brushes.Green;
                ConnectButtonEnabled = false;
                DisconnectButtonEnabled = true;
                SettingsButtonEnabled = false;
            }
            else if (e.PropertyName == "Disconnected") {
                if (Status.Contains("Error") || Status.Contains("Warning")) {
                    Delay(5000).ContinueWith(_ => Status = "Status: Disconnected");
                }
                else {
                    Status = "Status: Disconnected";
                }
                StatusColor = Brushes.Red;
                ConnectButtonEnabled = true;
                DisconnectButtonEnabled = false;
                SettingsButtonEnabled = true;
            }
            else if (e.PropertyName.StartsWith("Error")) {
                Status = e.PropertyName;
                StatusColor = Brushes.Red;
                ConnectButtonEnabled = true;
                DisconnectButtonEnabled = false;
                SettingsButtonEnabled = true;
            }
            else if (e.PropertyName.StartsWith("Warning")) {
                Status = e.PropertyName;
                StatusColor = Brushes.Orange;
            }
            else {
                Status = e.PropertyName;
                StatusColor = Brushes.Orange;
                ConnectButtonEnabled = false;
                DisconnectButtonEnabled = false;
                SettingsButtonEnabled = false;
            }
        }

        /* Method taken from: https://stackoverflow.com/questions/15341962/how-to-put-a-task-to-sleep-or-delay-in-c-sharp-4-0/15342122#15342122 */
        static Task Delay(int milliseconds) {
            var tcs = new TaskCompletionSource<object>();
            new Timer(_ => tcs.SetResult(null)).Change(milliseconds, -1);
            return tcs.Task;
        }
    }
}