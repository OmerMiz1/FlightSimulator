namespace FlightSimulatorApp.ViewModel
{
    public class VariableNamesManager
    {
        public string toPath(string name) {
            switch (name) {
                case "Heading": return "/instrumentation/heading-indicator/indicated-heading-deg";
                case "VerticalSpeed": return "/instrumentation/gps/indicated-vertical-speed";
                case "GroundSpeed": return "/instrumentation/gps/indicated-ground-speed-kt";
                case "Speed": return "/instrumentation/airspeed-indicator/indicated-speed-kt";
                case "AltitudeGps": return "/instrumentation/gps/indicated-altitude-ft";
                case "Roll": return "/instrumentation/attitude-indicator/internal-roll-deg";
                case "Pitch": return "/instrumentation/attitude-indicator/internal-pitch-deg";
                case "AltitudeAltimeter": return "/instrumentation/altimeter/indicated-altitude-ft";
                case "Longitude": return "/position/longitude-deg";
                case "Latitude": return "/position/latitude-deg";
                case "Altitude": return "/position/altitude-ft";
                case "Elevator": return "/controls/flight/elevator";
                case "Rudder": return "/controls/flight/rudder";
                case "Aileron": return "/controls/flight/aileron";
                case "Throttle": return "/controls/engines/engine/throttle";
                default: return "Variable Not Found"; /* Error Value */
            }
        }

        public string toName(string path) {
            switch (path) {
                case "/instrumentation/heading-indicator/indicated-heading-deg": return "Heading";
                case "/instrumentation/gps/indicated-vertical-speed": return "VerticalSpeed";
                case "/instrumentation/gps/indicated-ground-speed-kt": return "GroundSpeed";
                case "/instrumentation/airspeed-indicator/indicated-speed-kt": return "Speed";
                case "/instrumentation/gps/indicated-altitude-ft": return "AltitudeGps";
                case "/instrumentation/attitude-indicator/internal-roll-deg": return "Roll";
                case "/instrumentation/attitude-indicator/internal-pitch-deg": return "Pitch";
                case "/instrumentation/altimeter/indicated-altitude-ft": return "AltitudeAltimeter";
                case "/position/longitude-deg": return "Longitude";
                case "/position/latitude-deg": return "Latitude";
                case "/position/altitude-ft": return "Altitude";
                case "/controls/flight/elevator": return "Elevator";
                case "/controls/flight/rudder": return "Rudder";
                case "/controls/flight/aileron": return "Aileron";
                case "/controls/engines/engine/throttle": return "Throttle";
                default: return "Variable Not Found"; /* Error Value */
            }
        }
    }
}