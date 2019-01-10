using System;
using System.Collections.Generic;
using System.Windows;
using KBS2.CitySystem;

namespace KBS2.CarSystem.Sensors.ActiveSensors
{
    public class CollisionSensor : ActiveSensor
    {
        public CollisionSensor(Car car, Direction direction, double range) : base(car, direction)
        {
            Range = range;

            Controller = new CollisionSensorController(this);
        }
    }

    internal class CollisionSensorController : SensorController
    {
        public CollisionSensorController(CollisionSensor sensor) : base(sensor) { }

        /// <summary>
        /// Calls the SensorEvent
        /// </summary>
        public override void Update()
        {
            if (Sensor.Car.CurrentRoad == null) return;

            var entities = GetEntitiesInRange();
            if (entities.Count == 0) return;

            ((CollisionSensor) Sensor).CallEvent(new CollisionSensorEventArgs(((CollisionSensor)Sensor), entities));
        }

        /// <summary>
        /// Checks if there are any <see cref="Car"/>s in the range of the <see cref="CollisionSensor"/>
        /// </summary>
        /// <returns><see cref="List{IEntity}"/> of <see cref="Car"/>s in range</returns>
        private List<IEntity> GetEntitiesInRange()
        {
            var car = Sensor.Car;
            var range = Math.Min(car.CurrentRoad.Width / 4.0, Sensor.Range);
            var direction = car.Direction.RotateTo(Sensor.Direction);
            var add = Vector.Multiply(direction, car.Length / 2.0 + range / 2.0);
            var center = Vector.Add(car.Location, add);

            return City.Instance.Controller.GetEntitiesInRange(center, range)
                .FindAll(entity => !entity.Equals(car));
        }
    }

    internal class CollisionSensorEventArgs : SensorEventArgs
    {
        public List<IEntity> EntitiesInRange { get; }

        public CollisionSensorEventArgs(ActiveSensor sensor, List<IEntity> entitiesInRange) : base(sensor)
        {
            EntitiesInRange = entitiesInRange;
        }
    }
}