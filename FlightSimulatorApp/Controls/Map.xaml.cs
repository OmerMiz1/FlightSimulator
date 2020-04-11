﻿using Microsoft.Maps.MapControl.WPF;
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
        private MapVM _myVM;
        private Image _airplaneImage;
        private MapLayer layer = new MapLayer();
        public Map() { InitializeComponent(); }

        public void SetVM(MapVM viewModel)
        {
            _myVM = viewModel;
            _myVM.PropertyChanged += MyVM_PropertyChanged;
            _airplaneImage = new Image();
            _airplaneImage.Width = 30;
            _airplaneImage.Height = 30;
            
            layer.AddChild(_airplaneImage, new Location(0,0));
            myMap.Children.Add(layer);
        }

        private void MyVM_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            /** Explanation for below at, also creating a new Location instance is a MUST.
             * If not using a new instance, the map wont update the location of the pushpin! More at:
             * https://stackoverflow.com/questions/9732709/the-calling-thread-cannot-access-this-object-because-a-different-thread-owns-it
             */
            Dispatcher.Invoke(() => {
                AirplanePushpin.Heading = ((MapVM)sender).Heading;
                AirplanePushpin.Location = new Location((sender as MapVM)?.Location);
            });
        }
    }
}
