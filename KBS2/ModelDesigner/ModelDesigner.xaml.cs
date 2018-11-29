using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using KBS2.CarSystem;
using KBS2.CarSystem.Sensors;
using KBS2.CarSystem.Sensors.ActiveSensors;
using KBS2.CarSystem.Sensors.PassiveSensors;

namespace KBS2.ModelDesigner
{
    public partial class ModelDesigner : Window
    {
        public ModelDesigner()
        {
            InitializeComponent();

            var design = new CarDesign(this);

            #region TestSensors

            design.AddSensor(new SensorPrototype
            {
                Direction = Direction.Front,
                Range = 20,
                Create = Sensor.Sensors[typeof(CollisionSensor)]
            });
            
            design.AddSensor(new SensorPrototype
            {
                Direction = Direction.Left,
                Range = 10,
                Create = Sensor.Sensors[typeof(CollisionSensor)]
            });
            
            design.AddSensor(new SensorPrototype
            {
                Direction = Direction.Left,
                Create = Sensor.Sensors[typeof(LineSensor)]
            });
            
            design.AddSensor(new SensorPrototype
            {
                Direction = Direction.Right,
                Create = Sensor.Sensors[typeof(LineSensor)]
            });
            
            for (var i = 0; i < 3; ++i)
            {
                design.AddSensor(new SensorPrototype
                {
                    Direction = Direction.Global,
                    Range = 40,
                    Create = Sensor.Sensors[typeof(EntityRadar)]
                });
            }

            #endregion
            
            
            var img = new Image
            {
                Source = design.Brush,
                Stretch = Stretch.Uniform,
                Margin = new Thickness(50, 100, 50, 100),
                MinWidth = 200,
                MinHeight = 150
            };
            MainGrid.Children.Add(img);
        }
    }
}
