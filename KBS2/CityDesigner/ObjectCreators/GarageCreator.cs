using KBS2.CarSystem;
using KBS2.CitySystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

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

        public static void DrawGhost(int mouseX, int mouseY, Canvas canvas)
        {
            canvas.Children.Remove(garageGhost);
            Canvas.SetTop(garageGhost, (int)mouseY - garageGhost.Height / 2);
            Canvas.SetLeft(garageGhost, (int)mouseX - garageGhost.Width / 2);
            canvas.Children.Add(garageGhost);
        }

        public static Garage CreateGarage(int locationX, int locationY)
        {
            var returnGarage = new Garage(new System.Windows.Vector(locationX, locationY), standardSize , DirectionCar.North);

            if(ObjectHandler.LocationContainsObject(new System.Windows.Vector(locationX, locationY)))
            {
                return returnGarage;
            }
            else
            {
                throw new Exception("Could not create garage because is on road, intersection, building or garage");
            }
            
        }

        public static void drawGarage(Canvas canvas, Garage garage)
        {
            canvas.Children.Remove(garageRectangle);
            Canvas.SetTop(garageRectangle, (int)garage.Location.Y - garage.Size / 2);
            Canvas.SetLeft(garageRectangle, (int)garage.Location.X - garage.Size / 2);
            canvas.Children.Add(garageRectangle);
        }
    }
}
