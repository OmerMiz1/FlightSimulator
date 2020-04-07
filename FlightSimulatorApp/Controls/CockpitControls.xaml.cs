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
using FlightSimulatorApp.ViewModel;

namespace FlightSimulatorApp.Controls
{
    /// <summary>
    /// Interaction logic for CockpitControls.xaml
    /// </summary>
    public partial class CockpitControls : UserControl
    {

        private CockpitControlsVM _myVM;
        public CockpitControls()
        {
            InitializeComponent();
        }

        public void setVM(CockpitControlsVM newVM)
        {
            this._myVM = newVM;
            this.DataContext = this._myVM;
            Binding xToElevators = new Binding("Elevators");
            xToElevators.Source = this.Joystick.X;
        }
    }
}
