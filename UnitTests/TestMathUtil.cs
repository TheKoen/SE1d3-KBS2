using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using KBS2.CitySystem;
using KBS2.Util;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace UnitTests
{
    [TestFixture]
    public class TestMathUtil
    {
        [TestCase(200, 100, 100, 100, 0)]
        [TestCase(100, 200, 100, 100, 0)]
        [TestCase(100, 0, 100, 100, 0)]
        [TestCase(0, 100, 100, 100, 0)]

        [TestCase(200, 100, 0, 0, 141.42)]
        [TestCase(100, 200, 0, 0, 141.42)]
        [TestCase(100, 0, 0, 0, 100)]
        [TestCase(0, 100, 0, 0, 100)]

        [TestCase(200, 100, 200, 0, 100)]
        [TestCase(100, 200, 200, 0, 141.42)]
        [TestCase(100, 0, 200, 0, 100)]
        [TestCase(0, 100, 200, 0, 141.42)]

        [TestCase(200, 100, 200, 200, 100)]
        [TestCase(100, 200, 200, 200, 100)]
        [TestCase(100, 0, 200, 200, 141.42)]
        [TestCase(0, 100, 200, 200, 141.42)]


        [TestCase(200, 100, 0, 200, 141.42)]
        [TestCase(100, 200, 0, 200, 100)]
        [TestCase(100, 0, 0, 200, 141.42)]
        [TestCase(0, 100, 0, 200, 100)]
        public void TestDistanceToRoad(double rx, double ry, double px, double py, double expected)
        {
            var road = new Road(new Vector(100, 100), new Vector(rx, ry), 10, 100);
            var point = new Vector(px, py);

            var distance = MathUtil.DistanceToRoad(point, road);

            Assert.AreEqual(expected, distance, 0.01F);
        }
    }
}
