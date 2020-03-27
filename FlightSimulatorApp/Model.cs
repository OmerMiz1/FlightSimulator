using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;

namespace FlightSimulatorApp
{
    public partial class Model : Component
    {
        public Model()
        {
            InitializeComponent();
        }

        public Model(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
    }
}
