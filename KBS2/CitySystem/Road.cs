﻿using System;
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

        /// <summary>
        /// Checks if the given location is on the road
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public bool IsOnRoad(Vector location)
        {
            var laneWidth = Width / 2.0;
            var minX = IsXRoad()
                ? Math.Min(Start.X, End.X)
                : Start.X - laneWidth;
            var maxX = IsXRoad()
                ? Math.Max(Start.X, End.X)
                : Start.X + laneWidth;
            var minY = IsXRoad()
                ?Start.Y - laneWidth
                : Math.Min(Start.Y, End.Y);
            var maxY = IsXRoad()
                ? Start.Y + laneWidth
                : Math.Max(Start.Y, End.Y);

            return location.X >= minX && location.X <= maxX && location.Y >= minY && location.Y <= maxY;
        }
    }
}
