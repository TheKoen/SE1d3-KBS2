using System;
using System.Windows;

namespace KBS2.CitySystem
{
    public class Road
    {
        public Vector Start { get; set; }
        public Vector End { get; set; }
        public int Width { get; set; }
        public int MaxSpeed { get; set; }

        public Road(Vector s, Vector e, int w, int ms)
        {
            this.Start = s;
            this.End = e;
            this.Width = w;
            this.MaxSpeed = ms;
        }

        public bool IsXRoad()
        {
            return Math.Abs(Start.Y - End.Y) < 0.01;
        }
    }
}
