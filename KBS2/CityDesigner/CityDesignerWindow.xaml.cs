using KBS2.CityDesigner.ObjectCreators;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace KBS2.CityDesigner
{
    /// <summary>
    /// Interaction logic for CityDesignerWindow.xaml
    /// </summary>
    public partial class CityDesignerWindow : Window
    {
        public ObjectHandler Creator { get; set; }

        public Tools Tool { get; set; } = Tools.Cursor;



        public CityDesignerWindow()
        {
            InitializeComponent();
            Creator = new ObjectHandler(Canvas, this);
        }


        /// <summary>
        /// Save a city
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            
            // save window
            var popupWindow = new SaveFileDialog();
            popupWindow.Title = "Save City";
            popupWindow.Filter = "XML file | *.xml";
            popupWindow.ShowDialog();

            // save the city
            CitySaver.SaveCity(popupWindow.FileName, ObjectHandler.Roads, ObjectHandler.Buildings, ObjectHandler.Garages, ObjectHandler.Intersections);
        }

        /// <summary>
        /// Load a City
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            //load window
            var popupWindow = new OpenFileDialog();
            popupWindow.Title = "Load City";
            popupWindow.Filter = "XML file | *.xml";
            if(popupWindow.ShowDialog() == true)
            {
                //set the File name in textBox
                FileName.Text = System.IO.Path.GetFileNameWithoutExtension(popupWindow.FileName);
                CityLoader.LoadCity(popupWindow.FileName);
            }
        }

        /// <summary>
        /// Changes the tool when clicked on one of the tools
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChangeTool(object sender, RoutedEventArgs e)
        {
            if(e.Source == RoadButton)
            {
                //set tool
                Tool = Tools.Road;

                // set the buttons active and not active
                BuildingButton.IsEnabled = true;
                GarageButton.IsEnabled = true;
                CursorButton.IsEnabled = true;
                RoadButton.IsEnabled = false;
            }
            if(e.Source == CursorButton)
            {
                //set tool
                Tool = Tools.Cursor;

                // set the buttons active and not active
                BuildingButton.IsEnabled = true;
                GarageButton.IsEnabled = true;
                CursorButton.IsEnabled = false;
                RoadButton.IsEnabled = true;
            }
            if(e.Source == BuildingButton)
            {
                //set tool
                Tool = Tools.Building;

                // set the buttons active and not active
                BuildingButton.IsEnabled = false;
                GarageButton.IsEnabled = true;
                CursorButton.IsEnabled = true;
                RoadButton.IsEnabled = true;
            }
            if(e.Source == GarageButton)
            {
                //set tool
                Tool = Tools.Garage;

                // set the buttons active and not active
                BuildingButton.IsEnabled = true;
                GarageButton.IsEnabled = false;
                CursorButton.IsEnabled = true;
                RoadButton.IsEnabled = true;
            }
        }
        

        private bool leftButtonWasPressed;
        private void MouseMovesOnCanvasEventHandler(object sender, MouseEventArgs e)
        {
            //location mouse on canvas
            X.Text = ((int)e.GetPosition(Canvas).X).ToString();
            Y.Text = ((int)e.GetPosition(Canvas).Y).ToString();


            // drawing Ghost Building
            if (Tool == Tools.Building) { BuildingCreator.DrawGhost(e.GetPosition(Canvas), Canvas); }

            // drawin Ghost Garage
            if (Tool == Tools.Garage) { GarageCreator.DrawGhost(e.GetPosition(Canvas), Canvas); }

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                leftButtonWasPressed = true;
                // drawing Ghost Road
                if(Tool == Tools.Road) { RoadCreator.DrawGhost(e.GetPosition(Canvas), Canvas, ObjectHandler.Roads); }

            }
            if (e.LeftButton == MouseButtonState.Released && leftButtonWasPressed == true)
            {
                leftButtonWasPressed = false;
                // drawing Real Road
                if (Tool == Tools.Road) {
                    RoadCreator.CreateRoad(Canvas, ObjectHandler.Roads, ObjectHandler.Buildings, ObjectHandler.Garages);
                    
                }
            }
           
        }

        private void MouseClicksOnCanvasEventHandler(object sender, MouseEventArgs e)
        {
            if (Tool == Tools.Building) { BuildingCreator.CreateBuilding(e.GetPosition(Canvas), Canvas, ObjectHandler.Buildings); }
            if (Tool == Tools.Garage) { GarageCreator.CreateGarage(e.GetPosition(Canvas), Canvas, ObjectHandler.Garages); }
        }

        private void MouseLeaveCanvasEventHandler(object sender, MouseEventArgs e)
        {
            //remove ghost items when leaving canvas
            Creator.RemoveGhosts();
        }

        private void MouseRightCanvasEventHandler(object sender, MouseEventArgs e)
        {
            //Get data of Object
            Creator.SelectRoad = null;
            Creator.SelectBuildingGarage = null;
            if (Tool == Tools.Cursor) { Creator.GetObject(); }
        }

        private void MouseEntersCanvasEventHandler(object sender, MouseEventArgs e)
        {
            //Un-Display information 
            InformationBlockRoad.Visibility = Visibility.Hidden;
            InformationBlockBuilding.Visibility = Visibility.Hidden;
            Creator.SelectRoad = null;
            Creator.SelectBuildingGarage = null;
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            ObjectHandler.Buildings.Clear();
            ObjectHandler.Roads.Clear();
            ObjectHandler.Intersections.Clear();
            ObjectHandler.Garages.Clear();
            ObjectHandler.RedrawAllObjects(Canvas);
        }

        /// <summary>
        /// Remove object when selected with delete button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void KeydownEventHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                //Remove object hide information
                ObjectHandler.Roads.Remove(Creator.SelectRoad);
                ObjectHandler.RedrawAllObjects(Canvas);
                ObjectHandler.Buildings.Remove(Creator.SelectBuildingGarage);
                InformationBlockRoad.Visibility = Visibility.Hidden;
                InformationBlockBuilding.Visibility = Visibility.Hidden;
                Creator.SelectRoad = null;
                Creator.SelectRoad = null;
                Creator.SelectBuildingGarage = null;
                ObjectHandler.RedrawAllObjects(Canvas);
            }
        }

        /// <summary>
        /// Remove object when selected with remove button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveObjectButtonEventHandler(object sender, RoutedEventArgs e)
        {
            //Remove object hide information
            ObjectHandler.Roads.Remove(Creator.SelectRoad);
            ObjectHandler.RedrawAllObjects(Canvas);
            ObjectHandler.Buildings.Remove(Creator.SelectBuildingGarage);
            InformationBlockRoad.Visibility = Visibility.Hidden;
            InformationBlockBuilding.Visibility = Visibility.Hidden;
            Creator.SelectRoad = null;
            Creator.SelectRoad = null;
            Creator.SelectBuildingGarage = null;
            ObjectHandler.RedrawAllObjects(Canvas);
        }

        private void ChangeWidthRoadEventHandler(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                if (Creator.SelectRoad != null)
                {
                    Creator.SelectRoad.Width = NumericWidthRoad.Value;
                    ObjectHandler.RedrawAllObjects(Canvas);
                }
            }
            catch(NullReferenceException) { } // because creator does not exist before the first event is fired in XML
            
        }

        private void ChangeWidthBuildingEventHandler(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                if (Creator.SelectBuildingGarage != null)
                {
                    Creator.SelectBuildingGarage.Size = NumericSizeBuilding.Value;
                    if (ObjectHandler.Overlaps(Creator.SelectBuildingGarage)) 
                    {
                        Creator.SelectBuildingGarage.Size--;
                        NumericSizeBuilding.Value--;
                    }
                    ObjectHandler.RedrawAllObjects(Canvas);
                }
            }
            catch (NullReferenceException) { } // because creator does not exist before the first event is fired in XML
        }
        
        private void ChangeMaxSpeedRoadEventHandler(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                if (Creator.SelectBuildingGarage != null)
                {
                    Creator.SelectRoad.MaxSpeed = NumericMaxSpeedRoad.Value;
                    ObjectHandler.RedrawAllObjects(Canvas);
                }
            }
            catch (NullReferenceException) { } // because creator does not exist before the first event is fired in XML
        }
    }
}
