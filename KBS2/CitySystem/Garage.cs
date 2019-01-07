using System;
using System.Threading;
using System.Windows;
using KBS2.CarSystem;
using KBS2.GPS;
using KBS2.Util;

namespace KBS2.CitySystem
{
    public class Garage : Building
    {
        public DirectionCar Direction { get; private set; }
        public int AvailableCars { get; set; } = 2;
        public CarModel Model { get; set; } = CarModel.Get("TestModel");
        private bool hasDirection = false;

        public Garage(Vector location, int size) : base(location, size)
        {
        }

        public Car SpawnCar(int id, Destination finalDestination)
        {
            if (!hasDirection)
            {
                var nearest = GPSSystem.NearestRoad(Location);
                Direction = nearest.IsXRoad() ? DirectionCar.East : DirectionCar.North;
                hasDirection = true;
            }

            if (AvailableCars <= 0)
            {
                return null;
            }

            foreach (var cityCar in City.Instance.Cars)
            {
                if (MathUtil.Distance(Location, cityCar.Location) < 100)
                {
                    var thread = new Thread(() =>
                    {
                        Thread.Sleep(1000);
                        SpawnCar(id, finalDestination);
                    });
                    thread.Start();
                    return null;
                }
            }

            var road = GPS.GPSSystem.NearestRoad(Location);

            double x;
            double y;

            if (road.IsXRoad())
            {
                x = Location.X;
                y = Direction == DirectionCar.East ? road.Start.Y + road.Width / 3d : road.Start.Y - road.Width / 3d;
            } else
            {
                y = Location.Y;
                x = Direction == DirectionCar.South ? road.Start.X - road.Width / 3d : road.Start.X + road.Width / 3d;
            }

            var location = new Vector(x, y);
            var car = Model.CreateCar(id, location, this, Direction);
            Vector destination;
            switch (Direction)
            {
                case DirectionCar.North:
                    destination = new Vector(road.Start.X, Math.Min(road.Start.Y, road.End.Y));
                    break;
                case DirectionCar.South:
                    destination = new Vector(road.Start.X, Math.Max(road.Start.Y, road.End.Y));
                    break;
                case DirectionCar.East:
                    destination = new Vector(Math.Max(road.Start.X, road.End.X), road.Start.Y);
                    break;
                case DirectionCar.West:
                    destination = new Vector(Math.Min(road.Start.X, road.End.X), road.Start.Y);
                    break;
                default:
                    throw new ArgumentException("Unknown direction");

            }
            car.Destination = new Destination
            {
                Location = destination,
                Road = road
            };
            car.CurrentTarget = destination;
            City.Instance.Cars.Add(car);
            AvailableCars--;
            car.Destination = finalDestination;
            return car;
        }
    }
}
