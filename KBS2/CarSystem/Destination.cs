using KBS2.CitySystem;
using System;
using System.Windows;

namespace KBS2.CarSystem
{
    public struct Destination : IEquatable<Destination>
    {
        public Vector Location;
        public Road Road;

        public bool Equals(Destination other)
        {
            return Location.Equals(other.Location) && Road.Equals(other.Road);
        }
    }
}
