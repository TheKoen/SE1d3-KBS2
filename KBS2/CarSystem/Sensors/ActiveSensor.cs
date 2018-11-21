using System;
using System.Collections.Generic;
using KBS2.CitySystem;

namespace KBS2.CarSystem.Sensors
{
    public delegate void EventHandlerSensor(object sender, SensorEventArgs e);

    public abstract class ActiveSensor : Sensor
    {
        public List<IEntity> Entities { get; set; }

        public event EventHandlerSensor SensorEvent;
        
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
        ///     UnSubscribe to Sensor event
        /// </summary>
        /// <param name="source">method you want to unsubscribe</param>
        public void UnSubscribeSensorEvent(EventHandlerSensor source)
        {
            SensorEvent -= source;
        }

        /// <summary>
        ///     Use this method to throw the SensorEvent
        /// </summary>
        public void CallEvent()
        {
            if (Entities.Count != 0)
                SensorEvent?.Invoke(this, new SensorEventArgs(this, Entities));
            else
                throw new Exception("There are no entities in range of this sensor");
        }
    }
}