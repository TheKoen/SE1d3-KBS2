namespace KBS2.CarSystem.Sensors
{
    public abstract class SensorController
    {
        public Sensor Sensor { get; }

        protected SensorController(Sensor sensor)
        {
            Sensor = sensor;
        }

        public abstract void Update();
    }
}