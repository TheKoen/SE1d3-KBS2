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

        private StackPanel PropertyPanel { get; }
        private Car Car;
        private PropertySettings CarMaxSpeedProperties;
        private PropertySettings CarModelProperties;

        public CarInfoUserControl(Car car)
        {
            InitializeComponent();

            Car = car;
            LabelInfoCarID.Content = car.Id;
            LabelInfoCarModel.Content = car.Model.Name;
            LabelInfoCustomerCount.DataContext = car;
            LabelInfoDestinationCar.DataContext = car;
            LabelInfoLocationCar.DataContext = car;
            LabelInfoDistanceTraveled.DataContext = car;

            PropertyPanel = this.StackPanelCar;

            DetermineCarPicture(car);
            DisplayProperties();
        }

        /// <summary>
        /// creating and displaying the properties of car.
        /// </summary>
        private void DisplayProperties()
        {
            var propertyName = "Max Speed";
            var propertyValue = Car.MaxSpeed.ToString();

            CarMaxSpeedProperties = new PropertySettings(propertyName, propertyValue);
            PropertyPanel.Children.Add(CarMaxSpeedProperties);

            //var propertyName2 = "Model";
            //var propertyValue2 = Car.Model;

            //CarModelProperties = new PropertySettings(propertyName2, propertyValue);
            //PropertyPanel.Children.Add(CarModelProperties);
        }

        /// <summary>
        /// Takes the last character of car Id and that char determines the picture for that specific car.
        /// </summary>
        /// <param name="car"></param>
        private void DetermineCarPicture(Car car)
        {
            var last = int.Parse(car.Id.ToString()[car.Id.ToString().Length - 1].ToString());
            var car1 = new BitmapImage(new Uri($@" /KBS2;component/Images/CarPicture1.jpg", UriKind.Relative));
            var car2 = new BitmapImage(new Uri($@" /KBS2;component/Images/CarPicture2.jpg", UriKind.Relative));
            var car3 = new BitmapImage(new Uri($@" /KBS2;component/Images/CarPicture3.jpg", UriKind.Relative));
            CarPicture.Source = (last >= 0 && last < 4) ? car1 : (last >= 4 && last < 7) ? car2 : car3;
        }

        /// <summary>
        /// If any car property changed this method will save the new values.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCarSave_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (Car.MaxSpeed.ToString() != CarMaxSpeedProperties.TBCurrentValue.Text.ToString())
            {
                var newValue = double.Parse(CarMaxSpeedProperties.TBCurrentValue.Text.ToString());
                Car.MaxSpeed = newValue;
            }
        }
    }
}
