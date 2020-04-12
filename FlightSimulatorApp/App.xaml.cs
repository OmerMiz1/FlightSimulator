using System.Text.RegularExpressions;
using System.Windows;
using FlightSimulatorApp.Model;
using FlightSimulatorApp.ViewModel;

namespace FlightSimulatorApp
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private const string DEFAULT_IP = "127.0.0.1";
        private const int DEFAULT_PORT = 5402;
        public MapVM mapViewModel { get; internal set; }

        private void App_Startup(object sender, StartupEventArgs e)
        {
            var ValidIPRegex =
                new Regex(
                    "^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$");

            /* View */
            var mainWindow = new MainWindow();

            /* Model */
            var model = new SimulatorModel();
            /* No Args (or not enough) args so use default values */
            model.Ip = DEFAULT_IP;
            model.Port = DEFAULT_PORT;
            int port;

            /* Try to parse arguments into ip and port */
            foreach (var curArg in e.Args)
                if (ValidIPRegex.IsMatch(curArg))
                    model.Ip = curArg;
                else if (int.TryParse(curArg, out port) && port >= 0 && port <= 65535) model.Port = port;


            /* View Model */
            mainWindow.MyMap.SetVM(new MapVM(model));
            mainWindow.MyConnectionButtons.SetVM(new ConnectionButtonsVM(model));
            mainWindow.MyCockpitDashboard.SetVM(new CockpitDashboardVM(model));
            mainWindow.MyCockpitControls.setVM(new CockpitControlsVM(model));

            mainWindow.Show();
        }
    }
}