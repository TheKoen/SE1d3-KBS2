using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using KBS2.CitySystem;
using KBS2.CustomerSystem;

namespace KBS2.GPS.TSP.Route
{
    class AStar : RouteAlgoritme
    {
        public override List<Road> CalculatePath(Vector start, CustomerGroup group)
        {
            var startRoad = GPSSystem.NearestRoad(start);
            var endRoad = GPSSystem.NearestRoad(group.Location);

            var listPaths = new List<Path>();

            // if the startroad equals to the endRoad
            if (startRoad == endRoad) { return new List<Road> { startRoad }; };

            //add the first two intersections of the startRoad to the PathList;
            foreach (var item in GPSSystem.FindIntersectionsRoad(startRoad))
            {
                listPaths.Add(new Path(start, group.Location, item));
            }
            // order list
            orderListByPriority(ref listPaths);

            // loop until the end is found
            while (true)
            {          
                // new found intersections 
                var lastIntersectionOfPath = listPaths[0].Intersections.Last();

                var newFoundIntersections = GPSSystem.FindNextIntersections(lastIntersectionOfPath);

                // add the path of every new intersetion to the new ListPath
                foreach (var intersection in newFoundIntersections)
                {
                    var intersections = new List<Intersection>(listPaths[0].Intersections);
                    intersections.Add(intersection);
                    var newPath = new Path(start, group.Location, intersection);
                    listPaths.Add(newPath);

                    // check if endLocation is between these intersections

                    if ((endRoad.End == intersection.Location || endRoad.Start == intersection.Location) && (endRoad.End == lastIntersectionOfPath.Location || endRoad.Start == lastIntersectionOfPath.Location))
                    {
                        return newPath.PathToRoadList();
                    }
                }
                listPaths.RemoveAt(0);
                orderListByPriority(ref listPaths);
            }
        }

        private void orderListByPriority(ref List<Path> listPaths)
        {
            listPaths.OrderBy(i => i.CalculateDistance() + i.CalculateDistanceToEnd());
        }
    }
}
