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

        public CockpitDashboard()
        {
            InitializeComponent();
        }

        public void SetVM(CockpitDashboardVM viewModel)
        {
            _myVM = viewModel;
            DataContext = _myVM;
        }
    }
}