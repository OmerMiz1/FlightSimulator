using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using FlightSimulatorApp.Model;

namespace FlightSimulatorApp.ViewModel {
    public class CockpitDashboardVM : INotifyPropertyChanged
    {
        private SimulatorModel mySimulatorModel;

        public CockpitDashboardVM(SimulatorModel newSimulatorModel)
        {
            this.mySimulatorModel = newSimulatorModel;
            mySimulatorModel.PropertyChanged += Model_PropertyChanged;
        }

        private void Model_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            string propValueStr = (sender as SimulatorModel)?.GetVariable(e.PropertyName);
            double propValue = Convert.ToDouble(propValueStr);
            switch (e.PropertyName) {
                case "VerticalSpeed": {
                    VerticalSpeed = propValue;
                    break;
                }
                case "Heading": {
                    Heading = propValue;
                    break;
                }
                case "GroundSpeed": {
                    GroundSpeed = propValue;
                    break;
                }
                case "Speed": {
                    Speed = propValue;
                    break;
                }
                case "AltitudeGps": {
                    AltitudeGps = propValue;
                    break;
                }
                case "Roll": {
                    Roll = propValue;
                    break;
                }
                case "Pitch": {
                    Pitch = propValue;
                    break;
                }
                case "AltitudeAltimeter": {
                    AltitudeAltimeter = propValue;
                    break;
                }
                default:
                    break;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private double _verticalSpeed;
        private double _heading;
        private double _groundSpeed;
        private double _speed;
        private double _altitudeGps;
        private double _roll;
        private double _pitch;
        private double _altitudeAltimeter;

        public void NotifyPropertyChanged(string propName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        public double VerticalSpeed {
            get { return _verticalSpeed; }
            set { _verticalSpeed = value; NotifyPropertyChanged("VerticalSpeed"); }
        }
        public double Heading {
            get { return _heading; }
            set { _heading = value; NotifyPropertyChanged("Heading"); }
        }
        public double GroundSpeed {
            get { return _groundSpeed; }
            set { _groundSpeed = value; NotifyPropertyChanged("GroundSpeed"); }
        }
        public double Speed {
            get { return _speed; }
            set { _speed = value; NotifyPropertyChanged("Speed"); }
        }
        public double AltitudeGps {
            get { return _altitudeGps; }
            set { _altitudeGps = value; NotifyPropertyChanged("AltitudeGps"); }
        }
        public double Roll {
            get { return _roll; }
            set { _roll = value; NotifyPropertyChanged("Roll"); }
        }
        public double Pitch {
            get { return _pitch; }
            set { _pitch = value; NotifyPropertyChanged("Pitch"); }
        }
        public double AltitudeAltimeter {
            get { return _altitudeAltimeter; }
            set { _altitudeAltimeter = value; NotifyPropertyChanged("AltitudeAltimeter"); }
        }
    }
}
