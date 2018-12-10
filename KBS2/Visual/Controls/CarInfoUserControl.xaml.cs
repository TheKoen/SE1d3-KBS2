using System.Windows.Controls;
using KBS2.CarSystem;

namespace KBS2.Visual.Controls
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
            LabelInfoDistanceTraveled.DataContext = car;

        }
    }
}
