using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
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

        public bool ConnectButtonEnabled
        {
            get { return _connectButtonEnabled; }
            set { _connectButtonEnabled = value; NotifyPropertyChanged("ConnectButtonEnabled"); }
        }

        public bool DisconnectButtonEnabled {
            get { return _disconnectButtonEnabled; }
            set { _disconnectButtonEnabled = value; NotifyPropertyChanged("DisconnectButtonEnabled"); }
        }

        public bool SettingsButtonEnabled {
            get { return _settingsButtonEnabled; }
            set { _settingsButtonEnabled = value; NotifyPropertyChanged("SettingsButtonEnabled"); }
        }

        // private bool _isConnected = false;
        // private bool _connectionFailed = false;
        public string Status
        {
            get { return _status; }
            set { _status = value; NotifyPropertyChanged("Status"); }
        }

        public Brush StatusColor
        {
            get { return _statusColor; }
            set { _statusColor = value; NotifyPropertyChanged("StatusColor"); }
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
        // public bool IsConnected {
        //     get => _isConnected;
        //     set {
        //         if (_isConnected != value) {
        //             _isConnected = value;
        //             NotifyPropertyChanged("IsConnected");
        //         }
        //     }
        // }
        //
        // public bool ConnectionFailed {
        //     get => _connectionFailed;
        //     set { _connectionFailed = value; }
        // }

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

        // private void Model_ConnectionChanged(object sender, PropertyChangedEventArgs e) {
        //     switch (e.PropertyName) {
        //         case "Connected": {
        //             IsConnected = true;
        //             _connectionFailed = false;
        //             break;
        //         }
        //         case "Disconnected": {
        //             IsConnected = false;
        //             break;
        //         }
        //         case "Failed": {
        //             IsConnected = false;
        //             _connectionFailed = true;
        //             break;
        //         }
        //         default:
        //             break;
        //     }
        // }

        private void Model_ConnectionChanged(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == "Connected")
            {
                Status = "Status: Connected";
                StatusColor = Brushes.Green;
                ConnectButtonEnabled = false;
                DisconnectButtonEnabled = true;
                SettingsButtonEnabled = false;
            } else if (e.PropertyName == "Disconnected")
            {
                Status = "Status: Disconnected";
                StatusColor = Brushes.Red;
                ConnectButtonEnabled = true;
                DisconnectButtonEnabled = false;
                SettingsButtonEnabled = true;
            } else if (e.PropertyName.StartsWith("Error"))
            {
                Status = e.PropertyName;
                StatusColor = Brushes.Red;
                ConnectButtonEnabled = false;
                DisconnectButtonEnabled = true;
                SettingsButtonEnabled = false;
            } else if (e.PropertyName.StartsWith("Warning"))
            {
                Status = e.PropertyName;
                StatusColor = Brushes.Orange;
            }
            else
            {
                Status = e.PropertyName;
                StatusColor = Brushes.Orange;
                ConnectButtonEnabled = false;
                DisconnectButtonEnabled = false;
                SettingsButtonEnabled = false;
            }
        }
    }
}