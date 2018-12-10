using System;
using System.Windows;
using KBS2.CarSystem.Sensors;
using KBS2.Util;

namespace KBS2.CarSystem
{
    public enum DirectionCar
    {
        North,
        South,
        East,
        West
    }

    internal static class DirectionCarMethods
    {
        public static DirectionCar GetOpposite(this DirectionCar current)
        {
            switch (current)
            {
                case DirectionCar.North:
                    return DirectionCar.South;
                case DirectionCar.East:
                    return DirectionCar.West;
                case DirectionCar.South:
                    return DirectionCar.North;
                case DirectionCar.West:
                    return DirectionCar.East;
                default:
                    throw new ArgumentException($"Unknown direction {current}");
            }
        }

        public static DirectionCar GetNext(this DirectionCar current)
        {
            switch (current)
            {
                case DirectionCar.North:
                    return DirectionCar.East;
                case DirectionCar.East:
                    return DirectionCar.South;
                case DirectionCar.South:
                    return DirectionCar.West;
                case DirectionCar.West:
                    return DirectionCar.North;
                default:
                    throw new ArgumentException($"Unknown direction {current}");
            }
        }

        public static DirectionCar GetPrevious(this DirectionCar current)
        {
            switch (current)
            {
                case DirectionCar.North:
                    return DirectionCar.West;
                case DirectionCar.East:
                    return DirectionCar.North;
                case DirectionCar.South:
                    return DirectionCar.East;
                case DirectionCar.West:
                    return DirectionCar.South;
                default:
                    throw new ArgumentException($"Unknown direction {current}");
            }
        }

        public static Vector GetVector(this DirectionCar current)
        {
            switch (current)
            {
                case DirectionCar.North:
                    return new Vector(0, -1);
                case DirectionCar.East:
                    return new Vector(1, 0);
                case DirectionCar.South:
                    return new Vector(0, 1);
                case DirectionCar.West:
                    return new Vector(-1, 0);
                default:
                    throw new ArgumentException($"Unknown direction {current}");
            }
        }

        public static Vector RotateTo(this DirectionCar current, Direction rotation)
        {
            var direction = current.GetVector();
            switch (rotation)
            {
                case Direction.Front:
                    return direction;
                case Direction.Right:
                    return new Vector(-direction.Y, direction.X);
                case Direction.Left:
                    return new Vector(direction.Y, -direction.X);
                case Direction.Back:
                    return new Vector(-direction.X, -direction.Y);
                default:
                    throw new ArgumentException($"Unknown rotation {rotation}");
            }
        }

        public static DirectionCar Parse(Vector vector)
        {
            if (Math.Abs(MathUtil.VectorToAngle(vector, DirectionCar.North)) <= 45) return DirectionCar.North;
            if (Math.Abs(MathUtil.VectorToAngle(vector, DirectionCar.South)) <= 45) return DirectionCar.South;
            if (Math.Abs(MathUtil.VectorToAngle(vector, DirectionCar.East)) <= 45) return DirectionCar.East;
            if (Math.Abs(MathUtil.VectorToAngle(vector, DirectionCar.West)) <= 45) return DirectionCar.West;

            return DirectionCar.North;
        }
    }
}