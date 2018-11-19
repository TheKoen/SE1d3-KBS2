using System;

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
    }
}