using KBS2.CarSystem;
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
    public static class GarageCreator
    {
        /// <summary>
        /// Appearance of ghostgarage
        /// </summary>
        private static Rectangle garageGhost = new Rectangle()
        {
            Fill = Brushes.LightGoldenrodYellow,
            StrokeThickness = 4,
            Stroke = Brushes.Black,
            Width = standardSize,
            Height = standardSize,
        };

        /// <summary>
        /// Appearance of garage
        /// </summary>
        private static Rectangle garageRectangle = new Rectangle()
        {
            Fill = Brushes.LightYellow,
            StrokeThickness = 4,
            Stroke = Brushes.Black,
            Width = standardSize,
            Height = standardSize,
        };

        private static readonly int standardSize = 50;
        private static readonly int maximumDistanceToRoad = 200;

        /// <summary>
        /// Draws the ghost of a Garge on mouseLocation
        /// </summary>
        /// <param name="mouse">Which mouse to look at</param>
        /// <param name="canvas">Canvas where ghost is drawn on</param>
        public static void DrawGhost(Point mouse, Canvas canvas)
        {
            canvas.Children.Remove(garageGhost);
            Canvas.SetTop(garageGhost, (int)mouse.Y - garageGhost.Height / 2);
            Canvas.SetLeft(garageGhost, (int)mouse.X - garageGhost.Width / 2);
            garageGhost.Width = standardSize;
            garageGhost.Height = standardSize;
            canvas.Children.Add(garageGhost);
        }

        /// <summary>
        /// Creates a garages and adds this to the list 
        /// </summary>
        /// <param name="location">location where the garage is place at</param>
        /// <param name="canvas">canvas where the garage is draw on</param>
        /// <param name="garageList">list where garage is added at</param>
        /// <returns>The added garage</returns>
        public static Garage CreateGarage(Point location, Canvas canvas, List<Garage> garageList, List<Road> roadsList)
        {
            var returnGarage = new Garage(new System.Windows.Vector((int)location.X, (int)location.Y), standardSize, DirectionCar.North);
            determineDirection(roadsList, returnGarage);

            if(!ObjectHandler.Overlaps(returnGarage))
            {
                garageList.Add(returnGarage);
                drawGarage(canvas, returnGarage);
                return returnGarage;
            }
            return null;
        }

        /// <summary>
        /// Determines what the direction is of the garage
        /// </summary>
        /// <param name="roads"></param>
        /// <param name="garage"></param>
        /// <returns></returns>
        private static DirectionCar determineDirection(List<Road> roads, Garage garage)
        {
            var roadsY = roads.Find(r => !r.IsXRoad() && (Math.Abs((garage.Location.X + garage.Size / 2) - (r.Start.X - r.Width / 2)) <= maximumDistanceToRoad || Math.Abs((garage.Location.X - garage.Size / 2) - (r.Start.X + r.Width / 2)) <= maximumDistanceToRoad));
            return DirectionCar.North;
        }

        /// <summary>
        /// Draws specific garage on canvas
        /// </summary>
        /// <param name="canvas">Canvas where garage needs to be drawn on</param>
        /// <param name="garage">garge that needs to be drawn</param>
        public static void drawGarage(Canvas canvas, Garage garage)
        {
            canvas.Children.Remove(garageRectangle);
            Canvas.SetTop(garageRectangle, (int)garage.Location.Y - garage.Size / 2);
            Canvas.SetLeft(garageRectangle, (int)garage.Location.X - garage.Size / 2);
            garageRectangle.Width = garage.Size;
            garageRectangle.Height = garage.Size;
            canvas.Children.Add(clone(garageRectangle));
        }

        /// <summary>
        /// Dirty fix to copy Garage
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private static FrameworkElement clone(FrameworkElement e)
        {
            XmlDocument document = new XmlDocument();
            document.LoadXml(XamlWriter.Save(e));
            return (FrameworkElement)XamlReader.Load(new XmlNodeReader(document));
        }

        public static void RemoveGhost(Canvas canvas)
        {
            canvas.Children.Remove(garageGhost);
        }
    }
}
