using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using KBS2.CarSystem;
using KBS2.Console;
using KBS2.CustomerSystem;
using KBS2.Util;

namespace KBS2.CitySystem
{
    public class CityController
    {
        public static readonly Random Random = new Random();
        public static int CAR_ID;

        private Property customerSpawnRate = new Property(0.2F);

        private City City { get; }

        public CityController(City city)
        {
            City = city;
            CommandHandler.RegisterProperty("customerSpawnRate", ref customerSpawnRate);
        }

        public void Update()
        {
            if (customerSpawnRate.Value <= 0) return;
            var rand = (int) Math.Round((1.0F / customerSpawnRate.Value) * 10.0F);
            if (City.Customers.Count < City.CustomerCount && Random.Next(rand) == 1)
            {
                SpawnCustomerGroup();
            }
        }

        public void Reset()
        {
            City.Customers.Clear();
            City.Cars.Clear();
        }

        public List<IEntity> GetEntities()
        {
            return new List<IEntity>()
                .Concat(City.Cars)
                .Concat(City.Customers)
                .ToList();
        }

        public List<IEntity> GetEntitiesInRange(Vector location, double range)
        {
            return GetEntities()
                .FindAll(entity => entity.GetPoints()
                    .Any(point => MathUtil.Distance(point, location) < range));
        }

        public void SpawnCustomerGroup()
        {
            var buildings = City.Buildings
                .FindAll(build => !(build is Garage));
            var building = buildings[Random.Next(buildings.Count)];
            var target = buildings[Random.Next(buildings.Count)];
            var groupSize = Random.Next(1, (int) Math.Round(City.AvgGroupSize / 2.0 * 3.0));

            var group = new CustomerGroup(groupSize, building, target);

            City.AddGroup(group);
        }
    }
}