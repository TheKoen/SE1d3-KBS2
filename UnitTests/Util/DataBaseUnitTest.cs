using KBS2.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests.Util
{
    public class DataBaseUnitTest
    {
        /// <summary>
        /// Fill the Unit Test database with test data
        /// </summary>
        public static void FillUnitTestDatabaseWithTestData()
        {
            
            // cities 

            City city = new City
            {
                CityName = "TestWorld"
            };

            // cityInstances

            CityInstance cityInstance = new CityInstance
            {
                City = city
            };

            // simulations

            Simulation simulation = new Simulation()
            {
                Duration = 100,
                CityInstance = cityInstance
            };

            // garages

            Garage garage1 = new Garage
            {
                Location = new KBS2.Database.Vector()
                {
                    X = 100,
                    Y = 100,
                }
            };

            // cars

            Car car1 = new Car
            {
                CityInstance = cityInstance,
                Garage = garage1,
                Model = "Fiat"
            };

            // trips

            Trip trip1 = new Trip
            {
                Car = car1,
                Distance = 100,
                StartLocation = new KBS2.Database.Vector
                {
                    X = 100,
                    Y = 100
                },
                EndLocation = new KBS2.Database.Vector
                {
                    X = 200,
                    Y = 100
                },
                Price = 10
            };

            // customer groups

            CustomerGroup customerGroup1 = new CustomerGroup
            {
                CityInstance = cityInstance,
                Trip = trip1                    
            };

            // customers

            Customer customer1 = new Customer
            {
                FirstName = "Peter",
                LastName = "Donkers",
                Age = 20,
                Gender = new Gender
                {
                    Name =  "Man"
                },
                CustomerGroup = customerGroup1
            };

            // reviews

            Review review1 = new Review
            {
                Content = "Great",
                Customer = customer1,
                Rating = 5,
                Trip = trip1
            };

            using (var database = new MyDatabase("UnitTestDatabase"))
            {
                database.Cities.Add(city);
                database.Simulations.Add(simulation);
                database.Garages.Add(garage1);
                database.Cars.Add(car1);
                database.Trips.Add(trip1);
                database.CustomerGroups.Add(customerGroup1);
                database.Customers.Add(customer1);
                database.Reviews.Add(review1);

                database.SaveChanges();
            }
        }


        /// <summary>
        /// Clear the database
        /// </summary>
        public static void ClearDatabase()
        {
            using (var database = new MyDatabase("UnitTestDatabase"))
            {
                foreach(var city in database.Cities)
                {
                    database.Cities.Remove(city);
                }

                foreach(var simulation in database.Simulations)
                {
                    database.Simulations.Remove(simulation);
                }
                
                foreach(var garage in database.Garages)
                {
                    database.Garages.Remove(garage);
                }

                foreach(var trip in database.Trips)
                {
                    database.Trips.Remove(trip);
                }

                foreach(var customergroup in database.CustomerGroups)
                {
                    database.CustomerGroups.Remove(customergroup);
                }

                foreach(var customer in database.Customers)
                {
                    database.Customers.Remove(customer);
                }

                foreach(var review in database.Reviews)
                {
                    database.Reviews.Remove(review);
                }
            }
        }
    }
}
