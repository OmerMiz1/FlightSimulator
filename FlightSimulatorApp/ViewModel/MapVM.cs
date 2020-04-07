﻿using System;
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
        public SimulatorModel myModel { get; private set; }
        private VariableNamesManager varNamesMgr = new VariableNamesManager();
        public event PropertyChangedEventHandler PropertyChanged;

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

        /* NEVER USE THIS CTOR ! ONLY TO BUILD & TEST */
        public MapVM() {
            myModel = new SimulatorModel(new TcpClient());
            myModel.PropertyChanged += Model_PropertyChanged;
        }
        /* NEVER USE THIS CTOR ! ONLY TO BUILD & TEST */

        public MapVM(SimulatorModel model) {
            myModel = model;
            myModel.PropertyChanged += Model_PropertyChanged;
        }

        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Model_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            string propName = varNamesMgr.toName(e.PropertyName);
            string propValueStr = (sender as SimulatorModel)?.Variables[e.PropertyName];
            double propValue = Convert.ToDouble(propValueStr);
            switch (propName) {
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
                    /*TODO Currently empty, only needed if using airplane icon to show direction */
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
