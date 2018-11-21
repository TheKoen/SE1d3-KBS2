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
        [TestCase(100, 0, 20, 20, 20)]
        [TestCase(0, 100, 20, 20, 20)]
        [TestCase(0, 100, -20, -20, 20)]
        [TestCase(100, 0, -20, -20, 20)]
        public void TestDistanceToRoad(double rx, double ry, double px, double py, double expected)
        {
            var road = new Road(new Vector(0, 0), new Vector(rx, ry), 10, 100);
            var point = new Vector(px, py);

            var distance = MathUtil.DistanceToRoad(point, road);

            Assert.AreEqual(expected, distance, 0.01F);
        }
    }
}
