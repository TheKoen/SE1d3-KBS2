using KBS2.CitySystem;
using System.Collections.Generic;
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
        private const int StandardSize = 50;
        
        #region Properties & Fields

        private static readonly Rectangle BuildingGhost = new Rectangle()
        {
            Fill = Brushes.LightSteelBlue,
            StrokeThickness = 4,
            Stroke = Brushes.Black,
            Width = StandardSize,
            Height = StandardSize,
        };

        private static readonly Rectangle BuildingCreate = new Rectangle()
        {
            Fill = Brushes.Gray,
            StrokeThickness = 4,
            Stroke = Brushes.Black,
            Width = StandardSize,
            Height = StandardSize,
        };

        #endregion

        #region Public Methods

        /// <summary>
        /// Allows to draw ghost <see cref="Building"/>s on mouseLocation
        /// </summary>
        /// <param name="mouse"><see cref="Point"/> of mouse</param>
        /// <param name="canvas"><see cref="Canvas"/> to work with</param>
        public static void DrawGhost(Point mouse, Canvas canvas)
        {
            canvas.Children.Remove(BuildingGhost);
            Canvas.SetTop(BuildingGhost, (int)mouse.Y - BuildingGhost.Height / 2);
            Canvas.SetLeft(BuildingGhost, (int)mouse.X - BuildingGhost.Width / 2);
            BuildingGhost.Width = StandardSize;
            BuildingGhost.Height = StandardSize;
            canvas.Children.Add(BuildingGhost);
        }

        /// <summary>
        /// Creates a <see cref="Building"/> and adds this to the list of <see cref="Building"/>s
        /// </summary>
        /// <param name="location">Location of the created <see cref="Building"/></param>
        /// <param name="canvas"><see cref="Canvas"/> to work with</param>
        /// <param name="buildingList"><see cref="List{Building}"/> of <see cref="Building"/>s</param>
        public static void CreateBuilding(Point location, Canvas canvas, List<Building> buildingList)
        {
            var returnBuilding = new Building((Vector)location, StandardSize);

            if (ObjectHandler.Overlaps(returnBuilding)) return;
            buildingList.Add(returnBuilding);
            DrawBuilding(canvas, returnBuilding);
        }

        /// <summary>
        /// Draws a specific <see cref="Building"/> on a <see cref="Canvas"/>
        /// </summary>
        /// <param name="canvas"><see cref="Canvas"/> to work with</param>
        /// <param name="building"><see cref="Building"/> to draw</param>
        public static void DrawBuilding(Canvas canvas, Building building)
        {
            Canvas.SetLeft(BuildingCreate, (int)building.Location.X - building.Size / 2);
            Canvas.SetTop(BuildingCreate, (int)building.Location.Y - building.Size / 2);
            Panel.SetZIndex(BuildingCreate, 2);

            BuildingCreate.Width = building.Size;
            BuildingCreate.Height = building.Size;

            canvas.Children.Add(Clone(BuildingCreate));
        }

        /// <summary>
        /// Removes ghost <see cref="Building"/> from <see cref="Canvas"/>
        /// </summary>
        /// <param name="canvas"><see cref="Canvas"/> to remove from</param>
        public static void RemoveGhost(Canvas canvas)
        {
            canvas.Children.Remove(BuildingGhost);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Fix to copy <see cref="Building"/>s
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
