using System;
using System.Windows;
using KBS2.CitySystem;
using KBS2.CustomerSystem;
using NUnit.Framework;
using UnitTests.Util;

namespace UnitTests.CustomerSytem
{
    [TestFixture]
    public class CustomerGroupTest
    {
        [TestCase(39, 60, true)]
        [TestCase(39, 5, false)]
        public void TestLookForNearestRoad(double groupX, double groupY, bool road)
        {
            var building = new Building(new Vector(groupX, groupY), 8);
            var road1 = new Road(new Vector(0, 50), new Vector(10, 50), 10, 100);
            var road2 = new Road(new Vector(0, 20), new Vector(10, 20), 10, 100);
            var city = new CityBuilder()
                .Road(road1)
                .Road(road2)
                .Building(building)
                .Build();

            var group = new CustomerGroup(5, building, null);

            var result = group.Controller.LookForNearestRoad();

            Assert.AreEqual(road ? road1 : road2, result);
        }

        [Test]
        public void TestMoveToNearestRoad()
        {
            var building = new Building(new Vector(100, 100), 8);
            var road = new Road(new Vector(0, 50), new Vector(100, 50), 10, 100);
            var city = new CityBuilder()
                .Road(road)
                .Building(building)
                .Build();

            var group = new CustomerGroup(5, building, null);

            group.Controller.MoveToNearestRoad(road);

            Assert.AreEqual(new Vector(100, 56), group.Location);
        }
    }
}
