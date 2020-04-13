using System;
using System.ComponentModel;
using FlightSimulatorApp.Model;

namespace FlightSimulatorApp.ViewModel
{
    public class CockpitControlsVM : INotifyPropertyChanged
    {
        private double _aileron;
        private double _elevator;
        private double _rudder;
        private double _throttle;
        private readonly SimulatorModel mySimulatorModel;

        public CockpitControlsVM(SimulatorModel newSimulatorModel)
        {
            mySimulatorModel = newSimulatorModel;
            mySimulatorModel.PropertyChanged += Model_PropertyChanged;
        }

        public double Rudder
        {
            get => _rudder;
            set
            {
                _rudder = value;
                NotifyPropertyChanged("Rudder");
                mySimulatorModel.SetVariable("Rudder", value.ToString());
            }
        }

        public double Elevator
        {
            get => _elevator;
            set
            {
                _elevator = value;
                NotifyPropertyChanged("Elevator");
                mySimulatorModel.SetVariable("Elevator", value.ToString());
            }
        }

        public double Aileron
        {
            get => _aileron;
            set
            {
                _aileron = value;
                NotifyPropertyChanged("Aileron");
                mySimulatorModel.SetVariable("Aileron", value.ToString());
            }
        }

        public double Throttle
        {
            get => _throttle;
            set
            {
                _throttle = value;
                NotifyPropertyChanged("Throttle");
                mySimulatorModel.SetVariable("Throttle", value.ToString());
                //TODO: make sure the server is notified on a change!!!
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var propValueStr = (sender as SimulatorModel)?.GetVariable(e.PropertyName);
            /*TODO DEBUG*/
            if (e.PropertyName == "Longitude" || e.PropertyName == "Latitude") {
                return;
            }
            var propValue = Convert.ToDouble(propValueStr);
            switch (e.PropertyName)
            {
                case "Rudder":
                {
                    Rudder = propValue;
                    break;
                }
                case "Elevator":
                {
                    Elevator = propValue;
                    break;
                }
                case "Aileron":
                {
                    Aileron = propValue;
                    break;
                }
                case "Throttle":
                {
                    Throttle = propValue;
                    break;
                }
            }
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}