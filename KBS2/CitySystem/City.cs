using System.Collections.Generic;
using CommandSystem.PropertyManagement;
using KBS2.CarSystem;
using KBS2.CustomerSystem;

namespace KBS2.CitySystem
{
    public class City
    {
        public static City Instance { get; private set; }

        private readonly Property availableCars = new Property(5);
        public int AvailableCars {
            get => availableCars.Value;
            set => availableCars.Value = value;
        }

        private readonly Property customerCount = new Property(10);
        public int CustomerCount {
            get => customerCount.Value;
            set => customerCount.Value = value;
        }

        private readonly Property speedLimit = new Property(-1);
        public int SpeedLimit {
            get => speedLimit.Value;
            set => speedLimit.Value = value;
        }

        private readonly Property avgGroupSize = new Property(1);
        public int AvgGroupSize {
            get => avgGroupSize.Value;
            set => avgGroupSize.Value = value;
        }

        public List<Road> Roads { get; }
        public List<Building> Buildings { get; }
        public List<Intersection> Intersections { get; }
        public List<Car> Cars { get; }
        public List<Customer> Customers { get; }

        public CityController Controller { get; }

        public City()
        {
            Instance = this;

            Roads = new List<Road>();
            Buildings = new List<Building>();
            Intersections = new List<Intersection>();
            Cars = new List<Car>();
            Customers = new List<Customer>();

            Controller = new CityController(this);
            MainScreen.AILoop.Subscribe(Controller.Update);

            PropertyHandler.RegisterProperty("availableCars", ref availableCars);
            PropertyHandler.RegisterProperty("customerCount", ref customerCount);
            PropertyHandler.RegisterProperty("globalSpeedLimit", ref speedLimit);
            PropertyHandler.RegisterProperty("avgGroupSize", ref avgGroupSize);
        }

        public void AddGroup(CustomerGroup group)
        {
            Customers.AddRange(group.Customers);
        }
    }
}
