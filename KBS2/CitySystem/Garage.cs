using System.Collections.Generic;
using System.Windows;
using KBS2.CarSystem;
using KBS2.CarSystem.Sensors;

namespace KBS2.CitySystem
{
    class Garage : Building
    {
        public Garage(Vector location, int size) : base(location, size)
        {
        }

        public void SpawnCar(int id, CarModel model, DirectionCar direction)
        {

            var road = GPS.GPSSystem.NearestRoad(Location);

            double x;
            double y;

            if (road.IsXRoad())
            {
                x = Location.X;
                y = Location.Y > road.Start.Y ? road.Start.Y + (road.Width / 4d) : road.Start.Y - (road.Width / 4d);
            }   else
            {
                y = Location.Y;
                x = Location.X > road.Start.X ? road.Start.X + (road.Width / 4d) : road.Start.X - (road.Width / 4d);
            }

            var location = new Vector(x, y);

            City.Instance.Cars.Add(new Car(id, model, location, new List<Sensor>(), direction));

            

        }
    }
}
