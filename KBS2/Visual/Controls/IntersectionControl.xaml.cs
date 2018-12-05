using KBS2.CitySystem;
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
    /// Interaction logic for IntersectionControl.xaml
    /// </summary>
    public partial class IntersectionControl : UserControl
    {
        public IntersectionControl(Intersection intersection)
        {
            InitializeComponent();
            var size = intersection.Size;

            Height = size;
            Width = size;
            var c = size / 2d;
            setStripes();
            Margin = new Thickness(intersection.Location.X-c, intersection.Location.Y, 0 , 0);
            var Roads = intersection.GetRoads();

            foreach (var road in Roads)
            {
                if (road.IsXRoad())
                {
                    //links rechts
                    var RoadAvgX = (road.Start.X + road.End.X) / 2d;
                    if (intersection.Location.X > RoadAvgX)
                    {
                        IntersectionStripeLeft.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        IntersectionStripeRight.Visibility = Visibility.Visible;
                    }
                }
                else
                {   //boven onder
                    var RoadAvgY = (road.Start.Y + road.End.Y) / 2d;
                    if (intersection.Location.Y > RoadAvgY)
                    {
                        IntersectionStripeTop.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        IntersectionStripeBottom.Visibility = Visibility.Visible;
                    }
                }
            }
        }

        public void setStripes()
        {
            var mult = 0.125;
            // IntersectionStripeTop
            IntersectionStripeTop.Height = Height/2d;
            IntersectionStripeTop.Width = Width* mult;
            IntersectionStripeTop.Margin = new Thickness(Width/2d - IntersectionStripeTop.Width/2d, 0, 0, 0);

            // IntersectionStripeBottom
            IntersectionStripeBottom.Height = Height / 2d;
            IntersectionStripeBottom.Width = Width * mult;
            IntersectionStripeBottom.Margin = new Thickness(Width / 2d - IntersectionStripeTop.Width / 2d, Height / 2d, 0, 0);

            // IntersectionStripeLeft
            IntersectionStripeLeft.Height = Width * mult;
            IntersectionStripeLeft.Width = Height / 2d;
            IntersectionStripeLeft.Margin = new Thickness(0, Height / 2d - IntersectionStripeLeft.Height/2d, 0, 0);

            // IntersectionStripeRight
            IntersectionStripeRight.Height = Width * mult;
            IntersectionStripeRight.Width = Height / 2d;
            IntersectionStripeRight.Margin = new Thickness(Width / 2d, Height / 2d - IntersectionStripeLeft.Height/2d, 0, 0);
        }
    }
}
