using System.Windows;
using System.Windows.Controls;
using FlightSimulatorApp.ViewModel;

namespace FlightSimulatorApp.Controls
{
    /// <summary>
    ///     Interaction logic for CockpitControls.xaml
    /// </summary>
    public partial class CockpitControls : UserControl
    {
        private CockpitControlsVM? _myVM;

        public CockpitControls()
        {
            InitializeComponent();
            Joystick.AddXValueChanged(Rudder_ValueChanged);
            Joystick.AddYValueChanged(Elevator_ValueChanged);
        }

        public void setVM(CockpitControlsVM viewModel)
        {
            _myVM = viewModel;
        }

        private void AileronSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_myVM == null) return;
            _myVM.Aileron = e.NewValue;
        }

        private void ThrottleSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_myVM == null) return;
            _myVM.Throttle = e.NewValue;
        }

        private void Rudder_ValueChanged(double newValue)
        {
            if (_myVM == null) return;
            _myVM.Rudder = newValue;
        }

        private void Elevator_ValueChanged(double newValue)
        {
            if (_myVM == null) return;
            _myVM.Elevator = newValue;
        }
    }
}