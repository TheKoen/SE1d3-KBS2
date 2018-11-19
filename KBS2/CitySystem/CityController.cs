using System;
using System.Linq;
using KBS2.CarSystem;
using KBS2.CustomerSystem;

namespace KBS2.CitySystem
{
    public class CityController
    {
        public static readonly Random Random = new Random();
        private static int CAR_ID;

        private City City { get; }

        public CityController(City city)
        {
            City = city;
        }

        public void Update()
        {
            if (City.Customers.Count < City.CustomerCount && Random.Next(20) == 1)
            {
                SpawnCustomerGroup();
            }

            if (City.Cars.Count < City.AvailableCars)
            {
                SpawnCar();
            }
        }

        public void SpawnCustomerGroup()
        {
            var building = City.Buildings[Random.Next(City.Buildings.Count)];
            var target = City.Buildings[Random.Next(City.Buildings.Count)];
            var groupSize = Random.Next(1, (int) Math.Round(City.AvgGroupSize / 2.0 * 3.0));

            var group = new CustomerGroup(groupSize, building, target);

            City.AddGroup(group);
        }

        public void SpawnCar()
        {
            var garages = City.Buildings
                .FindAll(building => building is Garage)
                .Select(building => (Garage) building)
                .ToList();
            var garage = garages[Random.Next(garages.Count)];
            var model = CarModel.TestModel; //TODO: Select model based on settings
            var direction = DirectionCar.East;

            garage.SpawnCar(CAR_ID++, model, direction);
        }
    }
}