using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBS2.Car.Sensors.ActiveSensors
{
    public class ObjectSensor : PassiveSensor
    {
        public ObjectSensor(Direction direction, double range)
        {
            Range = range;
            SensorDirection = direction;


        }
    }

    class ItemSensorController : SensorController
    {
        public ObjectSensor Sensor { get; set; }

        public ItemSensorController(ObjectSensor sensor)
        {
            Sensor = sensor;
        }

        public override void Update()
        {
            if(Sensor.SensorDirection == Direction.Front)
            {

            }
            else if(Sensor.SensorDirection == Direction.Back)
            {

            }
            else if(Sensor.SensorDirection == Direction.Left)
            {

            }
            else if(Sensor.SensorDirection == Direction.Right)
            {

            }
        }

    }
}
