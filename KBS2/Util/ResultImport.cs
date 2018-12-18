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

        public static void ImportResult()
        {
            var doc = new XmlDocument();

            var popupWindow = new OpenFileDialog();
            popupWindow.Title = "Load City";
            popupWindow.Filter = "XML file | *.xml";
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

            var simulation = doc.SelectSingleNode("//Results/Simulation/");
            if (simulation == null) throw new XmlException("Missing simulation node.");

            var customers = doc.SelectSingleNode("//Results/Customers");
            if (customers == null) throw new XmlException("Missing customers node.");

            var cars = doc.SelectSingleNode("//Results/Cars");
            if (cars == null) throw new XmlException("Missing cars node.");

            var customerGroups = doc.SelectSingleNode("//Results/CustomerGroups");
            if (customerGroups == null) throw new XmlException("Missing customerGroups node.");

            var reviews = doc.SelectSingleNode("//Results/Reviews");
            if (reviews == null) throw new XmlException("Missing reviews node.");

            var garages = doc.SelectSingleNode("//Results/Garages");
            if (garages == null) throw new XmlException("Missing garages node.");

            // getting the data

            Simulation simulationDatabase = new Simulation();

            //simulation
            for(int i = 0; i < simulation.Attributes.Count; i++)
            {
                if (simulation.Attributes[i].Name == "SimulationId")
                {
                    simulationDatabase.ID = int.Parse(simulation.Attributes[i].Value);
                }
                else if(simulation.Attributes[i].Name == "Duration")
                {
                    simulationDatabase.Duration = int.Parse(simulation.Attributes[i].Value);
                }
                else if(simulation.Attributes[i].Name == "CityName")
                {
                    simulationDatabase.CityInstance.City.CityName = simulation.Attributes[i].Value;
                }
                else
                {
                    throw new XmlException("Error in attributes of simulation.");
                }
            }

            // customers

            var customersListDatabase = new List<Customer>();

            // loop over every attribute of every customer in customers
            for(int i = 0; i < customers.ChildNodes.Count; i++)
            {
                var c = new Customer();
                for(int j = 0; j < customers.ChildNodes[i].Attributes.Count; j++)
                {
                    if(customers.ChildNodes[i].Attributes[j].Name == "FirstName")
                    {
                        c.FirstName = customers.ChildNodes[i].Attributes[j].Value;
                    }
                    else if(customers.ChildNodes[i].Attributes[j].Name == "LastName")
                    {
                        c.LastName = customers.ChildNodes[i].Attributes[j].Value; 
                    }
                    else if(customers.ChildNodes[i].Attributes[j].Name == "Age")
                    {
                        c.Age = int.Parse(customers.ChildNodes[i].Attributes[j].Value);
                    }
                    else if(customers.ChildNodes[i].Attributes[j].Name == "Gender")
                    {
                        c.Gender.Name = customers.ChildNodes[i].Attributes[j].Value;
                    }
                    else if(customers.ChildNodes[i].Attributes[j].Name == "CustomerGroupId")
                    {
                        c.CustomerGroup.ID = int.Parse(customers.ChildNodes[i].Attributes[j].Value);
                    }
                    else
                    {
                        throw new XmlException("Error in attributes of customer.");
                    }
                }
                // add customer to list
                customersListDatabase.Add(c);
            }

            // cars

            var carsList = new List<Car>();

            for(int i = 0; i < cars.ChildNodes.Count; i++)
            {
                var c = new Car();
                for(int j = 0; j < cars.ChildNodes[i].Attributes.Count; j++)
                {
                    if(cars.ChildNodes[i].Attributes[j].Name == "Model")
                    {
                        c.Model = cars.ChildNodes[i].Attributes[j].Value;
                    }
                    else if(cars.ChildNodes[i].Attributes[j].Name == "GarageId")
                    {
                        c.Garage.ID = int.Parse(cars.ChildNodes[i].Attributes[j].Value);
                    }
                    else
                    {
                        throw new XmlException("Error in attributes Car");
                    }
                }
                carsList.Add(c);
            }

            // customerGroups

            var customerGroupsList = new List<CustomerGroup>();

            for(int i = 0; i < customerGroups.ChildNodes.Count; i++)
            {
                var c = new CustomerGroup();
                for(int j = 0; j < customerGroups.ChildNodes[i].Attributes.Count; j++)
                {
                    if(customerGroups.ChildNodes[i].Attributes[j].Name == "TripId")
                    {
                        c.Trip.ID = int.Parse(customerGroups.ChildNodes[i].Attributes[j].Value);
                    }
                    else
                    {
                        throw new XmlException("Error in attributes CustomerGroup");
                    }
                }
                customerGroupsList.Add(c);
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
                        r.Trip.ID = int.Parse(reviews.ChildNodes[i].Attributes[j].Value);
                    }
                    else if(reviews.ChildNodes[i].Attributes[j].Name == "CustomerId")
                    {
                        r.Customer.ID = int.Parse(reviews.ChildNodes[i].Attributes[j].Value);
                    }
                    else
                    {
                        throw new XmlException("Error in attributes review");
                    }
                }
                reviewsList.Add(r);
            }

            // garages 

            var garageList = new List<Garage>();

            for(int i = 0; i < garages.ChildNodes.Count; i++)
            {
                var g = new Garage();
                for(int j = 0; j < garages.ChildNodes[i].Attributes.Count; j++)
                {
                    if(garages.ChildNodes[i].Attributes[j].Name == "Location")
                    {
                        
                        g.Location.X = int.Parse(garages.ChildNodes[i].Attributes[j].Value.Split(",".ToCharArray()).First());
                        g.Location.Y = int.Parse(garages.ChildNodes[i].Attributes[j].Value.Split(",".ToCharArray()).Last());
                    }
                    else
                    {
                        throw new XmlException("Error in attributes garage");
                    }
                }
                garageList.Add(g);
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
