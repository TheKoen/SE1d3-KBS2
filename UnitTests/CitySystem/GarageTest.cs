using KBS2.CarSystem;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using UnitTests.Util;
using KBS2.CitySystem.Garage;

namespace UnitTests.CitySystem
{
    [TestFixture]
    public class GarageTest
    {

        [TestCase(40, 50, DirectionCar.South)]
        [TestCase(40, 50, DirectionCar.North)]
        [TestCase(60, 60, DirectionCar.East)]
        [TestCase(60, 60, DirectionCar.West)]




        public void TestSpawnCar(double gx, double gy, DirectionCar direction)
        {
            
            var city = new CityBuilder()
                .Road(new Vector(10, 0), new Vector(10, 100), 10)
                .Road(new Vector(0, 90), new Vector(100, 90), 10)
                .Build();

            var garage = new Garage(new Vector(gx, gy), 20, direction);
            city.Buildings.Add(garage);

            garage.SpawnCar(0, CarModel.TestModel);
        }

    }
}
