using System.Windows;
using CommandSystem.PropertyManagement;
using KBS2.CitySystem;

namespace UnitTests.Util
{
    public class CityBuilder
    {
        private City City { get; }

        public CityBuilder()
        {
            PropertyHandler.ResetProperties();
            City = new City();
        }

        public CityBuilder Road(Vector start, Vector end, int width)
        {
            City.Roads.Add(new Road(start, end, width, 100));
            return this;
        }

        public CityBuilder Road(Road road)
        {
            City.Roads.Add(road);
            return this;
        }

        public CityBuilder Building(Vector location, int size)
        {
            City.Buildings.Add(new Building(location, size));
            return this;
        }

        public CityBuilder Building(Building building)
        {
            City.Buildings.Add(building);
            return this;
        }

        public CityBuilder Intersection(Vector location, int size)
        {
            City.Intersections.Add(new Intersection(location, size));
            return this;
        }

        public CityBuilder Intersection(Intersection intersection)
        {
            City.Intersections.Add(intersection);
            return this;
        }

        public City Build()
        {
            return City;
        }
    }
}
