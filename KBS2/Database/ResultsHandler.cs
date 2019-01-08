using System;
using System.Collections.Generic;
using System.Linq;
using KBS2.CarSystem;
using KBS2.Visual;
using KBS2.Visual.Controls;

namespace KBS2.Database
{
    public class ResultsHandler
    {
        private CityInstance Instance { get; set; }
        private MainScreen Screen { get; }
        private int ticks;

        public ResultsHandler(MainScreen screen)
        {
            Screen = screen;

            MainScreen.CommandLoop.Subscribe(() =>
            {
                ticks++;
                if (ticks == 500)
                {
                    Update();
                }
            });

            CitySystem.CityController.OnCustomerGroupAdd += OnCustomerGroupAdd;
            SimulationControlHandler.SimulationLoad += OnSimulationLoad;
            CarController.TripEnd += OnTripEnd;
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
                                    Location = DatabaseHelper.CreateDBVector(building.Location)
                                }
                            );
                        }
                    }
                } else
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
                var garage = DatabaseHelper.GetObject(database.Garages,
                    g => DatabaseHelper.MatchVectors(g.Location, carObject.Garage.Location));
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
                    StartLocation = DatabaseHelper.CreateDBVector(start),
                    EndLocation = DatabaseHelper.CreateDBVector(end),
                    Price = 0.0
                };

                database.Trips.Add(trip);
                database.SaveChanges();
            });
        }

       
        public void Update()
        {
            ticks = 0;

            if (Instance == null) return;

            /*
             * Queue a request to the database
             * Read the summery of DatabaseHelper.QueueDatabaseRequest
             * for more detailed information
             */
            DatabaseHelper.QueueDatabaseRequest(
                // Request all the customers in the current CityInstance.
                database => (from customer in database.Customers
                    where customer.CustomerGroup.CityInstance.ID == Instance.ID
                    select customer).ToList(),
                // Update all the labels with the new data.
                data =>
                {
                    Screen.LabelResultCustomer.Content = data.Count;
                    Screen.LabelResultTotalCustomers.Content = data.Count;
                    if (data.Count == 0) return;
                    Screen.LabelResultAvgAge.Content = Math.Round(data.Average(customer => customer.Age));
                    Screen.LabelResultAvgMoral.Content = Math.Round(data.Average(customer => customer.Moral));
                    
                },
                // Labels can only be updated on the WPF (main) thread so we want to use that one.
                MainScreen.WPFLoop
            );

            DatabaseHelper.QueueDatabaseRequest(
                database => (from car in database.Cars
                    where car.CityInstance.ID == Instance.ID
                    select car).ToList(),
                data =>
                {
                    Screen.LabelResultCars.Content = data.Count;
                    Screen.LabelResultTotalCars.Content = data.Count;
                    if (data.Count == 0) return;
                    Screen.LabelResultDistanceTotalCar.Content = data.Sum(car => car.DistanceTravelled);
                    Screen.LabelResultDistanceAvarageCars.Content =
                        Math.Round(data.Average(car => car.DistanceTravelled));
                },
                MainScreen.WPFLoop
            );

            DatabaseHelper.QueueDatabaseRequest(
                database => (from review in database.Reviews
                    where review.Trip.Car.CityInstance.ID == Instance.ID
                    select review).ToList(),
                data =>
                {
                    if (data.Count == 0) return;
                    Screen.LabelResultAvgReviewRating.Content = Math.Round(data.Average(review => review.Rating));
                },
                MainScreen.WPFLoop
            );

            DatabaseHelper.QueueDatabaseRequest(
                database => (from trip in database.Trips
                    where trip.Car.CityInstance.ID == Instance.ID
                    select trip).ToList(),
                data =>
                {
                    Screen.LabelResultRide.Content = data.Count;
                    if (data.Count == 0) return;
                    Screen.LabelResultTotalEarned.Content = $"€{data.Sum(trip => trip.Price):0.00}";
                    Screen.LabelResultAvgPrice.Content = $"€{data.Sum(review => review.Price):0.00}";
                    Screen.LabelResultDistanceTotal.Content = data.Sum(trip => trip.Distance);
                    Screen.LabelResultDistanceAvarage.Content = Math.Round(data.Average(trip => trip.Distance));
                    
                },
                MainScreen.WPFLoop
            );
            
            DatabaseHelper.QueueDatabaseRequest(
                database => (from c in database.Customers
                             where c.CustomerGroup.Trip != null
                             join t in database.Trips
                                on c.CustomerGroup.Trip.ID equals t.ID
                             where c.CustomerGroup.CityInstance.ID == Instance.ID
                             select new
                             {
                                 customer = c, trip = t
                             }).ToList(),
                data =>
                {
                    if (data.Count == 0) return;
                    var trips = new List<Trip>();

                    foreach (var d in data)
                    {
                        if (trips.All(trip => trip.ID != d.trip.ID))
                        {
                            trips.Add(d.trip);
                            
                        }
                    }

                    var avarage = data.Count / trips.Count;
                    
                    Screen.LabelResultAvgCustomers.Content = avarage;
                },
                MainScreen.WPFLoop
            );
        }
    }
}