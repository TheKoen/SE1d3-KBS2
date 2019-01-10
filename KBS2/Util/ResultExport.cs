using KBS2.CarSystem;
using KBS2.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Xml;

namespace KBS2.Util
{
    public static class ResultExport
    {
        public static event EventHandler ResultExported;

        /// <summary>
        /// Export specific simulation to .xml file
        /// </summary>
        /// <param name="simulationId">specify which simulation you want to export</param>
        public static void ExportResult(int simulationId, string databaseName, Window window)
        {
            var popupWindow = new SaveFileDialog()
            {
                Title = "Save Results",
                Filter = "XML file | *.xml"
            };
            popupWindow.ShowDialog();
            
            if(string.IsNullOrWhiteSpace(popupWindow.FileName))
            {
                return;
            }

            XmlDocument doc = new XmlDocument();
            XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            XmlElement root = doc.DocumentElement;
            doc.InsertBefore(xmlDeclaration, root);

            XmlElement results = doc.CreateElement("Results");
            doc.AppendChild(results);

            DatabaseHelper.QueueDatabaseAction(database =>
            {
                var simulation = (from s in database.Simulations
                                  where s.ID == simulationId
                                  select s).First();

                // Simulation

                XmlElement simulationElement = doc.CreateElement("Simulation");
                results.AppendChild(simulationElement);

                XmlAttribute simulationIdAttribute = doc.CreateAttribute("SimulationId");
                simulationIdAttribute.Value = simulationId.ToString();
                XmlAttribute simulationDuration = doc.CreateAttribute("Duration");
                simulationDuration.Value = simulation.Duration.ToString();
                XmlAttribute simulationCityName = doc.CreateAttribute("CityName");
                simulationCityName.Value = simulation.CityInstance.City.CityName;

                simulationElement.Attributes.Append(simulationIdAttribute);
                simulationElement.Attributes.Append(simulationDuration);
                simulationElement.Attributes.Append(simulationCityName);


                // Customers

                var customers = (from c in database.Customers
                                 where c.CustomerGroup.CityInstance.ID == simulation.CityInstance.ID
                                 select c).ToList();

                XmlElement customersElement = doc.CreateElement("Customers");
                results.AppendChild(customersElement);

                foreach (var customer in customers)
                {
                    XmlElement customerElement = doc.CreateElement("Customer");
                    customersElement.AppendChild(customerElement);

                    XmlAttribute id = doc.CreateAttribute("Id");
                    id.Value = customer.ID.ToString();

                    XmlAttribute firstName = doc.CreateAttribute("FirstName");
                    firstName.Value = customer.FirstName;

                    XmlAttribute lastName = doc.CreateAttribute("LastName");
                    lastName.Value = customer.LastName;

                    XmlAttribute age = doc.CreateAttribute("Age");
                    age.Value = customer.Age.ToString();

                    XmlAttribute gender = doc.CreateAttribute("Gender");
                    gender.Value = customer.Gender.Name;

                    XmlAttribute customerGroupId = doc.CreateAttribute("CustomerGroupId");
                    customerGroupId.Value = customer.CustomerGroup.ID.ToString();

                    customerElement.Attributes.Append(id);
                    customerElement.Attributes.Append(firstName);
                    customerElement.Attributes.Append(lastName);
                    customerElement.Attributes.Append(age);
                    customerElement.Attributes.Append(gender);
                    customerElement.Attributes.Append(customerGroupId);
                }

                // cars

                var cars = (from c in database.Cars
                            where c.CityInstance.ID == simulation.CityInstance.ID
                            select c).ToList();


                XmlElement carsElement = doc.CreateElement("Cars");
                results.AppendChild(carsElement);

                foreach (var car in cars)
                {
                    XmlElement carElement = doc.CreateElement("Car");
                    carsElement.AppendChild(carElement);

                    XmlAttribute carId = doc.CreateAttribute("CarId");
                    carId.Value = car.ID.ToString();

                    XmlAttribute model = doc.CreateAttribute("Model");
                    model.Value = car.Model;

                    XmlAttribute garageId = doc.CreateAttribute("GarageId");
                    garageId.Value = car.Garage.ID.ToString();

                    carElement.Attributes.Append(carId);
                    carElement.Attributes.Append(model);
                    carElement.Attributes.Append(garageId);
                }

                // CustomerGroups

                var customerGroups = (from c in database.CustomerGroups
                                      where c.CityInstance.ID == simulation.CityInstance.ID
                                      select c).ToList();

                XmlElement customerGroupsElement = doc.CreateElement("CustomerGroups");
                results.AppendChild(customerGroupsElement);

                foreach (var customerGroup in customerGroups)
                {
                    XmlElement customerGroupElement = doc.CreateElement("CustomerGroup");
                    customerGroupsElement.AppendChild(customerGroupElement);

                    XmlAttribute id = doc.CreateAttribute("Id");
                    id.Value = customerGroup.ID.ToString();

                    XmlAttribute tripId = doc.CreateAttribute("TripId");
                    if (customerGroup.Trip == null)
                    {
                        tripId.Value = "null";
                    }
                    else
                    {
                        tripId.Value = customerGroup.Trip.ID.ToString();
                    }

                    customerGroupElement.Attributes.Append(id);
                    customerGroupElement.Attributes.Append(tripId);
                }

                var reviews = (from r in database.Reviews
                               where r.Customer.CustomerGroup.CityInstance.ID == simulation.CityInstance.ID
                               select r).ToList();

                // reviews

                XmlElement reviewsElement = doc.CreateElement("Reviews");
                results.AppendChild(reviewsElement);

                foreach (var review in reviews)
                {
                    XmlElement reviewElement = doc.CreateElement("Review");
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
                               where g.City.CityName == simulation.CityInstance.City.CityName
                               select g).ToList();

                XmlElement garagesElement = doc.CreateElement("Garages");
                results.AppendChild(garagesElement);

                foreach (var garage in garages)
                {
                    XmlElement garageElement = doc.CreateElement("Garage");
                    garagesElement.AppendChild(garageElement);

                    XmlAttribute id = doc.CreateAttribute("Id");
                    id.Value = garage.ID.ToString();

                    XmlAttribute garageLocation = doc.CreateAttribute("Location");
                    garageLocation.Value = $"{garage.Location.X}; {garage.Location.Y}";

                    garageElement.Attributes.Append(id);
                    garageElement.Attributes.Append(garageLocation);
                }

                // trips

                var trips = (from t in database.Trips
                             where t.Car.CityInstance.ID == simulation.CityInstance.ID
                             select t).ToList();

                XmlElement tripsElement = doc.CreateElement("Trips");
                results.AppendChild(tripsElement);

                foreach (var trip in trips)
                {
                    XmlElement tripElement = doc.CreateElement("Trip");
                    tripsElement.AppendChild(tripElement);

                    XmlAttribute id = doc.CreateAttribute("Id");
                    id.Value = trip.ID.ToString();

                    XmlAttribute startLocation = doc.CreateAttribute("StartLocation");
                    startLocation.Value = $"{trip.StartLocation.X};{trip.StartLocation.Y}";

                    XmlAttribute endLocation = doc.CreateAttribute("EndLocation");
                    endLocation.Value = $"{trip.EndLocation.X};{trip.EndLocation.Y}";

                    XmlAttribute car = doc.CreateAttribute("CarId");
                    car.Value = trip.Car.ID.ToString();

                    tripElement.Attributes.Append(id);
                    tripElement.Attributes.Append(startLocation);
                    tripElement.Attributes.Append(endLocation);
                }
                doc.Save(popupWindow.FileName);

                MainScreen.DrawingLoop.EnqueueAction(() =>
                {
                    System.Windows.MessageBox.Show(window, "Exported.", "Export", MessageBoxButton.OK);
                    ResultExported?.Invoke(null, EventArgs.Empty);
                });
            });
            
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
