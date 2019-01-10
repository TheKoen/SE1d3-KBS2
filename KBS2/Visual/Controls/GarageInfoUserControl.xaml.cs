using CommandSystem.PropertyManagement;
using KBS2.CitySystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using KBS2.CarSystem;

namespace KBS2.Visual.Controls
{
    /// <summary>
    /// Interaction logic for GarageInfoUserControl.xaml
    /// </summary>
    public partial class GarageInfoUserControl : UserControl
    {
        public Garage Building { get; set; }
        private StackPanel PropertyPanel { get; }
        private PropertySettings AvailableCarsProperty;
        private PropertySettings CarModelProperties;

        public GarageInfoUserControl(Building building)
        {
            InitializeComponent();
            PropertyPanel = this.StackPanelGarage;
            Building = (Garage)building;
            LabelGarageLocation.Content = Building.Location;
            displayProperties();
        }

        private void displayProperties()
        {
            var propertyName = "Available Cars";
            var propertyValue = Building.AvailableCars.ToString();

            AvailableCarsProperty = new PropertySettings(propertyName, propertyValue, true);
            PropertyPanel.Children.Add(AvailableCarsProperty);

            var propertyName2 = "Model";
            var propertyValue2 = Building.Model.Name;

            CarModelProperties = new PropertySettings(propertyName2, propertyValue2, false);
            CarModelProperties.PreviewTextInput -= CarModelProperties.NumberValidationTextBox;
            PropertyPanel.Children.Add(CarModelProperties);
        }

        private void BtnGarageSave_Click(object sender, RoutedEventArgs e)
        {
            //If value changed
            if(Building.AvailableCars.ToString() != AvailableCarsProperty.TBCurrentValue.Text)
            {
                var newValue = (int)Math.Round(double.Parse(AvailableCarsProperty.TBCurrentValue.Text));
                Building.AvailableCars = newValue;
            }

            if (Building.Model.ToString() != CarModelProperties.TBCurrentValue.Text)
            {
                var newValue = CarModelProperties.TBCurrentValue.Text;
                try
                {
                    Building.Model = CarModel.GetModel(newValue);
                }
                catch (Exception)
                {
                    MessageBox.Show("The changes have not been saved, the given model does not exist.", "Changes not saved.", MessageBoxButton.OK);
                }
            }
        }
    }
}
