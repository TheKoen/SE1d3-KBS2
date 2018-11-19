using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using KBS2.CarSystem;
using KBS2.CitySystem;

namespace KBS2.GPS
{
    public class GPSSystem
    {
        /// <summary>
        /// returns a road located at this location
        /// </summary>
        /// <param name="location">location you want to check for a road</param>
        /// <returns>the road that the location is at, if none it returns null</returns>
        public static Road GetRoad(Vector location)
        {
            var roads = City.Instance.Roads;

            foreach (var road in roads)
            {
                var laneWidth = road.Width / 2.0;
                var minX = road.IsXRoad()
                    ? Math.Min(road.Start.X, road.End.X)
                    : road.Start.X - laneWidth;
                var maxX = road.IsXRoad()
                    ? Math.Max(road.Start.X, road.End.X)
                    : road.Start.X + laneWidth;
                var minY = road.IsXRoad()
                    ? road.Start.Y - laneWidth
                    : Math.Min(road.Start.Y, road.End.Y);
                var maxY = road.IsXRoad()
                    ? road.Start.Y + laneWidth
                    : Math.Max(road.Start.Y, road.End.Y);

                if (location.X >= minX && location.X <= maxX && location.Y >= minY && location.Y <= maxY)
                {
                    return road;
                }
            }

            return null;
        }

        public static List<Road> GetRoadsInRange(Vector location, int range)
        {
            var roads = new List<Road>
            {
                GetRoad(new Vector(location.X + range, location.Y)),
                GetRoad(new Vector(location.X - range, location.Y)),
                GetRoad(new Vector(location.X, location.Y + range)),
                GetRoad(new Vector(location.X, location.Y + range)),
                GetRoad(new Vector(location.X, location.Y))
            };
            roads.RemoveAll(road => road == null);
            return roads.Distinct().ToList();
        }
    }
}