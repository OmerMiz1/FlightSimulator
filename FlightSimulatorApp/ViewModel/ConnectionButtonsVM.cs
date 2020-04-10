using System.ComponentModel;
using System.Runtime.CompilerServices;
using FlightSimulatorApp.Model;

namespace FlightSimulatorApp.ViewModel {
    public class ConnectionButtonsVM : INotifyPropertyChanged {
        private SimulatorModel _model;
        public event PropertyChangedEventHandler PropertyChanged;
        private string _ip;
        private int _port;
        private bool _isConnected;

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

        public ConnectionButtonsVM(SimulatorModel model) {
            _model = model;
            model.PropertyChanged += Model_PropertyChanged;
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

        private void Model_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            switch (e.PropertyName) {
                case "Connected": {
                    IsConnected = true;
                    break;
                }
                case "Disconnected": {
                    IsConnected = false;
                    break;
                }
                default:
                    break;
            }
        }
    }
}