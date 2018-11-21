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

        public static double DistanceToRoad(Vector point, Road road)
        {
            var axialDistance = road.IsXRoad()
                ? Math.Abs(road.Start.X - point.X)
                : Math.Abs(road.Start.Y - point.Y);

            var directDistance = Distance(point, road.Start);

            return Math.Sqrt(Math.Pow(directDistance, 2) - Math.Pow(axialDistance, 2));
        }
    }
}
