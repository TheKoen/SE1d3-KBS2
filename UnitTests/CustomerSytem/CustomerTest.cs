using KBS2.CustomerSystem;
using NUnit.Framework;
using System;

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
    }
}
