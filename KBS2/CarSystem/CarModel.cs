using KBS2.CarSystem.Sensors;
using System.Collections.Generic;

namespace KBS2.CarSystem
{
    public class CarModel
    {

        public static readonly CarModel TestModel = new CarModel(100, new List<Sensor>());

        public List<Sensor> sensors { get; set; }
        public double MaxSpeed { get; set; }

        public CarModel(double maxSpeed, List<Sensor> s)
        {
            this.MaxSpeed = maxSpeed;
            sensors = new List<Sensor>();
        }

        public static CarModel Get(string name)
        {
            return (CarModel)typeof(CarModel).GetField(name).GetValue(null);
        }
    }
}