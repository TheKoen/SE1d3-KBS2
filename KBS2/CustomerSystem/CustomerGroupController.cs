using System;
using System.Windows;
using KBS2.CitySystem;

namespace KBS2.CustomerSystem
{
    public class CustomerGroupController
    {
        public CustomerGroup Group { get; set; }

        public CustomerGroupController(CustomerGroup group)
        {
            Group = group;
            var road = LookForNearestRoad();
            if (checkRoad(road))
            {
               Group.Location = new Vector(Group.Location.X - (road.Width / 2), Group.Location.Y);
            }
            else
            {
                Group.Location = new Vector(Group.Location.X, Group.Location.Y - (road.Width / 2));
            }
        }

        public Road LookForNearestRoad()
        {
            City city = City.Instance();
            Road closestRoad = null;
            double closestDistance = double.MaxValue;
            
            foreach(Road road in city.Roads)
            {
                var roadstart = road.Start;
                var roadend = road.End;
                var distanceStart = Math.Sqrt(Math.Pow(Group.Location.X - roadstart.X, 2) + Math.Pow(Group.Location.Y - roadstart.Y, 2));
                var distanceEnd = Math.Sqrt(Math.Pow(Group.Location.X - roadend.X, 2) + Math.Pow(Group.Location.Y - roadend.Y, 2));

                if(distanceStart < closestDistance)
                {
                    closestRoad = road;
                    closestDistance = distanceStart;
                }

                if (distanceEnd < closestDistance)
                {
                    closestRoad = road;
                    closestDistance = distanceEnd;
                }
            }
            return closestRoad;
        }

        /// <summary>
        /// Checks if the road is an Horizontal road or an Vertical road
        /// </summary>
        /// <param name="road"></param>
        /// <returns>returns true with an Vertical road and false with an Horizontal road</returns>
        public bool checkRoad(Road road)
        {
            if(road.Start.Y == road.End.Y) {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Update() {

        }
    }
}
