using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using KBS2.CarSystem;
using KBS2.CitySystem;
using KBS2.Console;
using KBS2.CustomerSystem;
using KBS2.Util;

namespace KBS2.GPS
{
    public class GPSSystem
    {
        private static Property StartingPrice = new Property(1.50);
        private static Property PricePerKilometer = new Property(1.00);
        private static Property availableModel = new Property("TestModel");
        private static CarModel AvailableModel => CarModel.Get(availableModel.Value);

        public static void Setup()
        {
            CommandHandler.RegisterProperty("startingPrice", ref StartingPrice);
            CommandHandler.RegisterProperty("pricePerKilometer", ref PricePerKilometer);
            CommandHandler.RegisterProperty("availableModel", ref availableModel);
        }

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
                MainWindow.Console.Print(
                    $"Group #{group.GetHashCode()} has requested a car from {group.Location} to {destination.Location}. Total price: €{price:0.00}");
            }
            catch (Exception) { }

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

            if (nearestGarage == null) return;

            var car = nearestGarage.SpawnCar(CityController.CAR_ID++, AvailableModel);
            car.Destination = destination;
        }

        /// <summary>
        /// returns the direction of a car
        /// </summary>
        /// <param name="car"></param>
        /// <param name="intersection"></param>
        /// <returns></returns>
        public static Destination GetDirection(Car car, Intersection intersection)
        {
            var roadsAtInteresection = intersection.GetRoads();
            var roads = new List<Road>();

            foreach (var road in roadsAtInteresection)
            {
                if (!car.CurrentRoad.Equals(road)) roads.Add(road);
            }

            var shortestDistance = double.MaxValue;
            Road selectedRoad = null;
            var selectDestination = new Vector();

            foreach (var road in roads)
            {
                if (road.Equals(car.Destination.Road))
                {
                    var destination = car.Destination;
                    var distance = MathUtil.DistanceToRoad(destination.Location, road) - road.Width / 4d;
                    var direction = GetDirectionToRoad(destination.Location, road);
                    var delta = Vector.Multiply(direction.GetDirection(), distance);
                    var target = Vector.Add(destination.Location, delta);

                    return new Destination {Road = road, Location = target};
                }

                var tempDStart = MathUtil.Distance(road.Start, car.Location);
                var tempDEnd = MathUtil.Distance(road.End, car.Location);

                var closestPointToDestination = tempDStart > tempDEnd ? road.Start : road.End;

                var distanceToDestination = MathUtil.Distance(closestPointToDestination, car.Destination.Location);

                if (!(distanceToDestination < shortestDistance)) continue;
                shortestDistance = distanceToDestination;
                selectedRoad = road;
                selectDestination = closestPointToDestination;
            }

            return new Destination {Road = selectedRoad, Location = selectDestination};
        }

        public static DirectionCar GetDirectionToRoad(Vector point, Road road)
        {
            if (road.IsXRoad())
            {
                return road.Start.Y < point.Y ? DirectionCar.North : DirectionCar.South;
            }

            return road.Start.X < point.X ? DirectionCar.West : DirectionCar.East;
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
                MainWindow.Console.Print($"Unable to calculate route from {start} to {end}.", Colors.Red);
                return 0;
            }
        }

        private static double ExploreIntersection(Intersection intersection, Vector end, Road target, double distance,
            int cycles, ICollection<Intersection> past)
        {
            if (intersection == null)
            {
                MainWindow.Console.Print("Warning: Found a problem while exporing route: null terminating road",
                    Colors.Yellow);
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

            //MainWindow.Console.Print($"Arrived at intersection {intersection.Location}. Found {intersections.Count} options. Evaluating...");

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
            return ExploreIntersection(intersectionNext, end, target, distance, cycles, past);
        }

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

        public static double CalculatePrice(double distance)
        {
            return StartingPrice.Value + distance / 100.0 * PricePerKilometer.Value;
        }
    }
}