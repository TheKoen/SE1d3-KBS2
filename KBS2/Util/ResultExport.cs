using KBS2.CarSystem;
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
    public static class ResultExport
    {
        public static event EventHandler ResultExported;

        public static void ExportResult(int simulationId)
        {
            var popupWindow = new SaveFileDialog();
            popupWindow.Title = "Save Results";
            popupWindow.Filter = "XML file | *.xml";
            popupWindow.ShowDialog();

            XmlDocument doc = new XmlDocument();
            XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            XmlElement root = doc.DocumentElement;
            doc.InsertBefore(xmlDeclaration, root);

            using (var database = new MyDatabase("killakid"))
            {
                var simulation = (from s in database.Simulations
                                  where s.ID == simulationId
                                  select s).First();

                // Simulation

                XmlElement simulationElement = doc.CreateElement("Simulation");
                doc.AppendChild(simulationElement);

                XmlAttribute simulationIdAttribute = doc.CreateAttribute("SimulationId");
                simulationIdAttribute.Value = simulationId.ToString();
                XmlAttribute simulationDuration = doc.CreateAttribute("Duration");
                simulationDuration.Value = simulation.Duration.ToString();
                XmlAttribute simulationCityName = doc.CreateAttribute("CityName");
                simulationCityName.Value = simulation.CityInstance.City.CityName;

                simulationElement.Attributes.Append(simulationIdAttribute);
                simulationElement.Attributes.Append(simulationDuration);
                simulationElement.Attributes.Append(simulationCityName);

                //TODO: find all cars by cityInstance
                //TODO: find all customerGroups by cityInstance
                //TODO: find all customers by customerGroups
                //TODO: find all genders by customers
                //TODO: find all reviews by customers
                //TODO: find all garages by cityInstance
                //TODO: find all trips by cars



                // Customers

                var customers = (from c in database.Customers
                                 where c.CustomerGroup.CityInstance == simulation.CityInstance
                                 select c).ToList();

                XmlElement customersElement = doc.CreateElement("Customers");
                doc.AppendChild(customersElement);

                foreach (var customer in customers)
                {
                    XmlElement customerElement = doc.CreateElement("Customer");
                    customersElement.AppendChild(customerElement);

                    XmlAttribute firstName = doc.CreateAttribute("FirstName");
                    firstName.Value = customer.FirstName;

                    XmlAttribute lastName = doc.CreateAttribute("LastName");
                    lastName.Value = customer.LastName;

                    XmlAttribute age = doc.CreateAttribute("Age");
                    age.Value = customer.Age.ToString();

                    XmlAttribute gender = doc.CreateAttribute("Gender");
                    gender.Value = customer.Gender.Name;

                    XmlAttribute customerGroupId = doc.CreateAttribute("customerGroupId");
                    customerGroupId.Value = customer.CustomerGroup.ID.ToString();
                    
                    customerElement.Attributes.Append(firstName);
                    customerElement.Attributes.Append(lastName);
                    customerElement.Attributes.Append(age);
                    customerElement.Attributes.Append(gender);
                    customerElement.Attributes.Append(customerGroupId);
                }

                // cars

                var cars = (from c in database.Cars
                            where c.CityInstance == simulation.CityInstance
                            select c).ToList();


                XmlElement carsElement = doc.CreateElement("Cars");
                doc.AppendChild(carsElement);

                foreach (var car in cars)
                {
                    XmlElement carElement = doc.CreateElement("Car");
                    carsElement.AppendChild(carElement);

                    XmlAttribute model = doc.CreateAttribute("Model");
                    model.Value = car.Model;

                    XmlAttribute garageId = doc.CreateAttribute("GarageId");
                    garageId.Value = car.Garage.ID.ToString();
                    
                    carElement.Attributes.Append(model);
                    carElement.Attributes.Append(garageId);
                }

                // CustomerGroups

                var customerGroups = (from c in database.CustomerGroups
                                      where c.CityInstance == simulation.CityInstance
                                      select c).ToList();

                XmlElement customerGroupsElement = doc.CreateElement("CustomerGroups");
                doc.AppendChild(customerGroupsElement);

                foreach (var customerGroup in customerGroups)
                {
                    XmlElement customerGroupElement = doc.CreateElement("CustomerGroup");
                    customerGroupsElement.AppendChild(customerGroupsElement);

                    XmlAttribute tripId = doc.CreateAttribute("TripId");
                    tripId.Value = customerGroup.Trip.ID.ToString();

                    customerGroupElement.Attributes.Append(tripId);
                }

                var reviews = (from r in database.Reviews
                               where r.Customer.CustomerGroup.CityInstance == simulation.CityInstance
                               select r).ToList();

                // reviews

                XmlElement reviewsElement = doc.CreateElement("Reviews");
                doc.AppendChild(reviewsElement);

                foreach (var review in reviews)
                {
                    XmlElement reviewElement = doc.CreateElement("CustomerGroup");
                    reviewsElement.AppendChild(reviewElement);

                    XmlAttribute content = doc.CreateAttribute("Content");
                    content.Value = review.Content;

                    XmlAttribute rating = doc.CreateAttribute("Rating");
                    rating.Value = review.Rating.ToString();

                    XmlAttribute tripId = doc.CreateAttribute("TripId");
                    tripId.Value = review.Trip.ID.ToString();

                    XmlAttribute customerId = doc.CreateAttribute("CustomerId");
                    customerId.Value = review.Customer.ID.ToString();
                    
                    reviewElement.Attributes.Append(content);
                    reviewElement.Attributes.Append(rating);
                    reviewElement.Attributes.Append(tripId);
                    reviewElement.Attributes.Append(customerId);

                }

                // garages

                var garages = (from g in database.Garages
                               where g.City == simulation.CityInstance.City
                               select g).ToList();

                XmlElement garagesElement = doc.CreateElement("Garages");
                doc.AppendChild(garagesElement);

                foreach (var garage in garages)
                {
                    XmlElement garageElement = doc.CreateElement("Garage");
                    garagesElement.AppendChild(garageElement);

                    XmlAttribute garageLocation = doc.CreateAttribute("Location");
                    garageLocation.Value = $"{garage.Location.X}, {garage.Location.Y}";
                    
                    garageElement.Attributes.Append(garageLocation);
                }


                ResultExported?.Invoke(null, EventArgs.Empty);
            }
            
        }

        public static void SubscribeResultExported(EventHandler source)
        {
            ResultExported += source;
        }

        public static void UnSubscribeResultExported(EventHandler source)
        {
            ResultExported -= source;
        }
    }
}
