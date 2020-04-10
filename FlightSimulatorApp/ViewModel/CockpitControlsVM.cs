using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using FlightSimulatorApp.Annotations;
using FlightSimulatorApp.Model;

namespace FlightSimulatorApp.ViewModel {
    public class CockpitControlsVM : INotifyPropertyChanged
    {

        private SimulatorModel mySimulatorModel;
        private double _rudder;
        private double _elevators;
        private double _ailerons;
        private double _throttle;

        public CockpitControlsVM(SimulatorModel newSimulatorModel)
        {
            this.mySimulatorModel = newSimulatorModel;
            mySimulatorModel.PropertyChanged += Model_PropertyChanged;
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

        public double Rudder
        {
            get { return this._rudder; }
            set { this._rudder = value; OnPropertyChanged("Rudder"); }
        }
        public double Elevators {
            get { return this._elevators; }
            set { this._elevators = value; OnPropertyChanged("Elevators"); }
        }
        public double Ailerons {
            get { return this._ailerons; }
            set { this._ailerons = value; OnPropertyChanged("Ailerons"); }
        }
        public double Throttle {
            get { return this._throttle; }
            set { this._throttle = value; OnPropertyChanged("Throttle"); }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
