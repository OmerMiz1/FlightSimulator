using System.Configuration;
using System.Text.RegularExpressions;
using System.Windows;
using FlightSimulatorApp.Model;
using FlightSimulatorApp.ViewModel;

namespace FlightSimulatorApp
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        /* Default values declarations */
        public static string DefaultIp { get; } = "127.0.0.1";
        public static int DefaultPort { get; } = 5402;

        /* Custom startup method for the program.
         Accepts Port and IP in App.config. For each one of those, it will test validity and use if ok,
         other wise it will use the default value from above. */
        private void App_Startup(object sender, StartupEventArgs e)
        {
            var ValidIPRegex =
                new Regex(
                    "^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$");

            /* Initialize Model:
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

            /* Initialize Views & View Models */
            MainWindow mainWindow = new MainWindow();
            mainWindow.MyMap.ViewModel = new MapVM(model);
            mainWindow.MyConnectionButtons.ViewModel = new ConnectionButtonsVM(model);
            mainWindow.MyCockpitDashboard.ViewModel = new CockpitDashboardVM(model);
            mainWindow.MyCockpitControls.ViewModel = new CockpitControlsVM(model);

            mainWindow.Show();
        }
    }
}