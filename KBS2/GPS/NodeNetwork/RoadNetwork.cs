using System.Collections.Generic;
using System.Linq;
using System.Windows;
using KBS2.CitySystem;

namespace KBS2.GPS.NodeNetwork
{
    public static class RoadNetwork
    {
        public static Node[] Nodes { get; private set; }
        public static Link[] Links { get; private set; }

        public static void GenerateNetwork(List<Road> roads, List<Intersection> intersections)
        {
            Nodes = new Node[intersections.Count];
            Links = new Link[roads.Count];

            // Adding Nodes using Intersections
            for (var i = 0; i < Nodes.Length - 1; ++i)
            {
                var intersectionLocation = intersections[i].Location;
                Nodes[i] = new Node(intersectionLocation.X, intersectionLocation.Y);
            }

            // Adding Links using Roads
            for (var i = 0; i < Links.Length - 1; ++i)
            {
                var endpointA = new Node(GPSSystem.FindIntersection(roads[i].Start).Location);
                var endpointB = new Node(GPSSystem.FindIntersection(roads[i].End).Location);
                var nodeA = Nodes.Single(node => node.Equals(endpointA));
                var nodeB = Nodes.Single(node => node.Equals(endpointB));
                
                Links[i] = new Link(nodeA, nodeB);
            }
        }
        
        public static RoadNetworkCopy GetInstance() => new RoadNetworkCopy(Nodes, Links);
    }

    public struct RoadNetworkCopy
    {
        public Node[] Nodes { get; }
        public Link[] Links { get; }

        public RoadNetworkCopy(Node[] nodes, Link[] links)
        {
            Nodes = nodes;
            Links = links;
        }
    }
}