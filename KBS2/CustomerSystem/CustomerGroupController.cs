using System;
using System.Windows;
using KBS2.CitySystem;
using KBS2.Util;

namespace KBS2.CustomerSystem
{
    public class CustomerGroupController
    {
        public CustomerGroup Group { get; set; }

        public CustomerGroupController(CustomerGroup group)
        {
            Group = group;
            var road = LookForNearestRoad();
            MoveToNearestRoad(road);
            //Order car
        }

        /// <summary>
        /// Function to look at the shortest distance to the start and ending point to each road in the city comparing from customer current location.
        /// </summary>
        /// <returns>returns the road with shortest start or ending point distance.</returns>
        public Road LookForNearestRoad()
        {
            City city = City.Instance();
            Road closestRoad = null;
            double closestDistance = double.MaxValue;
            
            foreach(Road road in city.Roads)
            {
                Vector roadstart = road.Start;
                Vector roadend = road.End;
                double distanceStart = VectorUtil.Distance(roadstart, Group.Location);
                double distanceEnd = VectorUtil.Distance(roadend, Group.Location);

                if (distanceStart < closestDistance)
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
        /// <returns>returns true with an horizontal road and false with an vertical road</returns>
        public bool CheckRoadOrientation(Road road)
        {
            return road.Start.Y == road.End.Y;
        }

        /// <summary>
        /// Checks if the customer current position is above or underneath of the road.
        /// </summary>
        /// <param name="road"></param>
        /// <returns>Returns true if the road is underneath and false if the road is above the current customer position.</returns>
        public bool CheckHorizontalRoadPosition(Road road)
        {
            return (Group.Location.Y - road.Start.Y) < 0;
        }

        /// <summary>
        /// Checks if the customer current position is left or right of the road.
        /// </summary>
        /// <param name="road"></param>
        /// <returns>Returns true if the road is right and false if the road is left of the current customer position.</returns>
        public bool CheckVerticalRoadPosition(Road road)
        {
            return (Group.Location.X - road.Start.X) < 0;
        }

        /// <summary>
        /// Moves the group of customers to the nearest road. Depending on the position of the nearest road.
        /// </summary>
        /// <param name="road"></param>
        public void MoveToNearestRoad(Road road)
        {
            if (CheckRoadOrientation(road))
            {
                if (CheckHorizontalRoadPosition(road))
                {
                    Group.Location = new Vector(Group.Location.X, road.Start.Y - (road.Width / 2.0) - 1);
                }
                else
                {
                    Group.Location = new Vector(Group.Location.X, road.Start.Y + (road.Width / 2.0) + 1);
                }
            }
            else
            {
                if (CheckVerticalRoadPosition(road))
                {
                    Group.Location = new Vector(road.Start.X - (road.Width / 2.0) - 1, Group.Location.Y);
                }
                else
                {
                    Group.Location = new Vector(road.Start.X + (road.Width / 2.0) + 1, Group.Location.Y);
                }
            }
        }

        public void Update() {

        }
    }
}
