using KBS2.CityDesigner.ObjectCreators;
using KBS2.CitySystem;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

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

        private readonly Cursor _cursorOnCanvas;
        private const string PathCanvasCursor = "C:\\Windows\\Cursors\\cross_r.cur";
        private readonly bool _successFoundCursor = true;
        private readonly Cursor _defaultCursor = Mouse.OverrideCursor;
        
        private bool _leftButtonWasPressed;

        #endregion

        public CityDesignerWindow()
        {
            InitializeComponent();
            Creator = new ObjectHandler(Canvas, this);
            //look for cursor
            try { _cursorOnCanvas = new Cursor(PathCanvasCursor, true); } catch(ArgumentException) { _successFoundCursor = false;  }
        }

        #region EventHandlers
        
        protected override void OnClosing(CancelEventArgs e)
        {
            if (AllowClose) return;
            e.Cancel = true;
            Hide();
        }
        
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // saves the city
            try
            {
                CitySaver.SaveCity(ObjectHandler.Roads, ObjectHandler.Buildings, ObjectHandler.Garages, ObjectHandler.Intersections);
            }
            catch (Exception b)
            {
                MessageBox.Show(windowDesigner, b.Message, "Save error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            // loads a city
            try
            {
                CityLoader.LoadCity();
            }
            catch (Exception b)
            {
                MessageBox.Show(windowDesigner, b.Message, "Save error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void ChangeTool(object sender, RoutedEventArgs e)
        {
            if (Equals(e.Source, RoadButton))
            {
                //set tool
                Tool = Tools.Road;

                // set the buttons active and not active
                BuildingButton.IsEnabled = true;
                GarageButton.IsEnabled = true;
                RoadButton.IsEnabled = false;
            }
            else if (Equals(e.Source, BuildingButton))
            {
                //set tool
                Tool = Tools.Building;

                // set the buttons active and not active
                BuildingButton.IsEnabled = false;
                GarageButton.IsEnabled = true;
                RoadButton.IsEnabled = true;
            }
            else if (Equals(e.Source, GarageButton))
            {
                //set tool
                Tool = Tools.Garage;

                // set the buttons active and not active
                BuildingButton.IsEnabled = true;
                GarageButton.IsEnabled = false;
                RoadButton.IsEnabled = true;
            }
        }


        private void MouseMovesOnCanvasEventHandler(object sender, MouseEventArgs e)
        {
            //location mouse on canvas
            X.Text = ((int)e.GetPosition(Canvas).X).ToString();
            Y.Text = ((int)e.GetPosition(Canvas).Y).ToString();


            switch (Tool)
            {
                // drawing ghost Building
                case Tools.Building:
                    BuildingCreator.DrawGhost(e.GetPosition(Canvas), Canvas);
                    break;
                // drawing ghost Garage
                case Tools.Garage:
                    GarageCreator.DrawGhost(e.GetPosition(Canvas), Canvas);
                    break;
            }

            if (e.LeftButton != MouseButtonState.Pressed) return;
            _leftButtonWasPressed = true;
            
            // drawing ghost Road
            if (Tool == Tools.Road) { RoadCreator.DrawGhost(e.GetPosition(Canvas), Canvas, ObjectHandler.Roads); }
        }

        private void MouseReleaseOnCanvasEventHandler(object sender, MouseEventArgs e)
        {
            if (!_leftButtonWasPressed) return;
            
            _leftButtonWasPressed = false;
            // drawing Real Road
            if (Tool == Tools.Road)
            {
                RoadCreator.CreateRoad(Canvas, ObjectHandler.Roads, ObjectHandler.Buildings, ObjectHandler.Garages);
            }
        }

        private void MouseClicksOnCanvasEventHandler(object sender, MouseEventArgs e)
        {
            switch (Tool)
            {
                case Tools.Building:
                    BuildingCreator.CreateBuilding(e.GetPosition(Canvas), Canvas, ObjectHandler.Buildings);
                    break;
                case Tools.Garage:
                    GarageCreator.CreateGarage(e.GetPosition(Canvas), Canvas, ObjectHandler.Garages);
                    break;
            }
        }

        private void MouseLeaveCanvasEventHandler(object sender, MouseEventArgs e)
        {
            //remove ghost items when leaving canvas
            Creator.RemoveGhosts();
            ChangeCursorCanvas(false);
        }

        private void MouseRightCanvasEventHandler(object sender, MouseEventArgs e)
        {
            //Get data of Object
            Creator.SelectRoad = null;
            Creator.SelectBuildingGarage = null;
            Creator.GetObject();
        }

        private void MouseEntersCanvasEventHandler(object sender, MouseEventArgs e)
        {
            //Un-Display information 
            InformationBlockRoad.Visibility = Visibility.Hidden;
            InformationBlockBuilding.Visibility = Visibility.Hidden;
            Creator.SelectRoad = null;
            Creator.SelectBuildingGarage = null;
            ChangeCursorCanvas(true);
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            ObjectHandler.Buildings.Clear();
            ObjectHandler.Roads.Clear();
            ObjectHandler.Intersections.Clear();
            ObjectHandler.Garages.Clear();
            ObjectHandler.RedrawAllObjects(Canvas);
        }

        private void KeydownEventHandler(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Delete) return;
            
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

        private void ChangeWidthRoadEventHandler(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                if (Creator.SelectRoad == null) return;
                
                Creator.SelectRoad.Width = NumericWidthRoad.Value;
                IntersectionCreator.UpdateIntersection(Creator.SelectRoad, ObjectHandler.Intersections, ObjectHandler.Roads);
                ObjectHandler.RedrawAllObjects(Canvas);
            }
            catch (NullReferenceException) { } // because creator does not exist before the first event is fired in XML
        }

        private void ChangeWidthBuildingEventHandler(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                if (Creator.SelectBuildingGarage == null) return;
                
                Creator.SelectBuildingGarage.Size = NumericSizeBuilding.Value;
                if (ObjectHandler.Overlaps(Creator.SelectBuildingGarage))
                {
                    Creator.SelectBuildingGarage.Size--;
                    NumericSizeBuilding.Value--;
                }
                ObjectHandler.RedrawAllObjects(Canvas);
            }
            catch (NullReferenceException) { } // because creator does not exist before the first event is fired in XML
        }

        private void ChangeMaxSpeedRoadEventHandler(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                if (Creator.SelectBuildingGarage == null) return;
                
                Creator.SelectRoad.MaxSpeed = NumericMaxSpeedRoad.Value;
                ObjectHandler.RedrawAllObjects(Canvas);
            }
            catch (NullReferenceException) { } // because creator does not exist before the first event is fired in XML
        }

        #endregion

        #region Private Methods

        private void ChangeCursorCanvas(bool onCanvas)
        {
            Mouse.OverrideCursor = onCanvas && _successFoundCursor
                ? _cursorOnCanvas
                : _defaultCursor;
        }
        
        #endregion
    }
}
