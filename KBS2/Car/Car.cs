using KBS2.Car.Sensors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace KBS2.Car
{
    class Car
    {
        public Vector Location { get; set; }
        public Vector Direction { get; set; }
        public Vector Velocity { get; set; } = new Vector(0, 0);
        public List<Sensor> Sensors { get; set; } = new List<Sensor>();
        public CarController Controller { get; set; } = new CarController();
        public List<Customer> Passengers { get; set; } = new List<Customer>();
        public Road CurrentRoad { get; set; }

        /// <summary>
        /// Create a car 
        /// </summary>
        /// <param name="location">Location of the car</param>
        /// <param name="sensors">List with sensors for a car</param>
        public Car(Vector location, List<Sensor> sensors, Vector direction)
        {
            Direction = direction;
            Location = location;
            Sensors = sensors;
        }
    }
}
