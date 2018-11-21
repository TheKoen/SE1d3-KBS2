using KBS2.CarSystem;
using KBS2.CarSystem.Sensors;
using KBS2.CarSystem.Sensors.ActiveSensors;
using NUnit.Framework;
using System.Linq;
using System.Windows;
using UnitTests.Util;

namespace UnitTests.CarSystem.Sensors.ActiveSensor
{
    [TestFixture]
    class CollisionSensorTest
    {
        [TestCase(Direction.Front, DirectionCar.North, 20, 10, 24, 10, 12)]
        [TestCase(Direction.Back, DirectionCar.North, 20, 10, 12, 10, 24)]
        [TestCase(Direction.Back, DirectionCar.North, 20, 5, 12, 10, 24)]
        [TestCase(Direction.Back, DirectionCar.North, 20, 15, 12, 10, 24)]
        public void TestInRangeCollisionSensor(Direction directionSensor, DirectionCar directionCar, double range, double carX, double carY, double secCarX, double secCarY)
        {
            var city = new CityBuilder()
                .Road(new Vector(10, 0), new Vector(10, 50), 40)
                .Build();

            var sensorCar = new CarBuilder()
                .Location(new Vector(carX, carY))
                .Direction(directionCar)
                .Sensor(car => new CollisionSensor(car, directionSensor, range))
                .Build();
            city.Cars.Add(sensorCar);
            city.Cars.Add(new CarBuilder()
                .Location(new Vector(secCarX, secCarY))
                .Direction(directionCar)
                .Build());
            var sensor = sensorCar.Controller.GetSensors<CollisionSensor>(directionSensor).First();

            sensor.SubScribeSensorEvent(EventThrownGood);

            sensor.Controller.Update();

            Assert.Fail("Car was not detected!");
        }

        [TestCase(Direction.Front, DirectionCar.North, 20, 10, 40, 10, 12)]
        [TestCase(Direction.Back, DirectionCar.North, 20, 10, 12, 10, 40)]
        [TestCase(Direction.Back, DirectionCar.North, 20, 0, 12, 10, 24)]
        [TestCase(Direction.Back, DirectionCar.North, 20, 20, 12, 10, 24)]
        public void TestOutOfRangeCollisionSensor(Direction directionSensor, DirectionCar directionCar, double range, double carX, double carY, double secCarX, double secCarY)
        {
            var city = new CityBuilder()
                .Road(new Vector(10, 0), new Vector(10, 50), 40)
                .Build();

            var sensorCar = new CarBuilder()
                .Location(new Vector(carX, carY))
                .Direction(directionCar)
                .Sensor(car => new CollisionSensor(car, directionSensor, range))
                .Build();
            city.Cars.Add(sensorCar);
            city.Cars.Add(new CarBuilder()
                .Location(new Vector(secCarX, secCarY))
                .Direction(directionCar)
                .Build());
            var sensor = sensorCar.Controller.GetSensors<CollisionSensor>(directionSensor).First();

            sensor.SubScribeSensorEvent(EventThrownBad);

            sensor.Controller.Update();

            Assert.Pass();
        }

        private static void EventThrownGood(object source, SensorEventArgs e)
        {
            Assert.Pass();
        }

        private static void EventThrownBad(object source, SensorEventArgs e)
        {
            Assert.Fail("Car was detected!");
        }
    }
}
