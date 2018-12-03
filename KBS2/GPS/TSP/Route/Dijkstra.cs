using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using KBS2.CitySystem;
using KBS2.CustomerSystem;
using KBS2.GPS.TSP.Route;

namespace KBS2.GPS.TSP.Route
{
    class Dijkstra : RouteAlgoritme
    {
        /// <summary>
        /// Calculates the path to the customergroup using Dijkstra algorithm
        /// </summary>
        /// <param name="start">start path</param>
        /// <param name="group">group customers</param>
        /// <returns>list with the roads to take</returns>
        public override List<Road> CalculatePath(Vector start, CustomerGroup group)
        {
            var startRoad = GPSSystem.NearestRoad(start);
            var endRoad = GPSSystem.NearestRoad(group.Location);
            var hitEnd = false;

            var listPaths = new List<Path>();

            // if the startroad equals to the endRoad
            if (startRoad == endRoad) { return new List<Road> { startRoad }; };

            //add the first two intersections of the startRoad to the PathList;
            foreach (var item in GPSSystem.FindIntersectionsRoad(startRoad))
            {
                listPaths.Add(new Path(start, group.Location, item));
            }

            // loop until the end is found
            while (hitEnd == false)
            {
                var newListPaths = new List<Path>();
                foreach (var path in listPaths)
                {
                    // new found intersections 
                    var lastIntersectionOfPath = path.Intersections.Last();

                    var newFoundIntersections = GPSSystem.FindNextIntersections(lastIntersectionOfPath);

                    // add the path of every new intersetion to the new ListPath
                    foreach (var intersection in newFoundIntersections)
                    {
                        var intersections = new List<Intersection>(path.Intersections);
                        intersections.Add(intersection);
                        var newPath = new Path(start, group.Location, intersection);
                        newListPaths.Add(newPath);

                        // check if endLocation is between these intersections

                        if ((endRoad.End == intersection.Location || endRoad.Start == intersection.Location) && (endRoad.End == lastIntersectionOfPath.Location || endRoad.Start == lastIntersectionOfPath.Location))
                        {
                            return newPath.PathToRoadList();
                        }
                        else
                        {
                            throw new Exception("roads are not horizontal or vertical");
                        }
                    }
                }
                // set the new list
                listPaths = newListPaths;

            }
            return null;
        }

    }
}
