﻿using System.Windows;

namespace KBS2.CitySystem
{
    public class Building
    {

        public Vector Location { get; }
        public int Size { get; set; }

        public Building(Vector location, int size)
        {
            Location = location;
            Size = size;
        }
    }
}
