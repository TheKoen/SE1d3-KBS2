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
using System.Xml;
using System.IO;

namespace KBS2
{
    /// <summary>
    /// Interaction logic for MainScreen.xaml
    /// </summary>
    public partial class MainScreen : Window
    {
        public static readonly MainLoop Loop = new MainLoop("main");
        public static readonly MainLoop CommandLoop = new MainLoop("command");

        private string filePath;

        public MainScreen()
        {
            
            InitializeComponent();
            CommandLoop.Start();
            GPSSystem.Setup();
            

            // Registering commands
            CommandRegistrar.AutoRegisterCommands("KBS2.Console.Commands");

            // Showing list of properties in settings tab
            Loaded += (sender, args) => createPropertyList();
        }

        private void BtnSelect_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".xml";
            dlg.Filter = "XML documents (.xml)|*.xml";

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
            // Create a City
            var file = new XmlDocument();
            file.Load(filePath);
            CityParser.MakeCity(file);

        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            Loop.Start();
        }

        private void BtnPause_Click(object sender, RoutedEventArgs e)
        {
            Loop.Stop();
        }

        private void BtnStop_Click(object sender, RoutedEventArgs e)
        {

            Loop.Stop();
            City.Instance.Controller.Reset();
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
    }
}
