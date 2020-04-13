﻿using System.Windows;
using System.Windows.Controls;
using FlightSimulatorApp.ViewModel;

namespace FlightSimulatorApp.Controls
{
    /// <summary>
    ///     Interaction logic for ConnectionButtons.xaml
    /// </summary>
    public partial class ConnectionButtons : UserControl
    {
        private ConnectionButtonsVM _myVM;

        public ConnectionButtonsVM ViewModel {
            set {
                _myVM = value;
                DataContext = _myVM;
            }
        }

        public ConnectionButtons()
        {
            InitializeComponent();
        }

        public string IP {
            get {
                if (_myVM == null) {
                    return App.DefaultIp;
                }
                return _myVM.Ip;
            }
            set
            {
                if (_myVM == null) return;
                _myVM.Ip = value;
            }
        }

        public string Port {
            get {
                if (_myVM == null) {
                    return App.DefaultPort.ToString();
                }
                return _myVM.Port.ToString();
            }
            set
            {
                if (_myVM == null) return;
                _myVM.Port = int.Parse(value);
            }
        }

        private void connectButton_Click(object sender, RoutedEventArgs e)
        {
            _myVM.Connect();
        }

        private void disconnectButton_Click(object sender, RoutedEventArgs e)
        {
            _myVM.Disconnect();
        }

        private void settingsButton_Click(object sender, RoutedEventArgs e)
        {
            var mySettings = new SettingsWindow(IP, Port, this);
            _myVM.ConnectButtonEnabled = false;
            _myVM.DisconnectButtonEnabled = false;
            _myVM.SettingsButtonEnabled = false;
            mySettings.Show();
        }

        public void notifySettingsEnded(string newIP, string newPort)
        {
            IP = newIP;
            Port = newPort;
            _myVM.ConnectButtonEnabled = true;
            _myVM.DisconnectButtonEnabled = false;
            _myVM.SettingsButtonEnabled = true;
        }
    }
}