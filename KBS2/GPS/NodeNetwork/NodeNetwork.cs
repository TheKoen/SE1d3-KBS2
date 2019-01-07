using System.Collections.Generic;
using System.Linq;
using KBS2.CitySystem;

namespace KBS2.GPS.NodeNetwork
{
    public static class NodeNetwork
    {
        public static Node[] Nodes { get; private set; }
        public static Link[] Links { get; private set; }

        public static void GenerateNetwork(List<Road> roads, List<Intersection> intersections)
        {
            Nodes = new Node[intersections.Count];
            Links = new Link[roads.Count];

            // Adding Nodes using Intersections
            for (var i = 0; i < Nodes.Length; ++i)
            {
                var intersectionLocation = intersections[i].Location;
                Nodes[i] = new Node(intersectionLocation.X, intersectionLocation.Y);
            }

            // Adding Links using Roads
            for (var i = 0; i < Links.Length; ++i)
            {
                var endpointA = new Node(GPSSystem.FindIntersection(roads[i].Start).Location);
                var endpointB = new Node(GPSSystem.FindIntersection(roads[i].End).Location);
                var nodeA = Nodes.Single(node => node.Equals(endpointA));
                var nodeB = Nodes.Single(node => node.Equals(endpointB));
                
                Links[i] = new Link(ref nodeA, ref nodeB);
            }
        }

        public static NodeNetworkCopy GetInstance()
        {
            var nodes = new Node[Nodes.Length];
            var links = new Link[Links.Length];

            for (var ni = 0; ni < nodes.Length; ++ni)
                nodes[ni] = (Node) Nodes[ni].Clone();
            for (var li = 0; li < links.Length; ++li)
            {
                var nodeA = nodes.Single(n => n.Equals(Links[li].NodeA));
                var nodeB = nodes.Single(n => n.Equals(Links[li].NodeB));
                links[li] = new Link(ref nodeA, ref nodeB);
            }

            return new NodeNetworkCopy(nodes, links);
        }
    }

    public class NodeNetworkCopy
    {
        public Node[] Nodes { get; set; }
        public Link[] Links { get; set; }

        public NodeNetworkCopy(Node[] nodes, Link[] links)
        {
            Nodes = nodes;
            Links = links;
        }
    }
}