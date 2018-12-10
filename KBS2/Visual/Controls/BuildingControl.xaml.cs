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
using KBS2.CitySystem;

namespace KBS2.Visual.Controls
{
    /// <summary>
    /// Interaction logic for BuildingControl.xaml
    /// </summary>
    public partial class BuildingControl : UserControl
    {
        public BuildingControl(Building building)
        {
            var size = building.Size;
            var location = building.Location;

            var c = size / 2d;
            Margin = new Thickness(location.X - c, location.Y - c, 0, 0);
            
            Width = size;
            Height = size;

            Initialized += (sender, args) =>
            {
                if (building is Garage)
                {
                    Rectangle.Fill = new SolidColorBrush(Colors.LightSlateGray);
                }
            };

            InitializeComponent();
        }
    }
}
