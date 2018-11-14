﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using KBS2.CitySystem;

namespace UnitTests.Util
{
    public class CityBuilder
    {
        private City City { get; set; }

        public CityBuilder()
        {
            City = new City();
        }

        public CityBuilder Road(Vector start, Vector end, int width)
        {
            City.Roads.Add(new Road(start, end, width, 100));
            return this;
        }

        public CityBuilder Building(Vector location, int size)
        {
            City.Buildings.Add(new Building(location, size));
            return this;
        }

        public City Build()
        {
            return City;
        }
    }
}
