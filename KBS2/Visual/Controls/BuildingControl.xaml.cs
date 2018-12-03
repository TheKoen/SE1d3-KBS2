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
    /// Interaction logic for BuildingControl.xaml
    /// </summary>
    public partial class BuildingControl : UserControl
    {
        public BuildingControl(Vector location, int size)
        {
            double correction = size / 2d;
            Margin = new Thickness(location.X - correction, location.Y - correction, 0, 0);
            
            Width = size;
            Height = size;

            InitializeComponent();
        }
    }
}
