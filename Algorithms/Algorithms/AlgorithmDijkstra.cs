using System.Linq;
using System.Windows;
using Algorithms.NodeNetwork;
using KBS2;
using KBS2.CarSystem;
using KBS2.GPS;

namespace Algorithms.Algorithms
{
    public class AlgorithmDijkstra : IAlgorithm
    {
        public Destination Calculate(Destination carDestination, Destination endDestination)
        {
            App.Console.Print($"carDestination: {carDestination.Location}, endDestination: {endDestination.Location}");
            var network = NodeNetwork.NodeNetwork.GetInstance();
            var startNode = new Node(GPSSystem.FindIntersection(carDestination.Location).Location);
            var endNodes = AlgorithmTools.IntersectionTupleToNodeTuple(
                AlgorithmTools.GetIntersectionOrderForRoadSide(endDestination.Road, endDestination.Location));
            
            AssignNodeValues(ref network, ref startNode);

            if (startNode.Equals(endNodes.Item1))
                return endDestination;

            var nextNode = FindNextNodeOnBestRoute(ref network, endNodes.Item1);
            var roadX = (startNode.PositionX - nextNode.PositionX) / 2.0 + nextNode.PositionX;
            var roadY = (startNode.PositionY - nextNode.PositionY) / 2.0 + nextNode.PositionY;
            return new Destination
            {
                Location = new Vector(nextNode.PositionX, nextNode.PositionY),
                Road = GPSSystem.NearestRoad(new Vector(roadX, roadY))
            };
        }

        private static void AssignNodeValues(ref NodeNetworkCopy network, ref Node startNode)
        {
            if (startNode.Value == null)
            {
                startNode.Value = 0.0;
                network.Nodes[network.Nodes.ToList().IndexOf(startNode)].Value = 0.0;
            }

            var node = startNode;
            var connectedLinks = network.Links
                .Where(l => l.NodeA.Equals(node) || l.NodeB.Equals(node))
                .ToList();
            foreach (var l in connectedLinks)
            {
                var toNode = l.NodeA.Equals(startNode) ? l.NodeB : l.NodeA;
                if (toNode.Value != null && toNode.Value <= startNode.Value + l.Distance) continue;
                toNode.Value = startNode.Value + l.Distance; 
                AssignNodeValues(ref network, ref toNode);
            }
        }

        private static Node FindNextNodeOnBestRoute(ref NodeNetworkCopy network, Node endNode)
        {
            var currentNode = network.Nodes.Single(n => n.Equals(endNode));
            var previousNode = currentNode;
            
            while (currentNode.Value > 0.0)
            {
                var connectedNodes = network.Links
                    .Where(l => l.NodeA.Equals(currentNode) || l.NodeB.Equals(currentNode))
                    .Select(l => l.NodeA.Equals(currentNode) ? l.NodeB : l.NodeA)
                    .ToList();
                var bestNode = connectedNodes
                    .OrderBy(n => n.Value)
                    .First();
                previousNode = currentNode;
                currentNode = bestNode;
            }

            return previousNode;
        }
    }
}