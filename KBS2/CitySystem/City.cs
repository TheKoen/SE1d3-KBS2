using System.Collections.Generic;
using System.Linq;
using System.Xml;
using KBS2.CarSystem;
using KBS2.CustomerSystem;

namespace KBS2.CitySystem
{
    public class City
    {
        private static City instance;
        public List<Road> Roads { get; set; }
        public List<Building> Buildings { get; set; }
        public List<Car> Cars { get; set; }
        public List<Customer> Customers { get; set; }

        public City()
        {
            instance = this;
            Roads = new List<Road>();
            Buildings = new List<Building>();
            Cars = new List<Car>();
        }

        public static City Instance()
        {
            return instance;
        }

        public List<IEntity> GetEntities()
        {
            return new List<IEntity>()
                .Concat(Cars)
                .Concat(Customers)
                .ToList();
        }
    }
}
