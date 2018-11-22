using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using KBS2.CarSystem;
using KBS2.CitySystem;
using KBS2.CustomerSystem;
using KBS2.Util;

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

        public static Road NearestRoad(Vector location)
        {
            var city = City.Instance;
            Road closestRoad = null;
            var closestDistance = double.MaxValue;

            foreach (var road in city.Roads)
            {
                var distance = MathUtil.DistanceToRoad(location, road);
                if (!(distance < closestDistance)) continue;

                closestDistance = distance;
                closestRoad = road;
            }
            return closestRoad;
        }

        public static void RequestCar(Destination destination, CustomerGroup group)
        {
            // Look to nearest Garage.
            var city = City.Instance;
            var garages = city.Buildings
                .FindAll(building => building is Garage)
                .Select(building => (Garage)building)
                .Where(g => g.AvailableCars > 0)
                .ToList();

            Garage nearestGarage = null;
            var nearestDistance = double.MaxValue;

            foreach (Garage garage in garages)
            {
                var tempDistance = MathUtil.Distance(group.Location, garage.Location);
                if(tempDistance < nearestDistance)
                {
                    nearestGarage = garage;
                    nearestDistance = tempDistance;
                }
            }

            // Nearest garage sends car  *OPTIONAL*(if available car applies to group needs)
            var car = nearestGarage.SpawnCar(CityController.CAR_ID++, CarModel.TestModel);
            car.Destination = destination;

        }

        public static Destination GetDirection(Car car, Intersection intersection)
        {
            // Car uses Nearest Neighbor Algorithm and Distance to customer to receive direction.
            // Check with connected roads
            // from dest point -> End point of road
            
            //intersection returns dictionary with DirectionCar, Roads
            return new Destination();

        }

        public static double CalculateDistance(Vector start, Vector end)
        {
            var road = NearestRoad(start);

            var distance1 = MathUtil.Distance(start, road.Start);
            var distance2 = MathUtil.Distance(end, road.End);
            var target = NearestRoad(end);

            if(distance1 > distance2){
                return ExploreIntersection(FindIntersection(road.Start), road, end, target, 0.0);
            }
            else
            {
                return ExploreIntersection(FindIntersection(road.End), road, end, target, 0.0);
            }
        }

        private static double ExploreIntersection(Intersection intersection, Road source, Vector end, Road target, double distance)
        {
            foreach (var road in intersection.GetRoads())
            {
                if (road.Equals(target))
                {
                    return distance + MathUtil.Distance(intersection.Location, end);
                }              
            }

            var intersections = FindNextIntersections(intersection);
            var closestIntersection = double.MaxValue;
            Intersection intersectionNext = null;

            foreach (var next in intersections)
            {
                if (next.Equals(intersection))
                {
                    continue;
                }
                var dist = MathUtil.Distance(next.Location, end);
                if(dist < closestIntersection)
                {
                    closestIntersection = dist;
                    intersectionNext = next;
                }
            }

            if(intersectionNext == null)
            {
                throw new Exception("Route is impossible.");
            }
            distance += MathUtil.Distance(intersection.Location, intersectionNext.Location);
            return ExploreIntersection(intersectionNext, source, end, target, distance);
        }

        public static List<Intersection> FindNextIntersections(Intersection intersection)
        {
            return new List<Intersection>();
        }

        public static Intersection FindIntersection(Vector location)
        {
            return null;
        }
    }
}