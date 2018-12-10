using KBS2.CitySystem;
using System.Collections.Generic;

namespace KBS2.CityDesigner
{
    public class LoadedCityEventArgs
    {
        public List<Road> Roads { get; set; }
        public List<Intersection> Intersections { get; set; }
        public List<Building> Buildings { get; set; }
        public List<Garage> Garages { get; set; }

        public LoadedCityEventArgs(List<Road> roads, List<Building> buildings, List<Garage> garages, List<Intersection> intersections)
        {
            Garages = garages;
            Roads = roads;
            Buildings = buildings;
            Intersections = intersections;
        }
    }
}