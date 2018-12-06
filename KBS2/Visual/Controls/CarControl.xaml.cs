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
            var angle = MathUtil.VectorToAngle(car.Rotation, DirectionCar.North);
            App.Console.Print($"Car rotation {angle} ");
            RenderTransform = new RotateTransform(angle);
            var rotation = car.Rotation;
            var xoffset = MathUtil.RotateVector(rotation, 90);
            xoffset.Normalize();
            Vector.Multiply(xoffset, car.Width / 2d);
            var yoffset = MathUtil.RotateVector(xoffset, 90);
            yoffset.Normalize();
            Vector.Multiply(yoffset, car.Length / 2d);

            var location = new Vector(car.Location.X, car.Location.Y);
            Vector.Add(location, xoffset);
            Vector.Add(location, yoffset);

            Margin = new Thickness(location.X, location.Y, 0, 0);           
        }
    }
}
