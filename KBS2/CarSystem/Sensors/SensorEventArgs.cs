using System.Collections.Generic;
using KBS2.CitySystem;

namespace KBS2.CarSystem.Sensors
{
    public class SensorEventArgs
    {
        public SensorEventArgs(ActiveSensor sensor, List<IEntity> entitiesInRange)
        {
            Sensor = sensor;
            EntitiesInRange = entitiesInRange;
        }

        public ActiveSensor Sensor { get; set; }
        public List<IEntity> EntitiesInRange { get; set; }
    }
}