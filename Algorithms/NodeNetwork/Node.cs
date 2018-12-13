using System;
using System.Windows;

namespace Algorithms.NodeNetwork
{
    public class Node : IEquatable<Node>
    {
        public double PositionX { get; }
        public double PositionY { get; }
        public double? Value { get; set; }
        
        public Node(double positionX, double positionY)
        {
            PositionX = positionX;
            PositionY = positionY;
            Value = null;
        }

        public Node(Vector vector) : this(vector.X, vector.Y)
        {
        }

        public bool Equals(Node other)
        {
            return PositionX.Equals(other.PositionX) && PositionY.Equals(other.PositionY);
        }
    }
}