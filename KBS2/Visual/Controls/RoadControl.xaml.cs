using KBS2.CitySystem;
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
    /// Interaction logic for RoadControl1.xaml
    /// </summary>
    public partial class RoadControl : UserControl
    {
        public RoadControl(Vector start, Vector end, int width)
        {
            var length = MathUtil.Distance(start, end);
            this.Width = length;
            this.Height = width;

            Road road = new Road(start, end, width, 100);

            if (!road.IsXRoad())
            {
                RenderTransform = new RotateTransform(-90);
            }

            if ((start.X + start.Y) < (end.X + end.Y))
            {
                Margin = new Thickness(start.X, end.Y, 0, 0);
            }
            else
            {
                Margin = new Thickness(end.X, start.Y, 0, 0);
            }
            InitializeComponent();
        }
    }
}
