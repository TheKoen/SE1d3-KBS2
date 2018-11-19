using System;

namespace KBS2.CarSystem.Sensors
{
    public abstract class Sensor
    {
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