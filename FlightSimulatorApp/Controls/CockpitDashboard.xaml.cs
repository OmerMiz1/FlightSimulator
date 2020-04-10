using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using FlightSimulatorApp.Model;
using FlightSimulatorApp.ViewModel;

namespace FlightSimulatorApp.Controls {
    /// <summary>
    /// Interaction logic for CockpitDashboard.xaml
    /// </summary>
    public partial class CockpitDashboard : UserControl {
        private CockpitDashboardVM _myVM;

        public CockpitDashboard() { InitializeComponent(); }

        public void SetVM(CockpitDashboardVM viewModel) {
            _myVM = viewModel;
            DataContext = _myVM;
        }
    }
}