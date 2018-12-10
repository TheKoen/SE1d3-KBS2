using KBS2.CarSystem;
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

namespace KBS2.Windows
{
    /// <summary>
    /// Interaction logic for CarInfoUserControl.xaml
    /// </summary>
    public partial class CarInfoUserControl : UserControl
    {
        public CarInfoUserControl(Car car)
        {
            InitializeComponent();

            LabelInfoCarID.Content = car.Id;
            LabelInfoCarModel.Content = car.Model.Name;
            LabelInfoCustomerCount.Content = car.Passengers.Count;
            LabelInfoDestinationCar.Content = car.Destination;
            LabelInfoLocationCar.DataContext = car;
            LabelInfoDistanceTraveled.Content = car.DistanceTraveled;

        }
    }
}
