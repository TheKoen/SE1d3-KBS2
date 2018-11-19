using System.Collections.Generic;
using System.Windows;
using KBS2.CarSystem.Sensors;
using KBS2.CitySystem;
using KBS2.CustomerSystem;
using KBS2.GPS;

namespace KBS2.CarSystem
{
    public class Car : IEntity
    {
        public Vector Location { get; set; }
        public DirectionCar Direction { get; set; }
        public Vector Velocity { get; set; } = new Vector(0, 0);
        public List<Sensor> Sensors { get; set; }
        public CarController Controller { get; set; }
        public List<Customer> Passengers { get; set; } = new List<Customer>();
        public Road CurrentRoad { get; set; }
        public Vector CurrentTarget { get; set; }

        /// <summary>
        /// Create a car 
        /// </summary>
        /// <param name="location">Location of the car</param>
        /// <param name="sensors">List with sensors for a car</param>
        /// <param name="direction">Direction the car is facing</param>
        public Car(Vector location, List<Sensor> sensors, DirectionCar direction)
        {
            Direction = direction;
            Location = location;
            Sensors = sensors;
            Controller = new CarController(this);
            Sensors.ForEach(sensor => sensor.Car = this);
            CurrentRoad = GPSSystem.GetRoad(location);
        }

        public Vector GetLocation()
        {
            return Location;
        }
    }
}
