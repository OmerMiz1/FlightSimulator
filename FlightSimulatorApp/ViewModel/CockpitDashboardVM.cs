using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using FlightSimulatorApp.Model;

namespace FlightSimulatorApp.ViewModel {
    public class CockpitDashboardVM : INotifyPropertyChanged
    {
        private SimulatorModel mySimulatorModel;
        
        public CockpitDashboardVM(SimulatorModel newSimulatorModel)
        {
            this.mySimulatorModel = newSimulatorModel;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private string _gpsVerticalSpeed;
        private string _headingDegree;
        private string _gpsGroundSpeed;
        private string _airSpeedIndicator;
        private string _altitudeGps;
        private string _attitudeIndicatorInternalRollDeg;
        private string _attitudeIndicatorInternalPitchDeg;
        private string _altimeterIndicatedAltitudeFt;

        public void NotifyPropertyChanged(string propName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        public string GpsVerticalSpeed
        {
            get { return _gpsVerticalSpeed; }
            set { _gpsVerticalSpeed = value; NotifyPropertyChanged("GpsVerticalSpeed"); }
        }
        public string HeadingDegree {
            get { return _headingDegree; }
            set { _headingDegree = value; NotifyPropertyChanged("HeadingDegree"); }
        }
        public string GpsGroundSpeed {
            get { return _gpsGroundSpeed; }
            set { _gpsGroundSpeed = value; NotifyPropertyChanged("GpsGroundSpeed"); }
        }
        public string AirSpeedIndicator {
            get { return _airSpeedIndicator; }
            set { _airSpeedIndicator = value; NotifyPropertyChanged("AirSpeedIndicator"); }
        }
        public string AltitudeGps {
            get { return _altitudeGps; }
            set { _altitudeGps = value; NotifyPropertyChanged("AltitudeGps"); }
        }
        public string AttitudeIndicatorInternalRollDeg {
            get { return _attitudeIndicatorInternalRollDeg; }
            set { _attitudeIndicatorInternalRollDeg = value; NotifyPropertyChanged("AttitudeIndicatorInternalRollDeg"); }
        }
        public string AttitudeIndicatorInternalPitchDeg {
            get { return _attitudeIndicatorInternalPitchDeg; }
            set { _attitudeIndicatorInternalPitchDeg = value; NotifyPropertyChanged("AttitudeIndicatorInternalPitchDeg"); }
        }
        public string AltimeterIndicatedAltitudeFt {
            get { return _altimeterIndicatedAltitudeFt; }
            set { _altimeterIndicatedAltitudeFt = value; NotifyPropertyChanged("AltimeterIndicatedAltitudeFt"); }
        }
    }
}
