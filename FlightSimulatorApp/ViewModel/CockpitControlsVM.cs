using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using FlightSimulatorApp.Annotations;

namespace FlightSimulatorApp.ViewModel {
    public class CockpitControlsVM : INotifyPropertyChanged
    {

        private string _rudder;
        private string _elevators;
        private string _ailerons;
        private string _throttle;

        public string Rudder
        {
            get { return this._rudder; }
            set { this._rudder = value; OnPropertyChanged("Rudder"); }
        }
        public string Elevators {
            get { return this._elevators; }
            set { this._elevators = value; OnPropertyChanged("Elevators"); }
        }
        public string Ailerons {
            get { return this._ailerons; }
            set { this._ailerons = value; OnPropertyChanged("Ailerons"); }
        }
        public string Throttle {
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
