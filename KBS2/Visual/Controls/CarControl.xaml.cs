using KBS2.CarSystem;
using KBS2.Util;
using KBS2.Visual;
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
        public MainScreen Screen { get; set; }

        public CarControl(Car car, MainScreen screen)
        {
            Screen = screen;
            this.car = car;
            InitializeComponent();
            Update();
            MainScreen.ZoomLoop.Subscribe(Update);
        }

        public void Update()
        {
            if (double.IsNaN(car.Location.X))
            {
                MainScreen.DrawingLoop.EnqueueAction(() => Screen.CanvasMain.Children.Remove(this));
            }

            var angle = -MathUtil.VectorToAngle(car.Rotation, DirectionCar.North);
            while (angle < 0) angle += 360;
            if (angle >= 360) angle -= 360;

            var rotation = car.Rotation;
            var xoffset = Vector.Multiply(MathUtil.Normalize(MathUtil.RotateVector(rotation, 90)), car.Width / 2d);
            var yoffset = Vector.Multiply(MathUtil.Normalize(MathUtil.RotateVector(xoffset, 90)), car.Length / 2d);

            var location = new Vector(car.Location.X, car.Location.Y);
            location = Vector.Subtract(location, xoffset);
            location = Vector.Subtract(location, yoffset);

            if (double.IsNaN(location.Length)) return;

            var zoom = Screen.Zoom;
            MainScreen.DrawingLoop.EnqueueAction(() =>
            {
                RenderTransform = new RotateTransform(angle, 0.5, 0.5);
                Margin = new Thickness(location.X * zoom, location.Y * zoom, 0, 0);
                CarRectangle.Width = 5 * zoom;
                CarRectangle.Height = 10 * zoom;
            });
        }

        public void Car_Select(object sender, MouseButtonEventArgs e)
        {
            //Empty info tab
            Screen.TabItemInfo.Content = null;
            
            //Add info about this car
            CarInfoUserControl ci = new CarInfoUserControl(car, Screen);
            Screen.TabItemInfo.Content = ci;

            //Open the info tab of selected car
            Screen.TabItemInfo.IsSelected = true;
        }
    }
}
