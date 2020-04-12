using System;
using System.ComponentModel;
using System.Windows;
using FlightSimulatorApp.Model;

namespace FlightSimulatorApp.ViewModel
{
    public class CockpitDashboardVM : INotifyPropertyChanged
    {
        private double _verticalSpeed;
        private double _altitudeAltimeter;
        private double _altitudeGps;
        private double _groundSpeed;
        private double _heading;
        private double _pitch;
        private double _roll;
        private double _speed;

        public CockpitDashboardVM(SimulatorModel model)
        {
            model.PropertyChanged += Model_PropertyChanged;
        }
        public double VerticalSpeed
        {
            get => _verticalSpeed;
            set
            {
                _verticalSpeed = value;
                NotifyPropertyChanged("VerticalSpeed");
            }
        }
        public double Heading
        {
            get => _heading;
            set
            {
                _heading = value;
                NotifyPropertyChanged("Heading");
            }
        }
        public double GroundSpeed
        {
            get => _groundSpeed;
            set
            {
                _groundSpeed = value;
                NotifyPropertyChanged("GroundSpeed");
            }
        }
        public double Speed
        {
            get => _speed;
            set
            {
                _speed = value;
                NotifyPropertyChanged("Speed");
            }
        }
        public double AltitudeGps
        {
            get => _altitudeGps;
            set
            {
                _altitudeGps = value;
                NotifyPropertyChanged("AltitudeGps");
            }
        }
        public double Roll
        {
            get => _roll;
            set
            {
                _roll = value;
                NotifyPropertyChanged("Roll");
            }
        }
        public double Pitch
        {
            get => _pitch;
            set
            {
                _pitch = value;
                NotifyPropertyChanged("Pitch");
            }
        }
        public double AltitudeAltimeter
        {
            get => _altitudeAltimeter;
            set
            {
                _altitudeAltimeter = value;
                NotifyPropertyChanged("AltitudeAltimeter");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        private void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var propValueStr = (sender as SimulatorModel)?.GetVariable(e.PropertyName);
            var propValue = Convert.ToDouble(propValueStr);
            switch (e.PropertyName)
            {
                case "VerticalSpeed":
                {
                    VerticalSpeed = propValue;
                    break;
                }
                case "Heading":
                {
                    Heading = propValue;
                    break;
                }
                case "GroundSpeed":
                {
                    GroundSpeed = propValue;
                    break;
                }
                case "Speed":
                {
                    Speed = propValue;
                    break;
                }
                case "AltitudeGps":
                {
                    AltitudeGps = propValue;
                    break;
                }
                case "Roll":
                {
                    Roll = propValue;
                    break;
                }
                case "Pitch":
                {
                    Pitch = propValue;
                    break;
                }
                case "AltitudeAltimeter":
                {
                    AltitudeAltimeter = propValue;
                    break;
                }
            }
        }
    }
}