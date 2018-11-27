using System.Collections.Generic;
using System.Windows;
using KBS2.GPS;

namespace KBS2.CitySystem
{
    public class Intersection
    {
        public Vector Location { get; }
        public int Size { get; }

        public Intersection(Vector location, int size)
        {
            Location = location;
            Size = size;
        }

        public List<Road> GetRoads()
        {
            return GPSSystem.GetRoadsInRange(Location, Size);
        } 
    }
}
