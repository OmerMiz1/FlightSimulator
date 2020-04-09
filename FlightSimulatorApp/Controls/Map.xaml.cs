using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
    /// Interaction logic for Map.xaml
    /// </summary>
    public partial class Map : UserControl
    {
        private MapVM myVM;
        private Image airplaneImage;

        /* CTOR FOR DEBUG AND TESTING.. */
        public Map() 
        {
            InitializeComponent();
        }

        public Map(MapVM mapViewModel)
        {
            InitializeComponent();
            myVM = mapViewModel;
            myVM.PropertyChanged += MyVM_PropertyChanged;
            AirplanePushpin.
        }

        public void SetVM(MapVM mapViewModel)
        {
            myVM = mapViewModel;
            myVM.PropertyChanged += MyVM_PropertyChanged;
        }

        /***
         ** This is a work around so the map will get updated
         ** whenever the airplane location has updated.
         ***/
        private void MyVM_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            /** Explanation for below at:
             * https://stackoverflow.com/questions/9732709/the-calling-thread-cannot-access-this-object-because-a-different-thread-owns-it
             */
            Dispatcher.Invoke(() =>
            {
                // AirplanePushpin.Location = new Location((sender as MapVM).Location);
                AirplanePushpin.Location = new Location((sender as MapVM)?.Location);
                Debug.WriteLine("Inside Map.MyVM_PropertyChanged, Location="+AirplanePushpin.Location);
            });
            
        }
    }
}
