using System;
using System.Windows;
using KBS2.CarSystem;
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
            var px = road.End.X - road.Start.X;
            var py = road.End.Y - road.Start.Y;
            var temp = (px * px) + (py * py);
            var u = ((point.X - road.Start.X) * px + (point.Y - road.Start.Y) * py) / (temp);
            if (u > 1)
            {
                u = 1;
            }
            else if (u < 0)
            {
                u = 0;
            }

            var x = road.Start.X + u * px;
            var y = road.Start.Y + u * py;

            var dx = x - point.X;
            var dy = y - point.Y;
            var dist = Math.Sqrt(dx * dx + dy * dy);
            return dist;
        }

        /// <summary>
        /// Rotates a vector by a specified angle (in degrees).
        /// </summary>
        /// <param name="vector">Vector to rotate</param>
        /// <param name="angle">Angle in degrees</param>
        /// <returns>The rotated vector</returns>
        public static Vector RotateVector(Vector vector, double angle)
        {
            // Convert the angle from degrees to radians.
            var radians = (Math.PI / 180) * angle;

            var x = vector.X * Math.Cos(radians) - vector.Y * Math.Sin(radians);
            var y = vector.X * Math.Sin(radians) + vector.Y * Math.Cos(radians);

            // Rotate the vector.
            return new Vector(x, y);
        }

        /// <summary>
        /// Calculate the relative angle between an absolute direction and a vector.
        /// </summary>
        public static double VectorToAngle(Vector vector, DirectionCar direction)
        {
            return Vector.AngleBetween(vector, direction.GetVector());
        }

        public static Vector VelocityToRotation(Vector velocity)
        {
            var rotation = new Vector(velocity.X, velocity.Y);
            rotation.Normalize();
            return rotation;
        }
    }
}