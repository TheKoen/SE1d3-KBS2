using KBS2.CitySystem;
using System.Collections.Generic;

namespace KBS2.CarSystem.Sensors
{
    public class SensorEventArgs
    {
        public ActiveSensor Sensor { get; set; }
        public List<IEntity> EntitiesInRange { get; set; }

        public SensorEventArgs(ActiveSensor sensor, List<IEntity> entitiesInRange)
        {
            Sensor = sensor;
            EntitiesInRange = entitiesInRange;
        }
    }
}
