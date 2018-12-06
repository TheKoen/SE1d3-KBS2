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

        //values for the roads
        private Point startRoad;
        private Line fakeRoad = new Line()
        {
            Stroke = Brushes.LightSteelBlue,
            StrokeThickness = 20
        };

        //values for the buildings
        private Rectangle fakeBuilding = new Rectangle()
        {
            Fill = Brushes.LightSteelBlue,
            StrokeThickness = 4,
            Stroke = Brushes.Black,
            Width = 50,
            Height = 50,
        };
        private Rectangle realBuilding = new Rectangle()
        {
            Fill = Brushes.Blue,
            StrokeThickness = 4,
            Stroke = Brushes.Black,
            Width = 50,
            Height = 50,
        };

        //values for the intersections
        private Rectangle realIntersection = new Rectangle()
        {
            Fill = Brushes.Red,
            StrokeThickness = 4,
            Stroke = Brushes.Black,

        };

        //values for assistLine
        private Line assistLine = new Line()
        {
            Stroke = Brushes.Black,
            StrokeThickness = 1,
            StrokeDashArray = new DoubleCollection() { 2 }
        };

        // items in the city
        public List<Road> Roads = new List<Road>();
        public List<Intersection> Intersections = new List<Intersection>();
        public List<Building> Buildings = new List<Building>();

        // snaprange 
        private int snapRange = 20;
        private int minLengthRoad = 50; 

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
            RedrawAllObjects();
        }

        
        public void DrawGhostRoad(object sender, MouseEventArgs e)
        {
            int mouseX = (int)Mouse.GetPosition(Canvas).X;
            int mouseY = (int)Mouse.GetPosition(Canvas).Y;
            
            // set the start point of the road
            if (startRoad == new Point(0, 0)) { startRoad = Mouse.GetPosition(Canvas); }

            Canvas.Children.Remove(fakeRoad);
            Canvas.Children.Remove(assistLine);

            // FakeEndPoint
            var FakeEndPoint = Mouse.GetPosition(Canvas);

            fakeRoad.X1 = startRoad.X;
            fakeRoad.Y1 = startRoad.Y;

            fakeRoad.X2 = FakeEndPoint.X;
            fakeRoad.Y2 = FakeEndPoint.Y;
            
            //trying to snap the road to antoher road start point
            foreach (var road in Roads)
            {
                if (Util.MathUtil.Distance(new Vector(startRoad.X, startRoad.Y), road.End) <= snapRange)
                {
                    fakeRoad.X1 = road.End.X;
                    fakeRoad.Y1 = road.End.Y;
                    
                    //snapping straight


                }
                else if (Util.MathUtil.Distance(new Vector(startRoad.X, startRoad.Y), road.Start) <= snapRange)
                {
                    fakeRoad.X1 = road.Start.X;
                    fakeRoad.Y1 = road.Start.Y;
                }
            }

            //trying to snap the road straight 
            if (Math.Abs(fakeRoad.X2 - fakeRoad.X1) <= snapRange)
            {
                fakeRoad.X2 = fakeRoad.X1;
            }
            else if (Math.Abs(fakeRoad.Y2 - fakeRoad.Y1) <= snapRange)
            {
                fakeRoad.Y2 = fakeRoad.Y1;
            }

            
            //trying to snap the road to another road end point
            foreach (var road in Roads)
            {
                //check if road is in range
                if (Util.MathUtil.Distance(new Vector(fakeRoad.X2, fakeRoad.Y2), new Vector(road.Start.X, road.Start.Y)) <= snapRange)
                {
                    fakeRoad.X2 = road.Start.X;
                    fakeRoad.Y2 = road.Start.Y;
                    if (Math.Abs(fakeRoad.X2 - fakeRoad.X1) < Math.Abs(fakeRoad.Y2 - fakeRoad.Y1))
                    {
                        fakeRoad.X1 = fakeRoad.X2;
                    }
                    else if (Math.Abs(fakeRoad.X2 - fakeRoad.X1) > Math.Abs(fakeRoad.Y2 - fakeRoad.Y1))
                    {
                        fakeRoad.Y1 = fakeRoad.Y2;
                    }

                }
                else if (Util.MathUtil.Distance(new Vector(fakeRoad.X2, fakeRoad.Y2), new Vector(road.End.X, road.End.Y)) <= snapRange)
                {
                    fakeRoad.X2 = road.End.X;
                    fakeRoad.Y2 = road.End.Y;
                    if (Math.Abs(fakeRoad.X2 - fakeRoad.X1) < Math.Abs(fakeRoad.Y2 - fakeRoad.Y1))
                    {
                        fakeRoad.X1 = fakeRoad.X2;
                    }
                    else if (Math.Abs(fakeRoad.X2 - fakeRoad.X1) > Math.Abs(fakeRoad.Y2 - fakeRoad.Y1))
                    {
                        fakeRoad.Y1 = fakeRoad.Y2;
                    }
                }
            }

            //trying to snap the road to same level as nearby road (not done yet

            if (fakeRoad.X1 == fakeRoad.X2 && fakeRoad.Y1 != fakeRoad.Y2 && Math.Abs(fakeRoad.Y2 - fakeRoad.Y1) >= minLengthRoad) // Y GhostRoad and GhostRoad is minimumlength
            {
                var snapRoadStart = Roads.Find(r => Math.Abs(mouseY - r.Start.Y) <= snapRange && !r.IsXRoad()); //find first startNode in snapRange and road is not Xroad 
                var snapRoadEnd = Roads.Find(r => Math.Abs(mouseY - r.End.Y) <= snapRange && !r.IsXRoad()); //find first endNode in snapRange and road is not Xroad
                
                if (snapRoadStart != null) 
                {
                    fakeRoad.Y2 = (int)snapRoadStart.Start.Y;

                    assistLine.X1 = snapRoadStart.Start.X;
                    assistLine.Y1 = snapRoadStart.Start.Y;
                    assistLine.X2 = fakeRoad.X2;
                    assistLine.Y2 = fakeRoad.Y2;

                    Canvas.Children.Add(assistLine);
                }
                else if (snapRoadEnd != null)
                {
                    fakeRoad.Y2 = (int)snapRoadEnd.End.Y;

                    assistLine.X1 = snapRoadEnd.End.X;
                    assistLine.Y1 = snapRoadEnd.End.Y;
                    assistLine.X2 = fakeRoad.X2;
                    assistLine.Y2 = fakeRoad.Y2;

                    Canvas.Children.Add(assistLine);
                }
            }
            else if (fakeRoad.Y1 == fakeRoad.Y2 && fakeRoad.X1 != fakeRoad.X2 && Math.Abs(fakeRoad.X2 - fakeRoad.X1) >= minLengthRoad) // X road and GhostRoad is minimumlength
            {
                var snapRoadStart = Roads.Find(r => Math.Abs(mouseX - r.Start.X) <= snapRange && r.IsXRoad());
                var snapRoadEnd = Roads.Find(r => Math.Abs(mouseX - r.End.X) <= snapRange && r.IsXRoad());
                
                if (snapRoadStart != null)
                {
                    fakeRoad.X2 = (int)snapRoadStart.Start.X;

                    assistLine.X1 = snapRoadStart.Start.X;
                    assistLine.Y1 = snapRoadStart.Start.Y;
                    assistLine.X2 = fakeRoad.X2;
                    assistLine.Y2 = fakeRoad.Y2;

                    Canvas.Children.Add(assistLine);
                }
                else if (snapRoadEnd != null)
                {
                    fakeRoad.X2 = (int)snapRoadEnd.End.X;

                    assistLine.X1 = snapRoadEnd.End.X;
                    assistLine.Y1 = snapRoadEnd.End.Y;
                    assistLine.X2 = fakeRoad.X2;
                    assistLine.Y2 = fakeRoad.Y2;

                    Canvas.Children.Add(assistLine);
                }
            }

            Canvas.Children.Add(fakeRoad);
        }

        public void CreateRoad()
        {
            startRoad = new Point(0, 0);

            //check if road is long enough
            if(Util.MathUtil.Distance(new Vector(fakeRoad.X1, fakeRoad.Y1), new Vector(fakeRoad.X2, fakeRoad.Y2)) < minLengthRoad)
            {
                RemoveGhosts();
                return;
            }

            //check if road is vertical or horizontal
            if (fakeRoad.X1 == fakeRoad.X2 || fakeRoad.Y1 == fakeRoad.Y2)
            {
                
                // add road to roadsList
                Roads.Add(new Road(new Vector((int)fakeRoad.X1, (int)fakeRoad.Y1), new Vector((int)fakeRoad.X2, (int)fakeRoad.Y2), 20, 100));

                // create real road on canvas
                RedrawAllObjects();
            }
            Canvas.Children.Remove(fakeRoad);

            CreateIntersection((int)fakeRoad.X1, (int)fakeRoad.Y1, (int)fakeRoad.X2, (int)fakeRoad.Y2);
        }

        /// <summary>
        /// Checks if intersection is needed creates it
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        private void CreateIntersection(int x1, int y1, int x2, int y2)
        {
            // search for Intersection add when not excists and more then one Road is connected
            if (Intersections.Find(i => i.Location == new Vector(x1, y1)) == null && Roads.FindAll(r => r.Start == new Vector(x1, y1) || r.End == new Vector(x1, y1)).Count >= 2)
            {
                // get the MaxSize of the road connected to the Intersection
                var roadMaxSize = Roads.FindAll(r => r.Start == new Vector(x1, y1) || r.End == new Vector(x1, y1)).Max(r => r.Width);
                // add To list if not excist at start of new road
                Intersections.Add(new Intersection(new Vector(x1, y1), roadMaxSize));

                // add the intersection to canvas
                RedrawAllObjects();
            }
            // search for Intersection add when not excists and more then one Road is connected
            if (Intersections.Find(i => i.Location == new Vector(x2, y2)) == null && Roads.FindAll(r => r.Start == new Vector(x2, y2) || r.End == new Vector(x2, y2)).Count >= 2)
            {
                // get the MaxSize of the road connected to the Intersection
                var roadMaxSize = Roads.FindAll(r => r.Start == new Vector(x2, y2) || r.End == new Vector(x2, y2)).Max(r => r.Width);
                // add To list if not excist at end of new road
                Intersections.Add(new Intersection(new Vector(x2, y2), roadMaxSize));
                
                // add the intersection to canvas
                RedrawAllObjects();
            }

        }

        /// <summary>
        /// Creates a GhostBuilding
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void DrawGhostBuilding(object sender, MouseEventArgs e)
        {
            Canvas.Children.Remove(fakeBuilding);

            Canvas.SetTop(fakeBuilding, Mouse.GetPosition(Canvas).Y - fakeBuilding.Width / 2);
            Canvas.SetLeft(fakeBuilding, Mouse.GetPosition(Canvas).X - fakeBuilding.Height / 2);

            Canvas.Children.Add(fakeBuilding);
        }

        /// <summary>
        /// Creates a building out of a GhostBuilding
        /// </summary>
        public void CreateBuilding()
        {
            // add building to BuildingList
            Buildings.Add(new Building(new Vector((int)Mouse.GetPosition(Canvas).X, (int)Mouse.GetPosition(Canvas).Y), 50));

            // create Real building on Canvas out of a copy 
            var copyBuilding = (Rectangle)Clone(realBuilding);

            Canvas.SetTop(copyBuilding, (int)Mouse.GetPosition(Canvas).Y - realBuilding.Height / 2);
            Canvas.SetLeft(copyBuilding, (int)Mouse.GetPosition(Canvas).X - realBuilding.Width / 2);

            RedrawAllObjects();

            Canvas.Children.Remove(fakeBuilding);
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
            window.NumericWidthRoad.Value = road.Width;
        }

        /// <summary>
        /// Displays the information about a specific building given by click right click on a building (function GetObject)
        /// </summary>
        /// <param name="building"></param>
        private void displayInfoScreenObject(Building building)
        {
            SelectBuilding = building;
            window.InformationBlockBuilding.Visibility = Visibility.Visible;
            window.NumericSizeBuilding.Value = SelectBuilding.Size; //selected building loses size ?
            
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
            Canvas.Children.Remove(fakeBuilding);
            Canvas.Children.Remove(fakeRoad);

        }

        /// <summary>
        /// Redraw all objects on Canvas
        /// </summary>
        public void RedrawAllObjects()
        {
            //clear canvas
            Canvas.Children.Clear();

            //draw roads
            foreach (var road in Roads)
            {
                var l = new Line();
                l.X1 = (int)road.Start.X;
                l.Y1 = (int)road.Start.Y;
                l.X2 = (int)road.End.X;
                l.Y2 = (int)road.End.Y;

                l.Stroke = Brushes.LightBlue;
                l.StrokeThickness = road.Width;

                Canvas.Children.Add(l);
            }

            //draw Buildings
            foreach (var building in Buildings)
            {
                var copyBuilding = (Rectangle)Clone(realBuilding);
                copyBuilding.Height = building.Size;
                copyBuilding.Width = building.Size;
                Canvas.SetTop(copyBuilding, (int)building.Location.Y - building.Size / 2);
                Canvas.SetLeft(copyBuilding, (int)building.Location.X - building.Size / 2);


                Canvas.Children.Add(copyBuilding);

                Canvas.Children.Remove(fakeBuilding);
            }

            var stupidIntersectionWihoutRoads = new List<Intersection>();

            //draw Intersection 
            foreach (var intersection in Intersections)
            {
                if (Roads.FindAll(r => r.End == intersection.Location || r.Start == intersection.Location).Count > 1)
                {
                    var copyIntersection = (Rectangle)Clone(realIntersection);
                    copyIntersection.Width = intersection.Size;
                    copyIntersection.Height = intersection.Size;
                    Canvas.SetTop(copyIntersection, (int)intersection.Location.Y - intersection.Size / 2);
                    Canvas.SetLeft(copyIntersection, (int)intersection.Location.X - intersection.Size / 2);
                    Canvas.SetZIndex(copyIntersection, 2);

                    Canvas.Children.Add(copyIntersection);
                }
                else
                {
                    stupidIntersectionWihoutRoads.Add(intersection);
                }                
            }
            //delete the intersection without road connected
            foreach(var intersection in stupidIntersectionWihoutRoads)
            {
                Intersections.Remove(intersection);
            }
        }

    }
}
