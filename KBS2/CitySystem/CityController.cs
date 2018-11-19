using System;
using System.Linq;
using KBS2.Console;
using KBS2.CustomerSystem;
using KBS2.Utilities;

namespace KBS2.CitySystem
{
    public class CityController
    {
        public static readonly Random Random = new Random();

        private City City { get; }

        public CityController(City city)
        {
            City = city;
        }

        public void Update()
        {
            if (City.Customers.Count < City.CustomerCount && Random.Next(10) == 1)
            {
                SpawnCustomerGroup();
            }
        }

        public void SpawnCustomerGroup()
        {
            var building = City.Buildings[Random.Next(City.Buildings.Count)];
            var target = City.Buildings[Random.Next(City.Buildings.Count)];
            var groupSize = Random.Next(1, (int) Math.Round((City.AvgGroupSize / 2.0) * 3.0));

            var group = new CustomerGroup(groupSize, building, target);

            City.AddGroup(group);
        }

        public void SpawnCar()
        {
        }
    }
}
