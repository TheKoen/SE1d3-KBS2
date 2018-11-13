using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBS2.Car
{
    class CarController
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
            Car.Passengers.Remove();
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

        public void Update()
        {
            
        }

    }
}
