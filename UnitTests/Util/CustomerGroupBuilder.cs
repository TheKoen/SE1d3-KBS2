using KBS2.CitySystem;
using KBS2.Console;
using KBS2.CustomerSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CommandSystem.PropertyManagement;

namespace UnitTests.Util
{
    class CustomerGroupBuilder
    {
        private Random random = new Random();
        public CustomerGroup CustomerGroup { get; set; } 

        public CustomerGroupBuilder(Vector startLocation, Vector destination)
        {
            PropertyHandler.ResetProperties();
            CustomerGroup = new CustomerGroup(random.Next(1, (int)Math.Round(3 / 2.0 * 3.0)), new Building(startLocation, 10), new Building(destination, 10));
        }

    }
}
