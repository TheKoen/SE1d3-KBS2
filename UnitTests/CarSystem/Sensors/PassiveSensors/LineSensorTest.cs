using System.Linq;
using System.Windows;
using KBS2.CarSystem;
using KBS2.CarSystem.Sensors;
using KBS2.CarSystem.Sensors.PassiveSensors;
using NUnit.Framework;
using UnitTests.Util;

namespace UnitTests.CarSystem.Sensors.PassiveSensors
{
    [TestFixture]
    public class LineSensorTest
    {
        [TestCase(5, 23, 2.0, Direction.Right)]
        [TestCase(5, 23, 3.0, Direction.Left)]
        [TestCase(5, 20, 0.0, Direction.Left)]
        [TestCase(5, 20, 5.0, Direction.Right)]
        [TestCase(5, 20.5, 4.5, Direction.Right)]
        [TestCase(5, 20.5, 0.5, Direction.Left)]
        public void TestLineSensorDistance(double carX, double carY, double expected, Direction direction)
        {
            var city = new CityBuilder()
                .Road(new Vector(0, 50), new Vector(10, 50), 10)
                .Road(new Vector(0, 20), new Vector(10, 20), 10)
                .Build();
            var sensorCar = new CarBuilder()
                .Location(new Vector(carX, carY))
                .Direction(DirectionCar.East)
                .Sensor(car => new LineSensor(car, direction))
                .Build();
            city.Cars.Add(sensorCar);
            var sensor = sensorCar.Controller.GetSensors<LineSensor>(direction).First();

            sensor.Controller.Update();

            Assert.AreEqual(expected, sensor.Distance, 0.01);
        }
    }
}