using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using CommandSystem.PropertyManagement;
using KBS2.CarSystem;
using KBS2.CitySystem;
using KBS2.CustomerSystem;
using KBS2.GPS.Algorithms;
using KBS2.GPS.TSP;
using KBS2.Util;

namespace KBS2.GPS
{
    public class GPSSystem
    {
        public static Property StartingPrice = new Property(1.50);
        public static Property PricePerKilometer = new Property(1.00);
        public static Property availableModel = new Property("TestModel");
        public static CarModel AvailableModel => CarModel.Get(availableModel.Value);

        private static IAlgorithm Algorithm = new AlgorithmDijkstra();

        public static void Setup()
        {
            try
            {
                PropertyHandler.RegisterProperty("startingPrice", ref StartingPrice);
                PropertyHandler.RegisterProperty("pricePerKilometer", ref PricePerKilometer);
                //PropertyHandler.RegisterProperty("availableModel", ref availableModel);
            }
            catch (Exception)
            {
                App.Console?.Print($"Unable to register tickRate property for GPSSystem", Colors.Yellow);
            }
        }


        /// <summary>
        /// returns a road located at this location
        /// </summary>
        /// <param name="location">location you want to check for a road</param>
        /// <returns>the road that the location is at, if none it returns null</returns>
        public static Road GetRoad(Vector location)
        {
            var roads = GetRoadsInRange(location, 12);
            return roads.Count == 0 ? null : roads.First();
        }

        /// <summary>
        /// returns the Roads in range
        /// </summary>
        /// <param name="location">Vector point</param>
        /// <param name="range">int range</param>
        /// <returns>List<Road> roads</returns>
        public static List<Road> GetRoadsInRange(Vector location, int range)
        {
            return City.Instance.Roads
                .FindAll(road =>
                {
                    var dist = MathUtil.DistanceToRoad(location, road);
                    return dist <= range;
                });
        }

        /// <summary>
        /// returns the nearest road
        /// </summary>
        /// <param name="location">Vector location</param>
        /// <returns>Road road</returns>
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

        /// <summary>
        /// Request a car 
        /// </summary>
        /// <param name="destination">destination the group want to go</param>
        /// <param name="group">group who request a car</param>
        public static void RequestCar(Destination destination, CustomerGroup group)
        {
            try
            {
                var distance = CalculateDistance(group.Location, destination.Location);
                var price = CalculatePrice(distance);
                App.Console?.Print(
                    $"Group #{group.GetHashCode()} has requested a car from {group.Location} to {destination.Location}. Total price: €{price:0.00}"
                );
            }
            catch (Exception)
            {
                App.Console?.Print($"Unable to calculate route from {group.Location} to {destination.Location}", Colors.Red);
            }

            // Look to nearest Garage.
            var city = City.Instance;
            var garages = city.Buildings
                .FindAll(building => building is Garage)
                .Select(building => (Garage) building)
                .Where(g => g.AvailableCars > 0)
                .ToList();

            Garage nearestGarage = null;
            var nearestDistance = double.MaxValue;

            foreach (var garage in garages)
            {
                var tempDistance = MathUtil.Distance(group.Location, garage.Location);
                if (!(tempDistance < nearestDistance)) continue;
                nearestGarage = garage;
                nearestDistance = tempDistance;
            }

            nearestGarage?.SpawnCar(CityController.CAR_ID++, AvailableModel, new Destination
            {
                Location = group.Location,
                Road = NearestRoad(group.Location)
            });
        }

        /// <summary>
        /// returns 
        /// </summary>
        /// <param name="car"></param>
        /// <param name="intersection"></param>
        /// <returns></returns>
        public static Destination GetDirection(Car car, Intersection intersection)
        {
            var road = car.CurrentRoad;
            var location = MathUtil.Distance(road.Start, car.Location) <
                           MathUtil.Distance(road.End, car.Location)
                ? road.Start
                : road.End;
            return Algorithm.Calculate(new Destination
            {
                Location = location,
                Road = car.CurrentRoad
            }, car.Destination);
        }

        public static double CalculateDistance(Vector start, Vector end)
        {
            var road = NearestRoad(start);

            var distance1 = MathUtil.Distance(end, road.Start);
            var distance2 = MathUtil.Distance(end, road.End);
            var target = NearestRoad(end);

            try
            {
                return ExploreIntersection(distance1 > distance2
                        ? FindIntersection(road.End)
                        : FindIntersection(road.Start),
                    end, target, 0.0, 0, new List<Intersection>());
            }
            catch (Exception)
            {
                App.Console?.Print($"Unable to calculate route from {start} to {end}.", Colors.Red);
                return 0;
            }
        }

        private static double ExploreIntersection(Intersection intersection, Vector end, Road target, double distance, int cycles, ICollection<Intersection> past)
        {
            while (true)
            {
                if (intersection == null)
                {
                    App.Console?.Print("Warning: Found a problem while exporing route: null terminating road", Colors.Yellow);
                    return distance;
                }

                past.Add(intersection);

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

                //App.Console?.Print($"Arrived at intersection {intersection.Location}. Found {intersections.Count} options. Evaluating...");

                foreach (var next in intersections)
                {
                    if (next == null || past.Contains(next)) continue;

                    var dist = MathUtil.Distance(next.Location, end);
                    if (!(dist < closestIntersection)) continue;
                    closestIntersection = dist;
                    intersectionNext = next;
                }

                if (cycles++ == 20)
                {
                    throw new Exception("Route is impossible.");
                }

                if (intersectionNext == null)
                {
                    throw new Exception("Route is impossible.");
                }

                distance += MathUtil.Distance(intersection.Location, intersectionNext.Location);
                intersection = intersectionNext;
            }
        }

        /// <summary>
        /// Find the next intersection of an intersection
        /// </summary>
        /// <param name="intersection">intersection to use</param>
        /// <returns>List with the found intersections</returns>
        public static List<Intersection> FindNextIntersections(Intersection intersection)
        {
            var list = new List<Intersection>();
            var roads = intersection.GetRoads();
            foreach (var road in roads)
            {
                list.Add(MathUtil.Distance(intersection.Location, road.Start) >
                         MathUtil.Distance(intersection.Location, road.End)
                    ? FindIntersection(road.Start)
                    : FindIntersection(road.End));
            }

            list.RemoveAll(intersect => intersect == null || intersect.Equals(intersection));

            return list;
        }

        public static Intersection FindIntersection(Vector location)
        {
            var interserction = City.Instance.Intersections.Find(intersection =>
            {
                var size = intersection.Size / 2d;
                var point = intersection.Location;
                return point.X >= location.X - size && point.X <= location.X + size &&
                       point.Y >= location.Y - size && point.Y <= location.Y + size;
            });
            return interserction;
        }

        /// <summary>
        /// Find the intersections of a road
        /// </summary>
        /// <param name="road">road you want to explore</param>
        /// <returns>List with found Intersections</returns>
        public static List<Intersection> FindIntersectionsRoad(Road road)
        {
            return new List<Intersection>
            {
                FindIntersection(road.Start),
                FindIntersection(road.End)
            };
            ;
        }

        public static double CalculatePrice(double distance)
        {
            return StartingPrice.Value + distance / 100.0 * PricePerKilometer.Value;
        }
    }
}