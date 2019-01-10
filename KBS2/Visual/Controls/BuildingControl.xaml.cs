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
        private Building Building { get; }
        private MainScreen Screen { get; }

        private float lastZoom;

        public BuildingControl(MainScreen screen, Building building)
        {
            Screen = screen;
            Building = building;

            Initialized += (sender, args) =>
            {
                if (building is Garage)
                {
                    Rectangle.Fill = new SolidColorBrush(Colors.DeepPink);
                }
            };

            InitializeComponent();
            Update();

            MainScreen.ZoomLoop.Subscribe(Update);
        }

        private void Update()
        {
            if (Math.Abs(lastZoom - Screen.Zoom) < 0.01)
            {
                return;
            }
            lastZoom = Screen.Zoom;

            var zoom = Screen.Zoom;
            var size = Building.Size;
            var location = Building.Location;

            var offset = size / 2d;
            var newMargin = new Thickness((location.X - offset) * zoom, (location.Y - offset) * zoom, 0, 0);

            var newWidth = size * zoom;
            var newHeight = size * zoom;

            var newThickness = (size * zoom) / 8d;

            MainScreen.DrawingLoop.EnqueueAction(() =>
            {
                Margin = newMargin;
                Width = newWidth;
                Height = newHeight;

                Rectangle.StrokeThickness = newThickness;
            });
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
                var ci = new GarageInfoUserControl(Building);
                Screen.TabItemInfo.Content = ci;

                //Open the info tab of selected garage
                Screen.TabItemInfo.IsSelected = true;
            }
        }
    }
}
