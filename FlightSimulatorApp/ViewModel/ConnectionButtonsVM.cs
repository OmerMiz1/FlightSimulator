using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using FlightSimulatorApp.Model;

namespace FlightSimulatorApp.ViewModel {
    public class ConnectionButtonsVM : INotifyPropertyChanged {
        private SimulatorModel _model;
        public event PropertyChangedEventHandler PropertyChanged;
        private string _ip;
        private int _port;
        private bool _isConnected = false;
        private bool _connectionFailed = false;


        public string Ip {
            get => _ip;
            set {
                if (_ip != value) {
                    _ip = value;
                    NotifyPropertyChanged("Ip");
                }
            }
        }
        public int Port {
            get => _port;
            set {
                if (_port != value) {
                    _port = value;
                    NotifyPropertyChanged("Port");
                }
            }
        }
        public bool IsConnected {
            get => _isConnected;
            set {
                if (_isConnected != value) {
                    _isConnected = value;
                    NotifyPropertyChanged("IsConnected");
                }
            }
        }

        public bool ConnectionFailed {
            get => _connectionFailed;
            set { _connectionFailed = value; }
        }

        public ConnectionButtonsVM(SimulatorModel model) {
            _model = model;
            model.ConnectionChanged += Model_ConnectionChanged;
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
            switch (e.PropertyName) {
                case "Connected": {
                    IsConnected = true;
                    _connectionFailed = false;
                    break;
                }
                case "Disconnected": {
                    IsConnected = false;
                    break;
                }
                case "Failed": {
                    IsConnected = false;
                    _connectionFailed = true;
                    break;
                }
                default:
                    break;
            }
        }
    }
}