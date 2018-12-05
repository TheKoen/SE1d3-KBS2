using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using KBS2.CarSystem;
using KBS2.CarSystem.Sensors;
using KBS2.CitySystem;
using KBS2.GPS;

namespace UnitTests.Util
{
    public delegate Sensor CreateSensor(Car car);

    public class CarBuilder
    {
        public static int ID;

        private Vector location;
        private readonly List<CreateSensor> sensors = new List<CreateSensor>();
        private DirectionCar direction;
        private CarModel model = CarModel.TestModel;
        private int width;
        private int length;
        private Road currentRoad;

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

        public CarBuilder CurrentRoad(Road currentRoad)
        {
            this.currentRoad = currentRoad;
            return this;
        }

        public Car Build()
        {
            if (location == null)
            {
                throw new ArgumentException("Location cannot be null");
            }

            var car = new Car(ID++, model, location, new List<Sensor>(), direction, width, length)
            {
                CurrentRoad = currentRoad
            };
            sensors.ForEach(creator => car.Sensors.Add(creator(car)));

            if (car.CurrentRoad == null)
            {
                car.CurrentRoad = GPSSystem.GetRoad(location);
            }

            return car;
        }
    }
}
