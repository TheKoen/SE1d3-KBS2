using System.Collections.Generic;
using KBS2.CitySystem;

namespace KBS2.CarSystem.Sensors
{
    public class SensorEventArgs
    {
        public ActiveSensor Sensor { get; }

        public SensorEventArgs(ActiveSensor sensor)
        {
            Sensor = sensor;
        }
    }
}