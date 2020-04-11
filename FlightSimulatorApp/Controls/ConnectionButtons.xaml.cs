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

        public string IP
        {
            get
            {
                if (_myVM == null)
                {
                    return "127.0.0.1";
                }
                return _myVM.Ip;
            }
            set
            {
                if (_myVM == null)
                {
                    return;
                }
                _myVM.Ip = value;
            }
        }

        public string Port
        {
            get {
                if (_myVM == null) {
                    return "5402";
                }
                return _myVM.Port.ToString();
            }
            set {
                if (_myVM == null) {
                    return;
                }
                _myVM.Port = int.Parse(value);
            }
        }

        public ConnectionButtons() {
            InitializeComponent();
        }

        public void SetVM(ConnectionButtonsVM viewModel) {
            _myVM = viewModel;
            _myVM.PropertyChanged += VM_PropertyChanged;
            DataContext = _myVM;
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
            SettingsWindow mySettings = new SettingsWindow(IP, Port, this);
            connectButton.IsEnabled = false;
            disconnectButton.IsEnabled = false;
            settingsButton.IsEnabled = false;
            mySettings.Show();
        }

        private void VM_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (_myVM.StatusColor == Brushes.Green) {
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
            Port = newPort;
            connectButton.IsEnabled = true;
        }
    }
}
