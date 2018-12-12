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
        public Building Building { get; set; }
        public MainScreen Screen;
        public BuildingControl(Building building, MainScreen screen)
        {
            Building = building;
            Screen = screen;
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
                    Rectangle.Fill = new SolidColorBrush(Colors.DeepPink);
                }
            };

            InitializeComponent();
        }

        public void Building_Selected(object sender, MouseButtonEventArgs e)
        {
            //Check if building is garage else do nothing
            if(Building is Garage)
            {
                //Show garage info + Properties to change
                //Empty info tab
                Screen.TabItemInfo.Content = null;

                //Add info about this garage
                GarageInfoUserControl ci = new GarageInfoUserControl(Building);
                Screen.TabItemInfo.Content = ci;

                //Open the info tab of selected garage
                Screen.TabItemInfo.IsSelected = true;
            }
            else
            {
                return;
            }
        }
    }
}
