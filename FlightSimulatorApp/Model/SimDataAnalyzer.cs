using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace FlightSimulatorApp.Model
{
    public class SimDataAnalyzer
    {
        /*/* Properties FROM server #1#
        public String GpsIndicatedVerticalSpeed { get; set; }
        public String IndicatedHeadingDeg { get; set; }
        public String GpsIndicatedGroundSpeedKt { get; set; }
        public String AirspeedIndicatorIndicatedSpeedKt { get; set; }
        public String GpsIndicatedAltitudeFt { get; set; }
        public String AttitudeIndicatoInternalRollDeg { get; set; }
        public String AttitudeIndicatorInternalPitchDeg { get; set; }
        public String AltimeterIndicatedAltitudeFt { get; set; }

        /* Properties TO server#1#
        public String Rudder { get; set; }
        public String Elevators { get; set; }
        public String Ailerons { get; set; }
        public String Throttle { get; set; }*/

        private ConcurrentDictionary<String, String> DataFromServer;
        private ConcurrentDictionary<String, String> DataToServer;
        
        public void Parse(String data)
        {
            String[] lines = data.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            foreach (String line in lines)
            {
                String[] tokens = line.Split(" ");
                if (tokens[0] == "get")
                {
                    DataFromServer.AddOrUpdate(tokens[1], tokens[2], (key, value) => tokens[2]);
                }

                else if (tokens[0] == "set")
                {
                    DataToServer.AddOrUpdate(tokens[1], tokens[2], (key, value) => tokens[2]);
                }

                else
                {
                    Console.WriteLine("SimDataAnalyzer Error #1");
                    return;
                }
            }
        }
    }
}