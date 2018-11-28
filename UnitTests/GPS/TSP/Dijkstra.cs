using KBS2.CitySystem;
using KBS2.GPS.TSP;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using UnitTests.Util;

namespace UnitTests.GPS.TSP
{
    [TestFixture]
    class Dijkstra
    {
        [TestCase()]
        public void DijkstraTest()
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
                .Road(new Road(new Vector(100, 0), new Vector(100, 100), 10, 100))
                .Road(new Road(new Vector(0, 100), new Vector(100, 100), 10, 100))
                .Build();

            var route = NearestNeighbour.CalculateRouteToCustomerGroup(new Vector(0, 50), customer3);
        }
    }
}
