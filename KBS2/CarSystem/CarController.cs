using System.Collections.Generic;
using System.Linq;
using KBS2.CarSystem.Sensors;
using KBS2.CarSystem.Sensors.PassiveSensors;
using KBS2.CustomerSystem;

namespace KBS2.CarSystem
{
    public class CarController
    {
        public Car Car { get; set; }

        public CarController(Car car) 
        {
            Car = car;
        }

        /// <summary>
        /// Remove all customers out of the car
        /// </summary>
        /// <returns>List with all removed customers</returns>
        public List<Customer> RemoveCustomers()
        {
            var items = Car.Passengers;
            Car.Passengers.Clear();
            return items;
        }

        /// <summary>
        /// Remove specific customer out of the car
        /// </summary>
        /// <returns>Customer that is removed</returns>
        /// <param name="customer">customer that needs to be removed out of the car</param>
        public Customer RemoveCustomer(Customer customer)
        {
            Car.Passengers.Remove(customer);
            return customer;
        }

        /// <summary>
        /// Add customers to the car
        /// </summary>
        /// <param name="customers">List with customers that needs to be added</param>
        public void AddCustomers(IEnumerable<Customer> customers)
        {
            foreach (var item in customers)
            {
                Car.Passengers.Add(item);
            }
        }

        /// <summary>
        /// Add a customer to the car
        /// </summary>
        /// <param name="customer"></param>
        public void AddCustomer(Customer customer)
        {
            Car.Passengers.Add(customer);
        }

        public List<T> GetSensors<T>(Direction side) where T : Sensor
        {
            return Car.Sensors
                .FindAll(sensor => sensor.GetType() == typeof(T) && sensor.Direction.Equals(side))
                .ConvertAll(sensor => (T) sensor)
                .ToList();
        }

        /// <summary>
        /// Change direction of the car and throw an event
        /// </summary>
        /// <return>returns bool true if car is able to rotate</return>
        /// <param name="direction">change the direction of the car with DirectionCar</param>
        protected bool ChangeDirectionCar(DirectionCar direction) 
        {
            if(Car.Direction == direction)
            {
                return false;
            }
            switch (Car.Direction) {
                case DirectionCar.North when direction == DirectionCar.South:
                case DirectionCar.South when direction == DirectionCar.North:
                case DirectionCar.West when direction == DirectionCar.East:
                case DirectionCar.East when direction == DirectionCar.West:
                    return false;
                default:
                    Car.Direction = direction;
                    return true;
            }
        }

        public void Update()
        {
            var distanceToLeft = GetSensors<LineSensor>(Direction.Left).First().Distance;
            var distanceToRight = GetSensors<LineSensor>(Direction.Right).First().Distance;
            var velocity = Car.Velocity;

            if (distanceToLeft > distanceToRight && velocity.X < 0.5)
            {
                velocity.X -= 0.1;
            }
            else if (distanceToRight > distanceToLeft && velocity.X > -0.5)
            {
                velocity.X += 0.1;
            }
        }
    }
}
