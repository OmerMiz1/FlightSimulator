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
    public partial class ConnectionButtons : UserControl {
        private ConnectionButtonsVM _myVM;
        public string IP { get; set; }
        public string port { get; set; }

        public ConnectionButtons() {
            InitializeComponent();
        }

        public void SetVM(ConnectionButtonsVM viewModel) {
            _myVM = viewModel;
            _myVM.PropertyChanged += VM_PropertyChanged;
        }

        private void connectButton_Click(object sender, RoutedEventArgs e) {
            _myVM.Connect();
            connectButton.IsEnabled = false;
            disconnectButton.IsEnabled = true;
            settingsButton.IsEnabled = false;
        }

        private void disconnectButton_Click(object sender, RoutedEventArgs e) {
            _myVM.Disconnect();
            connectButton.IsEnabled = true;
            disconnectButton.IsEnabled = false;
            settingsButton.IsEnabled = true;
        }

        private void settingsButton_Click(object sender, RoutedEventArgs e) {
            SettingsWindow mySettings = new SettingsWindow(IP, port, this);
            connectButton.IsEnabled = false;
            disconnectButton.IsEnabled = false;
            settingsButton.IsEnabled = false;
            mySettings.Show();
        }

        private void VM_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (_myVM.IsConnected) {
                connectButton.IsEnabled = false;
                disconnectButton.IsEnabled = true;
                settingsButton.IsEnabled = false;
                return;
            }
            connectButton.IsEnabled = true; 
            disconnectButton.IsEnabled = false;
            settingsButton.IsEnabled = true;
        }

        public void notifySettingsEnded(string newIP, string newPort)
        {
            IP = newIP;
            port = newPort;
            connectButton.IsEnabled = true;
        }
    }
}
