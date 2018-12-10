using System;

namespace KBS2.GPS.NodeNetwork
{
    public struct Link
    {
        public Node NodeA;
        public Node NodeB;
        public double Distance { get; }

        public Link(Node nodeA, Node nodeB)
        {
            NodeA = nodeA;
            NodeB = nodeB;

            var deltaX = Math.Abs(nodeA.PositionX - nodeB.PositionX);
            var deltaY = Math.Abs(nodeA.PositionY - nodeB.PositionY);
            Distance = Math.Sqrt(Math.Pow(deltaX, 2) + Math.Pow(deltaY, 2));
        }
    }
}