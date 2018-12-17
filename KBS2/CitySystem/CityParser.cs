using System;
using System.Windows;
using System.Xml;
using CommandSystem.PropertyManagement;
using KBS2.CarSystem;
using KBS2.GPS;
using KBS2.GPS.NodeNetwork;

namespace KBS2.CitySystem
{
    public class CityParser
    {
        public static City MakeCity(XmlDocument city)
        {
            var properties = PropertyHandler.GetProperties();
            if (properties.ContainsKey("availableCars"))
            {
                PropertyHandler.ResetProperties();
            }

            var cityObject = new City();

            var root = city.DocumentElement;
            if (root == null) throw new XmlException("Missing root node");

            //selecting and adding roads to List
            var roads = city.SelectSingleNode("//City/Roads");
            if (roads == null)
                throw new XmlException("Missing roads in city.");
            foreach (var road in roads.ChildNodes)
            {
                cityObject.Roads.Add(ParseRoad((XmlNode)road));
            }

            //selecting and adding buildings to List
            var buildings = city.SelectSingleNode("//City/Buildings");
            if (buildings == null)
                throw new XmlException("Missing buildings in city.");
            foreach (var building in buildings.ChildNodes)
            {
                cityObject.Buildings.Add(ParseBuilding((XmlNode)building));
            }
            
            //selecting and adding intersections to list
            var intersections = city.SelectSingleNode("//City/Intersections");
            if (intersections == null)
                throw new XmlException("Missing intersections in city.");
            foreach (var intersection in intersections.ChildNodes)
            {
                cityObject.Intersections.Add(ParseIntersection((XmlNode)intersection));
            }
            
            RoadNetwork.GenerateNetwork(cityObject.Roads, cityObject.Intersections);
            
            return cityObject;
        }

        public static Vector ParseLocation(string locationString)
        {
            var split = locationString.Split(',');
            return new Vector(int.Parse(split[0]), int.Parse(split[1]));
        }

        public static Road ParseRoad(XmlNode node)
        {
            var start = ParseLocation(node.Attributes["Start"].InnerText);
            var end = ParseLocation(node.Attributes["End"].InnerText);
            var width = int.Parse(node.Attributes["Width"].InnerText);
            var maxspeed = int.Parse(node.Attributes["MaxSpeed"].InnerText);

            return new Road(start, end, width, maxspeed);
        }

        public static Building ParseBuilding(XmlNode node)
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
                    return null;
            }
        }

        public static Intersection ParseIntersection(XmlNode node)
        {
            var loc = ParseLocation(node.Attributes["Location"].InnerText);
            var size = int.Parse(node.Attributes["Size"].InnerText);

            return new Intersection(loc, size);
        }
    }
}
