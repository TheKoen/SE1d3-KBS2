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

        public GarageInfoUserControl(Building building)
        {
            InitializeComponent();
            PropertyPanel = this.StackPanelGarage;
            Building = (Garage)building;
            LabelGarageLocation.Content = Building.Location;
            DisplayProperties();
        }

        public void DisplayProperties()
        {
            var propertyName = "AvailableCars";
            var propertyValue = Building.AvailableCars.ToString();

            AvailableCarsProperty = new PropertySettings(propertyName, propertyValue);
            PropertyPanel.Children.Add(AvailableCarsProperty);
        }

        private void BtnGarageSave_Click(object sender, RoutedEventArgs e)
        {
            //If value changed
            if(Building.AvailableCars.ToString() == AvailableCarsProperty.TBCurrentValue.Text)
            {
                MessageBox.Show("The changes have not been saved, input is same as current value.", "Changes not saved.", MessageBoxButton.OK);
            }
            else
            {
                //Set new value.
                var newValue = (int)Math.Round(double.Parse(AvailableCarsProperty.TBCurrentValue.Text));
                Building.AvailableCars = newValue;
            }
        }
    }
}
