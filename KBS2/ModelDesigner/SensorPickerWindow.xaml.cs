using System.Collections.Generic;
using System.Linq;
using System.Windows;
using KBS2.CarSystem;
using KBS2.CarSystem.Sensors;

namespace KBS2.ModelDesigner
{
    public partial class SensorPickerWindow : Window
    {
        public SensorPrototype SelectedSensor { get; private set; }

        private List<SensorPrototype> _sensorList;

        public List<SensorPrototype> SensorList
        {
            get => _sensorList;
            set
            {
                _sensorList = value;
                ListBoxSensors.ItemsSource = _sensorList.Select(s => new SensorPrototypeDisplayData(s));
            }
        }
        
        public bool Success { get; private set; }

        public SensorPickerWindow()
        {
            InitializeComponent();

            Success = false;
        }

        private void ButtonOK_OnClick(object sender, RoutedEventArgs e)
        {
            if (ListBoxSensors.SelectedItem == null)
            {
                MessageBox.Show("Please select a model");
                return;
            }

            SelectedSensor = ((SensorPrototypeDisplayData) ListBoxSensors.SelectedItem).ThisSensor;

            Success = true;
            Close();
        }

        private void ButtonCancel_OnClick(object sender, RoutedEventArgs e)
        {
            throw new System.NotImplementedException();
        }


        internal struct SensorPrototypeDisplayData
        {
            public SensorPrototype ThisSensor { get; }
            public string DisplayText { get; set; }

            public SensorPrototypeDisplayData(SensorPrototype sensor) : this()
            {
                ThisSensor = sensor;
                DisplayText = string.Format("{0} ({1}, {2})",
                    Sensor.Sensors.First(s => s.Value.Method.GetHashCode() == sensor.Create.Method.GetHashCode()).Key.Name,
                    sensor.Direction, sensor.Range);
            }
        }
    }
}
