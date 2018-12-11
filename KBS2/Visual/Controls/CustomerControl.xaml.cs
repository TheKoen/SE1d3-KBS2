using KBS2.CustomerSystem;
using KBS2.Windows;
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
    /// Interaction logic for CustomerControl.xaml
    /// </summary>
    public partial class CustomerControl : UserControl
    {

        public Customer customer;
        public MainScreen Screen { get; set; }

        public CustomerControl(Customer customer, MainScreen screen)
        {
            Screen = screen;
            this.customer = customer;
            Update();
            InitializeComponent();
            MainScreen.WPFLoop.Subscribe(Update);
        }

        public void Customer_Select(object sender, MouseButtonEventArgs e)
        {
            //Empty info tab
            Screen.TabItemInfo.Content = null;

            //Add info about this customer
            CustomerInfoUserControl ci = new CustomerInfoUserControl(customer);
            Screen.TabItemInfo.Content = ci;

            //Open the info tab of selected customer
            Screen.TabItemInfo.IsSelected = true;
        }

        public void Update()
        {
            Margin = new Thickness(customer.Location.X, customer.Location.Y, 0, 0);
        }
    }
}
