﻿using KBS2.CitySystem;
using KBS2.Console;
using KBS2.GPS;
using System.Windows;
using System.Xml;
using System.IO;
using KBS2.Util;
using System.Collections.Generic;
using KBS2.Util.Loop;

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
            var file = new XmlDocument();
            file.Load(filePath);
            CityParser.MakeCity(file);
            //Teken stad. :^)
            var city = City.Instance;

            BtnStart.IsEnabled = true;
            BtnPause.IsEnabled = true;
            BtnStop.IsEnabled = true;
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
                PropertyLabels.Add(prop);
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
        
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {

            foreach (var property in PropertyLabels)
            {
                if (property.TBCurrentValue.Text != property.CurrentValue)
                {
                    var name = property.LabelPropertyName.ToString();
                    var value = property.TBCurrentValue.Text;

                    property.CurrentValue = property.TBCurrentValue.Text;

                    CommandHandler.ModifyProperty(name, value);

                }
            }
        }

        private void BtnDefault_Click(object sender, RoutedEventArgs e)
        {
            CommandHandler.ResetProperties();
            createPropertyList();
        }

        public void Update()
        {
            LabelSimulationAmountCostumer.Content = City.Instance.Customers.Count;
            createPropertyList();
        }
    }
}
