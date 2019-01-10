using System;
using System.Collections.Generic;
using System.Linq;
using CommandSystem.PropertyManagement;
using KBS2.Database;
using KBS2.Util;
using KBS2.Visual;
using CustomerGroup = KBS2.CustomerSystem.CustomerGroup;
using Vector = System.Windows.Vector;

namespace KBS2.CitySystem
{
    public delegate void CustomerGroupAddEvent(object source, GroupEventArgs args);

    public class CityController
    {
        public static int CAR_ID;

        public static event CustomerGroupAddEvent OnCustomerGroupAdd;

        private static readonly Random Random = new Random();
        private readonly Property customerSpawnRate = new Property(0.05F);

        private City City { get; }

        public CityController(City city)
        {
            City = city;
            PropertyHandler.RegisterProperty("customerSpawnRate", ref customerSpawnRate);
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

            if (building == target) return;
            var groupSize = Random.Next(1, (int) Math.Round(City.AvgGroupSize / 2.0 * 3.0));

            var group = new CustomerGroup(groupSize, building, target, (finishedGroup) =>
            {
                OnCustomerGroupAdd?.Invoke(this, new GroupEventArgs(finishedGroup, finishedGroup.Customers));
            });

            City.AddGroup(group);
            
        }
    }
}