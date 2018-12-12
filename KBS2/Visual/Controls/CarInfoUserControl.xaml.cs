using System;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
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
            LabelInfoCustomerCount.DataContext = car;
            LabelInfoDestinationCar.DataContext = car;
            LabelInfoLocationCar.DataContext = car;
            LabelInfoDistanceTraveled.DataContext = car;

            //Takes the last character of car Id and that char determines the picture for that specific car.
            var last = int.Parse(car.Id.ToString()[car.Id.ToString().Length - 1].ToString());
            var car1 = new BitmapImage(new Uri($@" /KBS2;component/Images/CarPicture1.jpg", UriKind.Relative));
            var car2 = new BitmapImage(new Uri($@" /KBS2;component/Images/CarPicture2.jpg", UriKind.Relative));
            var car3 = new BitmapImage(new Uri($@" /KBS2;component/Images/CarPicture3.jpg", UriKind.Relative));         
            CarPicture.Source = (last >= 0 && last < 4) ? car1 : (last >= 4 && last < 7) ? car2 : car3;
        }
    }
}
