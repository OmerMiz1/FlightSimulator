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

        public double Rudder { get; set; }
        public double Elevator { get; set; }

        private CockpitControlsVM? _myVM;
        public CockpitControls()
        {
            InitializeComponent();
            this.Joystick.AddsXValueChanged(Rudder_ValueChanged);
            this.Joystick.AddYValueChanged(Elevator_ValueChanged);
        }

        public void setVM(CockpitControlsVM newVM)
        {
            this._myVM = newVM;
        }

        private void AileronSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            if (_myVM == null)
            {
                return;
            }
            this._myVM.Ailerons = e.NewValue;
        }

        private void ThrottleSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            if (_myVM == null) {
                return;
            }
            this._myVM.Throttle = e.NewValue;
        }

        private void Rudder_ValueChanged(double newValue)
        {
            if (_myVM == null) {
                return;
            }
            this._myVM.Rudder = newValue;
        }
        
        private void Elevator_ValueChanged(double newValue)
        {
            if (_myVM == null) {
                return;
            }
            this._myVM.Elevators = newValue;
        }
    }
}
