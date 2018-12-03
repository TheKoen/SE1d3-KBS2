using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Media;
using KBS2.CarSystem;
using KBS2.CarSystem.Sensors;

namespace KBS2.ModelDesigner
{
    [Serializable]
    public class CarDesign : ISerializable
    {
        public int CarWidth { get; set; } = 10;
        public int CarHeight { get; set; } = 5;
        
        private List<SensorPrototype> SensorList = new List<SensorPrototype>();

        public DrawingImage Brush
        {
            get
            {
                // Creating the preview of the model's design
                var drawables = new List<Drawing>
                {
                    new GeometryDrawing // Body
                    {
                        Brush = Brushes.LightBlue,
                        Pen = new Pen(Brushes.LightSkyBlue, 0.5),
                        Geometry = new RectangleGeometry(new Rect(new Size(CarWidth, CarHeight)), 2, 2)
                    },
                    new GeometryDrawing // Arrow
                    {
                        Brush = Brushes.Black,
                        Geometry = new PathGeometry
                        {
                            Figures = new PathFigureCollection(new List<PathFigure>
                            {
                                new PathFigure(new Point(CarWidth - 1, CarHeight / 2.0), new List<PathSegment>
                                {
                                    new LineSegment(new Point(CarWidth - 2, CarHeight / 2.0 - 0.75), false),
                                    new LineSegment(new Point(CarWidth - 2, CarHeight / 2.0 + 0.75), false)
                                }, true)
                            }),
                            FillRule = FillRule.Nonzero
                        } // Making triangles is a mess
                    }
                };

                // Getting amount of sensors on each side
                double frontCount = SensorList.Count(s => s.Direction == Direction.Front);
                double backCount = SensorList.Count(s => s.Direction == Direction.Back);
                double leftCount = SensorList.Count(s => s.Direction == Direction.Left);
                double rightCount = SensorList.Count(s => s.Direction == Direction.Right);
                double globalCount = SensorList.Count(s => s.Direction == Direction.Global);

                // Looping through all front facing sensors
                for (var i = 0; i < frontCount; ++i)
                    drawables.Add(new GeometryDrawing
                    {
                        Geometry = new RectangleGeometry(
                            new Rect(CarWidth - .5, CarHeight / (1 + frontCount) * (1 + i) - .25, 0.5, 0.5),
                            0.1, 0.1
                        ),
                        Pen = new Pen(Brushes.Blue, 0.5),
                    });
                
                // Looping through all back facing sensors
                for (var i = 0; i < backCount; ++i)
                    drawables.Add(new GeometryDrawing
                    {
                        Geometry = new RectangleGeometry(
                            new Rect(0, CarHeight / (1 + backCount) * (1 + i) - .25, 0.5, 0.5),
                            0.1, 0.1
                        ),
                        Pen = new Pen(Brushes.Blue, 0.5),
                    });
                
                // Looping through all left facing sensors
                for (var i = 0; i < leftCount; ++i)
                    drawables.Add(new GeometryDrawing
                    {
                        Geometry = new RectangleGeometry(
                            new Rect(CarWidth / (1 + leftCount) * (1 + i) - .25, 0, 0.5, 0.5),
                            0.1, 0.1
                        ),
                        Pen = new Pen(Brushes.Blue, 0.5),
                    });
                
                // Looping through all right facing sensors
                for (var i = 0; i < rightCount; ++i)
                    drawables.Add(new GeometryDrawing
                    {
                        Geometry = new RectangleGeometry(
                            new Rect(CarWidth / (1 + rightCount) * (1 + i) - .25, CarHeight - .5, 0.5, 0.5),
                            0.1, 0.1
                        ),
                        Pen = new Pen(Brushes.Blue, 0.5),
                    });

                // Calculating the half-distance for the row of global sensors
                var medDistance = .45 * Math.Max(Math.Round(globalCount / 2.0) - 1, 0);
                
                // Looping through all global sensors
                for (var i = 0; i < globalCount; ++i)
                {
                    var last = (int)globalCount - i <= 1;
                    var even = (int)globalCount % 2 == 0;
                    
                    // This part is a mess, please don't look ;_;
                    drawables.Add(new GeometryDrawing
                    {
                        Geometry = new RectangleGeometry(
                            new Rect(CarWidth / 2.0 + medDistance - (.9 * Math.Floor(i / 2.0)) - .15, 
                                CarHeight / 2.0 - .6 + (last ? (even ? .9 : .45) : (i % 2 != 0 ? .9 : 0)),
                                0.3, 0.3), // Correctly positioning all global sensors, oh god
                            0.1, 0.1
                        ),
                        Pen = new Pen(Brushes.Blue, 0.3)
                    }); // I'm never touching this code again...
                }
                
                // Actually giving the caller their DrawingImage object, geez
                return new DrawingImage
                {
                    Drawing = new DrawingGroup
                    {
                        Children = new DrawingCollection(drawables)
                    }
                };
            }
        }

        public CarDesign()
        {
        }

        public CarDesign(SerializationInfo info, StreamingContext context)
        {
            CarWidth = (int) info.GetValue("width", typeof(int));
            CarHeight = (int) info.GetValue("height", typeof(int));
            SensorList = (List<SensorPrototype>) info.GetValue("sensors", typeof(List<SensorPrototype>));
        }


        public void AddSensor(SensorPrototype sensorPrototype) =>
            SensorList.Add(sensorPrototype);

        public void RemoveSensor(SensorPrototype sensorPrototype) =>
            SensorList.Remove(sensorPrototype);
        
        public CarModel GetAsModel(string name, double maxSpeed) =>
            new CarModel(maxSpeed, SensorList, name);

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("width", CarWidth, typeof(int));
            info.AddValue("height", CarHeight, typeof(int));
            info.AddValue("sensors", SensorList, typeof(List<SensorPrototype>));
        }
    }
}