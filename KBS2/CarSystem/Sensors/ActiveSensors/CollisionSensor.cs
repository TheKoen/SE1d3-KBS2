﻿using System;
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

        public override void Update()
        {
            if (Sensor.Car.CurrentRoad == null)
            {
                return;
            }

            var carDir = Sensor.Car.Direction;
            var sensorDir = GetAbsoluteDirection(carDir);
            var entities = GetEntitiesInRange(sensorDir);

            if (entities.Count == 0) return;

            ((CollisionSensor) Sensor).CallEvent(new CollisionSensorEventArgs(((CollisionSensor)Sensor), entities));
        }

        /// <summary>
        ///     Checks if there are any cars in the range of the sensor with the sensors direction
        /// </summary>
        /// <param name="sensorDir">the Direction of a sensor</param>
        /// <returns>a list of cars in range</returns>
        private List<IEntity> GetEntitiesInRange(DirectionCar sensorDir) // TODO: Why is sensorDir not used?
        {
            var car = Sensor.Car;
            var range = Math.Min(car.CurrentRoad.Width / 4.0, Sensor.Range);
            var direction = car.Direction.RotateTo(Sensor.Direction);
            var add = Vector.Multiply(direction, car.Length / 2.0 + range / 2.0);
            var center = Vector.Add(car.Location, add);

            return City.Instance.Controller.GetEntitiesInRange(center, range)
                .FindAll(entity => !entity.Equals(car));
        }

        /// <summary>
        ///     Converts the direction of the car into a direction of the sensor
        /// </summary>
        /// <param name="carDir">car direction</param>
        /// <returns>direction for the sensor</returns>
        private DirectionCar GetAbsoluteDirection(DirectionCar carDir)
        {
            switch (Sensor.Direction)
            {
                case Direction.Front:
                    return carDir;
                case Direction.Back:
                    return carDir.GetOpposite();
                case Direction.Left:
                    return carDir.GetPrevious();
                case Direction.Right:
                    return carDir.GetNext();
                default:
                    throw new ArgumentException($"Unknown direction {Sensor.Direction}");
            }
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