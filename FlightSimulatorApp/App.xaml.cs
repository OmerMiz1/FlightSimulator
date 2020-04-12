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

            MainWindow mainWindow = new MainWindow();

            /* Instantiate the model:
             If ip and port are provided in App.config parse them else use default values.*/
            SimulatorModel model = new SimulatorModel();
            model.Ip = DefaultIp;
            model.Port = DefaultPort;
            int port = -1;
            try {
                var appSettings = ConfigurationManager.AppSettings;
                if (appSettings.Count > 0) {
                    foreach (var argKey in appSettings.AllKeys) {
                        /* Parse ip and validate */
                        if (argKey.ToLower().Contains("ip") && ValidIPRegex.IsMatch(appSettings[argKey])) {
                            model.Ip = appSettings[argKey];
                        }
                        /* Parse port and validate */
                        else if (argKey.ToLower().Contains("port") && int.TryParse(appSettings[argKey], out port) &&
                                 port >= 0 && port <= 65535) {
                            model.Port = port;
                        }
                    }
                }
            }
            catch (ConfigurationErrorsException ce) {
                // DO NOTHING
            }
            /* Try to parse arguments into ip and port */ /*
            foreach (string curArg in e.Args) {
                if (ValidIPRegex.IsMatch(curArg)) {
                    model.Ip = curArg;
                }
                else if (int.TryParse(curArg, out port) && port >= 0 && port <= 65535) {
                    model.Port = port;
                }
            }*/

            /* View Model */
            mainWindow.MyMap.SetVM(new MapVM(model));
            mainWindow.MyConnectionButtons.SetVM(new ConnectionButtonsVM(model));
            mainWindow.MyCockpitDashboard.SetVM(new CockpitDashboardVM(model));
            mainWindow.MyCockpitControls.setVM(new CockpitControlsVM(model));

            mainWindow.Show();
        }
    }
}