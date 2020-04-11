using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using FlightSimulatorApp.Annotations;
using FlightSimulatorApp.Model;

namespace FlightSimulatorApp.ViewModel {
    public class CockpitControlsVM : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;
        private SimulatorModel mySimulatorModel;
        private double _rudder;
        private double _elevators;
        private double _ailerons;
        private double _throttle;

        public CockpitControlsVM(SimulatorModel newSimulatorModel) {
            mySimulatorModel = newSimulatorModel;
            mySimulatorModel.PropertyChanged += Model_PropertyChanged;
        }

        public double Rudder {
            get => _rudder;
            set {
                _rudder = value;
                NotifyPropertyChanged("Rudder");
                mySimulatorModel.SetVariable("Rudder", value.ToString());
            }
        }
        public double Elevators {
            get => this._elevators;
            set {
                _elevators = value;
                NotifyPropertyChanged("Elevators");
                mySimulatorModel.SetVariable("Elevators", value.ToString());
            }
        }
        public double Ailerons {
            get => _ailerons;
            set {
                _ailerons = value;
                NotifyPropertyChanged("Ailerons");
                mySimulatorModel.SetVariable("Ailerons", value.ToString());
            }
        }
        public double Throttle {
            get => _throttle;
            set {
                _throttle = value;
                NotifyPropertyChanged("Throttle");
                mySimulatorModel.SetVariable("Throttle", value.ToString());
            }
        }

        private void Model_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            string propValueStr = (sender as SimulatorModel)?.GetVariable(e.PropertyName);
            double propValue = Convert.ToDouble(propValueStr);
            switch (e.PropertyName) {
                case "Rudder": {
                    Rudder = propValue;
                    break;
                }
                case "Elevators": {
                    Elevators = propValue;
                    break;
                }
                case "Ailerons": {
                    Ailerons = propValue;
                    break;
                }
                case "Throttle": {
                    Throttle = propValue;
                    break;
                }
                default:
                    break;
            }
        }

        private void NotifyPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}