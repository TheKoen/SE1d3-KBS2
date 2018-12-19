using KBS2.CitySystem;
using KBS2.CustomerSystem;
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
    class NearestNeighbourTest
    {      
        /** old test without destination 
        [TestCase(0, 0, TestName = "Test 1")]
        public void CalculateOrderCustomersTest1(int startX, int startY)
        {
            var city = new CityBuilder()
                .Road(new Road(new Vector(0, 0), new Vector(300, 0), 10, 100))
                .Build();

            var customerGroups = new List<CustomerGroupBuilder>()
            {
                new CustomerGroupBuilder(new Vector(100, 0), new Vector(100, 100)),
                new CustomerGroupBuilder(new Vector(200, 0), new Vector(200, 100)),
                new CustomerGroupBuilder(new Vector(300, 0), new Vector(300, 100))
            };

            var listCustomerGroups = new List<CustomerGroup>();
            customerGroups.ForEach(c => listCustomerGroups.Add(c.CustomerGroup));
            var orderdList = NearestNeighbour.CalculateOrderCustomers(new Vector(startX, startY), listCustomerGroups);

            Assert.AreEqual(listCustomerGroups, orderdList);
        }

        [TestCase(0, 0, TestName = "Test 2")]
        public void CalculateOrderCustomersTest2(int startX, int startY)
        {
            var city = new CityBuilder()
                .Road(new Road(new Vector(0, 0), new Vector(300, 0), 10, 100))
                .Build();

            var customerGroups = new List<CustomerGroupBuilder>()
            {
                new CustomerGroupBuilder(new Vector(200, 0), new Vector(100, 100)),
                new CustomerGroupBuilder(new Vector(100, 0), new Vector(200, 100)),
                new CustomerGroupBuilder(new Vector(300, 0), new Vector(300, 100))
            };

            var listCustomerGroups = new List<CustomerGroup>();
            listCustomerGroups.Add(customerGroups[1].CustomerGroup);
            listCustomerGroups.Add(customerGroups[0].CustomerGroup);
            listCustomerGroups.Add(customerGroups[2].CustomerGroup);

            var orderdList = NearestNeighbour.CalculateOrderCustomers(new Vector(startX, startY), listCustomerGroups);

            Assert.AreEqual(listCustomerGroups, orderdList);
        }
        **/
        [TestCase()]
        public void NearestNeighbourAlgoTest()
        {
            var city = new CityBuilder()
                .Build();

            var customerGroups = new List<CustomerGroupBuilder>()
            {
                new CustomerGroupBuilder(new Vector(200, 0), new Vector(200, 100)),
                new CustomerGroupBuilder(new Vector(100, 0), new Vector(200, 50)),
                new CustomerGroupBuilder(new Vector(300, 0), new Vector(300, 100))
            };


            var estimatedVectorOrder = new List<Vector>()
            {
                new Vector(100, 0),
                new Vector(200, 0),
                new Vector(200, 50),
                new Vector(200, 100),
                new Vector(300, 0),
                new Vector(300, 100)
            };

            //var item = new NearestNeighbour2().Calculate(new Vector(0, 0), customerGroups.Select(c => c.CustomerGroup).ToList());

            //Assert.AreEqual(estimatedVectorOrder, item);
        }
    }
}
