using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using FlightSimulatorApp.Controls;
using FlightSimulatorApp.Model;
using FlightSimulatorApp.ViewModel;
using Microsoft.Maps.MapControl.WPF;

namespace FlightSimulatorApp {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        /* Default values declarations */
        public static string DefaultIp { get; } = "127.0.0.1";
        public static int DefaultPort { get; } = 5402;

        private void App_Startup(Object sender, StartupEventArgs e) {
            Regex ValidIPRegex =
                new Regex(
                    "^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$");
            
            /* View */
            MainWindow mainWindow = new MainWindow();

            /* Model */
            SimulatorModel model = new SimulatorModel();
            /* No Args (or not enough) args so use default values */
            model.Ip = DefaultIp;
            model.Port = DefaultPort;
            int port = -1;

            /* Try to parse arguments into ip and port */
            foreach (string curArg in e.Args) {
                    if (ValidIPRegex.IsMatch(curArg)) {
                        model.Ip = curArg;
                    }
                    else if (int.TryParse(curArg, out port) && port >= 0 && port <= 65535) {
                        model.Port = port;
                    }
                }
            

            /* View Model */
            mainWindow.MyMap.SetVM(new MapVM(model));
            mainWindow.MyConnectionButtons.SetVM(new ConnectionButtonsVM(model));
            mainWindow.MyCockpitDashboard.SetVM(new CockpitDashboardVM(model));
            mainWindow.MyCockpitControls.setVM(new CockpitControlsVM(model));

            mainWindow.Show();
        }
    }
}