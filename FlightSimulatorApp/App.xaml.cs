using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net.Sockets;
using System.Text.RegularExpressions;
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
        private const string DEFAULT_IP = "127.0.0.1";
        private const int DEFAULT_PORT = 5402;

        private void App_Startup(Object sender, StartupEventArgs e) {
            Regex ValidIPRegex =
                new Regex(
                    "^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$");
            MainWindow mainWindow = new MainWindow();
            SimulatorModel model = new SimulatorModel();

            /* No (or not enough) args so use default values */
            if (e.Args.Length < 2) {
                model.Ip = DEFAULT_IP;
                model.Port = DEFAULT_PORT;
            }
            /* Parse arguments into ip and port */
            else {
                int port = -1;
                foreach (string curArg in e.Args) {
                    if (ValidIPRegex.IsMatch(curArg)) {
                        model.Ip = curArg;
                    }
                    else if (int.TryParse(curArg, out port) && port >= 0 && port <= 65535) {
                        model.Port = port;
                    }
                }
            }

            /* Instantiate View, View Model and Model layers */
            Map map = new Map(new MapVM(model));
            CockpitDashboard cockpitDashboard = new CockpitDashboard(new CockpitDashboardVM(model));
            ConnectionButtons connectionBtns = new ConnectionButtons(new ConnectionButtonsVM(model));
            // CockpitControls cockpitControls = new CockpitControls(new CockpitControlsVM(model));

            mainWindow.Map = map;
            mainWindow.ConnectionButtons = connectionBtns;
            mainWindow.CockpitDashboard = cockpitDashboard;
            // mainWindow.CockpitControls = CockpitControls;
        }
    }
}