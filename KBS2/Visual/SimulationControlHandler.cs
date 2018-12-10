﻿using System;
using System.IO;
using System.Linq;
using System.Windows;
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

            try
            {
                file.Load(SelectedFilePath);
            }
            catch(Exception ex)
            {
                MessageBox.Show("Please select a City", "No city selected!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                CityParser.MakeCity(file);
            }
            catch (Exception ex)
            {
                MessageBox.Show("There is an error in the city file.", "Invalid City File", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
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
            Screen.BtnStart.IsEnabled = false;
            Screen.BtnPause.IsEnabled = true;
            Screen.BtnStop.IsEnabled = true;

            App.Console.Print("Start pressed");

            MainScreen.WPFLoop.Start();
            MainScreen.AILoop.Start();
        }

        public void PauseButtonClick()
        {
            Screen.BtnStart.IsEnabled = true;
            Screen.BtnPause.IsEnabled = false;
            App.Console.Print("Pause pressed");
            
            MainScreen.WPFLoop.Stop();
            MainScreen.AILoop.Stop();
        }

        public void ResetButtonClick()
        {
            Screen.BtnStart.IsEnabled = true;
            Screen.BtnPause.IsEnabled = false;
            Screen.BtnStop.IsEnabled = false;
            App.Console.Print("Reset pressed");

            LoadButtonClick();
            ResetLabels();
            Screen.Ticks = 0;
            Screen.SecondsRunning = 0;
            Screen.UpdateTimer();
            MainScreen.WPFLoop.Stop();
            MainScreen.AILoop.Stop();

            City.Instance.Controller.Reset();
        }

        public void ResetLabels()
        {
            Screen.LabelSimulationAmountCostumer.Content = 0;
            Screen.LabelSimulationSuccesfulRides.Content = 0;
            Screen.LabelSimulationRidesCancelled.Content = 0;
            Screen.LabelSimulationDeaths.Content = 0;

            Screen.LabelSimulationAmountCars.Content = 0;
            Screen.LabelSimulationDriving.Content = 0;
            Screen.LabelSimulationNotInUse.Content = 0;
        }
    }
}