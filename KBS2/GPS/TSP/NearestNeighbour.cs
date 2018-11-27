using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using KBS2.CitySystem;
using KBS2.CustomerSystem;

namespace KBS2.GPS.TSP
{
    public class NearestNeighbour : TSPAlgoritme
    {
        protected override List<Road> CalculatePath(Vector start, Vector end, List<CustomerGroup> customers)
        {
            var cityRoads = City.Instance.Roads;
            var cityIntersections = City.Instance.Intersections;
            var customerLocations = customers.Select(c => c.Location);

            var startRoad = GPSSystem.GetRoad(start);
            var endRoad = GPSSystem.GetRoad(end);
            var currentLocation = start;

            var orderOfCustomers = new List<CustomerGroup>();
            for (int i = 0; i < customers.Count; i++)
            {
                double minLength = double.PositiveInfinity;
                CustomerGroup customerToAddToOrder = null;
                foreach(var location in customerLocations)
                {
                    var calculatedDistance = GPSSystem.CalculateDistance(currentLocation, location);
                    if(calculatedDistance < minLength)
                    {
                        minLength = calculatedDistance;
                        customerToAddToOrder = customers[i];
                    }
                }
                currentLocation = customerToAddToOrder.Location;
                orderOfCustomers.Add(customerToAddToOrder);
            }

            var ListRoads = new List<Road>();

        }
    }
}
