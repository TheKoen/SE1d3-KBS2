using KBS2.CarSystem;
using NUnit.Framework;
using System.Linq;
using System.Windows;
using KBS2.CarSystem.Sensors;
using KBS2.CarSystem.Sensors.PassiveSensors;
using UnitTests.Util;

namespace UnitTests.CarSystem.Sensors.PassiveSensors
{
    [TestFixture]
    class EntityRadarTest
    {
        [TestCase(0, 0, 50, 0, 100)] // in front
        [TestCase(0, 0, -50, 0, 100)] // behind
        [TestCase(0, 0, 0, 50, 100)] // left
        [TestCase(0, 0, 0, -50, 100)] // right
        public void TestEntityRadarInRange(double carX, double carY, double secCarX, double secCarY, double range)
        {
            var city = new CityBuilder()
                .Road(new Vector(0, 50), new Vector(10, 50), 10)
                .Road(new Vector(0, 20), new Vector(10, 20), 10)
                .Build();
            var sensorCar = new CarBuilder()
                .Location(new Vector(carX, carY))
                .Direction(DirectionCar.East)
                .Sensor(car => new EntityRadar(car, range))
                .Build();
            city.Cars.Add(sensorCar);
            var secCar = (new CarBuilder()
                .Location(new Vector(secCarX, secCarY))
                .Direction(DirectionCar.East)
                .Build());
            city.Cars.Add(secCar);
            var sensor = sensorCar.Controller.GetSensors<EntityRadar>(Direction.Global).First();

            sensor.Controller.Update();

            if (sensor.EntitiesInRange.Any(car => car.Equals(secCar)))
            {
                Assert.Pass();
            }
            else
            {
                Assert.Fail();
            }

        }

        [TestCase(0, 0, 100, 0, 50)] // front out of range
        [TestCase(0, 0, -100, 0, 50)] // behind out of range
        [TestCase(0, 0, 0, 100, 50)] // left out of range
        [TestCase(0, 0, 0, -100, 50)] // right out of range
        public void TestEntityRadarOutOfRange(double carX, double carY, double secCarX, double secCarY, double range)
        {
            var city = new CityBuilder()
                .Road(new Vector(0, 50), new Vector(10, 50), 10)
                .Road(new Vector(0, 20), new Vector(10, 20), 10)
                .Build();
            var sensorCar = new CarBuilder()
                .Location(new Vector(carX, carY))
                .Direction(DirectionCar.East)
                .Sensor(car => new EntityRadar(car, range))
                .Build();
            city.Cars.Add(sensorCar);
            var secCar = (new CarBuilder()
                .Location(new Vector(secCarX, secCarY))
                .Direction(DirectionCar.East)
                .Build());
            city.Cars.Add(secCar);
            var sensor = sensorCar.Controller.GetSensors<EntityRadar>(Direction.Global).First();

            sensor.Controller.Update();

            if (sensor.EntitiesInRange.Count > 0)
            {
                Assert.Fail();
            }
            else
            {
                Assert.Pass();
            }
        }




        [TestCase(0, 0, 250, 0, 100, 25, 150)] //out of range length
        [TestCase(0, 0, 250, 105, 100, 25, 150)] // out of range left
        public void TestEntityRadarWidthOutOfRange(double carX, double carY, double secCarX, double secCarY, double range, int secCarWidth, int secCarLength)
        {
            var city = new CityBuilder()
                .Road(new Vector(0, 50), new Vector(10, 50), 10)
                .Road(new Vector(0, 20), new Vector(10, 20), 10)
                .Build();
            var sensorCar = new CarBuilder()
                .Location(new Vector(carX, carY))
                .Direction(DirectionCar.East)
                .Sensor(car => new EntityRadar(car, range))
                .Build();
            city.Cars.Add(sensorCar);

            var secCar = (new CarBuilder()
                .Location(new Vector(secCarX, secCarY))
                .Direction(DirectionCar.East)
                .Width(secCarWidth)
                .Length(secCarLength)
                .Build());
            city.Cars.Add(secCar);

            var sensor = sensorCar.Controller.GetSensors<EntityRadar>(Direction.Global).First();

            sensor.Controller.Update();

            if (sensor.EntitiesInRange.Any(car => car.Equals(secCar)))
            {
                Assert.Fail();
            }
            else
            {
                Assert.Pass();
            }

        }
    }
}
