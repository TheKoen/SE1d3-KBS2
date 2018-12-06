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
            var rotation = MathUtil.VectorToAngle(car.Rotation, DirectionCar.North);
            App.Console.Print($"Car rotation {rotation} ");
            RenderTransform = new RotateTransform(rotation);
            Margin = new Thickness(car.Location.X, car.Location.Y, 0, 0);           
        }
    }
}
