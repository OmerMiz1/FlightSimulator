using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Threading;
using FlightSimulatorApp.Model;
using Microsoft.Maps.MapControl.WPF;

namespace FlightSimulatorApp.ViewModel {
    public class MapVM : INotifyPropertyChanged {
        public SimulatorModel mySimulatorModel { get; private set; }
        private VariableNamesManager varNamesMgr = new VariableNamesManager();
        public event PropertyChangedEventHandler PropertyChanged;
        private double _heading;
        private Location location = new Location();
        public Location Location {
            get => location;
            set {
                if (location != value) {
                    location = value;
                    NotifyPropertyChanged("Location");
                }
            }
        }
        public double Longitude {
            get => location.Longitude;
            set {
                if (location.Longitude != value) {
                    location.Longitude = value;
                    NotifyPropertyChanged("Location");
                }
            }
        }
        public double Altitude {
            get => location.Altitude;
            set {
                if (location.Altitude != value) {
                    location.Altitude = value;
                    NotifyPropertyChanged("Location");
                }
            }
        }
        public double Latitude {
            get => location.Latitude;
            set {
                if (location.Latitude != value) {
                    location.Latitude = value;
                    NotifyPropertyChanged("Location");
                }
            }
        }

        public double Heading {
            get => _heading;
            set {
                _heading = value;
                NotifyPropertyChanged("Heading");
            }
        }

        public MapVM(SimulatorModel simulatorModel) {
            mySimulatorModel = simulatorModel;
            mySimulatorModel.PropertyChanged += Model_PropertyChanged;
        }

        private void NotifyPropertyChanged( string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Model_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            string propValueStr = (sender as SimulatorModel)?.GetVariable(e.PropertyName);
            Double propValue;
            if (!Double.TryParse(propValueStr, out propValue)) {
                return;
            }

            switch (e.PropertyName) {
                case "Altitude": {
                    Altitude = propValue;
                    break;
                }
                case "Latitude": {
                    Latitude = propValue;
                    break;
                }
                case "Longitude": {
                    Longitude = propValue;
                    break;
                }
                case "Heading": {
                    Heading = propValue;
                    break;
                }
                default:
                    break;
            }
        }

        /*private void InitVariables() {
            Variables = new DictionaryIndexer();
            Variables["Latitude"] = "NO_VALUE_YET";
            Variables["Longitude"] = "NO_VALUE_YET";
            Variables["Heading"] = "NO_VALUE_YET";
        }*/
        /*private Dictionary<string, string> NameToPath() {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict["Heading"] = "/orientation/heading-deg";
            dict["Longitude"] = "/position/longitude-deg";
            dict["Latitude"] = "/position/latitude-deg";
            return dict;
        }*/
    }
}
