using System;
using System.Collections.Generic;
using KBS2.CitySystem;

namespace KBS2.CarSystem.Sensors
{
    public delegate void EventHandlerSensor(object sender, SensorEventArgs e);

    public abstract class ActiveSensor : Sensor
    {
        private event EventHandlerSensor SensorEvent;
        
        protected ActiveSensor(Car car, Direction direction) : base(car, direction) { }

        /// <summary>
        ///     Subscribe to Sensors event
        /// </summary>
        /// <param name="source">method you want to subscribe</param>
        public void SubScribeSensorEvent(EventHandlerSensor source)
        {
            SensorEvent += source;
        }

        /// <summary>
        ///     Unsubscribe to Sensor event
        /// </summary>
        /// <param name="source">method you want to unsubscribe</param>
        public void UnsubscribeSensorEvent(EventHandlerSensor source)
        {
            SensorEvent -= source;
        }

        public void CallEvent(SensorEventArgs args)
        {
            SensorEvent?.Invoke(this, args);
        }
    }
}