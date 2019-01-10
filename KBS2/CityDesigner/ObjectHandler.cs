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
        #region Properties & Fields
        
        public Canvas Canvas { get; set; }
        private CityDesignerWindow Window { get; set; }

        // items in the city
        public static List<Road> Roads = new List<Road>();
        public static List<Intersection> Intersections = new List<Intersection>();
        public static List<Building> Buildings = new List<Building>();
        public static List<Garage> Garages = new List<Garage>();

       
        //selected items
        public Road SelectRoad { get; set; }
        public Building SelectBuildingGarage { get; set; }
        
        #endregion

        public ObjectHandler(Canvas canvas, CityDesignerWindow window)
        {
            Canvas = canvas;
            Window = window;

            //subscribe CityLoadedEvent
            CityLoader.SubscribeLoadedCity(LoadedCityToCanvas);
        }
        

        #region EventHandlers
        
        private void LoadedCityToCanvas(object sender, LoadedCityEventArgs e)
        {
            Roads = e.Roads;
            Intersections = e.Intersections;
            Buildings = e.Buildings;
            Garages = e.Garages;
            RedrawAllObjects(Canvas);
        }
        
        #endregion

              

        #region Public Methods
        
        /// <summary>
        /// Gets data of a right-clicked object
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
                            if(mouseX == road.Start.X ||
                               (mouseX <= (road.Start.X + road.Width/2)) &&
                               (mouseX >= (road.Start.X - road.Width/2)))
                            {
                                //display the information about the found road
                                DisplayInfoScreenObject(road);
                                return;
                            }
                        }
                    }

                    if (!(road.End.Y > road.Start.Y)) continue;
                    
                    if (!(mouseY > road.Start.Y) || !(mouseY < road.End.Y)) continue;
                        
                    if (mouseX != road.Start.X &&
                        ((!(mouseX <= (road.Start.X + road.Width / 2))) ||
                         (!(mouseX >= (road.Start.X - road.Width / 2))))) continue;
                            
                    //display the information about the found road
                    DisplayInfoScreenObject(road);
                    return;
                }

                if(road.Start.X > road.End.X)
                {
                    if (mouseX > road.End.X && mouseX < road.Start.X)
                    {
                        if (mouseY == road.Start.Y || (mouseY <= (road.Start.Y + road.Width / 2)) && (mouseY >= (road.Start.Y - road.Width / 2)))
                        {
                            //display the information about the found road
                            DisplayInfoScreenObject(road);
                            return;
                        }
                    }
                }

                if (!(road.End.X > road.Start.X)) continue;
                
                if (!(mouseX < road.End.X) || !(mouseX > road.Start.X)) continue;
                    
                if (mouseY != road.Start.Y && (!(mouseY <= road.Start.Y + road.Width / 2) || !(mouseY >= road.Start.Y - road.Width / 2))) continue;
                        
                //display the information about the found road
                DisplayInfoScreenObject(road);
                return;
            }

            //check if Cursor is on a Building
            foreach (var building in Buildings)
            {
                if (!(mouseX <= building.Location.X + building.Size / 2) ||
                    !(mouseX >= building.Location.X - building.Size / 2) ||
                    !(mouseY <= building.Location.Y + building.Size / 2) ||
                    !(mouseY >= building.Location.Y - building.Size / 2)) continue;
                DisplayInfoScreenObject(building);
                return;
            }

            //check if Cursor is on a Building
            foreach (var garage in Garages)
            {
                if (!(mouseX <= garage.Location.X + garage.Size / 2) ||
                    !(mouseX >= garage.Location.X - garage.Size / 2) ||
                    !(mouseY <= garage.Location.Y + garage.Size / 2) ||
                    !(mouseY >= garage.Location.Y - garage.Size / 2)) continue;
                DisplayInfoScreenObject(garage);
                return;
            }

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
        /// Redraw all objects on <see cref="Canvas"/>
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

            //draw Garages
            foreach (var garage in Garages)
            {
                GarageCreator.DrawGarage(canvas, garage);
            }

            // check for unnessesary intersection if so remove or nessesary
            IntersectionCreator.UpdateIntersections(Roads, Intersections);

            //draw Intersection 
            foreach (var intersection in Intersections)
            {
                IntersectionCreator.DrawIntersection(canvas, intersection);                            
            }
        }

        /// <summary>
        /// Checks if the given <see cref="Building"/> overlaps with any other objects
        /// </summary>
        /// <param name="buildingI"><see cref="Building"/> to check</param>
        /// <returns>returns true when the given <see cref="Building"/> overlaps anything</returns>
        public static bool Overlaps(Building buildingI)
        {
            //check for garages 
            var maxX = buildingI.Location.X + buildingI.Size / 2;
            var minX = buildingI.Location.X - buildingI.Size / 2;
            var maxY = buildingI.Location.Y + buildingI.Size / 2;
            var minY = buildingI.Location.Y - buildingI.Size / 2;

            foreach(var garage in Garages)
            {
                if(buildingI != garage && minX <= garage.Location.X + garage.Size/2 && maxX >= garage.Location.X - garage.Size/2 && minY <= garage.Location.Y + garage.Size / 2 && maxY >= garage.Location.Y - garage.Size / 2)
                {
                    return true;
                } 
            }
            foreach(var building in Buildings)
            {
                if (buildingI != building && minX <= building.Location.X + building.Size / 2 && maxX >= building.Location.X - building.Size / 2 && minY <= building.Location.Y + building.Size / 2 && maxY >= building.Location.Y - building.Size / 2)
                {
                    return true;
                }
            }
            foreach(var road in Roads)
            {
                if(road.IsXRoad())
                {
                    if (minX <= Math.Max(road.Start.X, road.End.X) && maxX >= Math.Min(road.Start.X, road.End.X) && minY <= road.Start.Y + road.Width/2 && maxY >= road.Start.Y - road.Width/2)
                    {
                        return true;
                    } 
                } 
                else
                {
                    if (minY <= Math.Max(road.Start.Y, road.End.Y) && maxY >= Math.Min(road.Start.Y, road.End.Y) && minX <= road.Start.X + road.Width/2 && maxX >= road.Start.X - road.Width/2)
                    {
                        return true;
                    }
                }
                
            }
            return false;
        }

        /// <summary>
        /// Checks if a location <see cref="Vector"/> is within an object
        /// </summary>
        /// <param name="location"><see cref="Vector"/> to check</param>
        /// <returns>True if <see cref="Vector"/> is within an object</returns>
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
                if (!(location.X >= building.Location.X - building.Size / 2) ||
                    !(location.X <= building.Location.X + building.Size / 2)) continue;
                
                if (location.Y >= building.Location.Y - building.Size / 2 && location.Y <= building.Location.Y + building.Size / 2)
                {
                    return true;
                }
            }
            foreach(var garage in Garages)
            {
                if (!(location.X >= garage.Location.X - garage.Size / 2) ||
                    !(location.X <= garage.Location.X + garage.Size / 2)) continue;
                
                if (location.Y >= garage.Location.Y - garage.Size / 2 && location.Y <= garage.Location.Y + garage.Size / 2)
                {
                    return true;
                }
            }
            return false;
        }
        
        #endregion
        
        #region Private Methods
        
        /// <summary>
        /// Displays the information about a specific road given by click right click on a road (function GetObject)
        /// </summary>
        /// <param name="road"></param>
        private void DisplayInfoScreenObject(Road road)
        {
            SelectRoad = road;
            Window.InformationBlockRoad.Visibility = Visibility.Visible;
            Window.InformationBlockBuilding.Visibility = Visibility.Hidden;
            Window.NumericWidthRoad.Value = road.Width;
            Window.NumericMaxSpeedRoad.Value = road.MaxSpeed;
        }

        /// <summary>
        /// Displays the information about a specific building given by click right click on a building (function GetObject)
        /// </summary>
        /// <param name="building"></param>
        private void DisplayInfoScreenObject(Building building)
        {
            SelectBuildingGarage = building;
            Window.InformationBlockBuilding.Visibility = Visibility.Visible;
            Window.InformationBlockRoad.Visibility = Visibility.Hidden;
            Window.NumericSizeBuilding.Value = SelectBuildingGarage.Size; 
            
        }


        /// <summary>
        /// Fix to copy Rectangles
        /// </summary>
        /// <param name="e"><see cref="FrameworkElement"/> to copy from</param>
        /// <returns>Copied <see cref="FrameworkElement"/></returns>
        private FrameworkElement Clone(FrameworkElement e)
        {
            var document = new XmlDocument();
            document.LoadXml(XamlWriter.Save(e));
            return (FrameworkElement)XamlReader.Load(new XmlNodeReader(document));
        }
        
        #endregion

    }
}
