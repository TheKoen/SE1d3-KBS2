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
                DrawIntersection(canvas, newIntersection);
                return newIntersection;
            }
            return null;
        }

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
