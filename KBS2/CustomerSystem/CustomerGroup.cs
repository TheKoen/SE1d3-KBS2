﻿using System;
using System.Collections.Generic;
using System.Windows;
using KBS2.CitySystem;

namespace KBS2.CustomerSystem
{
    public class CustomerGroup
    {
        private static Random random = new Random();
        public List<Customer> Customers { get; set; } = new List<Customer>();
        public Building Destination { get; set; }
        public CustomerGroupController Controller { get; set; }
        public Vector Location { get; set; } 

        public CustomerGroup(int customers, Building start, Building destination)
        {
            for(int i = 0; i < customers; i++)
            {
                Customers.Add(new Customer(start.Location, random.Next(4, 90), start, this));
            }
            Location = start.Location;
            Destination = destination;
            Controller = new CustomerGroupController(this);
        }
    }
}
