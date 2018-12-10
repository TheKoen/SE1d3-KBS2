using System.Linq;
using System.Windows;
using KBS2.CarSystem;
using KBS2.GPS.NodeNetwork;

namespace KBS2.GPS.Algorithms
{
    public class AlgorithmDijkstra : IAlgorithm
    {
        public Destination Calculate(Destination carDestination, Destination endDestination)
        {
            var network = RoadNetwork.GetInstance();
            var startNode = new Node(GPSSystem.FindIntersection(carDestination.Location).Location);
            var endNodes = AlgorithmTools.IntersectionTupleToNodeTuple(
                AlgorithmTools.GetIntersectionOrderForRoadSide(carDestination.Road, endDestination.Location));
            
            AssignNodeValues(ref network, startNode);

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

        private static void AssignNodeValues(ref RoadNetworkCopy network, Node startNode)
        {
            if (startNode.Value == null)
                startNode.Value = 0.0;
            var connectedLinks = network.Links
                .Where(l => l.NodeA.Equals(startNode) || l.NodeB.Equals(startNode))
                .ToList();
            foreach (var l in connectedLinks)
            {
                var toNode = l.NodeA.Equals(startNode) ? l.NodeB : l.NodeA;
                if (toNode.Value != null && toNode.Value < startNode.Value + l.Distance) continue;
                toNode.Value = startNode.Value + l.Distance;
                AssignNodeValues(ref network, toNode);
            }
        }

        private static Node FindNextNodeOnBestRoute(ref RoadNetworkCopy network, Node endNode)
        {
            var currentNode = endNode;
            var previousNode = endNode;
            
            while (currentNode.Value > 0.0)
            {
                var connectedNodes = network.Links
                    .Where(l => l.NodeA.Equals(endNode) || l.NodeB.Equals(endNode))
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