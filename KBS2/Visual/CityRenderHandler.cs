using System.Linq;
using System.Windows.Controls;
using KBS2.CitySystem;
using KBS2.Visual.Controls;

namespace KBS2.Visual
{
    public class CityRenderHandler : IRenderHandler
    {
        private MainScreen Screen { get; }
        public Canvas _Canvas { get; }

        public CityRenderHandler(MainScreen screen, Canvas canvas)
        {
            Screen = screen;
            _Canvas = canvas;

            MainScreen.WPFLoop.Subscribe(Update);
        }

        public void Update()
        {
            if (City.Instance != null)
            {
                Screen.LabelSimulationAmountCostumer.Content = City.Instance.Customers.Count;
                var notInUseCars = City.Instance.Buildings
                    .FindAll(building => building is Garage)
                    .Sum(building => ((Garage) building).AvailableCars);
                Screen.LabelSimulationAmountCars.Content = City.Instance.Cars.Count + notInUseCars;

                Screen.LabelSimulationDriving.Content = City.Instance.Cars.Count;
                Screen.LabelSimulationNotInUse.Content = notInUseCars;
            }
        }

        public void DrawCity(City city)
        {
            _Canvas.Children.Clear();

            foreach (var building in city.Buildings)
            {
                var b1 = new BuildingControl(Screen, building);
                _Canvas.Children.Add(b1);
            }

            foreach (var road in city.Roads)
            {
                _Canvas.Children.Add(new RoadControl(Screen, road));
            }

            foreach (var intersection in city.Intersections)
            {
                _Canvas.Children.Add(new IntersectionControl(Screen, intersection));
            }
        }
    }
}
