using System;
using System.Windows;

namespace KBS2.GPS.NodeNetwork
{
    public class Node : IEquatable<Node>, ICloneable
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
            return Math.Abs(PositionX - other.PositionX) < 0.1 && Math.Abs(PositionY - other.PositionY) < 0.1;
        }

        public object Clone()
        {
            return new Node(PositionX, PositionY);
        }
    }
}