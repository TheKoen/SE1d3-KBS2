using System.Collections.Generic;
using KBS2.CustomerSystem;

namespace KBS2.CarSystem
{
    public class CarController
    {
        public CarSystem.Car Car { get; set; }

        public CarController(CarSystem.Car car) 
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
        public void AddCustomers(List<Customer> customers)
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

        /// <summary>
        /// Change direction of the car and throw an event
        /// </summary>
        /// <return>returns bool true if car is able to rotate</return>
        /// <param name="direction">change the direction of the car with DirectionCar</param>
        private bool ChangeDirectionCar(DirectionCar direction) 
        {
            if(Car.Direction == direction)
            {
                return false;
            }
            if(Car.Direction == DirectionCar.North && direction == DirectionCar.South)
            {
                return false;
            }
            if(Car.Direction == DirectionCar.South && direction == DirectionCar.North)
            {
                return false;
            }
            if(Car.Direction == DirectionCar.West && direction == DirectionCar.East)
            {
                return false;
            }
            if(Car.Direction == DirectionCar.East && direction == DirectionCar.West)
            {
                return false;
            }
            else
            {
                // throw an event for the the sensors to change direction.
                return true;
            }

        }

        public void Update()
        {
            
        }

    }
}
