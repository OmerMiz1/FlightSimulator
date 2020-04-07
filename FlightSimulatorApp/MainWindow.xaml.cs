using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using FlightSimulatorApp.Controls;
using FlightSimulatorApp.Model;
using FlightSimulatorApp.ViewModel;

namespace FlightSimulatorApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            /*TODO get from StartUpEventArgs the args for tcp client (default)...*/
            /* Default Values.. TODO Move these to AppConfig!*/

            TcpClient tcpClient = new TcpClient("127.0.0.1", 5402);
            SimulatorModel simulatorModel = new SimulatorModel(tcpClient, true);
            simulatorModel.Connect("127.0.0.1", 5402);
            simulatorModel.Start();

            MyMap = new Map(new MapVM(simulatorModel));
        }
    }
}
