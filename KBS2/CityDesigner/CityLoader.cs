using KBS2.CitySystem;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Forms;
using System.Xml;

namespace KBS2.CityDesigner
{
    public delegate void LoadedCityEventHandler(object sender, LoadedCityEventArgs e);

    internal static class CityLoader
    {
        #region Properties & Fields

        public static event LoadedCityEventHandler LoadedCity;

        #endregion  

        #region Public Methods

        /// <summary>
        /// Loads a user-selected <see cref="City"/>
        /// </summary>
        public static void LoadCity()
        {
            var roads = new List<Road>();
            var intersections = new List<Intersection>();
            var buildings = new List<Building>();
            var garages = new List<Garage>();

            var doc = new XmlDocument();

            var popupWindow = new OpenFileDialog()
            {
                Title = "LoadCity",
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
                        
            var root = doc.DocumentElement;
            if (root == null) throw new XmlException("Missing root node");

            //selecting and adding roads to List
            var xmlRoads = doc.SelectSingleNode("//City/Roads");
            if (xmlRoads == null)
                throw new XmlException("Missing roads in city.");
            foreach (var road in xmlRoads.ChildNodes)
            {
                roads.Add(ParseRoad((XmlNode)road));
            }

            //selecting and adding buildings to List
            var xmlBuildings = doc.SelectSingleNode("//City/Buildings");
            if (xmlBuildings == null)
                throw new XmlException("Missing buildings in city.");
            foreach (var building in xmlBuildings.ChildNodes)
            {
                var b = ParseBuilding((XmlNode)building);
                if(b.GetType() == typeof(Garage))
                {
                    garages.Add((Garage)b);
                }
                else if(b.GetType() == typeof(Building))
                {
                    buildings.Add(b);
                }
                else
                {
                    throw new XmlException("Error casting garages and buildings");
                }
            }

            //selecting and adding intersections to list
            var xmlIntersections = doc.SelectSingleNode("//City/Intersections");
            if (xmlIntersections == null)
                throw new XmlException("Missing intersections in city.");
            foreach (var intersection in xmlIntersections.ChildNodes)
            {
                intersections.Add(ParseIntersection((XmlNode)intersection));
            }

            //Invoke the event
            LoadedCity?.Invoke(null, new LoadedCityEventArgs(roads, buildings, garages ,intersections, popupWindow.FileName));

        }

        /// <summary>
        /// Subscribe to LoadedCity
        /// </summary>
        /// <param name="source">Method to subscribe</param>
        public static void SubscribeLoadedCity(LoadedCityEventHandler source)
        {
            LoadedCity += source;
        }

        /// <summary>
        /// Unsubscribe to LoadedCity
        /// </summary>
        /// <param name="source">Method to unsubscribe</param>
        public static void UnsubcribeLoadedCity(LoadedCityEventHandler source)
        {
            LoadedCity -= source;
        }

        #endregion

        #region Private Methods

        private static Vector ParseLocation(string locationString)
        {
            var split = locationString.Split(',');
            return new Vector(int.Parse(split[0]), int.Parse(split[1]));
        }

        private static Road ParseRoad(XmlNode node)
        {
            try
            {
                var start = ParseLocation(node.Attributes["Start"].InnerText);
                var end = ParseLocation(node.Attributes["End"].InnerText);
                var width = int.Parse(node.Attributes["Width"].InnerText);
                var maxspeed = int.Parse(node.Attributes["MaxSpeed"].InnerText);

                return new Road(start, end, width, maxspeed);
            }
            catch(Exception)
            {
                throw new Exception("It looks like this file is not a city.");
            }
        }

        private static Building ParseBuilding(XmlNode node)
        {
            var loc = ParseLocation(node.Attributes["Location"].InnerText);
            var size = int.Parse(node.Attributes["Size"].InnerText);

            switch (node.Name)
            {
                //checking the type of building
                case "Building":
                    return new Building(loc, size);
                case "Garage":
                    return new Garage(loc, size);
                default:
                    throw new Exception("It looks like this file is not a city.");
            }
        }

        private static Intersection ParseIntersection(XmlNode node)
        {
            try
            {
                var loc = ParseLocation(node.Attributes["Location"].InnerText);
                var size = int.Parse(node.Attributes["Size"].InnerText);

                return new Intersection(loc, size);
            }
            catch(Exception)
            {
                throw new Exception("It looks like this file is not a city.");
            }
        }

        #endregion        
    }
}
