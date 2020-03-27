using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Automation;

namespace FlightSimulatorApp.Model
{
    class TCPClient
    {
        private TcpClient _client;
        private NetworkStream _stream;
        private string _ip;
        private Int32 _port;
        private Boolean running;

        /* Properties to receive from server */
        public string IndicatedHeadingDeg { get; set; }
        public string GpsIndicatedVerticalSpeed { get; set; }

        public string GpsIndicatedGroundSpeedKt{ get; set; }
        public string AirspeedIndicatorIndicatedSpeedKt{ get; set; }
        public string GpsIndicatedAltitudeFt{ get; set; }
        public string AttitudeIndicatoInternalRollDeg{ get; set; }
        public string AttitudeIndicatorInternalPitchDeg{ get; set; }
        public string AltimeterIndicatedAltitudeFt{ get; set; }
        
        /* Properties set by Client (send to server)*/
        public string Rudder{ get; set; }
        public string Elevators{ get; set; }
        public string Ailerons{ get; set; }
        public string Throttle{ get; set; }


private Dictionary<string, string> _valuesFromSim;
        private Dictionary<string, string> _valuesToSim;

        public string Ip
        {
            get { return _ip;}
            set { _ip = value;}
        }

        TCPClient(string ip, Int32 port)
        {
            _ip = ip;
            _port = port;

        }

        public void Connect()
        {
            try
            {
                /* Attempt connect to server */
                using (_client = new TcpClient(_ip, _port))
                {
                    /* Error connecting to server, O.W print OK message and continue.. */
                    if (!_client.Connected) 
                    {
                        Console.WriteLine("TCPClient Err #1");
                        this.running = false;
                        return;

                    }

                    Console.WriteLine("Client connected successfully to server...");
                    running = true;
                    using (_stream = _client.GetStream())
                    {
                        /* Start working */
                        Thread myThread = new Thread(() =>
                        {
                            while (running && _client.Connected)
                            {

                            }
                        });
                    }
                }
                
            } 
            
            catch (Exception e)
            {
                running = false;
                Console.WriteLine("TCPClient Err #2");
            }
            
        }

        public void Stop()
        {

        }

    }
}
