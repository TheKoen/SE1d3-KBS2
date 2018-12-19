using System;
using KBS2.CitySystem;
using KBS2.Util;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace KBS2.Visual.Controls
{
    public partial class RoadControl : UserControl
    {
        private Road Road { get; }
        private MainScreen Screen { get; }

        private float lastZoom;

        public RoadControl(MainScreen screen, Road road)
        {
            Screen = screen;
            Road = road;

            InitializeComponent();
            Update();

            RenderTransform = road.IsXRoad() ? new RotateTransform(0) : new RotateTransform(-90);
            
            MainScreen.CommandLoop.Subscribe(Update);
        }

        private void Update()
        {
            if (Math.Abs(lastZoom - Screen.Zoom) < 0.01)
            {
                return;
            }
            lastZoom = Screen.Zoom;

            var zoom = Screen.Zoom;
            var length = MathUtil.Distance(Road.Start, Road.End);
            Width = length * zoom;
            Height = Road.Width * zoom;

            var offset = Road.Width / 2d;
            if (Road.Start.X + Road.Start.Y < Road.End.X + Road.End.Y)
            {
                Margin = Road.IsXRoad() 
                    ? new Thickness(Road.Start.X * zoom, (Road.End.Y - offset) * zoom, 0, 0) 
                    : new Thickness((Road.Start.X - offset) * zoom, Road.End.Y * zoom, 0, 0);
            }
            else
            {
                Margin = Road.IsXRoad() 
                    ? new Thickness(Road.End.X * zoom, (Road.Start.Y - offset) * zoom, 0, 0) 
                    : new Thickness((Road.End.X - offset) * zoom, Road.Start.Y * zoom, 0, 0);
            }

            Line.Height = 2 * zoom;
            Line.Margin = new Thickness(0, (Height / 2d) - (Line.Height / 2d), 0, 0);
        }
    }
}
