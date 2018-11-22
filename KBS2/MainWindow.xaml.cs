using KBS2.CitySystem;
using KBS2.Console;
using KBS2.Console.Commands;
using KBS2.Util;
using KBS2.Utilities;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;

namespace KBS2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static readonly MainLoop Loop = new MainLoop();
        public static ConsoleControl Console { get; private set; }
        public City City { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            Console = MainConsole;
            
            var file = new XmlDocument();
            file.LoadXml("<City>\n\n" +
                         "<Roads>\n" +
                         "  <Road Start=\"200,0\" End=\"200,450\" Width=\"20\" MaxSpeed=\"50\"></Road>\n" +
                         "  <Road Start=\"0,400\" End=\"800,400\" Width=\"20\" MaxSpeed=\"50\"></Road>\n" +
                         "  <Road Start=\"200,200\" End=\"800,200\" Width=\"20\" MaxSpeed=\"50\"></Road>\n" +
                         "  <Road Start=\"200,300\" End=\"800,300\" Width=\"20\" MaxSpeed=\"50\"></Road>\n" +
                         "  <Road Start=\"600,0\" End=\"600,200\" Width=\"20\" MaxSpeed=\"50\"></Road>\n" +
                         "  <Road Start=\"400,200\" End=\"400,300\" Width=\"20\" MaxSpeed=\"50\"></Road>\n" +
                         "  <Road Start=\"600,300\" End=\"600,450\" Width=\"20\" MaxSpeed=\"50\"></Road>\n" +
                         "</Roads>\n\n" +
                         "<Buildings>\n" +
                         "  <Building Location=\"250,160\" Size=\"40\"></Building>\n" +
                         "  <Building Location=\"300,160\" Size=\"40\"></Building>\n" +
                         "  <Building Location=\"350,160\" Size=\"40\"></Building>\n" +
                         "  <Building Location=\"400,160\" Size=\"40\"></Building>\n" +
                         "  <Building Location=\"450,160\" Size=\"40\"></Building>\n" +
                         "  <Building Location=\"520,140\" Size=\"80\"></Building>\n" +
                         "  <Building Location=\"340,250\" Size=\"50\"></Building>\n" +
                         "  <Building Location=\"260,250\" Size=\"50\"></Building>\n" +
                         "</Buildings>\n\n" +
                         "<Intersections>\n" +
                         "  <Intersection Location =\"35,13\" Size=\"5\"></Intersection>\n" +
                         "</Intersections>\n\n" +
                         "</City>");

            CityParser.MakeCity(file);

            // Registering commands
            CommandHandler.RegisterCommand("Export", new CommandExport());
            CommandHandler.RegisterCommand("Set", new CommandSet());
            CommandHandler.RegisterCommand("Get", new CommandGet());
            CommandHandler.RegisterCommand("Map", new CommandMap());
            CommandHandler.RegisterCommand("Sensor", new CommandsSensors());
            
            // Console logic
            MainConsole.SendCommand += (sender, args) =>
            {
                var input = args.Command;

                try
                {
                    var output = CommandHandler.HandleInput(input);
                    MainConsole.Print(output);
                }
                catch (CommandException exception)
                {
                    MainConsole.Print(exception.Message, Colors.Red);
                }
            };
        }
    }
}
