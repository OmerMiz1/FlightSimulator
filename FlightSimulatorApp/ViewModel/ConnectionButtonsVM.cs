using System.ComponentModel;
using System.Runtime.CompilerServices;
using FlightSimulatorApp.Model;

namespace FlightSimulatorApp.ViewModel {
    public class ConnectionButtonsVM : INotifyPropertyChanged {
        private SimulatorModel myModel;
        public event PropertyChangedEventHandler PropertyChanged;
        private string ip;
        private int port;
        private bool isConnected;

        public string Ip {
            get => ip;
            set {
                if (ip != value) {
                    ip = value;
                    NotifyPropertyChanged("Ip");
                }
            }
        }
        public int Port {
            get => port;
            set {
                if (port != value) {
                    port = value;
                    NotifyPropertyChanged("Port");
                }
            }
        }

        public bool IsConnected {
            get => isConnected;
            set {
                if (isConnected != value) {
                    isConnected = value;
                    NotifyPropertyChanged("IsConnected");
                }
            }
        }

        public void setModel(SimulatorModel model) {
            myModel = model;

        }

        private void NotifyPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Connect() {
            myModel.Connect(ip, port);
        }

        public void Disconnect() {
            myModel.Disconnect();
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