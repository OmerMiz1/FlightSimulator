using System;
using System.Collections.Generic;
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
    public partial class Joystick : UserControl {
        private bool mouseIsDown = false;
        private Point startingPoint;
        private Storyboard myStoryboard;

        public Joystick() {
            InitializeComponent();
            myStoryboard = (Storyboard)Knob.FindResource("CenterKnob");
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
            }
        }

        private void centerKnob_Completed(object? sender, EventArgs e) {
            myStoryboard.Stop();
        }
    }
}
