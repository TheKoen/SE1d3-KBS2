using KBS2.CarSystem;
using NUnit.Framework;
using System.Windows;
using KBS2.CitySystem;
using UnitTests.Util;

namespace UnitTests.CitySystem
{
    [TestFixture]
    public class GarageTest
    {
        [TestCase(40, 20, DirectionCar.South, 6.6, 20)]
        [TestCase(40, 20, DirectionCar.North, 13.3, 20)]
        [TestCase(60, 60, DirectionCar.East, 60, 93.3)]
        [TestCase(60, 60, DirectionCar.West, 60, 86.6)]
        public void TestSpawnCar(double gx, double gy, double ex, double ey)
        {
            
            var city = new CityBuilder()
                .Road(new Vector(10, 0), new Vector(10, 100), 10)
                .Road(new Vector(0, 90), new Vector(100, 90), 10)
                .Build();

            var garage = new Garage(new Vector(gx, gy), 20);
            city.Buildings.Add(garage);

            var car = garage.SpawnCar(0, CarModel.Get("TestModel"), new Destination());

            Assert.AreEqual(ex, car.Location.X, 0.5);
            Assert.AreEqual(ey, car.Location.Y, 0.5);
        }

    }
}
