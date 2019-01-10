using KBS2.CitySystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml;

namespace KBS2.CityDesigner.ObjectCreators
{
    public static class IntersectionCreator
    {
        private static Rectangle IntersectionRectangle = new Rectangle()
        {
            Fill = Brushes.Red,
            StrokeThickness = 4,
            Stroke = Brushes.Black,
        };

        public static Intersection CreateIntersection(Canvas canvas, List<Road> roads, List<Intersection> intersections, Vector location) 
        {
            // search for Intersection add when not excists and more then one Road is connected
            if (intersections.Find(i => i.Location == location) == null && roads.FindAll(r => r.Start == location || r.End == location).Count >= 2)
            {
                // get the MaxSize of the road connected to the Intersection
                var roadMaxSize = roads.FindAll(r => r.Start == location || r.End == location).Max(r => r.Width);
                // add To list if not excist at start of new road
                var newIntersection = new Intersection(location, roadMaxSize);
                intersections.Add(newIntersection);
                DrawIntersection(canvas, newIntersection, roads, intersections);
                return newIntersection;
            }
            return null;
        }

        public static void UpdateIntersections(List<Road> roads, List<Intersection> intersections)
        {
            var stupidIntersectionWihoutRoads = new List<Intersection>();
            var neededIntersections = new List<Intersection>();

            foreach (var intersection in intersections)
            {
                // set the size of the intersection to the max  with of the roads connected
                intersection.Size = roads.FindAll(r => r.Start == intersection.Location || r.End == intersection.Location)
                    .OrderByDescending(r => r.Width)
                    .First()
                    .Width;

                if (roads.FindAll(r => r.End == intersection.Location || r.Start == intersection.Location).Count == 1)
                {

                    stupidIntersectionWihoutRoads.Add(intersection);
                }
            }
            roads.ForEach(r =>
            {
                if(!intersections.Any(i => i.Location == r.Start || i.Location == r.End) && roads.Any(x => (x.Start == r.Start || x.End == r.Start) && r != x))
                {
                    intersections.Add(new Intersection(r.Start, roads.FindAll(x => x.Start == r.Start || x.End == r.Start)
                        .OrderByDescending(x => x.Width)
                        .First()
                        .Width));
                }
                if(!intersections.Any(i => i.Location == r.Start || i.Location == r.End) && roads.Any(x => (x.Start == r.End || x.End == r.End) && r != x))
                {
                    intersections.Add(new Intersection(r.End, roads.FindAll(x => x.Start == r.End || x.End == r.End)
                        .OrderByDescending(x => x.Width)
                        .First()
                        .Width));
                }
            } );

            //delete the intersection without road connected
            foreach (var intersectionItem in stupidIntersectionWihoutRoads)
            {
                intersections.Remove(intersectionItem);
            }
        }

        /// <summary>
        /// Updates the intersections on a specific road
        /// </summary>
        /// <param name="road"></param>
        /// <param name="intersections"></param>
        public static void UpdateIntersection(Road road, List<Intersection> intersections, List<Road> roadsList)
        {
            var intersectionsNeedingUpdate = intersections.FindAll(i => i.Location == road.End || i.Location == road.Start);

            foreach(var intersection in intersectionsNeedingUpdate)
            {
                intersection.Size = roadsList
                    .FindAll(r => r.Start == intersection.Location || r.End == intersection.Location)
                    .Max(r => r.Width);
            }
        }

        /// <summary>
        /// Draws the intersection on a canvas
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="intersection"></param>
        public static void DrawIntersection(Canvas canvas, Intersection intersection)
        {
            Canvas.SetTop(IntersectionRectangle, (int)intersection.Location.Y - intersection.Size / 2);
            Canvas.SetLeft(IntersectionRectangle, (int)intersection.Location.X - intersection.Size / 2);
            Canvas.SetZIndex(IntersectionRectangle, 3);
            IntersectionRectangle.Width = intersection.Size;
            IntersectionRectangle.Height = intersection.Size;
            canvas.Children.Add(clone(IntersectionRectangle));
        }

        /// <summary>
        /// Dirty fix to copy Intersection
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private static FrameworkElement clone(FrameworkElement e)
        {
            XmlDocument document = new XmlDocument();
            document.LoadXml(XamlWriter.Save(e));
            return (FrameworkElement)XamlReader.Load(new XmlNodeReader(document));
        }
    }
}
