using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using KBS2.CarSystem;
using KBS2.GPS.NodeNetwork;

namespace KBS2.GPS.Algorithms
{
    public class AlgorithmAStar : IAlgorithm
    {
        private static int _debuggerIndex;
        
        // def node start
        // def node end
        // def list:node openlist
        // def list:node closedlist
        // 1. def node thisnode -> start
        // 2. add thisnode to openlist
        // 3. move thisnode to closedlist
        // 4. add {adjacent nodes} to openlist
        // 5. sort openlist by (dist node -> end) asc
        // 6. thisnode -> first in openlist
        // 7. mov 3
        
        public Destination Calculate(Destination carDestination, Destination endDestination)
        {
            var network = NodeNetwork.NodeNetwork.GetInstance();
            var startNode = new Node(GPSSystem.FindIntersection(carDestination.Location).Location);
            var endNodes = AlgorithmTools.IntersectionTupleToNodeTuple(
                AlgorithmTools.GetIntersectionOrderForRoadSide(endDestination.Road, endDestination.Location));
            
            if (startNode.Equals(endNodes.Item1))
            {
                AlgorithmDebuggerWindow.Instance.AddNetworkResult(_debuggerIndex++.ToString(), network, 
                    startNode, endNodes.Item2, endNodes);
                return endDestination;
            }

            var endNode = endNodes.Item1;
            var nextNode = CalculatePathNextNode(ref network, ref startNode, ref endNode);
            
            // Debugger Info
            var roadX = (startNode.PositionX - nextNode.PositionX) / 2.0 + nextNode.PositionX;
            var roadY = (startNode.PositionY - nextNode.PositionY) / 2.0 + nextNode.PositionY;
            return new Destination
            {
                Location = new Vector(nextNode.PositionX, nextNode.PositionY),
                Road = GPSSystem.NearestRoad(new Vector(roadX, roadY))
            };
        }

        private static Node CalculatePathNextNode(ref NodeNetworkCopy network, ref Node startNode, ref Node endNode)
        {
            var openNodes = new List<Node> { startNode as ConnectedNode };
            var closedNodes = new List<Node>();
            ConnectedNode lastNode;

            while (true)
            {
                // propogate
                var thisNode = openNodes.First();
                openNodes.Remove(thisNode);
                closedNodes.Add(thisNode);
                
                var neighbours = network.Links
                    .Where(l => l.NodeA.Equals(thisNode) || l.NodeB.Equals(thisNode))
                    .Select(l => l.NodeA.Equals(thisNode) ? l.NodeB as ConnectedNode : l.NodeA as ConnectedNode)
                    .ToList();
                
                // calculate
                var enode = endNode;
                var foundEnd = false;
                neighbours.ForEach(n =>
                {
                    if (n.Equals(enode))
                    {
                        foundEnd = true;
                        lastNode = n;
                    }

                    n.ConnectedTo = thisNode;
                    n.Value = Math.Sqrt(Math.Pow((n.PositionX - enode.PositionX), 2) + Math.Pow((n.PositionY - enode.PositionY), 2));
                    openNodes.Add(n);
                });
                if (foundEnd) break;

                // sort
                openNodes = openNodes.OrderBy(n => n.Value).ToList();
            }
            
            // find next node
            throw new NotImplementedException();
        }

        private class ConnectedNode : Node
        {
            public Node ConnectedTo { get; set; } = null;
            
            public ConnectedNode(double positionX, double positionY) : base(positionX, positionY)
            {
            }

            public ConnectedNode(Vector vector) : base(vector)
            {
            }
        }
    }
}