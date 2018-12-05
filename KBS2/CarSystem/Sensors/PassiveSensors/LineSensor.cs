using System;

namespace KBS2.CarSystem.Sensors.PassiveSensors
{
    public class LineSensor : PassiveSensor
    {
        /// <summary>
        ///     Create a lineSensor of a car
        /// </summary>
        /// <param name="car"></param>
        /// <param name="direction">Side where the sensor is located</param>
        public LineSensor(Car car, Direction direction) : base(car, direction)
        {
            Controller = new LineSensorController(this);
        }

        public double Distance { get; set; }
    }

    /// <summary>
    ///     Controller of a linesensor
    /// </summary>
    internal class LineSensorController : SensorController
    {
        public LineSensor Sensor { get; set; }
        public LineSensorController(LineSensor sensor) : base(sensor) {
            Sensor = sensor;
        }
        

        /// <summary>
        ///     Updates the distance to a line of a lane
        /// </summary>
        public override void Update()
        {
            if (Sensor.Car.CurrentRoad == null)
            {
                return;
            }

            var currentLoc = Sensor.Car.Location;
            var road = Sensor.Car.CurrentRoad;
            if (road == null) return;

            var currentValue = road.IsXRoad() ? currentLoc.Y : currentLoc.X;
            var roadValue = road.IsXRoad() ? road.Start.Y : road.Start.X;
            var positiveDir = Sensor.Car.Direction.Equals(DirectionCar.South) ||
                              Sensor.Car.Direction.Equals(DirectionCar.East);

            switch (Sensor.Direction)
            {
                case Direction.Left:
                    Sensor.Distance = Math.Abs(roadValue - currentValue);
                    break;
                case Direction.Right:
                    var laneWidth = road.Width / 2.0;
                    Sensor.Distance = positiveDir
                        ? Math.Abs(roadValue + laneWidth - currentValue)
                        : Math.Abs(roadValue - laneWidth - currentValue);
                    break;
                default:
                    throw new ArgumentException($"Unable to find line in direction {Sensor.Direction}");
            }
        }
    }
}