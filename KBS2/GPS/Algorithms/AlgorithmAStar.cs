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
        
        private static readonly Dictionary<long, Tuple<Destination, List<ConnectedNode>>> _calculationCache = new Dictionary<long, Tuple<Destination, List<ConnectedNode>>>();


        public static void ClearCache() => _calculationCache.Clear();
        
        public Destination Calculate(long callerId, Destination carDestination, Destination endDestination)
        {
            var network = NodeNetwork.NodeNetwork.GetInstance();
            var startNode = new Node(GPSSystem.FindIntersection(carDestination.Location).Location);
            var endNodes = AlgorithmTools.IntersectionTupleToNodeTuple(
                AlgorithmTools.GetIntersectionOrderForRoadSide(endDestination.Road, endDestination.Location));

            Node nextNode;
            double roadX, roadY;
            
            if (startNode.Equals(endNodes.Item1))
            {
                #if DEBUG
                AlgorithmDebuggerWindow.Instance.AddNetworkResult(_debuggerIndex++.ToString(), network, 
                    startNode, endNodes.Item2, endNodes);
                #endif
                return endDestination;
            }
            
            if (_calculationCache.ContainsKey(callerId))
            {
                var tuple = _calculationCache[callerId];
                if (tuple.Item1.Equals(endDestination))
                {
                    var path = tuple.Item2;
                    var thisNode = path.Single(n => n.Equals(startNode));
                    nextNode = thisNode.ConnectedTo;

                    #if DEBUG
                    AlgorithmDebuggerWindow.Instance.AddNetworkResult(_debuggerIndex++.ToString(), network,
                        startNode, nextNode, endNodes);
                    #endif
                    roadX = (startNode.PositionX - nextNode.PositionX) / 2.0 + nextNode.PositionX;
                    roadY = (startNode.PositionY - nextNode.PositionY) / 2.0 + nextNode.PositionY;
                    return new Destination
                    {
                        Location = new Vector(nextNode.PositionX, nextNode.PositionY),
                        Road = GPSSystem.NearestRoad(new Vector(roadX, roadY))
                    };
                }

                _calculationCache.Remove(callerId);
            }

            var endNode = endNodes.Item1;
            nextNode = CalculatePathNextNode(callerId, ref network, ref startNode, ref endNode, endDestination);
            
            #if DEBUG
            AlgorithmDebuggerWindow.Instance.AddNetworkResult("AS" + _debuggerIndex++, network,
                startNode, nextNode, endNodes);
            #endif
            roadX = (startNode.PositionX - nextNode.PositionX) / 2.0 + nextNode.PositionX;
            roadY = (startNode.PositionY - nextNode.PositionY) / 2.0 + nextNode.PositionY;
            return new Destination
            {
                Location = new Vector(nextNode.PositionX, nextNode.PositionY),
                Road = GPSSystem.NearestRoad(new Vector(roadX, roadY))
            };
        }

        private static Node CalculatePathNextNode(long callerId, ref NodeNetworkCopy network, ref Node startNode, ref Node endNode, Destination endDestination)
        {
            var openNodes = new List<ConnectedNode> { new ConnectedNode(endNode) };
            var closedNodes = new List<ConnectedNode>();
            ConnectedNode lastNode = null;

            var updateIndex = 0;
            while (true)
            {
                // propogate
                var thisNode = openNodes.First();
                openNodes.Remove(thisNode);
                closedNodes.Add(thisNode);
                
                var neighbours = network.Links
                    .Where(l => l.NodeA.Equals(thisNode) || l.NodeB.Equals(thisNode))
                    .Select(l => l.NodeA.Equals(thisNode) ? new ConnectedNode(l.NodeB) : new ConnectedNode(l.NodeA))
                    .ToList();
                
                // calculate
                var foundEnd = false;
                foreach (var n in neighbours)
                {
                    if (closedNodes.SingleOrDefault(cn => cn.Equals(n)) != null) continue;
                    
                    if (n.Equals(startNode))
                    {
                        foundEnd = true;
                        lastNode = n;
                    }

                    n.ConnectedTo = thisNode;
                    n.Value = Math.Sqrt(Math.Pow((n.PositionX - startNode.PositionX), 2) + Math.Pow((n.PositionY - startNode.PositionY), 2));
                    network.Nodes.Single(nn => nn.Equals(n)).Value = n.Value;
                    #if DEBUG
                    AlgorithmDebuggerWindow.Instance.AddNodeUpdateResult($"AS{_debuggerIndex}${updateIndex++}", network,
                        n, thisNode);
                    #endif
                    openNodes.Add(n);
                }

                if (foundEnd) break;

                // sort
                openNodes = openNodes.OrderBy(n => n.Value).ToList();
                
                // info
                #if DEBUG
                AlgorithmDebuggerWindow.Instance.AddNetworkResult($"AS{_debuggerIndex}#{openNodes.First().Value}", network,
                    thisNode, openNodes.First());
                #endif
            }
            
            var roadX = (startNode.PositionX - lastNode.ConnectedTo.PositionX) / 2.0 + lastNode.ConnectedTo.PositionX;
            var roadY = (startNode.PositionY - lastNode.ConnectedTo.PositionY) / 2.0 + lastNode.ConnectedTo.PositionY;
            var result =  new Destination
            {
                Location = new Vector(lastNode.ConnectedTo.PositionX, lastNode.ConnectedTo.PositionY),
                Road = GPSSystem.NearestRoad(new Vector(roadX, roadY))
            };
            
            if (_calculationCache.ContainsKey(callerId))
                _calculationCache.Remove(callerId);
            _calculationCache.Add(callerId, new Tuple<Destination, List<ConnectedNode>>(endDestination, closedNodes));
            
            // find next node
            return lastNode.ConnectedTo;
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

            public ConnectedNode(Node node) : base(node.PositionX, node.PositionY)
            {
            }
        }
    }
}