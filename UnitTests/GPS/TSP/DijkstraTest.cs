using KBS2.CitySystem;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using UnitTests.Util;
using KBS2.GPS.TSP.Route;

namespace UnitTests.GPS.TSP
{
    [TestFixture]
    class DijkstraTest
    {
        [TestCase()]
        public void DijkstraTest1()
        {
            var customerGroupBuilder = new List<CustomerGroupBuilder>()
            {
                new CustomerGroupBuilder(new Vector(0, 0), new Vector(0, 0)),
                new CustomerGroupBuilder(new Vector(50, 0), new Vector(0, 0)),
                new CustomerGroupBuilder(new Vector(100, 50), new Vector(0, 0))
            };

            var customer3 = customerGroupBuilder[2].CustomerGroup;

            var city = new CityBuilder()
                .Road(new Road(new Vector(0, 0), new Vector(0, 100), 10, 100))
                .Road(new Road(new Vector(0, 0), new Vector(100, 0), 10, 100))
                .Road(new Road(new Vector(0, 100), new Vector(100, 100), 10, 100))
                .Build();

            var estimatedRoads = new List<Road>()
            {
                new Road(new Vector(0, 0), new Vector(0, 100), 10, 100),
                new Road(new Vector(0, 100), new Vector(100, 100), 10, 100)
            };

            var route = new Dijkstra().CalculatePath(new Vector(0,0), customer3);

            Assert.AreEqual(estimatedRoads, route);

        }
    }
}
