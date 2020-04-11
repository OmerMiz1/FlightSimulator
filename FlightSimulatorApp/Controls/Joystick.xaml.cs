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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FlightSimulatorApp.Controls {
    /// <summary>
    /// Interaction logic for Joystick.xaml
    /// </summary>
    public partial class Joystick : UserControl, INotifyPropertyChanged {
        private bool mouseIsDown = false;
        private Point startingPoint;
        private Storyboard myStoryboard;
        private double _x;
        private double _y;
        public delegate void ValueChanged(double newX);

        private ValueChanged X_ValueChanged = null;
        private ValueChanged Y_ValueChanged = null;

        public double X
        {
            get { return _x; }
            set
            {
                if (_x == value) return;
                _x = value;
                OnPropertyChanged("X");
                X_ValueChanged?.Invoke(value);
            }
        }

        public double Y {
            get { return _y; }
            set {
                if (_y == value) return;
                _y = value;
                OnPropertyChanged("Y");
                Y_ValueChanged?.Invoke(value);
            }
        }

        public void AddXValueChanged(ValueChanged newDelegate)
        {
            X_ValueChanged += newDelegate;
        }

        public void AddYValueChanged(ValueChanged newDelegate) {
            Y_ValueChanged += newDelegate;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Joystick() {
            InitializeComponent();
            myStoryboard = (Storyboard)Knob.FindResource("CenterKnob");
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void Knob_MouseUp(object sender, MouseButtonEventArgs e) {
            Knob.ReleaseMouseCapture();
            myStoryboard.Begin();
            knobPosition.X = 0;
            knobPosition.Y = 0;
            mouseIsDown = false;
        }

        private void Knob_MouseDown(object sender, MouseButtonEventArgs e) {
            if (mouseIsDown == false) {
                mouseIsDown = true;
                startingPoint = e.GetPosition(this);
                Knob.CaptureMouse();
            }
        }

        private void Knob_MouseMove(object sender, MouseEventArgs e) {
            if (mouseIsDown) {
                Point endingPoint;
                endingPoint = e.GetPosition(this);
                double deltaX = endingPoint.X - startingPoint.X;
                double deltaY = endingPoint.Y - startingPoint.Y;
                double distance = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
                double radius = Base.Width / 2;
                if (distance <= radius) {
                    knobPosition.X = deltaX;
                    knobPosition.Y = deltaY;
                } else {
                    if (deltaX >= 0) {
                        double angle = Math.Atan(deltaY / deltaX);
                        knobPosition.X = radius * Math.Cos(angle);
                        knobPosition.Y = radius * Math.Sin(angle);
                    } else {
                        double angle = Math.Atan(deltaY / deltaX);
                        knobPosition.X = -radius * Math.Cos(angle);
                        knobPosition.Y = -radius * Math.Sin(angle);
                    }
                }

                X = knobPosition.X / radius;
                Y = knobPosition.Y / radius;
            }
        }

        private void centerKnob_Completed(object? sender, EventArgs e) {
            myStoryboard.Stop();
            X = 0;
            Y = 0;
        }
    }
}
