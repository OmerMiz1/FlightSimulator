using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows;
using FlightSimulatorApp.Model;
using FlightSimulatorApp.ViewModel;

namespace FlightSimulatorApp {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        public MapVM mapViewModel { get; internal set; }

        private void Application_Startup(Object sender, StartupEventArgs e) {
            /*TODO get from StartUpEventArgs the args for tcp client (default)...*/
            TcpClient tcpClient = new TcpClient();
            SimulatorModel simulatorModel = new SimulatorModel(tcpClient);
            mapViewModel = new MapVM(simulatorModel);
        }
    }
}
