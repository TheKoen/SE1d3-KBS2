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
        private static readonly Rectangle IntersectionRectangle = new Rectangle()
        {
            Fill = Brushes.Red,
            StrokeThickness = 4,
            Stroke = Brushes.Black,
        };

        #endregion

        #region Methods

        /// <summary>
        /// Updates on every <see cref="Road"/>
        /// </summary>
        /// <param name="roads"><see cref="List{Road}"/> to update to</param>
        /// <param name="intersections"><see cref="List{Intersection}"/> to update to</param>
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
        /// Updates the <see cref="Intersection"/>s on a specific <see cref="Road"/>
        /// </summary>
        /// <param name="road"><see cref="Road"/> to update from</param>
        /// <param name="intersections"><see cref="List{Intersection}"/> to update to</param>
        /// <param name="roadsList"><see cref="List{Road}"/> of <see cref="Road"/>s to use</param>
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
        /// Draws an <see cref="Intersection"/> on a <see cref="Canvas"/>
        /// </summary>
        /// <param name="canvas"><see cref="Canvas"/> to work with</param>
        /// <param name="intersection"><see cref="Intersection"/> to draw</param>
        public static void DrawIntersection(Canvas canvas, Intersection intersection)
        {
            Canvas.SetTop(IntersectionRectangle, (int)intersection.Location.Y - intersection.Size / 2);
            Canvas.SetLeft(IntersectionRectangle, (int)intersection.Location.X - intersection.Size / 2);
            Panel.SetZIndex(IntersectionRectangle, 3);
            
            IntersectionRectangle.Width = intersection.Size;
            IntersectionRectangle.Height = intersection.Size;
            canvas.Children.Add(Clone(IntersectionRectangle));
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Fix to copy <see cref="Intersection"/>s
        /// </summary>
        /// <param name="e"><see cref="FrameworkElement"/> to copy from</param>
        /// <returns>Copied <see cref="FrameworkElement"/></returns>
        private static FrameworkElement Clone(FrameworkElement e)
        {
            var document = new XmlDocument();
            document.LoadXml(XamlWriter.Save(e));
            return (FrameworkElement)XamlReader.Load(new XmlNodeReader(document));
        }

        #endregion
    }
}
