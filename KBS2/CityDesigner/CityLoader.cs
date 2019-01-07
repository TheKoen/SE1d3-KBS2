using KBS2.CarSystem;
using KBS2.CitySystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Xml;

namespace KBS2.CityDesigner
{
    public delegate void LoadedCityEventHandler(object sender, LoadedCityEventArgs e);

    class CityLoader
    {
        public static event LoadedCityEventHandler LoadedCity;

        public CityLoader Instance { get; private set; }

        public CityLoader()
        {
            Instance = this;
        }

        public static void LoadCity()
        {
            var _roads = new List<Road>();
            var _intersections = new List<Intersection>();
            var _buildings = new List<Building>();
            var _garages = new List<Garage>();

            var doc = new XmlDocument();

            var popupWindow = new OpenFileDialog();
            popupWindow.Title = "Load City";
            popupWindow.Filter = "XML file | *.xml";
            if(popupWindow.ShowDialog() == DialogResult.OK)
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
            var roads = doc.SelectSingleNode("//City/Roads");
            if (roads == null)
                throw new XmlException("Missing roads in city.");
            foreach (var road in roads.ChildNodes)
            {
                _roads.Add(ParseRoad((XmlNode)road));
            }

            //selecting and adding buildings to List
            var buildings = doc.SelectSingleNode("//City/Buildings");
            if (buildings == null)
                throw new XmlException("Missing buildings in city.");
            foreach (var building in buildings.ChildNodes)
            {
                var b = ParseBuilding((XmlNode)building);
                if(b.GetType() == typeof(Garage))
                {
                    _garages.Add((Garage)b);
                }
                else if(b.GetType() == typeof(Building))
                {
                    _buildings.Add(b);
                }
                else
                {
                    throw new XmlException("Error casting garages and buildings");
                }
            }

            //selecting and adding intersections to list
            var intersections = doc.SelectSingleNode("//City/Intersections");
            if (intersections == null)
                throw new XmlException("Missing intersections in city.");
            foreach (var intersection in intersections.ChildNodes)
            {
                _intersections.Add(ParseIntersection((XmlNode)intersection));
            }

            //Invoke the event
            LoadedCity?.Invoke(null, new LoadedCityEventArgs(_roads, _buildings, _garages ,_intersections, popupWindow.FileName));

        }
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

        public static void SubscribeLoadedCity(LoadedCityEventHandler source)
        {
            LoadedCity += source;
        }

        public static void UnsubcribeLoadedCity(LoadedCityEventHandler source)
        {
            LoadedCity -= source;
        }
    }
}
