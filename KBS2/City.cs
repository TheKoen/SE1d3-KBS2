using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace KBS2
{
    class City
    {
        private static City instance;
        public List<Road> Roads { get; set; }
        public List<Building> Buildings { get; set; }
        public List<Car> Cars { get; set; }

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

        public void loadCity(XmlDocument city)
        {
        }

    }
}
