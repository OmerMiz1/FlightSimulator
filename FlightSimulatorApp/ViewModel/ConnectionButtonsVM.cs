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
        // private bool _isConnected = false;
        // private bool _connectionFailed = false;
        public string Status { get; set; } = "Status: Disconnected";
        public Brush StatusColor { get; set; } = Brushes.Red;

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
            switch (e.PropertyName) {
                case "Connected":
                {
                    Status = "Status: Connected";
                    StatusColor = Brushes.Green;
                    break;
                }
                case "Disconnected": {
                    Status = "Status: Disconnected";
                    StatusColor = Brushes.Red;
                    break;
                }
                default: {
                    Status = "Status: " + e.PropertyName;
                    StatusColor = Brushes.Yellow;
                    break;
                }
            }
        }
    }
}