using KBS2.CitySystem;
using KBS2.Console;
using KBS2.GPS;
using System.Windows;
using System.Xml;
using System.IO;
using KBS2.Util;
using System.Collections.Generic;
using System.Linq;
using KBS2.Util.Loop;
using System.Linq;

namespace KBS2
{
    /// <summary>
    /// Interaction logic for MainScreen.xaml
    /// </summary>
    public partial class MainScreen : Window
    {
        public static readonly TickLoop Loop = new ThreadLoop("main");
        public static readonly TickLoop CommandLoop = new MainLoop("command");
        public List<PropertySettings> PropertyLabels = new List<PropertySettings>(); 

        private string filePath;

        public MainScreen()
        {
            
            InitializeComponent();
            Loop.Subscribe(Update);
            CommandLoop.Start();
            GPSSystem.Setup();
            

            // Registering commands
            CommandRegistrar.AutoRegisterCommands("KBS2.Console.Commands");

            // Showing list of properties in settings tab
            Loaded += (sender, args) => createPropertyList();
        }

        private void BtnSelect_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog
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
                TBCity.Text = Path.GetFileNameWithoutExtension(fileName);
            }
        }

        private void BtnLoad_Click(object sender, RoutedEventArgs e)
        {
            //Loads the city file and parses the information into a City.
            var file = new XmlDocument();
            file.Load(filePath);
            CityParser.MakeCity(file);
            var city = City.Instance;
            
            createPropertyList();

            //Enables buttons and tabs so the user can acces them.
            BtnStart.IsEnabled = true;
            BtnPause.IsEnabled = true;
            BtnStop.IsEnabled = true;
            TabItemSettings.IsEnabled = true;
            TabItemResults.IsEnabled = true;

            //Fills in the current City information that is needed.
            LabelSimulationRoad.Content = city.Roads.Count;
            LabelSimulationIntersection.Content = city.Intersections.Count;
            LabelSimulationBuilding.Content = city.Buildings.Count;
            LabelSimulationGarage.Content = city.Buildings.FindAll(building => building is Garage).Count;
            LabelSimulationAmountCostumer.Content = city.Customers.Count;
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            Loop.Start();
            LabelStateSim.Content = "Start pressed";
        }

        private void BtnPause_Click(object sender, RoutedEventArgs e)
        {
            Loop.Stop();

            LabelStateSim.Content = "Pause pressed";
        }

        private void BtnStop_Click(object sender, RoutedEventArgs e)
        {
            Loop.Stop();
            LabelStateSim.Content = "Stop pressed";
            City.Instance.Controller.Reset();
        }

        //creates a label for every property.
        public void createPropertyList()
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

        //Method for saving the new values the user has filled in in the Settings tab.
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        { 
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
            }
        }

        private void BtnDefault_Click(object sender, RoutedEventArgs e)
        {
            CommandHandler.ModifyProperty("main.tickRate", 30);
            CommandHandler.ModifyProperty("command.tickRate", 30);
            CommandHandler.ModifyProperty("startingPrice", 1.50);
            CommandHandler.ModifyProperty("pricePerKilometer", 1.00);
            CommandHandler.ModifyProperty("customerSpawnRate", 0.2f);
            CommandHandler.ModifyProperty("availableCars", 10);
            CommandHandler.ModifyProperty("customerCount", 10);
            CommandHandler.ModifyProperty("globalSpeedLimit", -1);
            CommandHandler.ModifyProperty("avgGroupSize", 10);
            createPropertyList();
        }

        public void Update()
        {
            LabelSimulationAmountCostumer.Content = City.Instance.Customers.Count;
            LabelSimulationAmountCars.Content = City.Instance.Cars.Count;
            
        }
    }
}
