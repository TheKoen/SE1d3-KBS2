using KBS2.CustomerSystem;
using NUnit.Framework;
using System;
using System.Windows;
using KBS2.CitySystem;
using KBS2.Util;
using UnitTests.Util;

namespace UnitTests.CustomerSytem
{
    [TestFixture]
    public class CustomerTest
    {
        [TestCase(1, Moral.Happy)]
        [TestCase(6, Moral.Neutral)]
        [TestCase(11, Moral.Annoyed)]
        [TestCase(16, Moral.Sad)]
        [TestCase(21, Moral.Mad)]
        public void TestMoral(int time, Moral expected)
        {
            var customer = new Customer(new System.Windows.Vector(0,0), 6, null, null);

            for(int i = 0; i < time * 100; i++)
            {
                customer.Controller.Update();
            }

            Assert.AreEqual(expected, customer.Mood);
        }

        [TestCase(50, 50, 1000)]
        [TestCase(100, 50, 1000)]
        [TestCase(150, 50, 1000)]
        [TestCase(50, 100, 1000)]
        [TestCase(150, 100, 1000)]
        [TestCase(50, 150, 1000)]
        [TestCase(100, 150, 1000)]
        [TestCase(150, 150, 1000)]
        public void TestMoveTowardsLocation(double tx, double ty, double time)
        {
            var building = new Building(new Vector(100, 100), 1);
            var city = new CityBuilder()
                .Building(building)
                .Build();

            var group = new CustomerGroup(1, building, building);
            var customer = group.Customers[0];

            var target = new Vector(tx, ty);
            for (var i = 0; i < time; i++)
            {
                customer.Controller.MoveTowardsLocation(target);
            }

            Assert.LessOrEqual(MathUtil.Distance(target, customer.Location), 1);
        }
    }
}
