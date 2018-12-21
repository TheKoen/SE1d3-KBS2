using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace KBS2.Database
{
    class ResultsHandler
    {
        private CityInstance Instance { get; set; }
        private MainScreen Screen { get; set; }

        public ResultsHandler(MainScreen screen)
        {
            Screen = screen;
        }

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

                foreach (var customer in groupObject.Customers)
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
                if (cities.Count == 0)
                {
                    city = new City()
                    {
                        CityName = e.CityName
                    };
                    database.Cities.Add(city);
                    foreach (var building in cityObject.Buildings)
                    {
                        if (building is CitySystem.Garage garage)
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
                var garage = DatabaseHelper.GetObject<Garage>(database.Garages,
                    g => DatabaseHelper.MatchVectors(g.Location, carObject.Garage));
                if (garage == null) throw new Exception($"Unknown garage {carObject.Garage}");

                var car = new Car
                {
                    CityInstance = Instance,
                    Garage = garage,
                    Model = carObject.Model.Name,
                    DistanceTravelled = (int) Math.Round(carObject.DistanceTraveled)
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

        public void Update()
        {
            DatabaseHelper.QueueDatabaseRequest(
                database => (from car in database.Cars
                             where car.CityInstance.ID == Instance.ID
                             select car).ToList(),
                data =>
                {
                    Screen.LabelResultTotalCars.Content = data.Count;
                    Screen.LabelResultDistanceTotalCar.Content = data.Sum(car => car.DistanceTravelled);
                    Screen.LabelResultDistanceAvarageCustomers.Content =
                        Math.Round((double) data.Sum(car => car.DistanceTravelled));
                },
                MainScreen.WPFLoop
            );

            DatabaseHelper.QueueDatabaseRequest(
                database => (from review in database.Reviews
                             where review.Trip.Car.CityInstance.ID == Instance.ID
                             select review).ToList(),
                data =>
                {
                    Screen.LabelResultAvgReviewRating.Content = Math.Round((double)data.Sum(review => review.Rating));
                },
                MainScreen.WPFLoop
            );

            DatabaseHelper.QueueDatabaseRequest(
                database => (from trip in database.Trips
                             where trip.Car.CityInstance.ID == Instance.ID
                             select trip).ToList(),
                data =>
                {
                    Screen.LabelResultTotalEarned.Content = $"€{data.Sum(trip => trip.Price):0.00}";
                    Screen.LabelResultAvgPrice.Content = $"€{data.Sum(review => review.Price):0.00}";
                },
                MainScreen.WPFLoop
            );
        }
    }
}