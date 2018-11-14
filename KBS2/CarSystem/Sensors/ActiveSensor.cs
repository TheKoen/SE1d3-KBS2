﻿
namespace KBS2.CarSystem.Sensors
{
    public delegate void EventHandlerSensor(object sender, SensorEventArgs e);
    
    public abstract class ActiveSensor : Sensor
    {
        public List<IEntity> Entities { get; set; }

        public event EventHandlerSensor SensorEvent;

        /// <summary>
        /// Subscribe to Sensors event
        /// </summary>
        /// <param name="source">method you want to subscribe</param>
        public void SubScribeSensorEvent(EventHandlerSensor source)
        {
            SensorEvent += source;
        }
        /// <summary>
        /// UnSubscribe to Sensor event
        /// </summary>
        /// <param name="source">method you want to unsubscribe</param>
        public void UnSubscribeSensorEvent(EventHandlerSensor source)
        {
            SensorEvent -= source;
        }

        /// <summary>
        /// Use this method to throw the SensorEvent
        /// </summary>
        protected void DetectedEntities()
        {
            if (!Entities.Empty)
                SensorEvent?.Invoke(this, new SensorEventArgs(this, Entities));
            else
                throw new System.Exception("There are no entities in range of this sensor");
        } 
    }
}
