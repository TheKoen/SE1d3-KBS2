using System.Windows;
using KBS2.CarSystem;
using KBS2.CarSystem.Sensors;
using KBS2.CarSystem.Sensors.PassiveSensors;
using KBS2.CitySystem;
using NUnit.Framework;
using UnitTests.Util;

namespace UnitTests.CarSystem
{
    [TestFixture]
    public class CarControllerTest
    {
        [TestCase(10.0, 60.0, 1.0)]
        [TestCase(10.0, 50.0, -1.0)]
        [TestCase(10.0, 55.0, 0.0)]
        public void TestHandleStayInLane(double carX, double carY, double expectedRotation)
        {
            var road = new Road(new Vector(0, 50), new Vector(50, 50), 20, 100);
            var city = new CityBuilder()
                .Road(road)
                .Build();

            var car = new CarBuilder()
                .Location(new Vector(carX, carY))
                .Direction(DirectionCar.East)
                .Sensor(Car => new LineSensor(Car, Direction.Left))
                .Sensor(Car => new LineSensor(Car, Direction.Right))
                .CurrentRoad(road)
                .Build();
            var controller = car.Controller;

            var velocity = new Vector();
            var yaw = 0.0;
            var addedRotation = 0.0;

            car.Sensors.ForEach(sensor => sensor.Controller.Update());
            controller.HandleStayInLane(ref velocity, ref yaw, ref addedRotation);

            Assert.AreEqual(new Vector(), velocity);
            Assert.AreEqual(0.0, yaw, 0.01);
            Assert.AreEqual(expectedRotation, addedRotation, 0.01);
        }

        [TestCase(0.0, 100.0, 5, 0.55)]
        [TestCase(0.5, 100.0, 5, 1.05)]
        [TestCase(1.0, 5.0, 5, 0.46)]
        [TestCase(0.0, 5.0, 5, 0.0)]
        public void TestHandlerAccelerate(double initialVelocity, double distanceToTarget, int cycles, double expectedVelocity)
        {
            var road = new Road(new Vector(0, 50), new Vector(50, 50), 20, 100);
            var city = new CityBuilder()
                .Road(road)
                .Build();

            var car = new CarBuilder()
                .Location(new Vector(10, 60))
                .Direction(DirectionCar.East)
                .Sensor(Car => new LineSensor(Car, Direction.Left))
                .Sensor(Car => new LineSensor(Car, Direction.Right))
                .CurrentRoad(road)
                .Build();
            var controller = car.Controller;

            var velocity = new Vector(initialVelocity, 0.0);

            for (var time = 0; time < cycles; time++)
            {
                car.Sensors.ForEach(sensor => sensor.Controller.Update());
                car.Controller.HandleAccelerate(ref velocity, ref distanceToTarget);
            }

            Assert.AreEqual(expectedVelocity, velocity.Length, 0.01);
            Assert.AreEqual(expectedVelocity, velocity.X, 0.01);
            Assert.AreEqual(0, velocity.Y, 0.01);
        }

        [TestCase(20.0, 65.0, 1.0, 0.86, 1.0)]
        [TestCase(20.0, 35.0, 1.0, 0.86, -1.0)]
        public void TestHandleApproachTarget(double targetX, double targetY, double initialVelocity, double expectedSpeed, double expectedRotation)
        {
            var road = new Road(new Vector(0, 50), new Vector(50, 50), 20, 100);
            var city = new CityBuilder()
                .Road(road)
                .Build();

            var car = new CarBuilder()
                .Location(new Vector(10, 55))
                .Direction(DirectionCar.East)
                .Sensor(Car => new LineSensor(Car, Direction.Left))
                .Sensor(Car => new LineSensor(Car, Direction.Right))
                .CurrentRoad(road)
                .Build();
            car.Destination = new Destination
            {
                Location = new Vector(targetX, targetY),
                Road = road
            };
            var controller = car.Controller;

            var velocity = new Vector(initialVelocity, 0.0);
            var yaw = 0.0;
            var addedRotation = 0.0;

            controller.HandleApproachTarget(ref velocity, ref yaw, ref addedRotation);

            Assert.AreEqual(expectedSpeed, velocity.Length, 0.01);
            Assert.AreEqual(expectedRotation, addedRotation, 0.01);
        }

        [TestCase(10.0, 55.0, 200.0, 55.0, 50, 100.0, 55.0)]
        [TestCase(10.0, 59.0, 200.0, 55.0, 50, 100.0, 55.0)]
        [TestCase(10.0, 51.0, 200.0, 55.0, 50, 100.0, 55.0)]
        [TestCase(10.0, 53.0, 200.0, 55.0, 50, 100.0, 55.0)]
        [TestCase(10.0, 58.0, 200.0, 55.0, 50, 100.0, 55.0)]
        [TestCase(10.0, 61.0, 200.0, 55.0, 50, 100.0, 55.0)]
        [TestCase(10.0, 49.0, 200.0, 55.0, 50, 100.0, 55.0)]
        public void TestUpdateStayInLane(double carX, double carY, double targetX, double targetY, int cycles, double expectedX, double expectedY)
        {
            var road = new Road(new Vector(0, 50), new Vector(50, 50), 20, 100);
            var city = new CityBuilder()
                .Road(road)
                .Build();

            var car = new CarBuilder()
                .Location(new Vector(carX, carY))
                .Direction(DirectionCar.East)
                .Sensor(Car => new LineSensor(Car, Direction.Left))
                .Sensor(Car => new LineSensor(Car, Direction.Right))
                .CurrentRoad(road)
                .Build();
            car.Destination = new Destination
            {
                Location = new Vector(targetX, targetY),
                Road = road
            };
            var controller = car.Controller;

            for (var time = 0; time < cycles; time++)
            {
                car.Sensors.ForEach(sensor => sensor.Controller.Update());
                car.Controller.Update();
            }
            
            Assert.AreEqual(expectedY, car.Location.Y, 1.0);
        }
    }
}
