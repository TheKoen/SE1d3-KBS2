using KBS2.CitySystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml;

namespace KBS2.CityDesigner
{


    class CitySaver
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

            XmlDocument doc = new XmlDocument();
            XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            XmlElement root = doc.DocumentElement;
            doc.InsertBefore(xmlDeclaration, root);

            XmlElement cityElement = doc.CreateElement("City");
            doc.AppendChild(cityElement);
            
            XmlElement roadsElement = doc.CreateElement("Roads");
            cityElement.AppendChild(roadsElement);

            foreach(var road in roads)
            {
                XmlElement roadElement = doc.CreateElement("Road");
                roadsElement.AppendChild(roadElement);

                XmlAttribute start = doc.CreateAttribute("Start");
                start.Value = road.Start.X.ToString() + ", "  + road.Start.Y.ToString();

                XmlAttribute end = doc.CreateAttribute("End");
                end.Value = road.End.X.ToString() + ", " + road.End.Y.ToString();

                XmlAttribute width = doc.CreateAttribute("Width");
                width.Value = road.Width.ToString();

                XmlAttribute maxSpeed = doc.CreateAttribute("MaxSpeed");
                maxSpeed.Value = road.MaxSpeed.ToString();

                roadElement.Attributes.Append(start);
                roadElement.Attributes.Append(end);
                roadElement.Attributes.Append(width);
                roadElement.Attributes.Append(maxSpeed);
            }

            XmlElement buildingsElement = doc.CreateElement("Buildings");
            cityElement.AppendChild(buildingsElement);

            foreach (var building in buildings)
            {
                XmlElement buildingElement = doc.CreateElement("Building");
                buildingsElement.AppendChild(buildingElement);

                XmlAttribute location = doc.CreateAttribute("Location");
                location.Value = ((int)building.Location.X).ToString() + ", " + ((int)building.Location.Y).ToString();

                XmlAttribute size = doc.CreateAttribute("Size");
                size.Value = building.Size.ToString();

                buildingElement.Attributes.Append(location);
                buildingElement.Attributes.Append(size);
            }

            foreach (var garage in garages)
            {
                XmlElement garageElement = doc.CreateElement("Garage");
                buildingsElement.AppendChild(garageElement);

                XmlAttribute location = doc.CreateAttribute("Location");
                location.Value = ((int)garage.Location.X).ToString() + ", " + ((int)garage.Location.Y).ToString();

                XmlAttribute size = doc.CreateAttribute("Size");
                size.Value = garage.Size.ToString();

                garageElement.Attributes.Append(location);
                garageElement.Attributes.Append(size);
            }


            XmlElement intersectionsElement = doc.CreateElement("Intersections");
            cityElement.AppendChild(intersectionsElement);

            foreach(var intersection in intersections)
            {
                XmlElement intersectionElement = doc.CreateElement("Intersection");
                intersectionsElement.AppendChild(intersectionElement);

                XmlAttribute location = doc.CreateAttribute("Location");
                location.Value = intersection.Location.X.ToString() + ", " + intersection.Location.Y.ToString();

                XmlAttribute size = doc.CreateAttribute("Size");
                size.Value = intersection.Size.ToString();

                intersectionElement.Attributes.Append(location);
                intersectionElement.Attributes.Append(size);
            }

            doc.Save(popupWindow.FileName);

            CitySaved?.Invoke(null, EventArgs.Empty);
        }

        public static void SubscribeCitySaved(EventHandler source)
        {
            CitySaved += source;
        }

        public static void UnsubscribeCitySaved(EventHandler source)
        {
            CitySaved -= source;
        }
    }
}
