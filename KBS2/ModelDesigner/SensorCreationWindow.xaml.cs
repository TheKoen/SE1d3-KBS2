using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using KBS2.CarSystem.Sensors;

namespace KBS2.ModelDesigner
{
    public partial class SensorCreationWindow
    {
        private static readonly Regex NotNumberRegex = new Regex("[^0-9]+");

        public Type SensorType { get; private set; }
        public Direction SensorDirection { get; private set; }
        public int SensorRange { get; private set; }
        public bool Success { get; private set; }
        
        public SensorCreationWindow()
        {
            InitializeComponent();

            ComboBoxType.ItemsSource = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(t => t.GetTypes())
                .Where(t => t.IsClass && !t.IsAbstract && typeof(Sensor).IsAssignableFrom(t));
            ComboBoxSide.ItemsSource = Enum.GetNames(typeof(Direction));

            Success = false;
        }
        
        
        private void TextBoxRange_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = NotNumberRegex.IsMatch(e.Text);
        }

        private void TextBox_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = e.Key == Key.Space;
        }

        private void ButtonOK_OnClick(object sender, RoutedEventArgs e)
        {
            if (ComboBoxType.SelectedItem == null ||
                ComboBoxSide.SelectedItem == null ||
                (TextBoxRange.IsEnabled && TextBoxRange.Text.Trim().Equals(string.Empty)))
            {
                MessageBox.Show("Please fill in every field");
                return;
            }
            
            SensorType = (Type) ComboBoxType.SelectedItem;
            SensorDirection = (Direction) Enum.Parse(typeof(Direction), (string) ComboBoxSide.SelectedItem);
            SensorRange = int.Parse(TextBoxRange.Text);
            
            Success = true;
            Close();
        }

        private void ButtonCancel_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
