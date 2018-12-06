using System.Windows.Controls;
using KBS2.CitySystem;
using KBS2.Visual.Controls;

namespace KBS2.Visual
{
    public class CityRenderHandler : IRenderHandler
    {
        public Canvas Canvas { get; }

        public CityRenderHandler(Canvas canvas)
        {
            Canvas = canvas;

            MainScreen.WPFLoop.Subscribe(Update);
        }

        public void Update()
        {

        }

        private void DrawCity(City city)
        {
            Canvas.Children.Clear();

            foreach (var building in city.Buildings)
            {
                Canvas.Children.Add(new BuildingControl(building.Location, building.Size));
            }

            foreach (var road in city.Roads)
            {
                Canvas.Children.Add(new RoadControl(road));
            }

            foreach (var intersection in city.Intersections)
            {
                Canvas.Children.Add(new IntersectionControl(intersection));
            }
        }
    }
}
