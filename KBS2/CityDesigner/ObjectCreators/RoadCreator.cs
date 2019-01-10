using KBS2.CitySystem;
using KBS2.Visual.Controls;
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
        private const int StandardRoadWidth = 20;
        private const int StandardMaxSpeed = 100;
        private const int SnapRange = 50;
        private const int MinLengthRoad = 50;
        
        #region Properties & Fields

        private static readonly Line RoadGhost = new Line()
        {
            Stroke = Brushes.LightSteelBlue,
            StrokeThickness = StandardRoadWidth
        };

        private static readonly Line RoadLine = new Line()
        {
            Stroke = Brushes.LightBlue,
            StrokeThickness = StandardRoadWidth
        };

        private static readonly Line AssistLine = new Line()
        {
            Stroke = Brushes.Black,
            StrokeThickness = 1,
            StrokeDashArray = new DoubleCollection() { 2 }
        };

        private static Point _startRoad = new Point(0, 0);

        #endregion

        #region Public Methods

        /// <summary>
        /// Draws a ghost <see cref="Road"/> on a <see cref="Canvas"/> and controls snapfunctions to other <see cref="Road"/>s
        /// </summary>
        /// <param name="mouse"><see cref="Point"/> of mouse</param>
        /// <param name="canvas"><see cref="Canvas"/> to work with</param>
        /// <param name="roads"><see cref="Road"/>s to snap to</param>
        public static void DrawGhost(Point mouse, Canvas canvas, List<Road> roads)
        {
            // set the start point of the road
            if (_startRoad == new Point(0, 0))
                _startRoad = mouse;

            canvas.Children.Remove(RoadGhost);
            canvas.Children.Remove(AssistLine);

            // FakeEndPoint
            var fakeEndPoint = mouse;

            RoadGhost.X1 = _startRoad.X;
            RoadGhost.Y1 = _startRoad.Y;

            RoadGhost.X2 = fakeEndPoint.X;
            RoadGhost.Y2 = fakeEndPoint.Y;

            // trying to snap the road to antoher road start point
            foreach (var road in roads)
            {
                // snapping straight
                if (Util.MathUtil.Distance(new Vector(_startRoad.X, _startRoad.Y), road.End) <= SnapRange)
                {
                    RoadGhost.X1 = road.End.X;
                    RoadGhost.Y1 = road.End.Y;
                    break;
                }
                // snapping straight
                else if (Util.MathUtil.Distance(new Vector(_startRoad.X, _startRoad.Y), road.Start) <= SnapRange)
                {
                    RoadGhost.X1 = road.Start.X;
                    RoadGhost.Y1 = road.Start.Y;
                    break;
                }
            }

            // trying to snap the road straight 
            if (Math.Abs(RoadGhost.X2 - RoadGhost.X1) <= SnapRange)
            {
                RoadGhost.X2 = RoadGhost.X1;
            }
            else if (Math.Abs(RoadGhost.Y2 - RoadGhost.Y1) <= SnapRange)
            {
                RoadGhost.Y2 = RoadGhost.Y1;
            }


            // trying to snap the road to another road end point
            foreach (var road in roads)
            {
                // check if road is in range
                if (Util.MathUtil.Distance(new Vector(RoadGhost.X2, RoadGhost.Y2), new Vector(road.Start.X, road.Start.Y)) <= SnapRange)
                {
                    RoadGhost.X2 = road.Start.X;
                    RoadGhost.Y2 = road.Start.Y;
                    if (Math.Abs(RoadGhost.X2 - RoadGhost.X1) < Math.Abs(RoadGhost.Y2 - RoadGhost.Y1))
                    {
                        RoadGhost.X1 = RoadGhost.X2;
                    }
                    else if (Math.Abs(RoadGhost.X2 - RoadGhost.X1) > Math.Abs(RoadGhost.Y2 - RoadGhost.Y1))
                    {
                        RoadGhost.Y1 = RoadGhost.Y2;
                    }

                }
                else if (Util.MathUtil.Distance(new Vector(RoadGhost.X2, RoadGhost.Y2), new Vector(road.End.X, road.End.Y)) <= SnapRange)
                {
                    RoadGhost.X2 = road.End.X;
                    RoadGhost.Y2 = road.End.Y;
                    if (Math.Abs(RoadGhost.X2 - RoadGhost.X1) < Math.Abs(RoadGhost.Y2 - RoadGhost.Y1))
                    {
                        RoadGhost.X1 = RoadGhost.X2;
                    }
                    else if (Math.Abs(RoadGhost.X2 - RoadGhost.X1) > Math.Abs(RoadGhost.Y2 - RoadGhost.Y1))
                    {
                        RoadGhost.Y1 = RoadGhost.Y2;
                    }
                }
            }

            // trying to snap the road to same level as nearby road

            if (RoadGhost.X1 == RoadGhost.X2 && RoadGhost.Y1 != RoadGhost.Y2 && Math.Abs(RoadGhost.Y2 - RoadGhost.Y1) >= MinLengthRoad) // Y GhostRoad and GhostRoad is minimumlength
            {
                var snaproadstart = roads.Find(r => Math.Abs(mouse.Y - r.Start.Y) <= SnapRange && !r.IsXRoad()); // find first startNode in snapRange and road is not Xroad 
                var snapRoadEnd = roads.Find(r => Math.Abs(mouse.Y - r.End.Y) <= SnapRange && !r.IsXRoad()); // find first endNode in snapRange and road is not Xroad

                if (snaproadstart != null)
                {
                    RoadGhost.Y2 = (int)snaproadstart.Start.Y;

                    AssistLine.X1 = snaproadstart.Start.X;
                    AssistLine.Y1 = snaproadstart.Start.Y;
                    AssistLine.X2 = RoadGhost.X2;
                    AssistLine.Y2 = RoadGhost.Y2;

                    canvas.Children.Add(AssistLine);
                }
                else if (snapRoadEnd != null)
                {
                    RoadGhost.Y2 = (int)snapRoadEnd.End.Y;

                    AssistLine.X1 = snapRoadEnd.End.X;
                    AssistLine.Y1 = snapRoadEnd.End.Y;
                    AssistLine.X2 = RoadGhost.X2;
                    AssistLine.Y2 = RoadGhost.Y2;

                    canvas.Children.Add(AssistLine);
                }
            }
            else if (RoadGhost.Y1 == RoadGhost.Y2 && RoadGhost.X1 != RoadGhost.X2 &&
                     Math.Abs(RoadGhost.X2 - RoadGhost.X1) >= MinLengthRoad) // X road and GhostRoad is minimumlength
            {
                var snaproadstart = roads.Find(r => Math.Abs(mouse.X - r.Start.X) <= SnapRange && r.IsXRoad());
                var snapRoadEnd = roads.Find(r => Math.Abs(mouse.X - r.End.X) <= SnapRange && r.IsXRoad());

                if (snaproadstart != null)
                {
                    RoadGhost.X2 = (int)snaproadstart.Start.X;

                    AssistLine.X1 = snaproadstart.Start.X;
                    AssistLine.Y1 = snaproadstart.Start.Y;
                    AssistLine.X2 = RoadGhost.X2;
                    AssistLine.Y2 = RoadGhost.Y2;

                    canvas.Children.Add(AssistLine);
                }
                else if (snapRoadEnd != null)
                {
                    RoadGhost.X2 = (int)snapRoadEnd.End.X;

                    AssistLine.X1 = snapRoadEnd.End.X;
                    AssistLine.Y1 = snapRoadEnd.End.Y;
                    AssistLine.X2 = RoadGhost.X2;
                    AssistLine.Y2 = RoadGhost.Y2;

                    canvas.Children.Add(AssistLine);
                }
            }

            foreach(var road in roads)
            {
                if (!road.IsXRoad() && RoadGhost.Y1 == RoadGhost.Y2 &&
                    Util.MathUtil.Distance(new Vector(RoadGhost.X1, RoadGhost.Y1), new Vector(road.Start.X, RoadGhost.Y1)) <= SnapRange) // ghostRoad X road
                {
                    RoadGhost.X1 = road.Start.X;
                    break;
                }

                if (road.IsXRoad() && RoadGhost.X1 == RoadGhost.X2 &&
                    Util.MathUtil.Distance(new Vector(RoadGhost.X1, RoadGhost.Y1), new Vector(RoadGhost.X1, road.Start.Y)) <= SnapRange) // ghostRoad Y road
                {
                    RoadGhost.Y1 = road.Start.Y;
                    break;
                }
            }

            RoadGhost.StrokeThickness = StandardRoadWidth;
            canvas.Children.Add(RoadGhost);
        }


        /// <summary>
        /// Creates a <see cref="Road"/> and adds this to the list of <see cref="Road"/>s
        /// </summary>
        /// <param name="canvas"><see cref="Canvas"/> to work with</param>
        /// <param name="roadsList"><see cref="List{Road}"/> of <see cref="Road"/>s</param>
        /// <param name="buildingsList"><see cref="List{Building}"/> of <see cref="Building"/>s</param>
        /// <param name="garagesList"><see cref="List{Garage}"/> of <see cref="Garage"/>s</param>
        public static void CreateRoad(Canvas canvas, List<Road> roadsList, List<Building> buildingsList,
            List<Garage> garagesList)
        {
            _startRoad = new Point(0, 0);

            var road = new Road(new Vector((int)RoadGhost.X1, (int)RoadGhost.Y1), new Vector((int)RoadGhost.X2, (int)RoadGhost.Y2), StandardRoadWidth, StandardMaxSpeed);

            //check if road is long enough and road 
            if (Util.MathUtil.Distance(new Vector(RoadGhost.X1, RoadGhost.Y1), new Vector(RoadGhost.X2, RoadGhost.Y2)) < MinLengthRoad)
            {
                RemoveGhost(canvas);
                return;
            }

            //check if road is not vertical or horizontal
            if (!(RoadGhost.X1 == RoadGhost.X2 || RoadGhost.Y1 == RoadGhost.Y2))
            {
                RemoveGhost(canvas);
                return;
            }
            // check for coliding with buildings or garages
            if (CrossesBuildingOrGarage(buildingsList, garagesList, canvas))
            {
                RemoveGhost(canvas);
                return;
            };

            var roadGhostStart = new Vector(RoadGhost.X1, RoadGhost.Y2);
            var roadGhostEnd = new Vector(RoadGhost.X2, RoadGhost.Y2);

            //check if road collides with other roads
            foreach (var roadI in roadsList)
            {
                if (roadI.IsXRoad() && RoadGhost.Y1 == RoadGhost.Y2 &&
                    RoadGhost.Y1 + RoadGhost.StrokeThickness/2 >= roadI.Start.Y - roadI.Width / 2 &&
                    RoadGhost.Y1 - RoadGhost.StrokeThickness/2 <= roadI.Start.Y + roadI.Width / 2) // check if road overlaps
                { 
                    if(Math.Min(RoadGhost.X1, RoadGhost.X2) < Math.Min(roadI.Start.X, roadI.End.X) &&
                       Math.Max(RoadGhost.X1, RoadGhost.X2) > Math.Min(roadI.Start.X, roadI.End.X)
                       ||
                       Math.Min(RoadGhost.X1, RoadGhost.X2) < Math.Max(roadI.Start.X, roadI.End.X) &&
                       Math.Max(RoadGhost.X1, RoadGhost.X2) > Math.Max(roadI.Start.X, roadI.End.X))
                    {
                        if ((roadGhostStart != roadI.Start && roadGhostStart != roadI.End && roadGhostEnd != roadI.Start && roadGhostEnd != roadI.End) || road.IsXRoad())
                        {
                            RemoveGhost(canvas);
                            return;
                        }
                    }
                }

                if (roadI.IsXRoad() || RoadGhost.X1 != RoadGhost.X2 ||
                    !(RoadGhost.X1 + RoadGhost.StrokeThickness / 2 >= roadI.Start.X - roadI.Width / 2) ||
                    !(RoadGhost.X1 - RoadGhost.StrokeThickness / 2 <= roadI.Start.X + roadI.Width / 2)) continue;
                
                if ((!(Math.Min(RoadGhost.Y1, RoadGhost.Y2) < Math.Min(roadI.Start.Y, roadI.End.Y)) ||
                     !(Math.Max(RoadGhost.Y1, RoadGhost.Y2) > Math.Min(roadI.Start.Y, roadI.End.Y))) &&
                    (!(Math.Min(RoadGhost.Y1, RoadGhost.Y2) < Math.Max(roadI.Start.Y, roadI.End.Y)) ||
                     !(Math.Max(RoadGhost.Y1, RoadGhost.Y2) > Math.Max(roadI.Start.Y, roadI.End.Y)))) continue;
                
                if ((roadGhostStart == roadI.Start || roadGhostStart == roadI.End ||
                     roadGhostEnd == roadI.Start || roadGhostEnd == roadI.End) && road.IsXRoad()) continue;
                
                RemoveGhost(canvas);
                return;       
            }
            
            if (CreateMultipleRoadsWhenCrossingEachother(road, roadsList, canvas) == true) // check if road is crossed by ghost road
            {
                RemoveGhost(canvas);
                return;
            }


            
            DrawRoad(canvas, road);

            RemoveGhost(canvas);
            roadsList.Add(road);
            //IntersectionCreator.UpdateIntersections(ObjectHandler.Roads, ObjectHandler.Intersections);
            ObjectHandler.RedrawAllObjects(canvas);
        }

        /// <summary>
        /// Removes ghost <see cref="Road"/> from <see cref="Canvas"/>
        /// </summary>
        /// <param name="canvas"><see cref="Canvas"/> to remove from</param>
        public static void RemoveGhost(Canvas canvas)
        {
            canvas.Children.Remove(RoadGhost);
        }


        /// <summary>
        /// Draws a specific <see cref="Road"/> on a <see cref="Canvas"/>
        /// </summary>
        /// <param name="canvas"><see cref="Canvas"/> to work with</param>
        /// <param name="road"><see cref="Road"/> to draw</param>
        public static void DrawRoad(Canvas canvas, Road road)
        {
            RoadLine.X1 = (int)road.Start.X;
            RoadLine.Y1 = (int)road.Start.Y;
            RoadLine.X2 = (int)road.End.X;
            RoadLine.Y2 = (int)road.End.Y;


            Panel.SetZIndex(RoadLine, 2);
            RoadLine.StrokeThickness = road.Width;

            canvas.Children.Add(Clone(RoadLine));
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Checks if the current ghost <see cref="Road"/> crosses <see cref="Building"/>s or <see cref="Garage"/>s
        /// </summary>
        /// <param name="buildingsList"><see cref="List{Building}"/> of <see cref="Building"/>s to check</param>
        /// <param name="garagesList"><see cref="List{Garage}"/> of <see cref="Garage"/>s to check</param>
        /// <param name="canvas"><see cref="Canvas"/> to work with</param>
        /// <returns></returns>
        private static bool CrossesBuildingOrGarage(List<Building> buildingsList, List<Garage> garagesList, Canvas canvas)
        {
            // Check if road crosses building
            foreach (var building in buildingsList)
            {
                if(RoadGhost.Y1 == RoadGhost.Y2) // X road
                {
                    if ((!(RoadGhost.Y1 - StandardRoadWidth / 2 <= building.Location.Y + building.Size / 2) ||
                         !(RoadGhost.Y1 + StandardRoadWidth / 2 >= building.Location.Y - building.Size / 2))) continue;
                    
                    if (!(Math.Max(RoadGhost.X1, RoadGhost.X2) >= building.Location.X - building.Size / 2) ||
                        !(Math.Min(RoadGhost.X1, RoadGhost.X2) <= building.Location.X + building.Size / 2))
                        continue;
                        
                    RemoveGhost(canvas);
                    return true;
                }

                if (RoadGhost.X1 != RoadGhost.X2) continue;
                
                if (!(RoadGhost.X1 - StandardRoadWidth / 2 <= building.Location.X + building.Size / 2) ||
                    !(RoadGhost.X1 + StandardRoadWidth / 2 >= building.Location.X - building.Size / 2)) continue;
                    
                if (!(Math.Max(RoadGhost.Y1, RoadGhost.Y2) >= building.Location.Y - building.Size / 2) ||
                    !(Math.Min(RoadGhost.Y1, RoadGhost.Y2) <= building.Location.Y + building.Size / 2))
                    continue;
                        
                RemoveGhost(canvas);
                return true;
            }

            //Check if road crosses Garage
            foreach (var garage in garagesList)
            {
                if (RoadGhost.Y1 == RoadGhost.Y2) // X road
                {
                    if ((!(RoadGhost.Y1 - StandardRoadWidth / 2 <= garage.Location.Y + garage.Size / 2) ||
                         !(RoadGhost.Y1 + StandardRoadWidth / 2 >= garage.Location.Y - garage.Size / 2))) continue;
                    
                    if (!(Math.Max(RoadGhost.X1, RoadGhost.X2) >= garage.Location.X - garage.Size / 2) ||
                        !(Math.Min(RoadGhost.X1, RoadGhost.X2) <= garage.Location.X + garage.Size / 2)) continue;
                        
                    RemoveGhost(canvas);
                    return true;
                }

                if (RoadGhost.X1 != RoadGhost.X1) continue;
                
                if ((!(RoadGhost.X1 - StandardRoadWidth / 2 <= garage.Location.X + garage.Size / 2) ||
                     !(RoadGhost.X1 + StandardRoadWidth / 2 >= garage.Location.X - garage.Size / 2))) continue;
                    
                if (!(Math.Max(RoadGhost.Y1, RoadGhost.Y2) >= garage.Location.Y - garage.Size / 2) ||
                    !(Math.Min(RoadGhost.Y1, RoadGhost.Y2) <= garage.Location.Y + garage.Size / 2)) continue;
                        
                RemoveGhost(canvas);
                return true;
            }
            return false;
        }

        
        /// <summary>
        /// Creates multiple <see cref="Road"/>s when different <see cref="Road"/>s intersect
        /// </summary>
        /// <param name="road1">New intersecting <see cref="Road"/></param>
        /// <param name="roadsList"><see cref="List{Road}"/> of <see cref="Road"/>s</param>
        /// <param name="canvas"><see cref="Canvas"/> to work with</param>
        /// <returns>True when <see cref="Road"/>s have been created</returns>
        private static bool CreateMultipleRoadsWhenCrossingEachother(Road road1, List<Road> roadsList, Canvas canvas)
        {
            var returnBool = false;

            var crossingRoads = roadsList.FindAll(r => // find all roads that are crossed and order from Low to high
            {
                if (road1.IsXRoad() && !r.IsXRoad() &&
                    road1.End != r.End && road1.End != r.Start &&
                    road1.Start != r.End && road1.Start != r.Start)
                {
                    if(Math.Max(r.Start.Y, r.End.Y) >= road1.Start.Y &&
                       Math.Min(r.Start.Y, r.End.Y) <= road1.Start.Y &&
                       Math.Max(road1.Start.X, road1.End.X) >= r.Start.X &&
                       Math.Min(road1.Start.X, road1.End.X) <= r.Start.X )
                        return true;
                }
                else if (!road1.IsXRoad() && r.IsXRoad() &&
                         road1.End != r.End && road1.End != r.Start &&
                         road1.Start != r.End && road1.Start != r.Start)
                {
                    if (Math.Max(r.Start.X, r.End.X) >= road1.Start.X &&
                        Math.Min(r.Start.X, r.End.X) <= road1.Start.X &&
                        Math.Max(road1.Start.Y, road1.End.Y) >= r.Start.Y &&
                        Math.Min(road1.Start.Y, road1.End.Y) <= r.Start.Y)
                        return true;
                }
                return false;
            }).OrderBy(r => r.IsXRoad() ? r.Start.Y : r.Start.X).ToList();

            if(!crossingRoads.Any())
                return false;

            if (road1.IsXRoad())
            {
                var location = (Math.Min(road1.Start.X, road1.End.X) == road1.Start.X) ? road1.Start : road1.End; // determine whats start location
                for (var i = 0; i < crossingRoads.Count + 1; i++)
                {
                    if(i < crossingRoads.Count &&
                       crossingRoads[i].Start.X == location.X &&
                       crossingRoads.Last() != crossingRoads[i]) // if there is not a piece of road behind the first cross
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
                        var roadNew = new Road(beginNewRoad, location, StandardRoadWidth, StandardMaxSpeed);
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
                        var roadNew = (Math.Max(road1.Start.X, road1.End.X) == road1.End.X)
                            ? new Road(beginNewRoad, road1.End, StandardRoadWidth, StandardMaxSpeed)
                            : new Road(beginNewRoad, road1.Start, StandardRoadWidth, StandardMaxSpeed);
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
                for (var i = 0; i < crossingRoads.Count + 1; i++)
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
                        var roadNew = new Road(beginNewRoad, location, StandardRoadWidth, StandardMaxSpeed);
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
                        var roadNew = (Math.Max(road1.Start.Y, road1.End.Y) == road1.End.Y)
                            ? new Road(beginNewRoad, road1.End, StandardRoadWidth, StandardMaxSpeed)
                            : new Road(beginNewRoad, road1.Start, StandardRoadWidth, StandardMaxSpeed);
                        if (!(Util.MathUtil.Distance(roadNew.Start, roadNew.End) > MinLengthRoad)) continue;
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
        /// Fix to copy <see cref="Road"/>s
        /// </summary>
        /// <param name="e"><see cref="FrameworkElement"/> to copy from</param>
        /// <returns>Copied <see cref="FrameworkElement"/></returns>
        private static FrameworkElement Clone(FrameworkElement e)
        {
            var document = new XmlDocument();
            document.LoadXml(XamlWriter.Save(e));
            return (FrameworkElement)XamlReader.Load(new XmlNodeReader(document));
        }

        #endregion
    }
}
