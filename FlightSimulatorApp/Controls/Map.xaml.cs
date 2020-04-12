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

        public MapVM ViewModel {
            set {
                _myVM = value;
                _myVM.PropertyChanged += MyVM_PropertyChanged;
            }
        }
        public Map()
        {
            InitializeComponent();
        }

        /*** Function retrieves the location value from its model.
         * In order to print the airplane at the correct location. The location property of the
         * airplane (pushpin) must get a new instance other wise the map wont update it location
         * in real time.
         ***/
        private void MyVM_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            /* More at:
             https://stackoverflow.com/questions/9732709/the-calling-thread-cannot-access-this-object-because-a-different-thread-owns-it */
            Dispatcher.Invoke(() =>
            {
                AirplanePushpin.Heading = ((MapVM) sender).Heading;
                AirplanePushpin.Location = new Location((sender as MapVM)?.Location);
            });
        }
    }
}