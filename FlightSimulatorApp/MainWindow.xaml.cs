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
        public Map Map {
            set { MyMap = value; }
        }
        public ConnectionButtons ConnectionButtons {
            set { MyConnectionButtons = value; }
        }
        public CockpitDashboard CockpitDashboard {
            set { MyCockpitDashboard = value; }
        }
        public CockpitControls CockpitControls {
            set { MyCockpitControls = value; }
        }

        public MainWindow()
        {
            InitializeComponent();
        }
    }
}
