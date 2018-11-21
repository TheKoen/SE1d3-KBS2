using KBS2.CitySystem;
using KBS2.GPS;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using UnitTests.Util;

namespace UnitTests.GPS
{
    [TestFixture]
    class GPSSystemTest
    {
        [TestCase(0, 0, 100, 0, 100, 100, 200, 100, 0, 50)] // get road location
        public void GetRoadTest(int x1b, int y1b, int x1e, int y1e, int x2b, int y2b, int x2e, int y2e, int sx, int sy)
        {
            var road1 = new Road(new Vector(x1b, y1b), new Vector(x1e, y1e), 20, 100);
            var road2 = new Road(new Vector(x2b, y2b), new Vector(x2e, y2e), 20, 100);

            var city = new CityBuilder()
                .Road(road1)
                .Road(road2)
                .Build();

            if (GPSSystem.GetRoad(new Vector(x1b, y1b)).Equals(road1))
            {
                Assert.Pass();
            }
            else
            {
                Assert.Fail();
            }  
        }
        
        [TestCase()]
        public void NearestRoadTest(int x1b, int y1b, int x1e, int y1e, int x2b, int y2b, int x2e, int y2e, int ex, int ey)
        {
            var road1 = new Road(new Vector(x1b, y1b), new Vector(x1e, y1e), 20, 100);
            var road2 = new Road(new Vector(x2b, y2b), new Vector(x2e, y2e), 20, 100);

            var city = new CityBuilder()
                .Road(road1)
                .Road(road2)
                .Build();

            var road = GPSSystem.NearestRoad(new Vector(ex, ey));

            if(road.Equals(road1))
            {
                Assert.Pass();
            }
            else
            {
                Assert.Fail("estamited is not equal to the first road");
            }
        }
    }
}
