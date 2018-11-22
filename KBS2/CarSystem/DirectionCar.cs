using System;
using System.Windows;
using KBS2.CarSystem.Sensors;

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

        public static Vector GetDirection(this DirectionCar current)
        {
            switch (current)
            {
                case DirectionCar.North:
                    return new Vector(0, -1);
                case DirectionCar.East:
                    return new Vector(1, 0);
                case DirectionCar.South:
                    return new Vector(1, 0);
                case DirectionCar.West:
                    return new Vector(0, -1);
                default:
                    throw new ArgumentException($"Unknown direction {current}");
            }
        }

        public static Vector RotateTo(this DirectionCar current, Direction rotation)
        {
            var direction = current.GetDirection();
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
    }
}