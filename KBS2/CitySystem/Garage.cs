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

            var road = GPS.GPSSystem.LookForNearestRoad(Location);

            int x;
            int y;

            if (road.IsXRoad())
            {
                x = (int)Location.X;
                y = Location.Y > road.Location.Y ? road.Location.Y + (road.Width / 4d) : road.Location.Y - (road.Width / 4d);
            }   else
            {
                y = (int)Location.Y;
                x = Location.X > road.Location.X ? road.Location.X + (road.Width / 4d) : road.Location.X - (road.Width / 4d);
            }

            var location = new Vector(x, y);

            City.Instance.Cars.Add(new Car(id, model, location, new List<Sensor>(), direction));

            

        }
    }
}
