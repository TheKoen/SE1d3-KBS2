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

            // Loading Xml file in Debug folder
            var xmlCity = new XmlDocument();
            xmlCity.Load("testcity.xml");
            CityParser.MakeCity(xmlCity);

            // Registering commands
            CommandHandler.RegisterCommand("Export", new CommandExport());
            CommandHandler.RegisterCommand("Set", new CommandSet());
            CommandHandler.RegisterCommand("Map", new CommandMap());

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
