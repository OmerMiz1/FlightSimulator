﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using FlightSimulatorApp.Model;
using Microsoft.Maps.MapControl.WPF;

namespace FlightSimulatorApp.ViewModel {
    public class MapVM : INotifyPropertyChanged {
        public SimulatorModel myModel { get; private set; }
        private VariableNamesManager varNamesMgr;
        public event PropertyChangedEventHandler PropertyChanged;

        private Location location;
        /*private double longtitude;
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
        }*/

        public MapVM(SimulatorModel model) {
            myModel = model;
            varNamesMgr = new VariableNamesManager();
            InitVariables();
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            string propName = varNamesMgr.toName(e.PropertyName);
            switch (propName) {
                case "Altitude": {
                    location.Altitude = 
                    break;
                }
                case "Latitude": {
                    /*TODO Fix name + Action*/
                    break;
                }
                case "Heading": {
                    /*TODO Fix name + Action*/
                    break;
                }
                default:
                    break;
            }
        }

        private void InitVariables() {
            Variables = new DictionaryIndexer();
            Variables["Latitude"] = "NO_VALUE_YET";
            Variables["Longitude"] = "NO_VALUE_YET";
            Variables["Heading"] = "NO_VALUE_YET";
        }

        private Dictionary<string, string> NameToPath() {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict["Heading"] = "/orientation/heading-deg";
            dict["Longitude"] = "/position/longitude-deg";
            dict["Latitude"] = "/position/latitude-deg";
            return dict;
        }


    }
}
