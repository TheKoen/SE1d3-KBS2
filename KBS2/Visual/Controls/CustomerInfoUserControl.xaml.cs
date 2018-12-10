using KBS2.CustomerSystem;
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
    /// Interaction logic for CustomerInfoUserControl.xaml
    /// </summary>
    public partial class CustomerInfoUserControl : UserControl
    {
        public CustomerInfoUserControl(Customer customer)
        {
            InitializeComponent();

            LabelInfoName.Content = customer.Name;
            LabelInfoAge.Content = customer.Age;
            LabelInfoGender.Content = customer.Gender;
            LabelInfoLocation.DataContext = customer;
            LabelInfoDestination.DataContext = customer;

            //Switch case for moral and change picture depending on moral (:
            MoralImage.Source = new BitmapImage(new Uri(@"/KBS2;component/Images/happy.png", UriKind.Relative));
            ProfilePicture.Source = new BitmapImage(new Uri(@"/KBS2;component/Images/Customer_Profile_Picture.png", UriKind.Relative));
        }
    }
}
