using KBS2.CustomerSystem;
using KBS2.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace KBS2.GPS.TSP
{
    public class NearestNeighbour2 
    {
        public List<Vector> Calculate(Vector start, List<CustomerGroup> customers)
        {
            var customersLocations = customers.Select(c=> c.Location).ToList();
            var customersDestinationsLocations = customers.Select(c => c.Destination.Location).ToList();
            var locationsToVisit = customers.Select(c => c.Location).ToList();
            var currentLocation = start;

            var ListOrderLocations = new List<Vector>();

            for(int i = 0; i < (customersLocations.Count * 2); i++)
            {
                //find the nearest Vector
                locationsToVisit = TakeNearestVectorAndPlaceFirstInList(locationsToVisit, currentLocation);
                //add this to the ListOrderLocation
                ListOrderLocations.Add(locationsToVisit.First());
                //add the destinationLocation  to locationsToVisit
                try
                {
                    locationsToVisit.Add(customers.Find(c => c.Location == locationsToVisit.First()).Destination.Location);
                }
                catch(Exception) { };
                //remove the visited location
                locationsToVisit.RemoveAt(0);
            }
            return ListOrderLocations;
        }

        public static List<Vector> TakeNearestVectorAndPlaceFirstInList(List<Vector> locations, Vector currentLocation)
        {
            double distance = double.MaxValue;
            Vector firstLocation = new Vector(-100, -100);
            foreach(var location in locations)
            {
                var calculatedDistance = MathUtil.Distance(location, currentLocation);
                if (calculatedDistance < distance)
                {
                    distance = calculatedDistance;
                    firstLocation = location;
                }
            }
            if (firstLocation.Equals(new Vector(-100, -100)))
            {
                throw new Exception("could not find Nearest location");
            }
            locations.Remove(firstLocation);
            locations.Insert(0, firstLocation);

            return locations;
        }
    }
}
