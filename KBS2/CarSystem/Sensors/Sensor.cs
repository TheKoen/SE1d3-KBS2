using KBS2.CarSystem.Sensors.PassiveSensors;
using System;

namespace KBS2.CarSystem.Sensors
{
    public delegate Sensor CreateSensor(Car car, Direction direction, int range);
    public struct SensorCreator
    {
        public Type Type;
        public CreateSensor Creator;
    }

    public abstract class Sensor
    {

        public static SensorCreator[] SENSORS =
        {
            new SensorCreator
            {
                Type = typeof(EntityRadar),
                Creator = (car, Direction, range) => new EntityRadar(car, range)
            }
        };

        public Car Car { get; }
        public Direction Direction { get; }

        private SensorController controller;
        public SensorController Controller
        {
            get => controller;
            protected set
            {
                if (controller != null) throw new InvalidOperationException("Controller is already set!");

                controller = value;
                MainWindow.Loop.Subscribe(controller.Update);
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