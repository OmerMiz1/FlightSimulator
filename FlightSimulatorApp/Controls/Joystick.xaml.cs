using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace FlightSimulatorApp.Controls
{
    /// <summary>
    ///     Interaction logic for Joystick.xaml
    /// </summary>
    public partial class Joystick : UserControl, INotifyPropertyChanged
    {
        public delegate void ValueChanged(double newX);

        private double _x;
        private double _y;
        private bool mouseIsDown;
        private readonly Storyboard myStoryboard;
        private Point startingPoint;

        private ValueChanged X_ValueChanged;
        private ValueChanged Y_ValueChanged;

        public Joystick()
        {
            InitializeComponent();
            myStoryboard = (Storyboard) Knob.FindResource("CenterKnob");
        }

        public double X
        {
            get => _x;
            set
            {
                if (_x == value) return;
                _x = value;
                OnPropertyChanged("X");
                X_ValueChanged(value);
            }
        }

        public double Y
        {
            get => _y;
            set
            {
                if (_y == value) return;
                _y = value;
                OnPropertyChanged("Y");
                Y_ValueChanged(value);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void AddXValueChanged(ValueChanged newDelegate)
        {
            X_ValueChanged += newDelegate;
        }

        public void AddYValueChanged(ValueChanged newDelegate)
        {
            Y_ValueChanged += newDelegate;
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void Knob_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Knob.ReleaseMouseCapture();
            myStoryboard.Begin();
            knobPosition.X = 0;
            knobPosition.Y = 0;
            mouseIsDown = false;
        }

        private void Knob_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (mouseIsDown == false)
            {
                mouseIsDown = true;
                startingPoint = e.GetPosition(this);
                Knob.CaptureMouse();
            }
        }

        private void Knob_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseIsDown)
            {
                Point endingPoint;
                endingPoint = e.GetPosition(this);
                var deltaX = endingPoint.X - startingPoint.X;
                var deltaY = endingPoint.Y - startingPoint.Y;
                var distance = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
                var radius = Base.Width / 2;
                if (distance <= radius)
                {
                    knobPosition.X = deltaX;
                    knobPosition.Y = deltaY;
                }
                else
                {
                    if (deltaX >= 0)
                    {
                        var angle = Math.Atan(deltaY / deltaX);
                        knobPosition.X = radius * Math.Cos(angle);
                        knobPosition.Y = radius * Math.Sin(angle);
                    }
                    else
                    {
                        var angle = Math.Atan(deltaY / deltaX);
                        knobPosition.X = -radius * Math.Cos(angle);
                        knobPosition.Y = -radius * Math.Sin(angle);
                    }
                }

                X = knobPosition.X / radius;
                Y = knobPosition.Y / radius;
            }
        }

        private void centerKnob_Completed(object? sender, EventArgs e)
        {
            myStoryboard.Stop();
            X = 0;
            Y = 0;
        }
    }
}