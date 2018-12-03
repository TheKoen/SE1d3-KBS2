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
        [TestCase(40, 20, DirectionCar.South, 12.5, 20)]
        [TestCase(40, 20, DirectionCar.North, 12.5, 20)]
        [TestCase(60, 60, DirectionCar.East, 60, 87.5)]
        [TestCase(60, 60, DirectionCar.West, 60, 87.5)]
        public void TestSpawnCar(double gx, double gy, DirectionCar direction, double ex, double ey)
        {
            
            var city = new CityBuilder()
                .Road(new Vector(10, 0), new Vector(10, 100), 10)
                .Road(new Vector(0, 90), new Vector(100, 90), 10)
                .Build();

            var garage = new Garage(new Vector(gx, gy), 20, direction);
            city.Buildings.Add(garage);

            var car = garage.SpawnCar(0, CarModel.Get("TestModel"));

            Assert.AreEqual(new Vector(ex, ey), car.Location);
        }

    }
}
