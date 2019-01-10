using KBS2.CitySystem;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

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

            MainScreen.ZoomLoop.Subscribe(Update);

            foreach (var road in Roads)
            {
                if (road.IsXRoad())
                {
                    //links rechts
                    var RoadAvgX = (road.Start.X + road.End.X) / 2d;
                    if (intersection.Location.X > RoadAvgX)
                    {
                        IntersectionStripeLeft.Visibility = Visibility.Visible;
                    } else
                    {
                        IntersectionStripeRight.Visibility = Visibility.Visible;
                    }
                } else
                {
                    //boven onder
                    var RoadAvgY = (road.Start.Y + road.End.Y) / 2d;
                    if (intersection.Location.Y > RoadAvgY)
                    {
                        IntersectionStripeTop.Visibility = Visibility.Visible;
                    } else
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
            var newHeight = size * zoom;
            var newWidth = size * zoom;
            var offset = size / 2d;
            var newMargin = new Thickness((Intersection.Location.X - offset) * zoom, (Intersection.Location.Y - offset) * zoom, 0, 0);

            MainScreen.DrawingLoop.EnqueueAction(() =>
            {
                Margin = newMargin;
                Height = newHeight;
                Width = newWidth;

                UpdateStripes();
            });
        }

        public void UpdateStripes()
        {
            const double mult = 0.1;
            var zoom = Screen.Zoom;

            // IntersectionStripeTop
            RescaleElement(
                IntersectionStripeTop,
                Width * mult, Height / 2d + zoom,
                new Thickness(Width / 2d - IntersectionStripeTop.Width / 2d, 0, 0, 0)
            );

            // IntersectionStripeBottom
            RescaleElement(
                IntersectionStripeBottom,
                Width * mult, (Height / 2d) + zoom,
                new Thickness(Width / 2d - IntersectionStripeTop.Width / 2d, Height / 2d, 0, 0)
            );

            // IntersectionStripeLeft
            RescaleElement(
                IntersectionStripeLeft,
                (Height / 2d) + zoom, Width * mult,
                new Thickness(0, Height / 2d - IntersectionStripeLeft.Height / 2d, 0, 0)
            );

            // IntersectionStripeRight
            RescaleElement(
                IntersectionStripeRight,
                (Height / 2d) + zoom, Width * mult,
                new Thickness(Width / 2d, Height / 2d - IntersectionStripeLeft.Height / 2d, 0, 0)
            );
        }

        private static void RescaleElement(FrameworkElement rectangle, double width, double height, Thickness margin)
        {
            if (double.IsNaN(width) || double.IsNaN(height)) return;

            rectangle.Width = width;
            rectangle.Height = height;
            rectangle.Margin = margin;
        }
    }
}