using System;
using System.Collections.Generic;
using System.Windows;
using KBS2.CarSystem;
using KBS2.CarSystem.Sensors;

namespace KBS2.CitySystem
{
    class Garage : Building
    {

        public DirectionCar Direction { get; set; }

        public Garage(Vector location, int size, DirectionCar direction) : base(location, size)
        {
            this.Direction = direction;
        }

        public Car SpawnCar(int id, CarModel model)
        {
            var road = GPS.GPSSystem.NearestRoad(Location);

            double x;
            double y;

            if (road.IsXRoad())
            {
                x = Location.X;
                y = Location.Y > road.Start.Y ? road.Start.Y + (road.Width / 4d) : road.Start.Y - (road.Width / 4d);
            } else
            {
                y = Location.Y;
                x = Location.X > road.Start.X ? road.Start.X + (road.Width / 4d) : road.Start.X - (road.Width / 4d);
            }

            var location = new Vector(x, y);
            var car = new Car(id, model, location, new List<Sensor>(), Direction);
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
            City.Instance.Cars.Add(car);
            return car;
        }
    }
}
