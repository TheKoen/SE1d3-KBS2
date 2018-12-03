using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using System.Windows.Controls;
using KBS2.CarSystem;
using KBS2.CarSystem.Sensors;
using Microsoft.Win32;

namespace KBS2.ModelDesigner
{
    public partial class ModelDesigner
    {
        // File extension for model exports
        private const string ModelFileExtension = "carmdl";
        
        private CarDesign _currentDesign = new CarDesign();
        private static readonly IFormatter Formatter = new BinaryFormatter();
        
        public ModelDesigner()
        {
            InitializeComponent();
            
            DesignDisplay.Source = _currentDesign?.Brush;
            SetSensorList(_currentDesign.GetSensors());

            ButtonNew.Click += (sender, args) =>
            {
                _currentDesign = new CarDesign();
                DesignDisplay.Source = _currentDesign.Brush;
                SetSensorList(_currentDesign.GetSensors());
            };
            ButtonExport.Click += (sender, args) =>
            {
                var fileDialog = new SaveFileDialog
                {
                    AddExtension = true,
                    DefaultExt = $".{ModelFileExtension}",
                    Filter = $"Car Model File|*.{ModelFileExtension}",
                    InitialDirectory = Environment.CurrentDirectory,
                    CheckPathExists = true,
                    OverwritePrompt = true,
                    Title = "Export Car Model"
                };

                // Exporting the current model, unless the user cancels the dialog
                if (fileDialog.ShowDialog() != true) return;
                ExportDesign(_currentDesign, fileDialog.FileName);
            };
            ButtonImport.Click += (sender, args) =>
            {
                var fileDialog = new OpenFileDialog
                {
                    Filter = $"Car Model Files|*.{ModelFileExtension}|All Files|*.*",
                    InitialDirectory = Environment.CurrentDirectory,
                    CheckFileExists = true,
                    CheckPathExists = true,
                    Title = "Import Car Model"
                };

                // Importing the selected model, unless the user cancels the dialog
                if (fileDialog.ShowDialog() != true) return;
                _currentDesign = ImportDesign(fileDialog.FileName) ?? _currentDesign;
                DesignDisplay.Source = _currentDesign?.Brush;
                SetSensorList(_currentDesign.GetSensors());
            };

            ButtonNewSensor.Click += (sender, args) =>
            {
                var window = new SensorCreationWindow();
                window.ShowDialog();

                if (!window.Success) return;
                var sensor = new SensorPrototype
                {
                    Direction = window.SensorDirection,
                    Range = window.SensorRange,
                    Create = Sensor.Sensors[window.SensorType]
                };
                _currentDesign.AddSensor(sensor);
                DesignDisplay.Source = _currentDesign?.Brush;
                SetSensorList(_currentDesign.GetSensors());
            };
        }


        /// <summary>
        /// Exports the given <see cref="CarDesign"/> to the given file
        /// </summary>
        /// <param name="design">The <see cref="CarDesign"/> to export</param>
        /// <param name="filename">The file to export to</param>
        private static void ExportDesign(CarDesign design, string filename)
        {
            // Creating the file, or overwriting it if it already exists
            var stream = new FileStream(filename, FileMode.Create);
            
            // Serializing the design into the file
            Formatter.Serialize(stream, design);
            stream.Close();
        }

        /// <summary>
        /// Imports a <see cref="CarDesign"/> from a given file
        /// </summary>
        /// <param name="filename">The file to import from</param>
        /// <returns>The imported <see cref="CarDesign"/></returns>
        private static CarDesign ImportDesign(string filename)
        {
            var stream = new FileStream(filename, FileMode.Open);
            try
            {
                return (CarDesign) Formatter.Deserialize(stream);
            }
            catch (SerializationException se)
            {
                MessageBox.Show($"Invalid {ModelFileExtension} file, could not import \"{filename}\"");
                return null;
            }
            finally
            {
                stream.Close();
            }
        }
        
        private static CarDesign DesignFromModel(CarModel model)
        {
            var newDesign = new CarDesign();
            foreach (var sensor in model.Sensors)
            {
                newDesign.AddSensor(sensor);
            }

            return newDesign;
        }

        private void SetSensorList(IEnumerable<SensorPrototype> list)
        {
            StackPanelSensors.Children.Clear();
            
            foreach (var sensor in list)
            {
                var label0 = new Label {Content = sensor.Direction};
                var label1 = new Label {Content = sensor.Range};
                var label2 = new Label {Content = Sensor.Sensors.First(s => s.Value.Method.GetHashCode() == sensor.Create.Method.GetHashCode()).Key.Name};
                label0.SetValue(Grid.ColumnProperty, 0);
                label1.SetValue(Grid.ColumnProperty, 1);
                label2.SetValue(Grid.ColumnProperty, 2);
                
                var grid = new Grid
                {
                    ColumnDefinitions =
                    {
                        new ColumnDefinition {Width = new GridLength(40)},
                        new ColumnDefinition {Width = new GridLength(30)},
                        new ColumnDefinition {Width = new GridLength(110)}
                    },
                    Children =
                    {
                        label0,
                        label1,
                        label2
                    }
                };

                StackPanelSensors.Children.Add(grid);
            }
        }
    }
}
