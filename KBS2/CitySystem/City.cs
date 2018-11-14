using System.Collections.Generic;
using System.Xml;
using KBS2.CarSystem;

namespace KBS2.CitySystem
{
    public class City
    {
        private static City instance;
        public List<Road> Roads { get; set; }
        public List<Building> Buildings { get; set; }
        public List<Intersection> Intersections { get; set; }
        public List<Car> Cars { get; set; }

        public City()
        {
            instance = this;
            Roads = new List<Road>();
            Buildings = new List<Building>();
            Intersections = new List<Intersection>();
            Cars = new List<Car>();
        }

        public static City Instance()
        {
            return instance;
        }
    }
}
