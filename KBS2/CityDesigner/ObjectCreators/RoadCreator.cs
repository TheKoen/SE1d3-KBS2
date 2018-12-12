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

namespace KBS2.CityDesigner.ObjectCreators
{
    public static class RoadCreator
    {
        private static Line roadGhost = new Line()
        {
            Stroke = Brushes.LightSteelBlue,
            StrokeThickness = standardRoadWidth
        };

        private static Line roadLine = new Line()
        {
            Stroke = Brushes.LightBlue,
            StrokeThickness = standardRoadWidth
        };

        private static Line assistLine = new Line()
        {
            Stroke = Brushes.Black,
            StrokeThickness = 1,
            StrokeDashArray = new DoubleCollection() { 2 }
        };

        private static readonly int standardRoadWidth = 20;
        private static readonly int standardMaxSpeed = 100;
        private static readonly int snapRange = 20;
        private static readonly int minLengthRoad = 50;



        private static Point startRoad = new Point(0, 0);

        /// <summary>
        /// Allows the user to draw a ghostRoad on canvas and controls snapfunctions to other roads
        /// </summary>
        /// <param name="mouse"></param>
        /// <param name="canvas"></param>
        /// <param name="roads"></param>
        public static void DrawGhost(Point mouse, Canvas canvas, List<Road> roads)
        {
            // set the start point of the road
            if (startRoad == new Point(0, 0)) { startRoad = mouse; }

            canvas.Children.Remove(roadGhost);
            canvas.Children.Remove(assistLine);

            // FakeEndPoint
            var FakeEndPoint = mouse;

            roadGhost.X1 = startRoad.X;
            roadGhost.Y1 = startRoad.Y;

            roadGhost.X2 = FakeEndPoint.X;
            roadGhost.Y2 = FakeEndPoint.Y;

            //trying to snap the road to antoher road start point
            foreach (var road in roads)
            {
                //snapping straight
                if (Util.MathUtil.Distance(new Vector(startRoad.X, startRoad.Y), road.End) <= snapRange)
                {
                    roadGhost.X1 = road.End.X;
                    roadGhost.Y1 = road.End.Y;
                    break;
                }
                //snapping straight
                else if (Util.MathUtil.Distance(new Vector(startRoad.X, startRoad.Y), road.Start) <= snapRange)
                {
                    roadGhost.X1 = road.Start.X;
                    roadGhost.Y1 = road.Start.Y;
                    break;
                }
            }

            //trying to snap the road straight 
            if (Math.Abs(roadGhost.X2 - roadGhost.X1) <= snapRange)
            {
                roadGhost.X2 = roadGhost.X1;
            }
            else if (Math.Abs(roadGhost.Y2 - roadGhost.Y1) <= snapRange)
            {
                roadGhost.Y2 = roadGhost.Y1;
            }


            //trying to snap the road to another road end point
            foreach (var road in roads)
            {
                //check if road is in range
                if (Util.MathUtil.Distance(new Vector(roadGhost.X2, roadGhost.Y2), new Vector(road.Start.X, road.Start.Y)) <= snapRange)
                {
                    roadGhost.X2 = road.Start.X;
                    roadGhost.Y2 = road.Start.Y;
                    if (Math.Abs(roadGhost.X2 - roadGhost.X1) < Math.Abs(roadGhost.Y2 - roadGhost.Y1))
                    {
                        roadGhost.X1 = roadGhost.X2;
                    }
                    else if (Math.Abs(roadGhost.X2 - roadGhost.X1) > Math.Abs(roadGhost.Y2 - roadGhost.Y1))
                    {
                        roadGhost.Y1 = roadGhost.Y2;
                    }

                }
                else if (Util.MathUtil.Distance(new Vector(roadGhost.X2, roadGhost.Y2), new Vector(road.End.X, road.End.Y)) <= snapRange)
                {
                    roadGhost.X2 = road.End.X;
                    roadGhost.Y2 = road.End.Y;
                    if (Math.Abs(roadGhost.X2 - roadGhost.X1) < Math.Abs(roadGhost.Y2 - roadGhost.Y1))
                    {
                        roadGhost.X1 = roadGhost.X2;
                    }
                    else if (Math.Abs(roadGhost.X2 - roadGhost.X1) > Math.Abs(roadGhost.Y2 - roadGhost.Y1))
                    {
                        roadGhost.Y1 = roadGhost.Y2;
                    }
                }
            }

            //trying to snap the road to same level as nearby road

            if (roadGhost.X1 == roadGhost.X2 && roadGhost.Y1 != roadGhost.Y2 && Math.Abs(roadGhost.Y2 - roadGhost.Y1) >= minLengthRoad) // Y GhostRoad and GhostRoad is minimumlength
            {
                var snaproadstart = roads.Find(r => Math.Abs(mouse.Y - r.Start.Y) <= snapRange && !r.IsXRoad()); //find first startNode in snapRange and road is not Xroad 
                var snapRoadEnd = roads.Find(r => Math.Abs(mouse.Y - r.End.Y) <= snapRange && !r.IsXRoad()); //find first endNode in snapRange and road is not Xroad

                if (snaproadstart != null)
                {
                    roadGhost.Y2 = (int)snaproadstart.Start.Y;

                    assistLine.X1 = snaproadstart.Start.X;
                    assistLine.Y1 = snaproadstart.Start.Y;
                    assistLine.X2 = roadGhost.X2;
                    assistLine.Y2 = roadGhost.Y2;

                    canvas.Children.Add(assistLine);
                }
                else if (snapRoadEnd != null)
                {
                    roadGhost.Y2 = (int)snapRoadEnd.End.Y;

                    assistLine.X1 = snapRoadEnd.End.X;
                    assistLine.Y1 = snapRoadEnd.End.Y;
                    assistLine.X2 = roadGhost.X2;
                    assistLine.Y2 = roadGhost.Y2;

                    canvas.Children.Add(assistLine);
                }
            }
            else if (roadGhost.Y1 == roadGhost.Y2 && roadGhost.X1 != roadGhost.X2 && Math.Abs(roadGhost.X2 - roadGhost.X1) >= minLengthRoad) // X road and GhostRoad is minimumlength
            {
                var snaproadstart = roads.Find(r => Math.Abs(mouse.X - r.Start.X) <= snapRange && r.IsXRoad());
                var snapRoadEnd = roads.Find(r => Math.Abs(mouse.X - r.End.X) <= snapRange && r.IsXRoad());

                if (snaproadstart != null)
                {
                    roadGhost.X2 = (int)snaproadstart.Start.X;

                    assistLine.X1 = snaproadstart.Start.X;
                    assistLine.Y1 = snaproadstart.Start.Y;
                    assistLine.X2 = roadGhost.X2;
                    assistLine.Y2 = roadGhost.Y2;

                    canvas.Children.Add(assistLine);
                }
                else if (snapRoadEnd != null)
                {
                    roadGhost.X2 = (int)snapRoadEnd.End.X;

                    assistLine.X1 = snapRoadEnd.End.X;
                    assistLine.Y1 = snapRoadEnd.End.Y;
                    assistLine.X2 = roadGhost.X2;
                    assistLine.Y2 = roadGhost.Y2;

                    canvas.Children.Add(assistLine);
                }
            }

            foreach(var road in roads)
            {
                if (!road.IsXRoad() && roadGhost.Y1 == roadGhost.Y2 && Util.MathUtil.Distance(new Vector(roadGhost.X1, roadGhost.Y1), new Vector(road.Start.X, roadGhost.Y1)) <= snapRange) // ghostRoad X road
                {
                    roadGhost.X1 = road.Start.X;
                    break;
                }
                else if (road.IsXRoad() && roadGhost.X1 == roadGhost.X2 && Util.MathUtil.Distance(new Vector(roadGhost.X1, roadGhost.Y1), new Vector(roadGhost.X1, road.Start.Y)) <= snapRange) // ghostRoad Y road
                {
                    roadGhost.Y1 = road.Start.Y;
                    break;
                }
            }


            roadGhost.StrokeThickness = standardRoadWidth;
            canvas.Children.Add(roadGhost);
        }


        /// <summary>
        /// Creates a road and adds this to roadsList
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="roadsList"></param>
        /// <returns></returns>
        public static Road CreateRoad(Canvas canvas, List<Road> roadsList, List<Building> buildingsList, List<Garage> garagesList)
        {
            startRoad = new Point(0, 0);

            Road road = new Road(new Vector((int)roadGhost.X1, (int)roadGhost.Y1), new Vector((int)roadGhost.X2, (int)roadGhost.Y2), standardRoadWidth, standardMaxSpeed);

            //check if road is long enough and road 
            if (Util.MathUtil.Distance(new Vector(roadGhost.X1, roadGhost.Y1), new Vector(roadGhost.X2, roadGhost.Y2)) < minLengthRoad)
            {
                RemoveGhost(canvas);
                return null;
            }

            //check if road is not vertical or horizontal
            if (!(roadGhost.X1 == roadGhost.X2 || roadGhost.Y1 == roadGhost.Y2))
            {
                RemoveGhost(canvas);
                return null;
            }
            // check for coliding with buildings or garages
            if (crossesBuildingOrGarage(buildingsList, garagesList, canvas))
            {
                RemoveGhost(canvas);
                return null;
            };

            var roadGhostStart = new Vector(roadGhost.X1, roadGhost.Y2);
            var roadGhostEnd = new Vector(roadGhost.X2, roadGhost.Y2);

            //check if road collides with other roads
            foreach (var roadI in roadsList)
            {
                if (roadI.IsXRoad() && roadGhost.Y1 == roadGhost.Y2 && roadGhost.Y1 + roadGhost.StrokeThickness/2 >= roadI.Start.Y - roadI.Width/2 && roadGhost.Y1 - roadGhost.StrokeThickness/2 <= roadI.Start.Y + roadI.Width/2) // check if road overlaps
                { 
                    if((Math.Min(roadGhost.X1, roadGhost.X2) < Math.Min(roadI.Start.X, roadI.End.X) && Math.Max(roadGhost.X1, roadGhost.X2) > Math.Min(roadI.Start.X, roadI.End.X)) || (Math.Min(roadGhost.X1, roadGhost.X2) < Math.Max(roadI.Start.X, roadI.End.X) && Math.Max(roadGhost.X1, roadGhost.X2) > Math.Max(roadI.Start.X, roadI.End.X)))
                    {
                        if ((roadGhostStart != roadI.Start && roadGhostStart != roadI.End && roadGhostEnd != roadI.Start && roadGhostEnd != roadI.End) || road.IsXRoad())
                        {
                            RemoveGhost(canvas);
                            return null;
                        }
                    }
                }
                if(!roadI.IsXRoad() && roadGhost.X1 == roadGhost.X2 && roadGhost.X1 + roadGhost.StrokeThickness/2 >= roadI.Start.X - roadI.Width/2 && roadGhost.X1 - roadGhost.StrokeThickness/2 <= roadI.Start.X + roadI.Width/2) // chekf if road overlaps
                {
                    if((Math.Min(roadGhost.Y1, roadGhost.Y2) < Math.Min(roadI.Start.Y, roadI.End.Y) && Math.Max(roadGhost.Y1, roadGhost.Y2) > Math.Min(roadI.Start.Y, roadI.End.Y)) || (Math.Min(roadGhost.Y1, roadGhost.Y2) < Math.Max(roadI.Start.Y, roadI.End.Y) && Math.Max(roadGhost.Y1, roadGhost.Y2) > Math.Max(roadI.Start.Y, roadI.End.Y)))
                    {
                        if ((roadGhostStart != roadI.Start && roadGhostStart != roadI.End && roadGhostEnd != roadI.Start && roadGhostEnd != roadI.End) || !road.IsXRoad())
                        {
                            RemoveGhost(canvas);
                            return null;
                        }
                    }
                }
                            
            }
            if (CreateMultipleRoadsWhenCrossingEachother(road, roadsList, canvas) == true) // check if road is crossed by ghost road
            {
                RemoveGhost(canvas);
                return null;
            }


            
            DrawRoad(canvas, road);

            RemoveGhost(canvas);
            roadsList.Add(road);
            IntersectionCreator.CreateIntersection(canvas, ObjectHandler.Roads, ObjectHandler.Intersections, road.Start);
            IntersectionCreator.CreateIntersection(canvas, ObjectHandler.Roads, ObjectHandler.Intersections, road.End);
            return road;
        }

        private static bool crossesBuildingOrGarage(List<Building> buildingsList, List<Garage> garagesList, Canvas canvas)
        {
            //Check if road crosses building
            foreach (var building in buildingsList)
            {
                if(roadGhost.Y1 == roadGhost.Y2) // X road
                {
                    if((roadGhost.Y1 - standardRoadWidth/2 <= building.Location.Y + building.Size/2 && roadGhost.Y1 + standardRoadWidth/2 >= building.Location.Y - building.Size/2)) // check if is on same Y level
                    {
                        if(Math.Max(roadGhost.X1, roadGhost.X2) >= building.Location.X - building.Size/2 && Math.Min(roadGhost.X1, roadGhost.X2) <= building.Location.X + building.Size/2)// check if if X is on same level
                        {
                            RemoveGhost(canvas);
                            return true;
                        }
                    }
                }
                else if(roadGhost.X1 == roadGhost.X2) // Y road
                {
                    if(roadGhost.X1 - standardRoadWidth/2 <= building.Location.X + building.Size / 2 && roadGhost.X1 + standardRoadWidth/2 >= building.Location.X - building.Size/2) // check if is on same Y level
                    {
                        if (Math.Max(roadGhost.Y1, roadGhost.Y2) >= building.Location.Y - building.Size / 2 && Math.Min(roadGhost.Y1, roadGhost.Y2) <= building.Location.Y + building.Size / 2)// check if if X is on same level
                        {
                            RemoveGhost(canvas);
                            return true;
                        }
                    }
                }
            }

            //Check if road crosses Garage
            foreach (var garage in garagesList)
            {
                if (roadGhost.Y1 == roadGhost.Y2) // X road
                {
                    if ((roadGhost.Y1 - standardRoadWidth / 2 <= garage.Location.Y + garage.Size / 2 && roadGhost.Y1 + standardRoadWidth / 2 >= garage.Location.Y - garage.Size / 2)) // check if is on same Y level
                    {
                        if (Math.Max(roadGhost.X1, roadGhost.X2) >= garage.Location.X - garage.Size / 2 && Math.Min(roadGhost.X1, roadGhost.X2) <= garage.Location.X + garage.Size / 2)// check if if X is on same level
                        {
                            RemoveGhost(canvas);
                            return true;
                        }
                    }
                }
                else if (roadGhost.X1 == roadGhost.X1) // Y road
                {
                    if ((roadGhost.X1 - standardRoadWidth / 2 <= garage.Location.X + garage.Size / 2 && roadGhost.X1 + standardRoadWidth / 2 >= garage.Location.X - garage.Size / 2)) // check if is on same Y level
                    {
                        if (Math.Max(roadGhost.Y1, roadGhost.Y2) >= garage.Location.Y - garage.Size / 2 && Math.Min(roadGhost.Y1, roadGhost.Y2) <= garage.Location.Y + garage.Size / 2)// check if if X is on same level
                        {
                            RemoveGhost(canvas);
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        
        private static bool CreateMultipleRoadsWhenCrossingEachother(Road road1, List<Road> roadsList, Canvas canvas)
        {
            bool returnBool = false;
            List<Road> removeRoads = new List<Road>();
            List<Road> addRoads = new List<Road>();

            var crossingRoads = roadsList.FindAll(r => // find all roads that are crossed and order from Low to high
            {
                if (road1.IsXRoad() && !r.IsXRoad() && (road1.End != r.End && road1.End != r.Start && road1.Start != r.End && road1.Start != r.Start))
                {
                    if(Math.Max(r.Start.Y, r.End.Y) >= road1.Start.Y && Math.Min(r.Start.Y, r.End.Y) <= road1.Start.Y && Math.Max(road1.Start.X, road1.End.X) >= r.Start.X && Math.Min(road1.Start.X, road1.End.X) <= r.Start.X )
                    {
                        return true;
                    }
                }
                else if (!road1.IsXRoad() && r.IsXRoad() && (road1.End != r.End && road1.End != r.Start && road1.Start != r.End && road1.Start != r.Start))
                {
                    if (Math.Max(r.Start.X, r.End.X) >= road1.Start.X && Math.Min(r.Start.X, r.End.X) <= road1.Start.X && Math.Max(road1.Start.Y, road1.End.Y) >= r.Start.Y && Math.Min(road1.Start.Y, road1.End.Y) <= r.Start.Y)
                    {
                        return true;
                    }
                }
                return false;
            }).OrderBy(r =>
            {
                if (r.IsXRoad())
                {
                    return r.Start.Y;
                }
                else
                {
                    return r.Start.X;
                }
            }).ToList();

            if(crossingRoads.Count() == 0)
            {
                return returnBool;
            }

            if (road1.IsXRoad())
            {
                var location = (Math.Min(road1.Start.X, road1.End.X) == road1.Start.X) ? road1.Start : road1.End; // determine whats start location
                for (int i = 0; i < crossingRoads.Count + 1; i++)
                {
                    if(i < crossingRoads.Count && crossingRoads[i].Start.X == location.X && crossingRoads.Last() != crossingRoads[i]) // if there is not a piece of road behind the first cross
                    {
                        var roadCrossNew1 = new Road(crossingRoads[i].Start, location, crossingRoads[i].Width, crossingRoads[i].MaxSpeed);
                        var roadCrossNew2 = new Road(location, crossingRoads[i].End, crossingRoads[i].Width, crossingRoads[i].MaxSpeed);
                        roadsList.Remove(crossingRoads[i]);
                        roadsList.Add(roadCrossNew1);
                        roadsList.Add(roadCrossNew2);
                        returnBool = true;
                    }
                    else if(i < crossingRoads.Count && location.X < crossingRoads[i].Start.X)
                    {
                        var beginNewRoad = location;
                        location = new Vector(crossingRoads[i].Start.X, location.Y);
                        var roadNew = new Road(beginNewRoad, location, standardRoadWidth, standardMaxSpeed);
                        var roadCrossNew1 = new Road(crossingRoads[i].Start, location, crossingRoads[i].Width, crossingRoads[i].MaxSpeed);
                        var roadCrossNew2 = new Road(location, crossingRoads[i].End, crossingRoads[i].Width, crossingRoads[i].MaxSpeed);
                        roadsList.Remove(crossingRoads[i]);
                        roadsList.Add(roadCrossNew1);
                        roadsList.Add(roadCrossNew2);
                        roadsList.Add(roadNew);
                        returnBool = true;
                    }
                    else if(location.X == crossingRoads.Last().Start.X) // last part of road
                    {
                        var beginNewRoad = location;
                        var roadNew = (Math.Max(road1.Start.X, road1.End.X) == road1.End.X) ? new Road(beginNewRoad, road1.End, standardRoadWidth, standardMaxSpeed) : new Road(beginNewRoad, road1.Start, standardRoadWidth, standardMaxSpeed);
                        roadsList.Add(roadNew);
                        returnBool = true;
                    }
                    else if(location == road1.End || location == road1.Start) // no last part of road
                    {
                    }
                    else
                    {
                        throw new Exception("Error creating Multiple Roads When Crossing Eachother.");
                    }
                }
            }
            else if(!road1.IsXRoad())
            {
                var location = (Math.Min(road1.Start.Y, road1.End.Y) == road1.Start.Y) ? road1.Start : road1.End; // determine whats start location
                for (int i = 0; i < crossingRoads.Count + 1; i++)
                {
                    if(i < crossingRoads.Count && crossingRoads[i].Start.Y == location.Y && crossingRoads.Last() != crossingRoads[i]) // if there is not a piece of road behind the first cross
                    {
                        var roadCrossNew1 = new Road(crossingRoads[i].Start, location, crossingRoads[i].Width, crossingRoads[i].MaxSpeed);
                        var roadCrossNew2 = new Road(location, crossingRoads[i].End, crossingRoads[i].Width, crossingRoads[i].MaxSpeed);
                        roadsList.Remove(crossingRoads[i]);
                        roadsList.Add(roadCrossNew1);
                        roadsList.Add(roadCrossNew2);
                        returnBool = true;
                    }
                    else if(i < crossingRoads.Count && location.Y < crossingRoads[i].Start.Y)
                    {
                        var beginNewRoad = location;
                        location = new Vector(location.X, crossingRoads[i].Start.Y);
                        var roadNew = new Road(beginNewRoad, location, standardRoadWidth, standardMaxSpeed);
                        var roadCrossNew1 = new Road(crossingRoads[i].Start, location, crossingRoads[i].Width, crossingRoads[i].MaxSpeed);
                        var roadCrossNew2 = new Road(location, crossingRoads[i].End, crossingRoads[i].Width, crossingRoads[i].MaxSpeed);
                        roadsList.Remove(crossingRoads[i]);
                        roadsList.Add(roadCrossNew1);
                        roadsList.Add(roadCrossNew2);
                        roadsList.Add(roadNew);
                        returnBool = true;
                    }
                    else if(location.Y == crossingRoads.Last().Start.Y) // last part of road
                    {
                        var beginNewRoad = location;
                        var roadNew = (Math.Max(road1.Start.Y, road1.End.Y) == road1.End.Y) ? new Road(beginNewRoad, road1.End, standardRoadWidth, standardMaxSpeed) : new Road(beginNewRoad, road1.Start, standardRoadWidth, standardMaxSpeed);
                        roadsList.Add(roadNew);
                        returnBool = true;
                    }
                    else if(location == road1.End || location == road1.Start) // no last part of road
                    {
                    }
                    else
                    {
                        throw new Exception("Error creating Multiple Roads When Crossing Eachother.");
                    }
                }
            }
            else
            {
                throw new Exception("road you want to place is not horizontal or vertical.");
            }
            
            ObjectHandler.RedrawAllObjects(canvas);
            return returnBool;
        }

        /// <summary>
        /// Removes the ghost road
        /// </summary>
        /// <param name="canvas"></param>
        public static void RemoveGhost(Canvas canvas)
        {
            canvas.Children.Remove(roadGhost);
        }

        

        public static void DrawRoad(Canvas canvas, Road road)
        {
            
            roadLine.X1 = (int)road.Start.X;
            roadLine.Y1 = (int)road.Start.Y;
            roadLine.X2 = (int)road.End.X;
            roadLine.Y2 = (int)road.End.Y;

            Canvas.SetZIndex(roadLine, 2);
            roadLine.StrokeThickness = road.Width;

            
            canvas.Children.Add(clone(roadLine));
        }

        /// <summary>
        /// Dirty fix to copy Roads
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private static FrameworkElement clone(FrameworkElement e)
        {
            XmlDocument document = new XmlDocument();
            document.LoadXml(XamlWriter.Save(e));
            return (FrameworkElement)XamlReader.Load(new XmlNodeReader(document));
        }

    }
}
