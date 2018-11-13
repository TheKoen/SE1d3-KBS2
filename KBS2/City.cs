using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBS2
{
    class City
    {
        public List<Road> roads { get; set; }
        public List<Building> buildings { get; set; }
        public List<Car> cars { get; set; }

        public City()
        {
            roads = new List<Road>();
            buildings = new List<Building>();
            cars = new List<Car>();
        }
    }
}
