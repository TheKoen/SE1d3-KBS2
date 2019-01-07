using System;

namespace KBS2.GPS.NodeNetwork
{
    public class Link
    {
        public Node NodeA { get; }
        public Node NodeB { get; }
        public double Distance { get; }

        public Link(ref Node nodeA, ref Node nodeB)
        {
            NodeA = nodeA;
            NodeB = nodeB;

            var deltaX = Math.Abs(nodeA.PositionX - nodeB.PositionX);
            var deltaY = Math.Abs(nodeA.PositionY - nodeB.PositionY);
            Distance = Math.Sqrt(Math.Pow(deltaX, 2) + Math.Pow(deltaY, 2));
        }
    }
}