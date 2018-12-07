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
    public static class BuildingCreator
    {
        private static Rectangle buildingGhost = new Rectangle()
        {
            Fill = Brushes.LightSteelBlue,
            StrokeThickness = 4,
            Stroke = Brushes.Black,
            Width = standardSize,
            Height = standardSize,
        };

        private static Rectangle buildingRectangle = new Rectangle()
        {
            Fill = Brushes.Blue,
            StrokeThickness = 4,
            Stroke = Brushes.Black,
            Width = standardSize,
            Height = standardSize,
        };

        private static readonly int standardSize = 50;

        /// <summary>
        /// allows to draw Ghost buildings on mouseLocation
        /// </summary>
        /// <param name="mouse"></param>
        /// <param name="canvas"></param>
        public static void DrawGhost(Point mouse, Canvas canvas)
        {
            canvas.Children.Remove(buildingGhost);
            Canvas.SetTop(buildingGhost, (int)mouse.Y - buildingGhost.Height / 2);
            Canvas.SetLeft(buildingGhost, (int)mouse.X - buildingGhost.Width / 2);
            buildingGhost.Width = standardSize;
            buildingGhost.Height = standardSize;
            canvas.Children.Add(buildingGhost);
        }

        /// <summary>
        /// Creates a building and add this to the list of buildings
        /// </summary>
        /// <param name="location"></param>
        /// <param name="canvas"></param>
        /// <param name="buildingList"></param>
        /// <returns></returns>
        public static Building CreateBuilding(Vector location, Canvas canvas, List<Building> buildingList)
        {
            var returnBuilding = new Building(location, standardSize);

            if (!ObjectHandler.LocationContainsObject(location))
            {
                buildingList.Add(returnBuilding);
                DrawBuilding(canvas, returnBuilding);
                return returnBuilding;
            }
            else
            {
                throw new Exception("Could not create building because is on road, intersection, building or garage");
            }
        }

        /// <summary>
        /// Draws a specific building on a canvas
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="building"></param>
        public static void DrawBuilding(Canvas canvas, Building building)
        {
            RemoveGhost(canvas);
            Canvas.SetTop(buildingRectangle, (int)building.Location.Y - building.Size / 2);
            Canvas.SetLeft(buildingRectangle, (int)building.Location.X - building.Size / 2);
            buildingRectangle.Width = building.Size;
            buildingRectangle.Height = building.Size;
            canvas.Children.Add(clone(buildingRectangle));
        }

        /// <summary>
        /// remove ghostBuilding from canvas
        /// </summary>
        /// <param name="canvas"></param>
        public static void RemoveGhost(Canvas canvas)
        {
            canvas.Children.Remove(buildingGhost);
        }


        /// <summary>
        /// Dirty fix to copy Buildings
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
