using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FlightSimulatorApp.Controls {
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window {

        // Regex taken from https://www.regextester.com/22
        Regex ValidIPRegex = new Regex("^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$");
        private bool portIsValid = true;
        private bool IPIsValid = true;
        private string initialIP;
        private string initialPort;
        private ConnectionButtons caller;

        public SettingsWindow(string IP, string Port, ConnectionButtons connectionButtons) {
            InitializeComponent();
            this.initialIP = IP;
            IPTextBox.Text = IP;
            this.initialPort = Port;
            PortTextBox.Text = Port;
            this.caller = connectionButtons;
        }

        private void IPTextBox_TextChanged(object sender, TextChangedEventArgs e) {
            MatchCollection matches = ValidIPRegex.Matches(IPTextBox.Text);
            if (matches.Count > 0)
            {
                IPIsValid = true;
            }
            else
            {
                IPIsValid = false;
            }
            updateStatus();
        }

        private void PortTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            int port = -1;
            if (int.TryParse(PortTextBox.Text, out port) && port >= 0 && port <= 65535)
            {
                portIsValid = true;
            }
            else
            {
                portIsValid = false;
            }
            updateStatus();
        }

        private void updateStatus()
        {
            if (StatusLabel == null) {
                return;
            }
            StatusLabel.Content = "Status: ";
            if (IPIsValid)
            {
                StatusLabel.Content += "IP is valid, ";
            }
            else
            {
                StatusLabel.Content += "IP isn't valid, ";
            }
            if (portIsValid)
            {
                StatusLabel.Content += "port is valid.";
            }
            else
            {
                StatusLabel.Content += "port isn't valid.";
            }
            if (IPIsValid && portIsValid)
            {
                StatusLabel.Foreground = Brushes.Green;
                OKButton.IsEnabled = true;
            }
            else
            {
                StatusLabel.Foreground = Brushes.Red;
                OKButton.IsEnabled = false;
            }
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.caller.notifySettingsEnded(IPTextBox.Text, PortTextBox.Text);
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e) {
            
            this.caller.notifySettingsEnded(initialIP, initialPort);
            this.Close();
        }

        private void CancelButton_Click(object sender, System.ComponentModel.CancelEventArgs e) {

        }
    }
}
