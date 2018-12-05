using Microsoft.Win32;
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

namespace KBS2.CityDesigner
{
    /// <summary>
    /// Interaction logic for CityDesignerWindow.xaml
    /// </summary>
    public partial class CityDesignerWindow : Window
    {
        public ObjectHandler Creator { get; set; }

        public Tools Tool = Tools.Cursor;



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
            // check 

            // save window
            var popupWindow = new SaveFileDialog();
            popupWindow.Title = "Save City";
            popupWindow.Filter = "XML file | *.xml";
            popupWindow.ShowDialog();
                       

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
            }
        }


        private void ChangeTool(object sender, RoutedEventArgs e)
        {
            if(e.Source == RoadButton)
            {
                //set tool
                Tool = Tools.Road;

                // set the buttons active and not active
                BuildingButton.IsEnabled = true;
                IntersectionButton.IsEnabled = true;
                CursorButton.IsEnabled = true;
                RoadButton.IsEnabled = false;
            }
            if(e.Source == CursorButton)
            {
                //set tool
                Tool = Tools.Cursor;

                // set the buttons active and not active
                BuildingButton.IsEnabled = true;
                IntersectionButton.IsEnabled = true;
                CursorButton.IsEnabled = false;
                RoadButton.IsEnabled = true;
            }
            if(e.Source == BuildingButton)
            {
                //set tool
                Tool = Tools.Building;

                // set the buttons active and not active
                BuildingButton.IsEnabled = false;
                IntersectionButton.IsEnabled = true;
                CursorButton.IsEnabled = true;
                RoadButton.IsEnabled = true;
            }
            if(e.Source == IntersectionButton)
            {
                //set tool
                Tool = Tools.Intersection;

                // set the buttons active and not active
                BuildingButton.IsEnabled = true;
                IntersectionButton.IsEnabled = false;
                CursorButton.IsEnabled = true;
                RoadButton.IsEnabled = true;
            }
        }
        

        

        private bool leftButtonWasPressed;
        private void MouseMovesOnCanvasEventHandler(object sender, MouseEventArgs e)
        {
            
            // cursor popup Point
            if (!PopupCanvas.IsOpen) { PopupCanvas.IsOpen = true; }    
            var x = (int)e.MouseDevice.GetPosition(Canvas).X;
            var y = (int)e.MouseDevice.GetPosition(Canvas).Y;
            X.Text = x.ToString();
            Y.Text = y.ToString();
            PopupCanvas.HorizontalOffset = (int)e.MouseDevice.GetPosition(this).X;
            PopupCanvas.VerticalOffset = (int)e.MouseDevice.GetPosition(this).Y;
            PopupTextBlock.Text = e.MouseDevice.GetPosition(this).ToString();

            
            //drawing 

            // drawing Ghost Building
            if (Tool == Tools.Building) { Creator.DrawGhostBuilding(sender, e); }

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                leftButtonWasPressed = true;
                // drawing Ghost Road
                if(Tool == Tools.Road) { Creator.DrawGhostRoad(sender, e); }
                
            }
            if (e.LeftButton == MouseButtonState.Released && leftButtonWasPressed == true)
            {
                leftButtonWasPressed = false;
                // drawing Real Road
                if (Tool == Tools.Road) { Creator.CreateRoad(); }
            }
           
        }

        private void MouseClicksOnCanvasEventHandler(object sender, MouseEventArgs e)
        {
            if (Tool == Tools.Building) { Creator.CreateBuilding(); }
        }

        private void MouseLeaveCanvasEventHandler(object sender, MouseEventArgs e)
        {
            //invisible Popup dit doet raar met als je nog wel op het canvas zit voor some reason
            //PopupCanvas.IsOpen = false;

            //remove ghost items when leaving canvas
            Creator.RemoveFakes();
        }

        private void MouseRightCanvasEventHandler(object sender, MouseEventArgs e)
        {
            //Get data of Object
            Creator.SelectRoad = null;
            if (Tool == Tools.Cursor) { Creator.GetObject(); }
        }

        private void MouseEntersCanvasEventHandler(object sneder, MouseEventArgs e)
        {
            //Un-Display information 
            InformationBlockObjects.Visibility = Visibility.Hidden;
            Creator.SelectRoad = null;
        }

        private void RemoveObjectButtonEventHandler(object sender, RoutedEventArgs e)
        {
            //Remove object hide information
            Creator.Roads.Remove(Creator.SelectRoad);
            InformationBlockObjects.Visibility = Visibility.Hidden;
            Creator.SelectRoad = null;
            Creator.RedrawAllObjects();
        }
    }
}
