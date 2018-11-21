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
    public delegate Sensor CreateSensor(Car car);

    public class CarBuilder
    {
        public static int ID = 0;

        private Vector location;
        private List<CreateSensor> sensors = new List<CreateSensor>();
        private DirectionCar direction;
        private CarModel model = CarModel.TestModel;
        private int width;
        private int length;

        public CarBuilder Location(Vector location)
        {
            this.location = location;
            return this;
        }

        public CarBuilder Sensor(CreateSensor sensor)
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

        public CarBuilder Model(CarModel model)
        {
            this.model = model;
            return this;
        }

        public Car Build()
        {
            if (location == null)
            {
                throw new ArgumentException("Location cannot be null");
            }

            var car = new Car(ID++, model, location, new List<Sensor>(), direction, width, length);
            sensors.ForEach(creator => car.Sensors.Add(creator(car)));

            return car;
        }
    }
}
