using System;
using System.Windows;
using KBS2.CitySystem;

namespace KBS2.Util
{
    public class MathUtil
    {
        /// <summary>
        /// Calculates the distance between two vectors.
        /// </summary>
        /// <param name="pointA"></param>
        /// <param name="pointB"></param>
        /// <returns></returns>
        public static double Distance(Vector pointA, Vector pointB)
        {
            return Math.Sqrt(Math.Pow(pointA.X - pointB.X, 2) + Math.Pow(pointA.Y - pointB.Y, 2));
        }

        /// <summary>
        /// reutrns the distance form a point to a road
        /// </summary>
        /// <param name="point">Vetor point</param>
        /// <param name="road">Road road</param>
        /// <returns>double distance</returns>
        public static double DistanceToRoad(Vector point, Road road)
        {
            return ShortestDistance(road.Start, road.End, point);
        }

        private static double ShortestDistance(Vector lineStart, Vector lineEnd, Vector point)
        {
            var px = lineEnd.X - lineStart.X;
            var py = lineEnd.Y - lineStart.Y;
            var temp = (px * px) + (py * py);
            var u = ((point.X - lineStart.X) * px + (point.Y - lineStart.Y) * py) / (temp);
            if (u > 1)
            {
                u = 1;
            }
            else if (u < 0)
            {
                u = 0;
            }
            var x = lineStart.X + u * px;
            var y = lineStart.Y + u * py;

            var dx = x - point.X;
            var dy = y - point.Y;
            var dist = Math.Sqrt(dx * dx + dy * dy);
            return dist;
        }
    }
}