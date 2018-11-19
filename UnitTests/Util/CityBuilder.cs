using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using KBS2.CitySystem;
using KBS2.Console;

namespace UnitTests.Util
{
    public class CityBuilder
    {
        private City City { get; set; }

        public CityBuilder()
        {
            CommandHandler.Reset();
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

        public City Build()
        {
            return City;
        }
    }
}
