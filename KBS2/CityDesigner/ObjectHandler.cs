using KBS2.CityDesigner.ObjectCreators;
using KBS2.CitySystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml;

namespace KBS2.CityDesigner
{
    public class ObjectHandler
    {
        public Canvas Canvas { get; set; }
        private CityDesignerWindow window { get; set; }

        // items in the city
        public static List<Road> Roads = new List<Road>();
        public static List<Intersection> Intersections = new List<Intersection>();
        public static List<Building> Buildings = new List<Building>();
        public static List<Garage> Garages = new List<Garage>();

       
        //selected items
        public Road SelectRoad { get; set; }
        public Building SelectBuilding { get; set; }


        public ObjectHandler(Canvas canvas, CityDesignerWindow window)
        {
            Canvas = canvas;
            this.window = window;

            //subscribe CityLoadedEvent
            CityLoader.SubscribeLoadedCity(LoadedCityToCanvas);
        }
        
        
        private void LoadedCityToCanvas(object sender, LoadedCityEventArgs e)
        {
            Roads = e.Roads;
            Intersections = e.Intersections;
            Buildings = e.Buildings;
            RedrawAllObjects(Canvas);
        }

              

        /// <summary>
        /// Get data of a Right clicked object
        /// </summary>
        public void GetObject()
        {
            var mouseX = (int)Mouse.GetPosition(Canvas).X;
            var mouseY = (int)Mouse.GetPosition(Canvas).Y;
            //check if Cursor is on Road and check if the location of the cursor is on top of a road
            foreach(var road in Roads)
            {
                if(!road.IsXRoad())
                {
                    if(road.Start.Y > road.End.Y)
                    {
                        if(mouseY < road.Start.Y && mouseY > road.End.Y)
                        {
                            if(mouseX == road.Start.X ||(mouseX <= (road.Start.X + road.Width/2)) && (mouseX >= (road.Start.X - road.Width/2)))
                            {
                                //display the information about the found road
                                displayInfoScreenObject(road);
                                return;
                            }
                        }
                    }
                    if(road.End.Y > road.Start.Y)
                    {
                        if (mouseY > road.Start.Y && mouseY < road.End.Y)
                        {
                            if (mouseX == road.Start.X || (mouseX <= (road.Start.X + road.Width / 2)) && (mouseX >= (road.Start.X - road.Width / 2)))
                            {
                                //display the information about the found road
                                displayInfoScreenObject(road);
                                return;
                            }
                        }
                    }
                }
                else
                {
                    if(road.Start.X > road.End.X)
                    {
                        if (mouseX > road.End.X && mouseX < road.Start.X)
                        {
                            if (mouseY == road.Start.Y || (mouseY <= (road.Start.Y + road.Width / 2)) && (mouseY >= (road.Start.Y - road.Width / 2)))
                            {
                                //display the information about the found road
                                displayInfoScreenObject(road);
                                return;
                            }
                        }
                    }
                    if(road.End.X > road.Start.X)
                    {
                        if (mouseX < road.End.X && mouseX > road.Start.X)
                        {
                            if (mouseY == road.Start.Y || (mouseY <= (road.Start.Y + road.Width / 2)) && (mouseY >= (road.Start.Y - road.Width / 2)))
                            {
                                //display the information about the found road
                                displayInfoScreenObject(road);
                                return;
                            }
                        }
                    }
                }
            }

            //check if Cursor is on a Building
            foreach (var building in Buildings)
            {
                if (mouseX <= building.Location.X + building.Size/2 && mouseX >= building.Location.X - building.Size/2 && mouseY <= building.Location.Y + building.Size/2 && mouseY >= building.Location.Y - building.Size/2)
                {
                    displayInfoScreenObject(building);
                    return;
                }
            }

        }
        
        /// <summary>
        /// Displays the information about a specific road given by click right click on a road (function GetObject)
        /// </summary>
        /// <param name="road"></param>
        private void displayInfoScreenObject(Road road)
        {
            SelectRoad = road;
            window.InformationBlockRoad.Visibility = Visibility.Visible;
            window.InformationBlockBuilding.Visibility = Visibility.Hidden;
            window.NumericWidthRoad.Value = road.Width;
            window.NumericMaxSpeedRoad.Value = road.MaxSpeed;
        }

        /// <summary>
        /// Displays the information about a specific building given by click right click on a building (function GetObject)
        /// </summary>
        /// <param name="building"></param>
        private void displayInfoScreenObject(Building building)
        {
            SelectBuilding = building;
            window.InformationBlockBuilding.Visibility = Visibility.Visible;
            window.InformationBlockRoad.Visibility = Visibility.Hidden;
            window.NumericSizeBuilding.Value = SelectBuilding.Size; 
            
        }


        /// <summary>
        /// Dirty fix to copy rectangles
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private FrameworkElement Clone(FrameworkElement e)
        {
            XmlDocument document = new XmlDocument();
            document.LoadXml(XamlWriter.Save(e));
            return (FrameworkElement)XamlReader.Load(new XmlNodeReader(document));
        }

        /// <summary>
        /// Remove ghosts
        /// </summary>
        public void RemoveGhosts()
        {
            BuildingCreator.RemoveGhost(Canvas);
            RoadCreator.RemoveGhost(Canvas);
            GarageCreator.RemoveGhost(Canvas);
        }

        /// <summary>
        /// Redraw all objects on Canvas
        /// </summary>
        public static void RedrawAllObjects(Canvas canvas)
        {
            //clear canvas
            canvas.Children.Clear();

            //draw roads
            foreach (var road in Roads)
            {
                RoadCreator.DrawRoad(canvas, road);
            }

            //draw Buildings
            foreach (var building in Buildings)
            {
                BuildingCreator.DrawBuilding(canvas, building);
            }

            // check for unnessesary intersection if so remove
            IntersectionCreator.UpdateIntersections(Roads, Intersections);

            //draw Intersection 
            foreach (var intersection in Intersections)
            {
                IntersectionCreator.DrawIntersection(canvas, intersection, Roads, Intersections);                            
            }
        }

        /// <summary>
        /// Location contains object returns true if so
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public static bool LocationContainsObject(Vector location)
        {
            foreach(var road in Roads)
            {
                if(road.IsOnRoad(location))
                {
                    return true;
                }
            }
            foreach(var building in Buildings)
            {
                if(location.X >= building.Location.X - building.Size/2 && location.X <= building.Location.X + building.Size/2)
                {
                    if (location.Y >= building.Location.Y - building.Size / 2 && location.Y <= building.Location.Y + building.Size / 2)
                    {
                        return true;
                    }
                }
            }
            foreach(var garage in Garages)
            {
                if(location.X >= garage.Location.X - garage.Size/2 && location.X <= garage.Location.X + garage.Size/2)
                {
                    if (location.Y >= garage.Location.Y - garage.Size / 2 && location.Y <= garage.Location.Y + garage.Size / 2)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

    }
}
