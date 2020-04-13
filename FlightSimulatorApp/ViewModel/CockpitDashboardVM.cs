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
        private String _altitudeAltimeter;
        private String _altitudeGps;
        private String _verticalSpeed;
        private String _groundSpeed;
        private String _heading;
        private String _pitch;
        private String _roll;
        private String _speed;

        public CockpitDashboardVM(SimulatorModel model)
        {
            model.PropertyChanged += Model_PropertyChanged;
        }
        public String VerticalSpeed
        {
            get => _verticalSpeed;
            set
            {
                _verticalSpeed = value;
                NotifyPropertyChanged("VerticalSpeed");
            }
        }
        public String Heading
        {
            get => _heading;
            set
            {
                _heading = value;
                NotifyPropertyChanged("Heading");
            }
        }
        public String GroundSpeed
        {
            get => _groundSpeed;
            set
            {
                _groundSpeed = value;
                NotifyPropertyChanged("GroundSpeed");
            }
        }
        public String Speed
        {
            get => _speed;
            set
            {
                _speed = value;
                NotifyPropertyChanged("Speed");
            }
        }
        public String AltitudeGps
        {
            get => _altitudeGps;
            set
            {
                _altitudeGps = value;
                NotifyPropertyChanged("AltitudeGps");
            }
        }
        public String Roll
        {
            get => _roll;
            set
            {
                _roll = value;
                NotifyPropertyChanged("Roll");
            }
        }
        public String Pitch
        {
            get => _pitch;
            set
            {
                _pitch = value;
                NotifyPropertyChanged("Pitch");
            }
        }
        public String AltitudeAltimeter
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

        private String ToDecimalFormat(String str) {
            return ToDecimalFormat(str, 0);
        }

        private String ToDecimalFormat(String str, int precision) {
            Double myValue = Double.NaN;
            /* Test if string's a number (either double or int are ok)*/
            if (!double.TryParse(str, out myValue)) {
                return str; // Value of str here is supposed to be "ERR"
            }

            /* If string's a number, generate its format */
            String myFormat = "{0:#################0";
            //if (precision > 0) {
            //    myFormat += '.';
            //    for (int i = 1; i <= precision; ++i) {
            //        myFormat += '0';
            //    }
            //}
            myFormat = "{0:N";
            myFormat += precision;
            myFormat += '}';

            return String.Format(myFormat, myValue);
        }
    }
}