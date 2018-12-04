using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using KBS2.CarSystem;
using KBS2.CarSystem.Sensors;
using Microsoft.Win32;

namespace KBS2.ModelDesigner
{
    public partial class ModelDesigner
    {
        private static readonly Regex NotNumberRegex = new Regex("[^0-9\\.]+");
        
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
                TextBoxDesignName.Text = string.Empty;
                TextBoxMaxSpeed.Text = string.Empty;
            };
            ButtonSave.Click += (sender, args) =>
            {
                var name = TextBoxDesignName.Text.Trim();
                if (name.Equals(string.Empty))
                {
                    MessageBox.Show("Please give the design a name");
                    return;
                }

                if (CarModel.Contains(name))
                {
                    MessageBox.Show($"A model with the name \"{name}\" already exists");
                    return;
                }

                double maxSpeed;
                try
                {
                    maxSpeed = double.Parse(TextBoxMaxSpeed.Text);
                }
                catch (FormatException)
                {
                    MessageBox.Show("Bad input for Speed");
                    return;
                }
                
                CarModel.Set(new CarModel(maxSpeed, _currentDesign.GetSensors(), name));
                MessageBox.Show($"Model \"{name}\" was successfully added to the simulation");
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
            ButtonLoad.Click += (sender, args) =>
            {
                var window = new ModelPickerWindow();
                window.ShowDialog();
                
                if (!window.Success) return;
                _currentDesign = DesignFromModel(window.SelectedModel);
                DesignDisplay.Source = _currentDesign.Brush;
                SetSensorList(_currentDesign.GetSensors());
                TextBoxDesignName.Text = window.SelectedModel.Name;
                TextBoxMaxSpeed.Text = window.SelectedModel.MaxSpeed.ToString(CultureInfo.InvariantCulture);
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
            ButtonRemoveSensor.Click += (sender, args) =>
            {
                var window = new SensorPickerWindow
                {
                    SensorList = new List<SensorPrototype>(_currentDesign.GetSensors())
                };
                window.ShowDialog();
                
                if (!window.Success) return;
                _currentDesign.RemoveSensor(window.SelectedSensor);
                DesignDisplay.Source = _currentDesign.Brush;
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
            var sensorList = list.Select(sensor => string.Format("{0} ({1}, {2})",
                Sensor.Sensors.First(s => s.Value.Method.GetHashCode() == sensor.Create.Method.GetHashCode()).Key.Name,
                sensor.Direction, sensor.Range)).ToList();

            ItemsControlSensors.ItemsSource = sensorList;
        }

        private void TextBoxMaxSpeed_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = NotNumberRegex.IsMatch(e.Text);
        }
    }
}
