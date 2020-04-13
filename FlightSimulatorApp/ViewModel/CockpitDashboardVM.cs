using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using FlightSimulatorApp.Model;

namespace FlightSimulatorApp.ViewModel
{
    public class CockpitDashboardVM : INotifyPropertyChanged
    {

        /* Fields & Properties declarations */
        private string _altitudeAltimeter;
        private string _altitudeGps;
        private string _verticalSpeed;
        private string _groundSpeed;
        private string _heading;
        private string _pitch;
        private string _roll;
        private string _speed;

        public CockpitDashboardVM(SimulatorModel model)
        {
            model.PropertyChanged += Model_PropertyChanged;
        }
        public string VerticalSpeed
        {
            get => _verticalSpeed;
            set
            {
                _verticalSpeed = value;
                NotifyPropertyChanged("VerticalSpeed");
            }
        }
        public string Heading
        {
            get => _heading;
            set
            {
                _heading = value;
                NotifyPropertyChanged("Heading");
            }
        }
        public string GroundSpeed
        {
            get => _groundSpeed;
            set
            {
                _groundSpeed = value;
                NotifyPropertyChanged("GroundSpeed");
            }
        }
        public string Speed
        {
            get => _speed;
            set
            {
                _speed = value;
                NotifyPropertyChanged("Speed");
            }
        }
        public string AltitudeGps
        {
            get => _altitudeGps;
            set
            {
                _altitudeGps = value;
                NotifyPropertyChanged("AltitudeGps");
            }
        }
        public string Roll
        {
            get => _roll;
            set
            {
                _roll = value;
                NotifyPropertyChanged("Roll");
            }
        }
        public string Pitch
        {
            get => _pitch;
            set
            {
                _pitch = value;
                NotifyPropertyChanged("Pitch");
            }
        }
        public string AltitudeAltimeter
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
            //var propValue = Convert.ToDouble(propValueStr);
            switch (e.PropertyName)
            {
                case "VerticalSpeed":
                {
                    VerticalSpeed = ToDecimalFormat(propValueStr, 0);
                    break;
                }
                case "Heading":
                {
                    Heading = ToDecimalFormat(propValueStr, 2);
                    break;
                }
                case "GroundSpeed":
                {
                    GroundSpeed = ToDecimalFormat(propValueStr, 0);
                    break;
                }
                case "Speed":
                {
                    Speed = ToDecimalFormat(propValueStr, 0);
                    break;
                }
                case "AltitudeGps":
                {
                    AltitudeGps = ToDecimalFormat(propValueStr, 2);
                    break;
                }
                case "Roll":
                {
                    Roll = ToDecimalFormat(propValueStr, 2);
                    break;
                }
                case "Pitch":
                {
                    Pitch = ToDecimalFormat(propValueStr, 2);
                    break;
                }
                case "AltitudeAltimeter":
                {
                    AltitudeAltimeter = ToDecimalFormat(propValueStr, 2);
                    break;
                }
            }
        }

        private string ToDecimalFormat(string str) {
            return ToDecimalFormat(str, 0);
        }

        private string ToDecimalFormat(string str, int precision) {
            var myValue = double.NaN;
            /* Test if string's a number (either double or int are ok)*/
            if (!double.TryParse(str, out myValue)) {
                return str; // Value of str here is supposed to be "ERR"
            }

            /* If string's a number, generate its format */
            return string.Format("{0:N" + precision + "}", myValue);
        }
    }
}