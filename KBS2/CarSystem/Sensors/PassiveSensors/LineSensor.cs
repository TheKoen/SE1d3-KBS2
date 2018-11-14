using System;
using KBS2.GPS;

namespace KBS2.CarSystem.Sensors.PassiveSensors
{
    public class LineSensor : PassiveSensor
    {
        public double Distance { get; set; }

        /// <summary>
        /// Create a lineSensor of a car
        /// </summary>
        /// <param name="directionSensor">Side where the sensor is located</param>
        public LineSensor(Direction directionSensor)
        {
            SensorDirection = directionSensor;
            Controller = new LineSensorController(this);
        }
    }

    /// <summary>
    /// Controller of a linesensor
    /// </summary>
    class LineSensorController : SensorController
    {
        public LineSensor Sensor { get; set; }

        public LineSensorController(LineSensor sensor)
        {
            Sensor = sensor;
        }

        /// <summary>
        /// Updates the distance to a line of a lane
        /// </summary>
        public override void Update()
        {
            var currentLoc = Sensor.Car.Location;
            var road = GPSSystem.GetRoad(currentLoc);
            var currentValue = road.IsXRoad() ? currentLoc.Y : currentLoc.X;
            var roadValue = road.IsXRoad() ? road.Start.Y : road.Start.X;
            var positiveDir = Sensor.Car.Direction.Equals(DirectionCar.South) ||
                              Sensor.Car.Direction.Equals(DirectionCar.East);

            switch (Sensor.SensorDirection)
            {
                case Direction.Left:
                    Sensor.Distance = Math.Abs(roadValue - currentValue);
                    break;
                case Direction.Right:
                    var laneWidth = road.Width / 2.0;
                    Sensor.Distance = positiveDir
                        ? Math.Abs((roadValue + laneWidth) - currentValue)
                        : Math.Abs((roadValue - laneWidth) - currentValue);
                    break;
                default:
                    throw new ArgumentException($"Unable to find line in direction {Sensor.SensorDirection}");
            }
        }
    }
}