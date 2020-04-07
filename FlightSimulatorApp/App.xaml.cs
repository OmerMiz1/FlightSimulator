using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows;
using FlightSimulatorApp.Controls;
using FlightSimulatorApp.Model;
using FlightSimulatorApp.ViewModel;

namespace FlightSimulatorApp {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        public MapVM mapViewModel { get; internal set; }

        private void Application_Startup(Object sender, StartupEventArgs e) {
            InitializeComponent();
            /*TODO get from StartUpEventArgs the args for tcp client (default)...*/
            /* Default Values.. TODO Move these to AppConfig!*/
            TcpClient tcpClient = new TcpClient("127.0.0.1", 5402); 
            SimulatorModel simulatorModel = new SimulatorModel(tcpClient, true);
            MapVM mapViewModel = new MapVM(simulatorModel);
            Map map = new Map(mapViewModel);
        }
    }
}
