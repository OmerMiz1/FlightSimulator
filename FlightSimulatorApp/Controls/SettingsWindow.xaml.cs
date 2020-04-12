using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace FlightSimulatorApp.Controls
{
    /// <summary>
    ///     Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private readonly ConnectionButtons caller;
        private readonly string initialIP;
        private readonly string initialPort;
        private bool IPIsValid = true;
        private bool portIsValid = true;

        // Regex that validates that a certain string is a legit IP address (taken from https://www.regextester.com/22)
        private readonly Regex ValidIPRegex =
            new Regex(
                "^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$");

        public SettingsWindow(string IP, string Port, ConnectionButtons connectionButtons)
        {
            InitializeComponent();
            initialIP = IP;
            IPTextBox.Text = IP;
            initialPort = Port;
            PortTextBox.Text = Port;
            caller = connectionButtons;
        }

        private void IPTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var matches = ValidIPRegex.Matches(IPTextBox.Text);
            if (matches.Count > 0)
                IPIsValid = true;
            else
                IPIsValid = false;
            updateStatus();
        }

        private void PortTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            int port;
            if (int.TryParse(PortTextBox.Text, out port) && port >= 0 && port <= 65535)
                portIsValid = true;
            else
                portIsValid = false;
            updateStatus();
        }

        private void updateStatus()
        {
            if (StatusLabel == null) return;
            StatusLabel.Content = "Status: ";
            if (IPIsValid)
                StatusLabel.Content += "IP is valid, ";
            else
                StatusLabel.Content += "IP isn't valid, ";
            if (portIsValid)
                StatusLabel.Content += "Port is valid.";
            else
                StatusLabel.Content += "Port isn't valid.";
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
            caller.notifySettingsEnded(IPTextBox.Text, PortTextBox.Text);
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            caller.notifySettingsEnded(initialIP, initialPort);
            Close();
        }

        private void CancelButton_Click(object sender, CancelEventArgs e)
        {
            caller.notifySettingsEnded(initialIP, initialPort);
            Close();
        }
    }
}