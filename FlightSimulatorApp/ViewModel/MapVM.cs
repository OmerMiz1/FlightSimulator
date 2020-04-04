using System.ComponentModel;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using FlightSimulatorApp.Model;

namespace FlightSimulatorApp.ViewModel {
    public class MapVM : INotifyPropertyChanged {
        public SimulatorModel _simulatorModel { get; private set; }
        public event PropertyChangedEventHandler PropertyChanged;

        private double longtitude;
        public double Longtitude {
            get => longtitude;
            set {
                if (longtitude != value) {
                    longtitude = value;
                    OnPropertyChanged(nameof(Longtitude));
                }
            }
        }

        private double altitude;
        public double Altitude {
            get => altitude;
            set {
                if (altitude != value) {
                    altitude = value;
                    OnPropertyChanged(nameof(Altitude));
                }
            }
        }

        private double heading;
        public double Heading {
            get => heading;
            set {
                if (heading != value) {
                    heading = value;
                    OnPropertyChanged(nameof(Heading));
                }
            }
        }

        public MapVM(SimulatorModel simulatorModel) {
            _simulatorModel = simulatorModel;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            string propName = e.PropertyName;
            if (propName == nameof(Altitude))
            {
                Altitude = _simulatorModel._valuesToSim
            }
        }

    }
}
}