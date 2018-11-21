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
            
            var car = nearestGarage.SpawnCar(CityController.CAR_ID++, CarModel.TestModel);
            car.Destination = destination;

        }

        public static Destination GetDirection(Car car, Intersection intersection)
        {
            var roadsAtInteresection = intersection.GetRoads();
            List<Road> roads = new List<Road>();
            
            foreach(var road in roadsAtInteresection)
            {
               if (!car.CurrentRoad.Equals(road)) roads.Add(road);
            }
            
            Vector closestPointToDestination;
            var shortestDistance = Double.MaxValue;
            Road selectedRoad = null;
            Vector selectDestination = new Vector();

            foreach (var road in roads)
            {
                if (road.Equals(car.Destination.Road))
                {

                }
                //kijk naar verste afstand t.o.v auto
                var furthestPoint = 0d;
                var tempDStart = VectorUtil.Distance(road.Start, car.Location);
                var tempDEnd = VectorUtil.Distance(road.End, car.Location);

                furthestPoint = (tempDStart > tempDEnd) ? tempDStart : tempDEnd;

                if(tempDStart > tempDEnd)
                {
                    furthestPoint = tempDStart;
                    closestPointToDestination = road.Start;
                }
                else
                {
                    furthestPoint = tempDEnd;
                    closestPointToDestination = road.End;
                }

                // verste afstand t.o.v  klant
                var distanceToDestination = VectorUtil.Distance(closestPointToDestination, car.Destination.Location);

                if(distanceToDestination < shortestDistance)
                {
                    shortestDistance = distanceToDestination;
                    selectedRoad = road;
                    selectDestination = closestPointToDestination;
                }
                
            }
            
            return new Destination { Road = selectedRoad, Location = selectDestination};
        }
    }
}