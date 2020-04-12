using System.ComponentModel;
using FlightSimulatorApp.Model;
using Microsoft.Maps.MapControl.WPF;

namespace FlightSimulatorApp.ViewModel
{
    public class MapVM : INotifyPropertyChanged
    {
        private double _heading;
        private Location location = new Location();
        private VariableNamesManager varNamesMgr = new VariableNamesManager();

        public MapVM(SimulatorModel simulatorModel)
        {
            mySimulatorModel = simulatorModel;
            mySimulatorModel.PropertyChanged += Model_PropertyChanged;
        }

        public SimulatorModel mySimulatorModel { get; }

        public Location Location
        {
            get => location;
            set
            {
                if (location != value)
                {
                    location = value;
                    NotifyPropertyChanged("Location");
                }
            }
        }

        public double Longitude
        {
            get => location.Longitude;
            set {
                if (value < Location.MinLongitude) {
                    /*TODO Show status: "Warning: Airplane out of bounds\n Longitude is below minimal bound"*/
                } else if (Location.MaxLongitude > value) {
                    /*TODO Show status: "Warning: Airplane out of bounds\n Longitude is above maximal bound"*/
                }
                location.Longitude = value;
                NotifyPropertyChanged("Longitude");
            }
        }

        public double Latitude
        {
            get => location.Latitude;
            set
            {
                var normalized = value;
                /** Once airplane reaches left\right boundry move to the other side of the map. More about the equation at:
                 https://stackoverflow.com/questions/7594508/modulo-operator-with-negative-values*/
                if (value < Location.MinLatitude) {
                    normalized = Location.MinLatitude;
                    /*TODO Show status: "Warning: Airplane out of bounds\n Latitude is below minimal bound"*/
                    
                } else if (value > Location.MaxLatitude) {
                    normalized = Location.MaxLatitude;
                    /*TODO Show status: "Warning: Airplane out of bounds\n Latitude is above maximal bound"*/
                }
                location.Latitude = normalized;
                if (value != normalized) {
                    NotifyPropertyChanged("Latitude");
                }
            }
        }

        public double Altitude
        {
            get => location.Altitude;
            set
            {
                if (location.Altitude != value)
                {
                    location.Altitude = value;
                    NotifyPropertyChanged("Location");
                }
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

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var propValueStr = (sender as SimulatorModel)?.GetVariable(e.PropertyName);
            double propValue;
            if (!double.TryParse(propValueStr, out propValue)) return;

            switch (e.PropertyName)
            {
                case "Altitude":
                {
                    Altitude = propValue;
                    break;
                }
                case "Latitude":
                {
                    Latitude = propValue;
                    break;
                }
                case "Longitude":
                {
                    Longitude = propValue;
                    break;
                }
                case "Heading":
                {
                    Heading = propValue;
                    break;
                }
            }
        }

        /*** Currently not in use.
         * Mainly: Makes the longitude property cyclic in the range [MinLongitude , MaxLongitude]
         */
        private double NormalizeLongitude(double longitude) {
            /* Once airplane reaches top\bottom boundries, show as if airplane is at
             * the min/max possible value. More about the equation at:
             * https://stackoverflow.com/questions/7594508/modulo-operator-with-negative-values */
            while (longitude > Location.MaxLongitude) {
                longitude -= Location.MaxLongitude * 2;
            }
            while (longitude < Location.MinLongitude) {
                longitude += Location.MaxLongitude * 2;
            }

            return longitude;
        }
    }
}