using KBS2.CitySystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace KBS2.GPS.TSP
{
    class Path
    {
        public List<Intersection> Intersections { get; set; } = new List<Intersection>();
        public double Length { get; set; } = 0;
        public Vector Start { get; set; }
        public Vector End { get; set; }
        
        public Path(Vector start, Vector end, Intersection intersection)
        {
            Start = start;
            End = end;
            Intersections.Add(intersection);
            CalculateDistance();
        }
        
        public Path(Vector start, Vector end, List<Intersection> intersections)
        {
            Start = start;
            End = end;
            Intersections = intersections;
            CalculateDistance();
        }

        /// <summary>
        /// calculates the distance from end Intersection to the End location
        /// </summary>
        /// <returns></returns>
        public double CalculateDistanceToEnd()
        {
            return Util.MathUtil.Distance(Intersections.Last().Location, End);
        }

        public double CalculateDistance()
        {
            var currentLocation = Start;

            foreach(var intersection in Intersections)
            {
                Length += Util.MathUtil.Distance(currentLocation, intersection.Location);
            }
            return Length;
        }

        public List<Road> PathToRoadList()
        {
            var listRoads = new List<Road>();
            foreach(var road in City.Instance.Roads)
            {
                if(Intersections.FindAll(i => i.Location == road.Start)
                    .FindAll(i => i.Location == road.End).Count > 0)
                {
                    listRoads.Add(road);
                }
                  
            }
            listRoads.Add(GPSSystem.NearestRoad(Start));
            listRoads.Add(GPSSystem.NearestRoad(End));

            return listRoads;
        }
    }
}
