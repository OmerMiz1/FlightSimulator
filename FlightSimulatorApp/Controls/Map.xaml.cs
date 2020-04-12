using System.ComponentModel;
using System.Windows.Controls;
using FlightSimulatorApp.ViewModel;
using Microsoft.Maps.MapControl.WPF;

namespace FlightSimulatorApp.Controls
{
    /// <summary>
    ///     Interaction logic for Map.xaml
    /// </summary>
    public partial class Map : UserControl
    {
        private MapVM _myVM;

        public Map()
        {
            InitializeComponent();
        }

        public void SetVM(MapVM viewModel)
        {
            _myVM = viewModel;
            _myVM.PropertyChanged += MyVM_PropertyChanged;
        }

        private void MyVM_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            /** Explanation for below at, also creating a new Location instance is a MUST.
             * If not using a new instance, the map wont update the location of the pushpin! More at:
             * https://stackoverflow.com/questions/9732709/the-calling-thread-cannot-access-this-object-because-a-different-thread-owns-it
             */
            Dispatcher.Invoke(() =>
            {
                AirplanePushpin.Heading = ((MapVM) sender).Heading;
                AirplanePushpin.Location = new Location((sender as MapVM)?.Location);
            });
        }
    }
}