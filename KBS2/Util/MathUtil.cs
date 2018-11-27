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
            return LineToPointDistance(road.Start, road.End, point);
        }

        public static double DotProduct(Vector pointA, Vector pointB, Vector pointC)
        {
            var ab = new Vector(pointB.X - pointA.X, pointB.Y - pointA.Y);
            var bc = new Vector(pointC.X - pointB.X, pointC.Y - pointB.Y);
            return ab.X * bc.X + ab.Y * bc.Y;
        }

        public static double CrossProduct(Vector pointA, Vector pointB, Vector pointC)
        {
            var ab = new Vector(pointB.X - pointA.X, pointB.Y - pointA.Y);
            var ac = new Vector(pointC.X - pointA.X, pointC.Y - pointA.Y);
            return ab.X * ac.Y - ab.Y * ac.Y;
        }

        public static double LineToPointDistance(Vector pointA, Vector pointB, Vector pointC)
        {
            var dist = CrossProduct(pointA, pointB, pointC) / Distance(pointA, pointB);
            var dot1 = DotProduct(pointA, pointB, pointC);
            if (dot1 > 0)
                return Distance(pointB, pointC);

            var dot2 = DotProduct(pointB, pointA, pointC);
            return dot2 > 0 ? Distance(pointA, pointC) : Math.Abs(dist);
        }
    }
}