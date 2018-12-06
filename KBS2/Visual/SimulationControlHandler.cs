using System.IO;
using System.Linq;
using System.Xml;
using KBS2.CitySystem;
using KBS2.Visual.Controls;

namespace KBS2.Visual
{
    public class SimulationControlHandler
    {
        private MainScreen Screen { get; }
        private string SelectedFilePath { get; set; }

        public SimulationControlHandler(MainScreen screen)
        {
            Screen = screen;
        }

        public void SelectButtonClick()
        {
            var dlg = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".xml",
                Filter = "XML documents (.xml)|*.xml"
            };

            // Display OpenFileDialog by calling ShowDialog method
            var result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox
            if (result == true)
            {

                // Open document
                var fileName = dlg.FileName;
                SelectedFilePath = fileName;

                var cityname = Path.GetFileNameWithoutExtension(fileName);
                Screen.TBCity.Text = cityname;
                Screen.CityName.Content = "City : " + cityname.First().ToString().ToUpper() + cityname.Substring(1);
            }
        }

        public void LoadButtonClick()
        {
            // Loads the city file and parses the information into a City.
            var file = new XmlDocument();
            file.Load(SelectedFilePath);
            CityParser.MakeCity(file);
            var city = City.Instance;

            Screen.CityRenderHandler.DrawCity(city);

            Screen.PropertyDisplayHandler.UpdateProperties();

            EnableButtonsAndTabs();
            UpdateCountLabels(city);
        }

        private void EnableButtonsAndTabs()
        {
            Screen.BtnStart.IsEnabled = true;
            Screen.TabItemSettings.IsEnabled = true;
            Screen.TabItemResults.IsEnabled = true;
        }

        private void UpdateCountLabels(City city)
        {
            Screen.LabelSimulationRoad.Content = city.Roads.Count;
            Screen.LabelSimulationIntersection.Content = city.Intersections.Count;
            Screen.LabelSimulationBuilding.Content = city.Buildings.Count;
            Screen.LabelSimulationGarage.Content = city.Buildings.FindAll(building => building is Garage).Count;
        }

        public void StartButtonClick()
        {
            App.Console.Print("Start pressed");

            MainScreen.WPFLoop.Start();
            MainScreen.AILoop.Start();
            
            var city = City.Instance;
            foreach (var car in city.Cars)
            {
                Screen.CanvasMain.Children.Add(new CarControl(car));
            }

            Screen.BtnPause.IsEnabled = true;
            Screen.BtnStop.IsEnabled = true;
            Screen.BtnStart.IsEnabled = false;
        }

        public void PauseButtonClick()
        {
            Screen.BtnStart.IsEnabled = true;
            App.Console.Print("Pause pressed");
            
            MainScreen.WPFLoop.Stop();
            MainScreen.AILoop.Stop();
        }

        public void ResetButtonClick()
        {
            Screen.BtnStart.IsEnabled = true;
            App.Console.Print("Reset pressed");

            MainScreen.WPFLoop.Stop();
            MainScreen.AILoop.Stop();

            City.Instance.Controller.Reset();
        }
    }
}
