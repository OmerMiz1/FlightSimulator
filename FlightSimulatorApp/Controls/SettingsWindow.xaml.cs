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

        public SettingsWindow() {
            InitializeComponent();
        }

        private void IPTextBox_TextChanged(object sender, TextChangedEventArgs e) {
            if (StatusLabel == null)
            {
                return;
            }
            MatchCollection matches = ValidIPRegex.Matches(IPTextBox.Text);
            if (matches.Count > 0)
            {
                this.StatusLabel.Content = "Status: Good";
            }
            else
            {
                this.StatusLabel.Content = "Status: Bad";
            }
        }
    }
}
