using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
using System.Windows;
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
            
            // Setting the brush and sensor list for the default design, if it exists
            DesignDisplay.Source = _currentDesign?.Brush;
            SetSensorList(_currentDesign?.SensorList);

            ButtonNew.Click += (sender, args) =>
            {
                // Setting the design to a fresh one, resetting the brush, and resetting the sensor list
                _currentDesign = new CarDesign();
                DesignDisplay.Source = _currentDesign.Brush;
                SetSensorList(_currentDesign.SensorList);
                // Clearing the TextBoxes
                TextBoxDesignName.Text = string.Empty;
                TextBoxMaxSpeed.Text = string.Empty;
            };
            ButtonSave.Click += (sender, args) =>
            {
                // Checking if a name has been given
                var name = TextBoxDesignName.Text.Trim();
                if (name.Equals(string.Empty))
                {
                    MessageBox.Show("Please give the design a name");
                    return;
                }

                // Checking if a model with that name already exists
                if (CarModel.Contains(name))
                {
                    MessageBox.Show($"A model with the name \"{name}\" already exists");
                    return;
                }

                // Checking if the max speed has been formatted properly
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
                
                // Adding the model to the list for the simulation
                CarModel.Set(new CarModel(maxSpeed, _currentDesign.SensorList, name));
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
                // Setting the new design and other properties
                _currentDesign = DesignFromModel(window.SelectedModel);
                DesignDisplay.Source = _currentDesign.Brush;
                SetSensorList(_currentDesign.SensorList);
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
                SetSensorList(_currentDesign.SensorList);
            };

            ButtonNewSensor.Click += (sender, args) =>
            {
                var window = new SensorCreationWindow();
                window.ShowDialog();

                if (!window.Success) return;
                // Adding the new sensor and setting other properties
                var sensor = new SensorPrototype
                {
                    Direction = window.SensorDirection,
                    Range = window.SensorRange,
                    Create = Sensor.Sensors[window.SensorType]
                };
                _currentDesign.SensorList.Add(sensor);
                DesignDisplay.Source = _currentDesign?.Brush;
                SetSensorList(_currentDesign.SensorList);
            };
            ButtonRemoveSensor.Click += (sender, args) =>
            {
                var window = new SensorPickerWindow
                {
                    SensorList = new List<SensorPrototype>(_currentDesign.SensorList)
                };
                window.ShowDialog();
                
                if (!window.Success) return;
                // Removing the selected sensor and setting other properties
                _currentDesign.SensorList.Remove(window.SelectedSensor);
                DesignDisplay.Source = _currentDesign.Brush;
                SetSensorList(_currentDesign.SensorList);
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
            catch (SerializationException)
            {
                MessageBox.Show($"Invalid {ModelFileExtension} file, could not import \"{filename}\"");
                return null;
            }
            finally
            {
                stream.Close();
            }
        }
        
        /// <summary>
        /// Creates a <see cref="CarDesign"/> from a given <see cref="CarModel"/>
        /// </summary>
        /// <param name="model">The <see cref="CarModel"/> to be used</param>
        /// <returns>The new <see cref="CarDesign"/></returns>
        private static CarDesign DesignFromModel(CarModel model)
        {
            var newDesign = new CarDesign();
            newDesign.SensorList.AddRange(model.Sensors);

            return newDesign;
        }

        /// <summary>
        /// Fills the list of <see cref="SensorPrototype"/>s
        /// </summary>
        /// <param name="list"><see cref="List{SensorPrototype}"/> to be used</param>
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
