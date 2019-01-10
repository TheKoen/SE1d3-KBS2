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
    public static class GarageCreator
    {
        private const int StandardSize = 50;

        #region Properties & Fields

        /// <summary>
        /// Appearance of ghost <see cref="Garage"/>
        /// </summary>
        private static readonly Rectangle GarageGhost = new Rectangle()
        {
            Fill = Brushes.LightGoldenrodYellow,
            StrokeThickness = 4,
            Stroke = Brushes.Black,
            Width = StandardSize,
            Height = StandardSize,
        };

        /// <summary>
        /// Appearance of <see cref="Garage"/>
        /// </summary>
        private static readonly Rectangle GarageRectangle = new Rectangle()
        {
            Fill = Brushes.LightYellow,
            StrokeThickness = 4,
            Stroke = Brushes.Black,
            Width = StandardSize,
            Height = StandardSize,
        };

        #endregion

        #region Public Methods

        /// <summary>
        /// Draws the ghost of a <see cref="Garage"/> on mouseLocation
        /// </summary>
        /// <param name="mouse"><see cref="Point"/> of mouse</param>
        /// <param name="canvas"><see cref="Canvas"/> to work with</param>
        public static void DrawGhost(Point mouse, Canvas canvas)
        {
            canvas.Children.Remove(GarageGhost);
            Canvas.SetTop(GarageGhost, (int)mouse.Y - GarageGhost.Height / 2);
            Canvas.SetLeft(GarageGhost, (int)mouse.X - GarageGhost.Width / 2);
            GarageGhost.Width = StandardSize;
            GarageGhost.Height = StandardSize;
            canvas.Children.Add(GarageGhost);
        }

        /// <summary>
        /// Creates a <see cref="Garage"/> and adds this to the list of <see cref="Garage"/>s
        /// </summary>
        /// <param name="location">Location of the created <see cref="Garage"/></param>
        /// <param name="canvas"><see cref="Canvas"/> to work with</param>
        /// <param name="garageList"><see cref="List{Garage}"/> to add the <see cref="Garage"/> to</param>
        public static void CreateGarage(Point location, Canvas canvas, List<Garage> garageList)
        {
            var returnGarage = new Garage(new Vector((int)location.X, (int)location.Y), StandardSize);

            if (ObjectHandler.Overlaps(returnGarage)) return;
            garageList.Add(returnGarage);
            DrawGarage(canvas, returnGarage);
        }
  

        /// <summary>
        /// Draws a specific <see cref="Garage"/> on a <see cref="Canvas"/>
        /// </summary>
        /// <param name="canvas"><see cref="Canvas"/> to work with</param>
        /// <param name="garage"><see cref="Garage"/> to draw</param>
        public static void DrawGarage(Canvas canvas, Garage garage)
        {
            canvas.Children.Remove(GarageRectangle);
            Canvas.SetTop(GarageRectangle, (int)garage.Location.Y - garage.Size / 2);
            Canvas.SetLeft(GarageRectangle, (int)garage.Location.X - garage.Size / 2);
            GarageRectangle.Width = garage.Size;
            GarageRectangle.Height = garage.Size;
            canvas.Children.Add(Clone(GarageRectangle));
        }

        /// <summary>
        /// Removes ghost <see cref="Garage"/> from <see cref="Canvas"/>
        /// </summary>
        /// <param name="canvas"><see cref="Canvas"/> to remove from</param>
        public static void RemoveGhost(Canvas canvas)
        {
            canvas.Children.Remove(GarageGhost);
        }
        
        #endregion

        #region Private Methods

        /// <summary>
        /// Fix to copy <see cref="Garage"/>s
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
