using System.Windows.Controls;
using KBS2.CitySystem;
using KBS2.Visual.Controls;

namespace KBS2.Visual
{
    public class CityRenderHandler : IRenderHandler
    {
        private MainScreen Screen { get; }
        public Canvas Canvas { get; }

        public CityRenderHandler(MainScreen screen, Canvas canvas)
        {
            Screen = screen;
            Canvas = canvas;

            MainScreen.WPFLoop.Subscribe(Update);
        }

        public void Update()
        {
            if (City.Instance != null)
            {
                Screen.LabelSimulationAmountCostumer.Content = City.Instance.Customers.Count;
                Screen.LabelSimulationAmountCars.Content = City.Instance.Cars.Count;
            }
        }

        public void DrawCity(City city)
        {
            Canvas.Children.Clear();

            foreach (var building in city.Buildings)
            {
                Canvas.Children.Add(new BuildingControl(building));
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
