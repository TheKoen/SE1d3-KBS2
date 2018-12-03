using KBS2.CitySystem;
using KBS2.Console;
using KBS2.GPS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;

namespace KBS2
{
    /// <summary>
    /// Interaction logic for MainScreen.xaml
    /// </summary>
    public partial class MainScreen : Window
    {
        public static readonly MainLoop Loop = new MainLoop("main");
        public static readonly MainLoop CommandLoop = new MainLoop("command");

        public MainScreen()
        {
            
            InitializeComponent();
            CommandLoop.Start();
            GPSSystem.Setup();

            // Create a City
            var file = new XmlDocument();
            file.LoadXml("<City>\n\n" +
                         "<Roads>\n" +
                         "  <Road Start=\"200,0\" End=\"200,200\" Width=\"20\" MaxSpeed=\"50\"></Road>\n" +
                         "  <Road Start=\"200,200\" End=\"200,300\" Width=\"20\" MaxSpeed=\"50\"></Road>\n" +
                         "  <Road Start=\"200,300\" End=\"200,400\" Width=\"20\" MaxSpeed=\"50\"></Road>\n" +
                         "  <Road Start=\"200,400\" End=\"200,450\" Width=\"20\" MaxSpeed=\"50\"></Road>\n" +
                         "  <Road Start=\"200,200\" End=\"400,200\" Width=\"20\" MaxSpeed=\"50\"></Road>\n" +
                         "  <Road Start=\"400,200\" End=\"600,200\" Width=\"20\" MaxSpeed=\"50\"></Road>\n" +
                         "  <Road Start=\"600,200\" End=\"800,200\" Width=\"20\" MaxSpeed=\"50\"></Road>\n" +
                         "  <Road Start=\"200,300\" End=\"400,300\" Width=\"20\" MaxSpeed=\"50\"></Road>\n" +
                         "  <Road Start=\"400,300\" End=\"600,300\" Width=\"20\" MaxSpeed=\"50\"></Road>\n" +
                         "  <Road Start=\"600,300\" End=\"800,300\" Width=\"20\" MaxSpeed=\"50\"></Road>\n" +
                         "  <Road Start=\"0,400\" End=\"200,400\" Width=\"20\" MaxSpeed=\"50\"></Road>\n" +
                         "  <Road Start=\"200,400\" End=\"600,400\" Width=\"20\" MaxSpeed=\"50\"></Road>\n" +
                         "  <Road Start=\"600,400\" End=\"800,400\" Width=\"20\" MaxSpeed=\"50\"></Road>\n" +
                         "  <Road Start=\"600,0\" End=\"600,200\" Width=\"20\" MaxSpeed=\"50\"></Road>\n" +
                         "  <Road Start=\"400,200\" End=\"400,300\" Width=\"20\" MaxSpeed=\"50\"></Road>\n" +
                         "  <Road Start=\"600,300\" End=\"600,400\" Width=\"20\" MaxSpeed=\"50\"></Road>\n" +
                         "  <Road Start=\"600,600\" End=\"600,450\" Width=\"20\" MaxSpeed=\"50\"></Road>\n" +
                         "  <Road Start=\"800,200\" End=\"800,300\" Width=\"20\" MaxSpeed=\"50\"></Road>\n" +
                         "</Roads>\n\n" +
                         "<Buildings>\n" +
                         "  <Building Location=\"300,160\" Size=\"40\"></Building>\n" +
                         "  <Building Location=\"400,160\" Size=\"40\"></Building>\n" +
                         "  <Building Location=\"450,160\" Size=\"40\"></Building>\n" +
                         "  <Building Location=\"250,250\" Size=\"40\"></Building>\n" +
                         "  <Building Location=\"640,360\" Size=\"40\"></Building>\n" +
                         "</Buildings>\n\n" +
                         "<Intersections>\n" +
                         "  <Intersection Location =\"200,200\" Size=\"20\"></Intersection>\n" +
                         "  <Intersection Location =\"400,200\" Size=\"20\"></Intersection>\n" +
                         "  <Intersection Location =\"600,200\" Size=\"20\"></Intersection>\n" +
                         "  <Intersection Location =\"200,300\" Size=\"20\"></Intersection>\n" +
                         "  <Intersection Location =\"400,300\" Size=\"20\"></Intersection>\n" +
                         "  <Intersection Location =\"600,300\" Size=\"20\"></Intersection>\n" +
                         "  <Intersection Location =\"200,400\" Size=\"20\"></Intersection>\n" +
                         "  <Intersection Location =\"600,400\" Size=\"20\"></Intersection>\n" +
                         "  <Intersection Location =\"800,200\" Size=\"20\"></Intersection>\n" +
                         "  <Intersection Location =\"800,300\" Size=\"20\"></Intersection>\n" +
                         "</Intersections>\n\n" +
                         "</City>");
            CityParser.MakeCity(file);

            // Registering commands
            CommandRegistrar.AutoRegisterCommands("KBS2.Console.Commands");

            // Showing list of properties in settings tab
            Loaded += (sender, args) => createPropertyList();
        }

        private void BtnSelect_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnLoad_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnPause_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnStop_Click(object sender, RoutedEventArgs e)
        {

        }

        public void createPropertyList()
        {
            var properties = Console.CommandHandler.GetProperties();
            foreach (var property in properties)
            {
                var propname = property.Key.ToString();
                var propvalue = property.Value.Value.ToString();
                
                StackPanelSettings.Children.Add(new PropertySettings(propname, propvalue));
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            /*
            CommandHandler.ModifyProperty("startingPrice", PropertySettings.TBCurrentValue.ToString());
            
            CommandHandler.ModifyProperty(, );
            CommandHandler.ModifyProperty(, );
            CommandHandler.ModifyProperty(, );
            CommandHandler.ModifyProperty(, );
            CommandHandler.ModifyProperty(, );
            CommandHandler.ModifyProperty(, );
            CommandHandler.ModifyProperty(, );
            CommandHandler.ModifyProperty(, );
            */
        }

        private void BtnDefault_Click(object sender, RoutedEventArgs e)
        {
            CommandHandler.ResetProperties();
        }
    }
}
