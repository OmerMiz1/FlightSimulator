using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
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

namespace FlightSimulatorApp.Controls {
    /// <summary>
    /// Interaction logic for ConnectionButtons.xaml
    /// </summary>
    public partial class ConnectionButtons : UserControl, INotifyPropertyChanged {
        private ConnectionButtonsVM myVM;
        public event PropertyChangedEventHandler PropertyChanged;

        public ConnectionButtons() {
            InitializeComponent();
        }

        public void SetVM(ConnectionButtonsVM viewModel) {
            myVM = viewModel;
            myVM.PropertyChanged += VM_PropertyChanged;
        }

        private void connectButton_Click(object sender, RoutedEventArgs e) {
            myVM.Connect();
        }

        private void disconnectButton_Click(object sender, RoutedEventArgs e) {
            myVM.Disconnect();
        }

        private void settingsButton_Click(object sender, RoutedEventArgs e) {
            SettingsWindow mySettings = new SettingsWindow();
            mySettings.Show();
        }
        
        public void NotifyPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void VM_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (myVM.IsConnected) {
                connectButton.IsEnabled = false;
                disconnectButton.IsEnabled = true;
                settingsButton.IsEnabled = false;
                return;
            }
            connectButton.IsEnabled = true; 
            disconnectButton.IsEnabled = false;
            settingsButton.IsEnabled = true;
        }


    }
}
