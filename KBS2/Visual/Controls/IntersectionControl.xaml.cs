using KBS2.CitySystem;
using System;
using System.Windows;
using System.Windows.Controls;

namespace KBS2.Visual.Controls
{
    /// <summary>
    /// Interaction logic for IntersectionControl.xaml
    /// </summary>
    public partial class IntersectionControl : UserControl
    {
        private Intersection Intersection { get; }
        private MainScreen Screen { get; }

        private float lastZoom;

        public IntersectionControl(MainScreen screen, Intersection intersection)
        {
            Screen = screen;
            Intersection = intersection;
            InitializeComponent();
            
            Update();
            var Roads = intersection.GetRoads();

            MainScreen.CommandLoop.Subscribe(Update);

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
                {
                    //boven onder
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

        private void Update()
        {
            if (Math.Abs(lastZoom - Screen.Zoom) < 0.01)
            {
                return;
            }
            lastZoom = Screen.Zoom;

            var zoom = Screen.Zoom;
            var size = Intersection.Size;
            Height = size * zoom;
            Width = size * zoom;
            var offset = size / 2d;
            Margin = new Thickness((Intersection.Location.X - offset) * zoom, (Intersection.Location.Y - offset) * zoom, 0, 0);
            UpdateStripes();
        }

        public void UpdateStripes()
        {
            const double mult = 0.1;
            var zoom = Screen.Zoom;

            // IntersectionStripeTop
            IntersectionStripeTop.Height = (Height / 2d) + zoom;
            IntersectionStripeTop.Width = Width * mult;
            IntersectionStripeTop.Margin = 
                new Thickness(Width / 2d - IntersectionStripeTop.Width / 2d, 0, 0, 0);

            // IntersectionStripeBottom
            IntersectionStripeBottom.Height = (Height / 2d) + zoom;
            IntersectionStripeBottom.Width = Width * mult;
            IntersectionStripeBottom.Margin =
                new Thickness(Width / 2d - IntersectionStripeTop.Width / 2d, Height / 2d, 0, 0);

            // IntersectionStripeLeft
            IntersectionStripeLeft.Height = Width * mult;
            IntersectionStripeLeft.Width = (Height / 2d) + zoom;
            IntersectionStripeLeft.Margin = new Thickness(0, Height / 2d - IntersectionStripeLeft.Height / 2d, 0, 0);

            // IntersectionStripeRight
            IntersectionStripeRight.Height = Width * mult;
            IntersectionStripeRight.Width = (Height / 2d) + zoom;
            IntersectionStripeRight.Margin =
                new Thickness(Width / 2d, Height / 2d - IntersectionStripeLeft.Height / 2d, 0, 0);
        }
    }
}