using System.Windows;
using KBS2.CarSystem;

namespace KBS2.CitySystem
{
    class Garage : Building
    {
        public int AvailableCars { get; set; }
        public Garage(Vector location, int size) : base(location, size)
        {
        }

        public void SpawnCar(int id, CarModel model)
        {

        }
    }
}
