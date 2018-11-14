namespace KBS2.CarSystem.Sensors
{
    public class SensorEventArgs
    {
        public ActiveSensor Sensor { get; set; }
        public List<Entity> EntitiesInRange { get; set; }

        public SensorEventArgs(ActiveSensor sensor, List<IEntity> entitiesInRange)
        {
            Sensor = sensor;
            EntitiesInRange = entitiesInRange;
        }
    }
}
