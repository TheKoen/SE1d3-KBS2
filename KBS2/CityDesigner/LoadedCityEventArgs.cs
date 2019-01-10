using KBS2.CitySystem;
using System.Collections.Generic;

namespace KBS2.CityDesigner
{
    public class LoadedCityEventArgs
    {
        public List<Road> Roads { get; }
        public List<Intersection> Intersections { get; }
        public List<Building> Buildings { get; }
        public List<Garage> Garages { get; }
        public string Path { get; }

        public LoadedCityEventArgs(List<Road> roads, List<Building> buildings, List<Garage> garages, List<Intersection> intersections, string path)
        {
            Garages = garages;
            Roads = roads;
            Buildings = buildings;
            Intersections = intersections;
            Path = path;
        }
    }
}