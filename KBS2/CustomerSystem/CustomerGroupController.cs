using System.Linq;
using System.Windows;
using System.Windows.Media;
using KBS2.CarSystem;
using KBS2.CitySystem;
using KBS2.GPS;
using KBS2.Util;

namespace KBS2.CustomerSystem
{
    public class CustomerGroupController
    {
        public const int GroupDistanceFromRoad = 5;
        public const int GroupRadius = 30;
        public CustomerGroup Group { get; set; }

        public bool RequestedCar;


        public CustomerGroupController(CustomerGroup group)
        {
            Group = group;
            var road = GPSSystem.NearestRoad(group.Location);
            MoveToNearestRoad(road);
            Group.RoadsNear = GPSSystem.GetRoadsInRange(Group.Location, GroupRadius);
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
            return Group.Location.Y - road.Start.Y < 0;
        }

        /// <summary>
        /// Checks if the customer current position is left or right of the road.
        /// </summary>
        /// <param name="road"></param>
        /// <returns>Returns true if the road is right and false if the road is left of the current customer position.</returns>
        public bool CheckVerticalRoadPosition(Road road)
        {
            return Group.Location.X - road.Start.X < 0;
        }

        /// <summary>
        /// Moves the group of customers to the nearest road. Depending on the position of the nearest road.
        /// </summary>
        /// <param name="road"></param>
        public void MoveToNearestRoad(Road road)
        {
            if (road == null)
            {
                return;
            }
            if (CheckRoadOrientation(road))
            {
                Group.Location = CheckHorizontalRoadPosition(road)
                    ? new Vector(Group.Location.X, road.Start.Y - road.Width / 2.0 - GroupDistanceFromRoad)
                    : new Vector(Group.Location.X, road.Start.Y + road.Width / 2.0 + GroupDistanceFromRoad);
            }
            else
            {
                Group.Location = CheckVerticalRoadPosition(road)
                    ? new Vector(road.Start.X - road.Width / 2.0 - GroupDistanceFromRoad, Group.Location.Y)
                    : new Vector(road.Start.X + road.Width / 2.0 + GroupDistanceFromRoad, Group.Location.Y);
            }
        }

        public void Update() {
            var buitenRange = Group.Customers.Any(c => MathUtil.Distance(c.Location, Group.Location) > GroupRadius);

            if (RequestedCar)
            {
                var car = City.Instance.Cars.Find(c => MathUtil.Distance(c.Location, Group.Location) < 20);
                if (car == null || car.Passengers.Count > 0) return;
                
                Group.Customers.ForEach(customer => customer.Controller.Destroy());
                MainScreen.AILoop.Unsubscribe(Update);
                car.Passengers.AddRange(Group.Customers);
                var destination = Group.Destination.Location;
                App.Console.Print($"Passengers entered car {car.Id} with destination {destination}", Colors.Blue);
                car.Destination = new Destination {Location = destination, Road = GPSSystem.NearestRoad(destination)};
                car.Controller.PassengersReady();
                return;
            }

            if (buitenRange) return;
            var road = GPSSystem.NearestRoad(Group.Destination.Location);
            GPSSystem.RequestCar(new CarSystem.Destination { Location = Group.Destination.Location, Road = road }, Group);
            RequestedCar = true;
        }
    }
}
