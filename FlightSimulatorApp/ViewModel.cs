using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;

namespace FlightSimulatorApp
{
    public partial class ViewModel : Component
    {
        public ViewModel()
        {
            InitializeComponent();
        }

        public ViewModel(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
    }
}
