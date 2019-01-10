namespace KBS2.CarSystem.Sensors
{
    public delegate void EventHandlerSensor(object sender, SensorEventArgs e);

    public abstract class ActiveSensor : Sensor
    {
        private event EventHandlerSensor SensorEvent;
        
        protected ActiveSensor(Car car, Direction direction) : base(car, direction) { }

        /// <summary>
        /// Subscribe to SensorEvent
        /// </summary>
        /// <param name="source">Method to subscribe</param>
        public void SubScribeSensorEvent(EventHandlerSensor source)
        {
            SensorEvent += source;
        }

        /// <summary>
        /// Unsubscribe to SensorEvent
        /// </summary>
        /// <param name="source">Method to unsubscribe</param>
        public void UnsubscribeSensorEvent(EventHandlerSensor source)
        {
            SensorEvent -= source;
        }

        /// <summary>
        /// Calls the SensorEvent
        /// </summary>
        /// <param name="args"><see cref="SensorEventArgs"/> passed to the event</param>
        public void CallEvent(SensorEventArgs args)
        {
            SensorEvent?.Invoke(this, args);
        }
    }
}