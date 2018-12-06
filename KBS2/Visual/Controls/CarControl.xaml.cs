using KBS2.CarSystem;
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

        public CarControl(Car c)
        {
            this.car = c;
            RenderTransform = new RotateTransform();
            Margin = new Thickness(c.Location.X, c.Location.Y, 0, 0);
            InitializeComponent();
        }

        public void ()
        {
            return ;
        }
    }
}
