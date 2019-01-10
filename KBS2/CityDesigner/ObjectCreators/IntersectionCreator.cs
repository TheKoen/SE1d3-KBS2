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
        #region Properties & Fields

        /// <summary>
        /// Appearance of the intersection
        /// </summary>
        private static Rectangle IntersectionRectangle = new Rectangle()
        {
            Fill = Brushes.Red,
            StrokeThickness = 4,
            Stroke = Brushes.Black,
        };

        #endregion

        #region Methods

        /// <summary>
        /// Updates on every road
        /// </summary>
        /// <param name="roads">roads list</param>
        /// <param name="intersections">intersections </param>
        public static void UpdateIntersections(List<Road> roads, List<Intersection> intersections)
        {
            foreach (var road in roads)
            {
                if (intersections.Find(i => i.Location == road.Start) == null)
                {
                    var size = roads.FindAll(r => r.Start == road.Start || r.End == road.End).Max(r => r.Width);
                    intersections.Add(new Intersection(road.Start, size));
                }
                if (intersections.Find(i => i.Location == road.End) == null)
                {
                    var size = roads.FindAll(r => r.Start == road.Start || r.End == road.End).Max(r => r.Width);
                    intersections.Add(new Intersection(road.End, size));
                }
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

            foreach (var intersection in intersectionsNeedingUpdate)
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

        #endregion

        #region Private Methods

        /// <summary>
        /// Allowes to clone intersections
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private static FrameworkElement clone(FrameworkElement e)
        {
            XmlDocument document = new XmlDocument();
            document.LoadXml(XamlWriter.Save(e));
            return (FrameworkElement)XamlReader.Load(new XmlNodeReader(document));
        }

        #endregion
    }
}
