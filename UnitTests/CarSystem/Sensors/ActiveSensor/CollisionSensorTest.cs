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
        [TestCase(Direction.Front, DirectionCar.North, 100, 0, 0, 0, 100)] // car in range 
        [TestCase(Direction.Front, DirectionCar.North, 100, 0, 0, 50, 50)] // car right front but in range
        public void TestInRangeCollisionSensor(Direction directionSensor, DirectionCar directionCar, double range, double carX, double carY, double secCarX, double secCarY)
        {
            var city = new CityBuilder()
                .Road(new Vector(0, 50), new Vector(10, 50), 10)
                .Road(new Vector(0, 20), new Vector(10, 20), 10)
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

            Assert.Fail();
        }

        [TestCase(Direction.Front, DirectionCar.North, 100, 0, 0, 0, 200)] // out of range
        [TestCase(Direction.Front, DirectionCar.North, 100, 0, 0, 0, -100)] // car behind
        [TestCase(Direction.Front, DirectionCar.East, 100, 0, 0, 0, 200)] // car left of other car but out of range
        public void TestOutOfRangeCollisionSensor(Direction directionSensor, DirectionCar directionCar, double range, double carX, double carY, double secCarX, double secCarY)
        {
            var city = new CityBuilder()
                .Road(new Vector(0, 50), new Vector(10, 50), 10)
                .Road(new Vector(0, 20), new Vector(10, 20), 10)
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
            return;
        }

        private static void EventThrownBad(object source, SensorEventArgs e)
        {
            Assert.Fail();
            return;
        }
    }
}
