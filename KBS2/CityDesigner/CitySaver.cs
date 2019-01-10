using KBS2.CitySystem;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml;

namespace KBS2.CityDesigner
{
    internal static class CitySaver
    {
        public static EventHandler CitySaved;


        public static void SaveCity(List<Road> roads, List<Building> buildings, List<Garage> garages, List<Intersection> intersections)
        {
            if(roads.Count == 0)
            {
                throw new Exception("Please give the city at least one road.");
            }
            if(buildings.Count == 0)
            {
                throw new Exception("Please give the city at least one building.");
            }
            if(garages.Count == 0)
            {
                throw new Exception("Please give the city at least one garage.");
            }

            // save window
            var popupWindow = new SaveFileDialog()
            {
                Title = "Save City",
                Filter = "Xml file | *.xml"
            };
            popupWindow.ShowDialog();

            var doc = new XmlDocument();
            var xmlDeclaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            var root = doc.DocumentElement;
            doc.InsertBefore(xmlDeclaration, root);

            var cityElement = doc.CreateElement("City");
            doc.AppendChild(cityElement);
            
            // saving roads
            var roadsElement = doc.CreateElement("Roads");
            cityElement.AppendChild(roadsElement);

            foreach(var road in roads)
            {
                var roadElement = doc.CreateElement("Road");
                roadsElement.AppendChild(roadElement);

                var start = doc.CreateAttribute("Start");
                start.Value = road.Start.X.ToString() + ", "  + road.Start.Y.ToString();

                var end = doc.CreateAttribute("End");
                end.Value = road.End.X.ToString() + ", " + road.End.Y.ToString();

                var width = doc.CreateAttribute("Width");
                width.Value = road.Width.ToString();

                var maxSpeed = doc.CreateAttribute("MaxSpeed");
                maxSpeed.Value = road.MaxSpeed.ToString();

                roadElement.Attributes.Append(start);
                roadElement.Attributes.Append(end);
                roadElement.Attributes.Append(width);
                roadElement.Attributes.Append(maxSpeed);
            }

            // saving buildings
            var buildingsElement = doc.CreateElement("Buildings");
            cityElement.AppendChild(buildingsElement);

            foreach (var building in buildings)
            {
                var buildingElement = doc.CreateElement("Building");
                buildingsElement.AppendChild(buildingElement);

                var location = doc.CreateAttribute("Location");
                location.Value = ((int)building.Location.X).ToString() + ", " + ((int)building.Location.Y).ToString();

                var size = doc.CreateAttribute("Size");
                size.Value = building.Size.ToString();

                buildingElement.Attributes.Append(location);
                buildingElement.Attributes.Append(size);
            }

            // saving garages
            foreach (var garage in garages)
            {
                var garageElement = doc.CreateElement("Garage");
                buildingsElement.AppendChild(garageElement);

                var location = doc.CreateAttribute("Location");
                location.Value = ((int)garage.Location.X).ToString() + ", " + ((int)garage.Location.Y).ToString();

                var size = doc.CreateAttribute("Size");
                size.Value = garage.Size.ToString();

                garageElement.Attributes.Append(location);
                garageElement.Attributes.Append(size);
            }


            // saving intersections
            var intersectionsElement = doc.CreateElement("Intersections");
            cityElement.AppendChild(intersectionsElement);

            foreach(var intersection in intersections)
            {
                var intersectionElement = doc.CreateElement("Intersection");
                intersectionsElement.AppendChild(intersectionElement);

                var location = doc.CreateAttribute("Location");
                location.Value = intersection.Location.X.ToString() + ", " + intersection.Location.Y.ToString();

                var size = doc.CreateAttribute("Size");
                size.Value = intersection.Size.ToString();

                intersectionElement.Attributes.Append(location);
                intersectionElement.Attributes.Append(size);
            }

            doc.Save(popupWindow.FileName);

            CitySaved?.Invoke(null, EventArgs.Empty);
        }

        /// <summary>
        /// Subscribe to CitySaved
        /// </summary>
        /// <param name="source">Method to subscribe</param>
        public static void SubscribeCitySaved(EventHandler source)
        {
            CitySaved += source;
        }

        /// <summary>
        /// Unsubscribe to CitySaved
        /// </summary>
        /// <param name="source">Method to unsubscribe</param>
        public static void UnsubscribeCitySaved(EventHandler source)
        {
            CitySaved -= source;
        }
    }
}
