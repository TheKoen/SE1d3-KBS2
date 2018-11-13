using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBS2.Car.Sensors.PassiveSensors
{
    public class LineSensor : PassiveSensor
    {
        public double Distance { get; set; }
        private LineSensorController controller;

        /// <summary>
        /// Create a lineSensor of a car
        /// </summary>
        /// <param name="directionSensor">Side where the sensor is located</param>
        public LineSensor(Direction directionSensor)
        {
            SensorDirection = directionSensor;
            controller = new LineSensorController(this);
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
            
        }
    }
}
