using System.Windows;
using System.Windows.Controls;
using FlightSimulatorApp.ViewModel;

namespace FlightSimulatorApp.Controls
{
    /// <summary>
    ///     Interaction logic for CockpitDashboard.xaml
    /// </summary>
    public partial class CockpitDashboard : UserControl
    {
        private CockpitDashboardVM _myVM;

        public CockpitDashboardVM ViewModel {
            set {
                _myVM = value;
                DataContext = _myVM;
            }
        }

        public CockpitDashboard()
        {
            InitializeComponent();
        }
    }
}