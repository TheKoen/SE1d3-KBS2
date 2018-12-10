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
        private static Rectangle garageGhost = new Rectangle()
        {
            Fill = Brushes.LightGoldenrodYellow,
            StrokeThickness = 4,
            Stroke = Brushes.Black,
            Width = standardSize,
            Height = standardSize,
        };

        private static Rectangle garageRectangle = new Rectangle()
        {
            Fill = Brushes.LightYellow,
            StrokeThickness = 4,
            Stroke = Brushes.Black,
            Width = standardSize,
            Height = standardSize,
        };

        private static readonly int standardSize = 50;

        public static void DrawGhost(Point mouse, Canvas canvas)
        {
            canvas.Children.Remove(garageGhost);
            Canvas.SetTop(garageGhost, (int)mouse.Y - garageGhost.Height / 2);
            Canvas.SetLeft(garageGhost, (int)mouse.X - garageGhost.Width / 2);
            garageGhost.Width = standardSize;
            garageGhost.Height = standardSize;
            canvas.Children.Add(garageGhost);
        }

        public static Garage CreateGarage(Point location, Canvas canvas, List<Garage> garageList)
        {
            var returnGarage = new Garage(new System.Windows.Vector(location.X, location.Y), standardSize , DirectionCar.North);

            if(!ObjectHandler.Overlaps(returnGarage))
            {
                garageList.Add(returnGarage);
                drawGarage(canvas, returnGarage);
                return returnGarage;
            }
            return null;
        }

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
