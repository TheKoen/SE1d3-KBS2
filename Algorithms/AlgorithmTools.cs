using System;
using System.Windows;
using Algorithms.NodeNetwork;
using KBS2.CitySystem;
using KBS2.GPS;

namespace Algorithms
{
    public static class AlgorithmTools
    {
        /// <summary>
        /// Gets the order of <see cref="Intersection"/>s in which one must travel to arrive on the correct side of the road
        /// </summary>
        /// <param name="road">The <see cref="Road"/> to arrive on</param>
        /// <param name="location">The location to arrive at</param>
        /// <returns>A <see cref="Tuple"/> where the first item is the first <see cref="Intersection"/> to arrive at</returns>
        public static Tuple<Intersection, Intersection> GetIntersectionOrderForRoadSide(Road road, Vector location)
        {
            if (road.IsXRoad())
            {
                var leftIntersection = road.Start.X < road.End.X ? road.Start : road.End;
                var rightIntersection = road.Start.X >= road.End.X ? road.Start : road.End;
                
                if (location.Y < road.Start.Y)
                    return new Tuple<Intersection, Intersection>(
                        GPSSystem.FindIntersection(rightIntersection),
                        GPSSystem.FindIntersection(leftIntersection));
                return new Tuple<Intersection, Intersection>(
                    GPSSystem.FindIntersection(leftIntersection),
                    GPSSystem.FindIntersection(rightIntersection));
            }

            var upIntersection = road.Start.Y < road.End.Y ? road.Start : road.End;
            var downIntersection = road.Start.Y >= road.End.Y ? road.Start : road.End;
                
            if (location.X < road.Start.X)
                return new Tuple<Intersection, Intersection>(
                    GPSSystem.FindIntersection(upIntersection),
                    GPSSystem.FindIntersection(downIntersection));
            return new Tuple<Intersection, Intersection>(
                GPSSystem.FindIntersection(downIntersection),
                GPSSystem.FindIntersection(upIntersection));
        }

        public static Tuple<Node, Node> IntersectionTupleToNodeTuple(Tuple<Intersection, Intersection> tuple)
        {
            var value1 = new Node(tuple.Item1.Location);
            var value2 = new Node(tuple.Item2.Location);
            return new Tuple<Node, Node>(value1, value2);
        }
    }
}