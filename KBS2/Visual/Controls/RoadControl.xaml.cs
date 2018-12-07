using KBS2.CitySystem;
using KBS2.Util;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace KBS2.Visual.Controls
{
    /// <summary>
    /// Interaction logic for RoadControl1.xaml
    /// </summary>
    public partial class RoadControl : UserControl
    {
        public RoadControl(Road road)
        {
            var length = MathUtil.Distance(road.Start, road.End);
            Width = length;
            Height = road.Width;

            var offset = road.Width / 2d;

            if (road.Start.X + road.Start.Y < road.End.X + road.End.Y)
            {
                Margin = road.IsXRoad() ? new Thickness(road.Start.X, road.End.Y - offset, 0, 0) : new Thickness(road.Start.X - offset, road.End.Y, 0, 0);
            }
            else
            {
                Margin = road.IsXRoad() ? new Thickness(road.End.X, road.Start.Y - offset, 0, 0) : new Thickness(road.End.X - offset, road.Start.Y, 0, 0);
                
            }

            this.RenderTransform = road.IsXRoad() ? new RotateTransform(0) : new RotateTransform(-90);


            InitializeComponent();
        }
    }
}
