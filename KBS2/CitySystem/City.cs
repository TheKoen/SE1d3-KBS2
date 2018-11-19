using System.Collections.Generic;
using System.Linq;
using System.Xml;
using KBS2.CarSystem;
using KBS2.Console;
using KBS2.CustomerSystem;
using KBS2.Utilities;

namespace KBS2.CitySystem
{
    public class City
    {
        public static City Instance { get; private set; }

        private Property availableCars = new Property(5);
        public int AvailableCars {
            get => availableCars.Value;
            set => availableCars.Value = value;
        }

        private Property customerCount = new Property(20);
        public int CustomerCount {
            get => customerCount.Value;
            set => customerCount.Value = value;
        }

        private Property speedLimit = new Property(-1);
        public int SpeedLimit {
            get => speedLimit.Value;
            set => speedLimit.Value = value;
        }

        private Property avgGroupSize = new Property(3);
        public int AvgGroupSize {
            get => avgGroupSize.Value;
            set => avgGroupSize.Value = value;
        }

        public List<Road> Roads { get; set; }
        public List<Building> Buildings { get; set; }
        public List<Intersection> Intersections { get; set; }
        public List<Car> Cars { get; set; }
        public List<Customer> Customers { get; set; }

        public City()
        {
            Instance = this;

            Roads = new List<Road>();
            Buildings = new List<Building>();
            Intersections = new List<Intersection>();
            Cars = new List<Car>();

            CommandHandler.RegisterProperty("availableCars", ref availableCars);
            CommandHandler.RegisterProperty("customerCount", ref customerCount);
            CommandHandler.RegisterProperty("globalSpeedLimit", ref speedLimit);
            CommandHandler.RegisterProperty("avgGroupSize", ref avgGroupSize);
        }

        public List<IEntity> GetEntities()
        {
            return new List<IEntity>()
                .Concat(Cars)
                .Concat(Customers)
                .ToList();
        }

        public void AddGroup(CustomerGroup group)
        {
            Customers.AddRange(group.Customers);
        }
    }
}
