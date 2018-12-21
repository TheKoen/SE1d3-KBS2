using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace KBS2.Database
{
    class Results
    {
        public DatabaseHandler DatabaseHandler { get; private set; } = new DatabaseHandler();

        //lijst met info
        public List<Database.Customer> CustomerList { get; set; }
        private CityInstance Instance { get; set; }
        //event based van simulatie de data opslaan, elke 5 seconde stuurt hij de data naar db.

        public void OnCustomerGroupAdd(object source, GroupEventArgs e)
        {
            CustomerSystem.CustomerGroup groupObject = e.Group;
            DatabaseHelper.QueueDatabaseAction((database) =>
            {
                var group = new CustomerGroup
                {
                    CityInstance = Instance,
                    Trip = null
                };

                foreach(var customer in groupObject.Customers)
                {
                    database.Customers.Add(new Customer
                    {
                        FirstName = customer.Name.Split(' ')[0],
                        LastName = customer.Name.Split(' ')[1],
                        Age = customer.Age,
                        Gender = DatabaseHelper.GetGender(customer.Gender),
                        CustomerGroup = group
                    });
                }

                database.SaveChanges();
            });              
        }

        public void OnSimulationLoad(object source, SimulationEventArgs e)
        {

            CitySystem.City cityObject = e.City;
            DatabaseHelper.QueueDatabaseAction((database) =>
            {
                var cities = (from c in database.Cities
                            where c.CityName.Equals(e.CityName)
                            select c).ToList();
                City city;
                if(cities.Count == 0)
                {
                    city = new City()
                    {
                        CityName = e.CityName
                    };
                    database.Cities.Add(city);
                    foreach(var building in cityObject.Buildings)
                    {
                        if(building is CitySystem.Garage garage)
                        {
                            database.Garages.Add(
                                new Garage
                                {
                                    City = city,
                                    Location = DatabaseHelper.CreateVector(building.Location)
                                }
                            );
                        }
                    }
                }
                else
                {
                    city = cities.First();
                }

                var instance = new CityInstance
                {
                    City = city
                };

                Instance = instance;

                var simulation = new Simulation
                {
                    CityInstance = instance,
                    Duration = 0
                };

                database.Simulations.Add(simulation);
                database.SaveChanges();
            });
        } 

        public void OnTripEnd(object source, TripEventArgs e)
        {
            CustomerSystem.CustomerGroup group = e.Group;
            System.Windows.Vector start = e.Start;
            System.Windows.Vector end = e.End;
            CarSystem.Car carObject = e.Car;


            DatabaseHelper.QueueDatabaseAction((database) =>
            {
                var garage = DatabaseHelper.GetObject<Garage>(database.Garages, g => DatabaseHelper.MatchVectors(g.Location, carObject.Garage));
                if (garage == null) throw new Exception($"Unknown garage {carObject.Garage}");

                var car = new Car
                {
                    CityInstance = Instance,
                    Garage = garage,
                    Model = carObject.Model.Name
                };

                var trip = new Trip
                {
                    Car = car,
                    Distance = carObject.DistanceTraveled,
                    StartLocation = DatabaseHelper.CreateVector(start),
                    EndLocation = DatabaseHelper.CreateVector(end),
                    Price = 0.0
                };

                database.Trips.Add(trip);
                database.SaveChanges();
            });
        }

        public void Setup()
        {
            DatabaseHandler.Setup();
        }

        public void Update()
        {

            DatabaseHandler.Update(this);
            
        }
    }

}
