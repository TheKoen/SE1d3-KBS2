using KBS2.CarSystem.Sensors.PassiveSensors;
using System;
using System.Collections.Generic;
using KBS2.CarSystem.Sensors.ActiveSensors;

namespace KBS2.CarSystem.Sensors
{
    public delegate Sensor CreateSensor(Car car, Direction direction, int range);

    public abstract class Sensor
    {
        public static readonly Dictionary<Type, CreateSensor> Sensors = new Dictionary<Type, CreateSensor>
        {
            {typeof(EntityRadar), (car, direction, range) => new EntityRadar(car, range)},
            {typeof(CollisionSensor), (car, direction, range) => new CollisionSensor(car, direction, range)},
            {typeof(LineSensor), (car, direction, range) => new LineSensor(car, direction)}
        };

        public Car Car { get; }
        public Direction Direction { get; }

        private SensorController _controller;

        public SensorController Controller
        {
            get => _controller;
            protected set
            {
                if (_controller != null) throw new InvalidOperationException("Controller is already set!");

                _controller = value;
                MainWindow.Loop.Subscribe(_controller.Update);
            }
        }

        public double Range { get; protected set; }

        protected Sensor(Car car, Direction direction)
        {
            Car = car;
            Direction = direction;
        }
    }
}