using KBS2.CityDesigner.ObjectCreators;
using KBS2.CitySystem;
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
        #region Properties & Fields

        public ObjectHandler Creator { get; set; }
        public bool AllowClose = false;
        public Tools Tool { get; set; } = Tools.Road;

        private Cursor cursorOnCanvas;
        private readonly string pathCanvasCursor = "C:\\Windows\\Cursors\\cross_r.cur";
        private bool successFoundCursor = true;
        private Cursor defaultCursor = Mouse.OverrideCursor;

        #endregion

        #region Constructor

        public CityDesignerWindow()
        {
            MainScreen.CommandLoop.Start();
            InitializeComponent();
            Creator = new ObjectHandler(Canvas, this);
            //look for cursor
            try { cursorOnCanvas = new Cursor(pathCanvasCursor, true); } catch(ArgumentException) { successFoundCursor = false;  }
        }
        
        protected override void OnClosing(CancelEventArgs e)
        {
            if (!AllowClose)
            {
                e.Cancel = true;
                Hide();
            }
        }

        #endregion

        #region EventHandlers
        /// <summary>
        /// Save a city
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // save the city
            try
            {
                CitySaver.SaveCity(ObjectHandler.Roads, ObjectHandler.Buildings, ObjectHandler.Garages, ObjectHandler.Intersections);
            }
            catch (Exception b)
            {
                MessageBox.Show(windowDesigner, b.Message, "Save error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        /// <summary>
        /// Load a City
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CityLoader.LoadCity();
            }
            catch (Exception b)
            {
                MessageBox.Show(windowDesigner, b.Message, "Save error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChangeFileName(object sender, LoadedCityEventArgs e)
        {
            FileName.Text = e.Path;
        }

        /// <summary>
        /// Changes the tool when clicked on one of the tools
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChangeTool(object sender, RoutedEventArgs e)
        {
            if (e.Source == RoadButton)
            {
                //set tool
                Tool = Tools.Road;

                // set the buttons active and not active
                BuildingButton.IsEnabled = true;
                GarageButton.IsEnabled = true;
                RoadButton.IsEnabled = false;
            }
            else if (e.Source == BuildingButton)
            {
                //set tool
                Tool = Tools.Building;

                // set the buttons active and not active
                BuildingButton.IsEnabled = false;
                GarageButton.IsEnabled = true;
                RoadButton.IsEnabled = true;
            }
            else if (e.Source == GarageButton)
            {
                //set tool
                Tool = Tools.Garage;

                // set the buttons active and not active
                BuildingButton.IsEnabled = true;
                GarageButton.IsEnabled = false;
                RoadButton.IsEnabled = true;
            }
        }


        private bool leftButtonWasPressed;

        /// <summary>
        /// Fired when the mouse moves over canvas
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                if (Tool == Tools.Road) { RoadCreator.DrawGhost(e.GetPosition(Canvas), Canvas, ObjectHandler.Roads); }

            }
        }

        private void MouseReleaseOnCanvasEventHandler(object sender, MouseEventArgs e)
        {
            if (leftButtonWasPressed == true)
            {
                leftButtonWasPressed = false;
                // drawing Real Road
                if (Tool == Tools.Road)
                {
                    RoadCreator.CreateRoad(Canvas, ObjectHandler.Roads, ObjectHandler.Buildings, ObjectHandler.Garages);

                }
            }
        }

        /// <summary>
        /// Fired when the mouse clicks on canvas
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MouseClicksOnCanvasEventHandler(object sender, MouseEventArgs e)
        {
            if (Tool == Tools.Building) { BuildingCreator.CreateBuilding(e.GetPosition(Canvas), Canvas, ObjectHandler.Buildings); }
            if (Tool == Tools.Garage) { GarageCreator.CreateGarage(e.GetPosition(Canvas), Canvas, ObjectHandler.Garages, ObjectHandler.Roads); }
        }

        /// <summary>
        /// Fired when the mouse leaves the canvas
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MouseLeaveCanvasEventHandler(object sender, MouseEventArgs e)
        {
            //remove ghost items when leaving canvas
            Creator.RemoveGhosts();
            changeCursorCanvas(false);
        }

        /// <summary>
        /// Fired when the mouse rightclicks on the canvas
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MouseRightCanvasEventHandler(object sender, MouseEventArgs e)
        {
            //Get data of Object
            Creator.SelectRoad = null;
            Creator.SelectBuildingGarage = null;
            Creator.GetObject();
        }

        /// <summary>
        /// Fired when the mouse enters the canvas
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MouseEntersCanvasEventHandler(object sender, MouseEventArgs e)
        {
            //Un-Display information 
            InformationBlockRoad.Visibility = Visibility.Hidden;
            InformationBlockBuilding.Visibility = Visibility.Hidden;
            Creator.SelectRoad = null;
            Creator.SelectBuildingGarage = null;
            changeCursorCanvas(true);
        }

        /// <summary>
        /// Cleans the canvas form all objects
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                if (Creator.SelectBuildingGarage != null && Creator.SelectBuildingGarage.GetType() == typeof(Garage))
                {
                    ObjectHandler.Garages.Remove((Garage)Creator.SelectBuildingGarage);
                }
                else if (Creator.SelectBuildingGarage != null)
                {
                    ObjectHandler.Buildings.Remove(Creator.SelectBuildingGarage);
                }
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
            if (Creator.SelectBuildingGarage != null && Creator.SelectBuildingGarage.GetType() == typeof(Garage))
            {
                ObjectHandler.Garages.Remove((Garage)Creator.SelectBuildingGarage);
            }
            else if (Creator.SelectBuildingGarage != null)
            {
                ObjectHandler.Buildings.Remove(Creator.SelectBuildingGarage);
            }
            InformationBlockRoad.Visibility = Visibility.Hidden;
            InformationBlockBuilding.Visibility = Visibility.Hidden;
            Creator.SelectRoad = null;
            Creator.SelectRoad = null;
            Creator.SelectBuildingGarage = null;
            ObjectHandler.RedrawAllObjects(Canvas);
        }

        /// <summary>
        /// EventHandler that changes the width of a road
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
            catch (NullReferenceException) { } // because creator does not exist before the first event is fired in XML

        }

        /// <summary>
        /// EventHandler that changes the width of a building
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// EventHandler that changes the max speed of a road 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        #endregion

        #region Private Methods

        private void changeCursorCanvas(bool onCanvas)
        {
            if (onCanvas && successFoundCursor)
            {
                Mouse.OverrideCursor = cursorOnCanvas;
            }
            else
            {
                Mouse.OverrideCursor = defaultCursor;
            }
        }
        #endregion
    }
}
