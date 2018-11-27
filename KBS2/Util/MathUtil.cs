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
            return LineToPointDistance(road.Start, road.End, point);
        }

        public static double DotProduct(Vector pointA, Vector pointB, Vector pointC)
        {
            var AB = new Vector(pointB.X - pointA.X, pointB.Y - pointA.Y);
            var BC = new Vector(pointC.X - pointB.X, pointC.Y - pointB.Y);
            return AB.X * BC.X + AB.Y * BC.Y;
        }

        public static double CrossProduct(Vector pointA, Vector pointB, Vector pointC)
        {
            var AB = new Vector(pointB.X - pointA.X, pointB.Y - pointA.Y);
            var AC = new Vector(pointC.X - pointA.X, pointC.Y - pointA.Y);
            return AB.X * AC.Y - AB.Y * AC.Y;
        }

        public static double LineToPointDistance(Vector pointA, Vector pointB, Vector pointC)
        {
            var dist = CrossProduct(pointA, pointB, pointC) / Distance(pointA, pointB);
            var dot1 = DotProduct(pointA, pointB, pointC);
            if (dot1 > 0)
                return Distance(pointB, pointC);

            var dot2 = DotProduct(pointB, pointA, pointC);
            if (dot2 > 0)
                return Distance(pointA, pointC);

            return Math.Abs(dist);
        }
    }
}