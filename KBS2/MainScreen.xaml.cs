using KBS2.CitySystem;
using KBS2.Console;
using KBS2.GPS;
using System.Windows;
using System.Xml;
using System.IO;
using KBS2.Util;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using KBS2.Util.Loop;
using System.Linq;
using KBS2.Visual.Controls;
using System;
using KBS2.CarSystem;
using KBS2.Visual;

namespace KBS2
{
    /// <summary>
    /// Interaction logic for MainScreen.xaml
    /// </summary>
    public partial class MainScreen : Window
    {
        /*
         * A note on loops:
         * There's 3 different loops, a CommandLoop, a WPFLoop and an AILoop.
         * Make sure you understand what they are before subscribing to one!
         *
         * The CommandLoop is only used for the Console, and should NEVER be
         * used for anything else! Just pretend like it doesn't exist and
         * don't use it.
         *
         * The WPFLoop for anything related to the visuals. It's important
         * for a smooth interaface that this loop runs fast, so DON'T do
         * anything complicated or time-intensive on there.
         *
         * The AILoop is for any AI logic. This is where things like the
         * customers, car AI, car sensors, etc are supposed to run. Still
         * needs to run fast, but can more easily deal with irregular
         * refresh rates.
         */
        public static readonly TickLoop CommandLoop = new MainLoop("Command");
        public static readonly TickLoop WPFLoop = new MainLoop("Main");
        public static readonly TickLoop AILoop = new ThreadLoop("AI");

        private readonly ConsoleWindow consoleWindow;

        private readonly CityRenderHandler cityRenderHandler;

        private string filePath;

        public int Ticks { get; set; }
        public double SecondsRunning { get; private set; }

        public MainScreen()
        {
            consoleWindow = new ConsoleWindow();

            cityRenderHandler = new CityRenderHandler(CanvasMain);

            InitializeComponent();
            WPFLoop.Subscribe(Update);
            CommandLoop.Start();
            GPSSystem.Setup();

            // Showing list of properties in settings tab
            Loaded += (sender, args) => UpdatePropertyList();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            consoleWindow.AllowClose = true;
            consoleWindow.Close();
        }

        private void BtnSelect_Click(object sender, RoutedEventArgs e)
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
                filePath = fileName;
                var cityname = Path.GetFileNameWithoutExtension(fileName);
                TBCity.Text = cityname;
                CityName.Content = "City : " + cityname.First().ToString().ToUpper() + cityname.Substring(1);
            }
        }

        private void BtnLoad_Click(object sender, RoutedEventArgs e)
        {
            // Loads the city file and parses the information into a City.
            var file = new XmlDocument();
            file.Load(filePath);
            CityParser.MakeCity(file);
            var city = City.Instance;

            //TODO: Draw city.

            UpdatePropertyList();
            //Enables or disables buttons and tabs so the user can acces them or not.                
            TabItemSettings.IsEnabled = true;
            TabItemResults.IsEnabled = true;
            BtnStart.IsEnabled = true;
            BtnPause.IsEnabled = false;
            BtnStop.IsEnabled = false;

            // Fills in the current City information that is needed.
            LabelSimulationRoad.Content = city.Roads.Count;
            LabelSimulationIntersection.Content = city.Intersections.Count;
            LabelSimulationBuilding.Content = city.Buildings.Count;
            LabelSimulationGarage.Content = city.Buildings.FindAll(building => building is Garage).Count;
            LabelSimulationAmountCostumer.Content = city.Customers.Count;
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            WPFLoop.Start();
            AILoop.Start();
            BtnStart.IsEnabled = false;
            BtnPause.IsEnabled = true;
            BtnStop.IsEnabled = true;
            App.Console.Print("Start pressed");
        }

        private void BtnPause_Click(object sender, RoutedEventArgs e)
        {
            WPFLoop.Stop();
            AILoop.Stop();
            BtnStart.IsEnabled = true;
            App.Console.Print("Pause pressed");
        }

        private void BtnStop_Click(object sender, RoutedEventArgs e)
        {
            WPFLoop.Stop();
            AILoop.Stop();
            City.Instance.Controller.Reset();
            BtnStart.IsEnabled = true;
            App.Console.Print("Reset pressed");
        }

        // Creates a label for every property.
        public void UpdatePropertyList()
        { 
            StackPanelSettings.Children.Clear();
            var properties = CommandHandler.GetProperties();
            foreach (var property in properties)
            {
                var propname = property.Key.ToString();
                var propvalue = property.Value.Value.ToString();

                var prop = new PropertySettings(propname, propvalue);
                StackPanelSettings.Children.Add(prop);
            }
        }

        private void BtnImport_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnLoadResult_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnShow_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnShowLatest_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnSaveSim_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnExport_Click(object sender, RoutedEventArgs e)
        {

        }

        // Method for saving the new values the user has filled in in the Settings tab.
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            var s = "1,5";
            var y = "1";
            foreach (var child in StackPanelSettings.Children)
            {
                var propertyControl = (PropertySettings)child;
                var name = propertyControl.LabelPropertyName.Content.ToString();
                var property = CommandHandler.GetProperties().First(p => p.Key == name);

                if (propertyControl.TBCurrentValue.Text != property.Value.ToString())
                {
                    var value = propertyControl.TBCurrentValue.Text;
                    CommandHandler.HandleInput($"set { name } { value }");
                    propertyControl.CurrentValue = propertyControl.TBCurrentValue.Text;
                }
         
                switch (propertyControl.LabelPropertyName.Content.ToString()) {
                    case "startingPrice":
                        s = propertyControl.TBCurrentValue.Text;
                        break;
                    case "pricePerKilometer":
                        y = propertyControl.TBCurrentValue.Text;
                        break;
                }
            }
            LabelSimulationPriceFormula.Content = $" {s} + {y} * km";
        }

        private void BtnDefault_Click(object sender, RoutedEventArgs e)
        {
            //TODO: Does this need to be hardcoded? Can't we use a more dynamic solution?
            CommandHandler.ModifyProperty("main.tickRate", 30);
            CommandHandler.ModifyProperty("command.tickRate", 30);
            CommandHandler.ModifyProperty("startingPrice", 1.50);
            CommandHandler.ModifyProperty("pricePerKilometer", 1.00);
            CommandHandler.ModifyProperty("customerSpawnRate", 0.2f);
            CommandHandler.ModifyProperty("availableCars", 10);
            CommandHandler.ModifyProperty("customerCount", 10);
            CommandHandler.ModifyProperty("globalSpeedLimit", -1);
            CommandHandler.ModifyProperty("avgGroupSize", 10);
            UpdatePropertyList();
        }

        public void UpdateTimer()
        {
            LabelSimulationTime.Content = SecondsRunning;
        }

        public double GetSeconds()
        {
            return Math.Round(Ticks / 100.0, 2);
        }

        public void Update()
        {
            LabelSimulationAmountCostumer.Content = City.Instance.Customers.Count;
            LabelSimulationAmountCars.Content = City.Instance.Cars.Count;

            Ticks++;
            SecondsRunning = GetSeconds();
            UpdateTimer();
        }

        private void BtnConsole_Click(object sender, RoutedEventArgs e)
        {
            if (consoleWindow.IsVisible)
            {
                consoleWindow.Hide();
            }
            else
            {
                consoleWindow.Show();
            }
        }
    }
}
