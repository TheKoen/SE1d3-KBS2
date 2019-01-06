using KBS2.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace KBS2.Util
{
    public static class ResultImport
    {
        public static event EventHandler ResultImported;

        /// <summary>
        /// Import results from .xml file into database
        /// </summary>
        public static void ImportResult()
        {
            var doc = new XmlDocument();

            var popupWindow = new OpenFileDialog()
            {
                Title = "Load City",
                Filter = "XML file | *.xml"
            };
            if (popupWindow.ShowDialog() == DialogResult.OK)
            {

                doc.Load(popupWindow.FileName);
            }
            else
            {
                throw new Exception("Please select a document.");
            }

            // Exception handeling
            var root = doc.DocumentElement;
            if (root == null) throw new XmlException("Missing root node.");

            var simulation = doc.SelectSingleNode("//Results/Simulation");
            if (simulation == null) throw new XmlException("Missing simulation node.");

            var customers = doc.SelectSingleNode("//Results/Customers");
            if (customers == null) throw new XmlException("Missing customers node.");

            var cars = doc.SelectSingleNode("//Results/Cars");
            if (cars == null) throw new XmlException("Missing cars node.");

            var trips = doc.SelectSingleNode("//Results/Trips");
            if (trips == null) throw new XmlException("Missing trips node.");

            var customerGroups = doc.SelectSingleNode("//Results/CustomerGroups");
            if (customerGroups == null) throw new XmlException("Missing customerGroups node.");

            var reviews = doc.SelectSingleNode("//Results/Reviews");
            if (reviews == null) throw new XmlException("Missing reviews node.");

            var garages = doc.SelectSingleNode("//Results/Garages");
            if (garages == null) throw new XmlException("Missing garages node.");

            // getting the data

            Simulation simulationDatabase = new Simulation();
            string cityName = "";
            //simulation
            for(int i = 0; i < simulation.Attributes.Count; i++)
            {
                if (simulation.Attributes[i].Name == "SimulationId")
                {
                    //simulationDatabase.ID = int.Parse(simulation.Attributes[i].Value);
                }
                else if (simulation.Attributes[i].Name == "Duration")
                {
                    simulationDatabase.Duration = int.Parse(simulation.Attributes[i].Value);
                }
                else if (simulation.Attributes[i].Name == "CityName")
                {
                    simulationDatabase.CityInstance = new CityInstance();
                    cityName = simulation.Attributes[i].Value;
                }
                else
                {
                    throw new XmlException("Error in attributes of simulation.");
                }
            }

            // garages 

            var garageList = new Dictionary<int, Garage>();

            for (int i = 0; i < garages.ChildNodes.Count; i++)
            {
                var g = new Garage();
                var id = 0;
                for (int j = 0; j < garages.ChildNodes[i].Attributes.Count; j++)
                {
                    if(garages.ChildNodes[i].Attributes[j].Name == "Id")
                    {
                        id = int.Parse(garages.ChildNodes[i].Attributes[j].Value);
                    }
                    else if (garages.ChildNodes[i].Attributes[j].Name == "Location")
                    {
                        g.Location = new Vector
                        {
                            X = int.Parse(garages.ChildNodes[i].Attributes[j].Value.Split(",".ToCharArray()).First()),
                            Y = int.Parse(garages.ChildNodes[i].Attributes[j].Value.Split(",".ToCharArray()).Last())
                        };
                    }
                    else
                    {
                        throw new XmlException("Error in attributes garage");
                    }
                }
                garageList.Add(id, g);
            }

            // cars

            var carsList = new Dictionary<int,Car>();
            
            for(int i = 0; i < cars.ChildNodes.Count; i++)
            {
                var c = new Car();
                var id = 0;
                for(int j = 0; j < cars.ChildNodes[i].Attributes.Count; j++)
                {
                    if(cars.ChildNodes[i].Attributes[j].Name == "CarId")
                    {
                        id = int.Parse(cars.ChildNodes[i].Attributes[j].Value);
                    }
                    else if(cars.ChildNodes[i].Attributes[j].Name == "Model")
                    {
                        c.Model = cars.ChildNodes[i].Attributes[j].Value;
                    }
                    else if(cars.ChildNodes[i].Attributes[j].Name == "GarageId")
                    {
                        c.Garage = garageList[int.Parse(cars.ChildNodes[i].Attributes[j].Value)];
                    }
                    else
                    {
                        throw new XmlException("Error in attributes Car");
                    }
                }
                carsList.Add(id, c);
            }

            // trips 

            var tripsList = new Dictionary<int, Trip>();

            for (int i = 0; i < trips.ChildNodes.Count; i++)
            {
                var t = new Trip();
                var id = 0;
                for (int j = 0; j < trips.ChildNodes[i].Attributes.Count; j++)
                {
                    if(trips.ChildNodes[i].Attributes[j].Name == "Id")
                    {
                        id = int.Parse(trips.ChildNodes[i].Attributes[j].Value);
                    }
                    else if (trips.ChildNodes[i].Attributes[j].Name == "StartLocation")
                    {
                        t.StartLocation = new Database.Vector
                        {
                            X = int.Parse(trips.ChildNodes[i].Attributes[j].Value.Split(",".ToCharArray()).First()),
                            Y = int.Parse(trips.ChildNodes[i].Attributes[j].Value.Split(",".ToCharArray()).Last())
                        };
                    }
                    else if (trips.ChildNodes[i].Attributes[j].Name == "EndLocation")
                    {
                        t.EndLocation = new Database.Vector
                        {
                            X = int.Parse(trips.ChildNodes[i].Attributes[j].Value.Split(",".ToCharArray()).First()),
                            Y = int.Parse(trips.ChildNodes[i].Attributes[j].Value.Split(",".ToCharArray()).Last())
                        };
                    }
                    else if (trips.ChildNodes[i].Attributes[j].Name == "CarId")
                    {
                        t.Car = carsList[int.Parse(trips.ChildNodes[i].Attributes[j].Value)];
                    }
                    else
                    {
                        throw new XmlException("Error in attributes trip");
                    }
                }
                tripsList.Add(id, t);
            }

            // customerGroups

            var customerGroupsList = new Dictionary<int, CustomerGroup>();

            for(int i = 0; i < customerGroups.ChildNodes.Count; i++)
            {
                var c = new CustomerGroup();
                var id = 0;
                for (int j = 0; j < customerGroups.ChildNodes[i].Attributes.Count; j++)
                {
                    if(customerGroups.ChildNodes[i].Attributes[j].Name == "Id")
                    {
                        id = int.Parse(customerGroups.ChildNodes[i].Attributes[j].Value);
                    }
                    else if (customerGroups.ChildNodes[i].Attributes[j].Name == "TripId")
                    {
                        c.Trip = tripsList[int.Parse(customerGroups.ChildNodes[i].Attributes[j].Value)];
                    }
                    else
                    {
                        throw new XmlException("Error in attributes CustomerGroup");
                    }
                }
                customerGroupsList.Add(id, c);
            }

            // customers

            var customersList = new Dictionary<int, Customer>();

            // loop over every attribute of every customer in customers
            for (int i = 0; i < customers.ChildNodes.Count; i++)
            {
                var c = new Customer();
                var id = 0;
                for (int j = 0; j < customers.ChildNodes[i].Attributes.Count; j++)
                {
                    if(customers.ChildNodes[i].Attributes[j].Name == "Id")
                    {
                        id = int.Parse(customers.ChildNodes[i].Attributes[j].Value);
                    }
                    else if (customers.ChildNodes[i].Attributes[j].Name == "FirstName")
                    {
                        c.FirstName = customers.ChildNodes[i].Attributes[j].Value;
                    }
                    else if (customers.ChildNodes[i].Attributes[j].Name == "LastName")
                    {
                        c.LastName = customers.ChildNodes[i].Attributes[j].Value;
                    }
                    else if (customers.ChildNodes[i].Attributes[j].Name == "Age")
                    {
                        c.Age = int.Parse(customers.ChildNodes[i].Attributes[j].Value);
                    }
                    else if (customers.ChildNodes[i].Attributes[j].Name == "Gender")
                    {
                        c.Gender = new Gender
                        {
                            Name = customers.ChildNodes[i].Attributes[j].Value
                        };
                    }
                    else if (customers.ChildNodes[i].Attributes[j].Name == "CustomerGroupId")
                    {
                        c.CustomerGroup = customerGroupsList[int.Parse(customers.ChildNodes[i].Attributes[j].Value)];
                    }
                    else
                    {
                        throw new XmlException("Error in attributes of customer.");
                    }
                }
                // add customer to list
                customersList.Add(id, c);
            }

            // reviews

            var reviewsList = new List<Review>();

            for(int i = 0; i < reviews.ChildNodes.Count; i++)
            {
                var r = new Review();
                for(int j = 0; j < reviews.ChildNodes[i].Attributes.Count; j++)
                {
                    if(reviews.ChildNodes[i].Attributes[j].Name == "Content")
                    {
                        r.Content = reviews.ChildNodes[i].Attributes[j].Value;
                    }
                    else if(reviews.ChildNodes[i].Attributes[j].Name == "Rating")
                    {
                        r.Rating = int.Parse(reviews.ChildNodes[i].Attributes[j].Value);
                    }
                    else if(reviews.ChildNodes[i].Attributes[j].Name == "TripId")
                    {
                        r.Trip = tripsList[int.Parse(reviews.ChildNodes[i].Attributes[j].Value)];
                    }
                    else if(reviews.ChildNodes[i].Attributes[j].Name == "CustomerId")
                    {
                        r.Customer = customersList[int.Parse(reviews.ChildNodes[i].Attributes[j].Value)];
                    }
                    else
                    {
                        throw new XmlException("Error in attributes review");
                    }
                }
                reviewsList.Add(r);
            }                

            // database connection add all elements

            using (var dataBase = new MyDatabase("killakid"))
            {

                // find city with same name
                City i = (from c in dataBase.Cities
                         where c.CityName == cityName
                         select c)?.First();

                // add new city by name if name is not found
                if (i == null)
                {
                    i = new City
                    {
                        CityName = simulationDatabase.CityInstance.City.CityName
                    };
                    dataBase.Cities.Add(i);

                    // add garages 

                    foreach (var garage in garageList)
                    {
                        dataBase.Garages.Add(garage.Value);
                    }
                }
                dataBase.Simulations.Add(simulationDatabase);
                simulationDatabase.CityInstance.City = i;

                // cars
                foreach (var car in carsList)
                {
                    dataBase.Cars.Add(car.Value);
                }

                // trips
                foreach(var trip in tripsList)
                {
                    dataBase.Trips.Add(trip.Value);
                }

                //customerGroups
                foreach(var customerGroup in customerGroupsList)
                {
                    dataBase.CustomerGroups.Add(customerGroup.Value);
                }

                //customers
                foreach(var customer in customersList)
                {
                    dataBase.Customers.Add(customer.Value);
                }

                //reviews 
                foreach(var review in reviewsList)
                {
                    dataBase.Reviews.Add(review);
                }

                dataBase.SaveChanges();

                ResultImported?.Invoke(null, EventArgs.Empty);
            }
        }
    
        public static void SubscribeResultImported(EventHandler source)
        {
            ResultImported += source;
        }

        public static void UnSubscribeResultImported(EventHandler source)
        {
            ResultImported -= source;
        }
    }
}
