using System.Windows;

namespace FlightSimulatorApp
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //TODO if window is closed make sure to disconnect server
        }
    }
}