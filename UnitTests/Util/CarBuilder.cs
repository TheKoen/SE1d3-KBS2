using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using KBS2.CarSystem;
using KBS2.CarSystem.Sensors;

namespace UnitTests.Util
{
    public class CarBuilder
    {
        private Vector location;
        private List<Sensor> sensors = new List<Sensor>();
        private DirectionCar direction;
        private int width;
        private int length;

        public CarBuilder Location(Vector location)
        {
            this.location = location;
            return this;
        }

        public CarBuilder Sensor(Sensor sensor)
        {
            sensors.Add(sensor);
            return this;
        }

        public CarBuilder Width(int width)
        {
            this.width =width;
            return this;
        }

        public CarBuilder Length(int length)
        {
            this.length = length;
            return this;
        }

        public CarBuilder Direction(DirectionCar direction)
        {
            this.direction = direction;
            return this;
        }

        public Car Build()
        {
            if (location == null)
            {
                throw new ArgumentException("Location cannot be null");
            }
            return new Car(location, sensors, direction, width, length);
        }
    }
}
