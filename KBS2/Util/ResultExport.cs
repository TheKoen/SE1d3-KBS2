using KBS2.CarSystem;
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

            // Simulation

            XmlElement simulationElement = doc.CreateElement("Simulation");
            doc.AppendChild(simulationElement);

            XmlAttribute simulationIdAttribute = doc.CreateAttribute("SimulationId");
            simulationIdAttribute.Value = ;
            XmlAttribute simulationDuration = doc.CreateAttribute("Duration");
            simulationDuration.Value = ;
            XmlAttribute simulationCityName = doc.CreateAttribute("CityName");
            simulationCityName.Value = ;

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

            XmlElement customersElement = doc.CreateElement("Customers");
            doc.AppendChild(customersElement);

            foreach(var customer in customers)
            {
                XmlElement customerElement = doc.CreateElement("Customer");
                customersElement.AppendChild(customerElement);

                XmlAttribute customerId = doc.CreateAttribute("CustomerId");
                customerId.Value = ;

                XmlAttribute firstName = doc.CreateAttribute("FirstName");
                firstName.Value = ;

                XmlAttribute lastName = doc.CreateAttribute("LastName");
                lastName.Value = ;

                XmlAttribute age = doc.CreateAttribute("Age");
                age.Value = ;

                XmlAttribute genderId = doc.CreateAttribute("Gender");
                genderId.Value = ;

                XmlAttribute customerGroupId = doc.CreateAttribute("customerGroupId");
                customerGroupId.Value = ;

                customerElement.Attributes.Append(customerId);
                customerElement.Attributes.Append(firstName);
                customerElement.Attributes.Append(lastName);
                customerElement.Attributes.Append(age);
                customerElement.Attributes.Append(genderId);
                customerElement.Attributes.Append(customerGroupId);
            }

            XmlElement carsElement = doc.CreateElement("Cars");
            doc.AppendChild(carsElement);

            foreach (var car in cars)
            {
                XmlElement carElement = doc.CreateElement("Car");
                carsElement.AppendChild(carElement);

                XmlAttribute carId = doc.CreateAttribute("CarId");
                carId.Value = ;

                XmlAttribute model = doc.CreateAttribute("Model");
                model.Value = ;

                XmlAttribute garageId = doc.CreateAttribute("GarageId");
                garageId.Value = ;

                XmlAttribute cityInstanceId = doc.CreateAttribute("cityInstanceId");
                cityInstanceId.Value = ;

                carElement.Attributes.Append(carId);
                carElement.Attributes.Append(model);
                carElement.Attributes.Append(garageId);
                carElement.Attributes.Append(cityInstanceId);
            }

            // Cars

            XmlElement customerGroupsElement = doc.CreateElement("CustomersGroup");
            doc.AppendChild(customerGroupsElement);

            foreach (var customerGroup customerGroups)
            {
                XmlElement customerGroupElement = doc.CreateElement("CustomerGroup");
                customerGroupsElement.AppendChild(customerGroupsElement);

                XmlAttribute customerGroupId = doc.CreateAttribute("CustomerGroupId");
                customerGroupId.Value = ;

                XmlAttribute tripId = doc.CreateAttribute("TripId");
                tripId.Value = ;

                XmlAttribute cityInstanceId = doc.CreateAttribute("CityInstanceId");
                cityInstanceId.Value = ;

                customerGroupElement.Attributes.Append(customerGroupId);
                customerGroupElement.Attributes.Append(tripId);
                customerGroupElement.Attributes.Append(cityInstanceId);
            }

            // reviews

            XmlElement reviewsElement = doc.CreateElement("Reviews");
            doc.AppendChild(reviewsElement);

            foreach(var review in reviews)
            {
                XmlElement reviewElement = doc.CreateElement("CustomerGroup");
                reviewsElement.AppendChild(reviewElement);

                XmlAttribute reviewId = doc.CreateAttribute("ReviewId");
                reviewId.Value = ;

                XmlAttribute content = doc.CreateAttribute("Content");
                content.Value = ;

                XmlAttribute rating = doc.CreateAttribute("Rating");
                rating.Value = ;

                XmlAttribute tripId = doc.CreateAttribute("TripId");
                tripId.Value = ;

                XmlAttribute customerId = doc.CreateAttribute("CustomerId");
                customerId.Value = ;

                reviewElement.Attributes.Append(reviewId);
                reviewElement.Attributes.Append(content);
                reviewElement.Attributes.Append(rating);
                reviewElement.Attributes.Append(tripId);
                reviewElement.Attributes.Append(customerId);
                
            }

            // garages

            XmlElement garagesElement = doc.CreateElement("Garages");
            doc.AppendChild(garagesElement);

            foreach (var garage in garages)
            {
                XmlElement garageElement = doc.CreateElement("Garage");
                garagesElement.AppendChild(garageElement);

                XmlAttribute garageId = doc.CreateAttribute("GarageId");
                garageId.Value = ;

                XmlAttribute garageLocation = doc.CreateAttribute("Location");
                garageLocation.Value = ;

                XmlAttribute cityInstanceId  = doc.CreateAttribute("GarageId");
                cityInstanceId.Value = ;

                garageElement.Attributes.Append(garageId);
                garageElement.Attributes.Append(garageLocation);
                garageElement.Attributes.Append(cityInstanceId);
            }


            ResultExported?.Invoke(null, EventArgs.Empty);
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
