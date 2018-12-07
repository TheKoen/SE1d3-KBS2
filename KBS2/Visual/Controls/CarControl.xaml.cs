using KBS2.CarSystem;
using KBS2.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KBS2.Visual.Controls
{
    /// <summary>
    /// Interaction logic for CarControl.xaml
    /// </summary>
    public partial class CarControl : UserControl
    {

        public Car car { get; set; }

        public CarControl(Car car)
        {
            this.car = car;            
            Update();
            MainScreen.WPFLoop.Subscribe(Update);
            InitializeComponent();
        }

        public void Update()
        {
            var angle = -MathUtil.VectorToAngle(car.Rotation, DirectionCar.North);
            while (angle < 0) angle += 360;
            if (angle >= 360) angle -= 360;
            RenderTransform = new RotateTransform(angle, 0.5, 0.5);

            var rotation = car.Rotation;
            var xoffset = Vector.Multiply(MathUtil.Normalize(MathUtil.RotateVector(rotation, 90)), car.Width / 2d);
            var yoffset = Vector.Multiply(MathUtil.Normalize(MathUtil.RotateVector(xoffset, 90)), car.Length / 2d);

            var location = new Vector(car.Location.X, car.Location.Y);
            location = Vector.Subtract(location, xoffset);
            location = Vector.Subtract(location, yoffset);

            Margin = new Thickness(location.X, location.Y, 0, 0);
        }
    }
}
