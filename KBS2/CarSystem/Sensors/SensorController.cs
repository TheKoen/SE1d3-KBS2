namespace KBS2.CarSystem.Sensors
{
    public abstract class SensorController
    {
        public Sensor Sensor { get; }

        protected SensorController(Sensor sensor)
        {
            Sensor = sensor;
        }

        /// <summary>
        ///     Updates the distance to a line of a lane
        /// </summary>
        public abstract void Update();
    }
}