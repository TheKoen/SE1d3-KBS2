﻿using KBS2.CustomerSystem;
using KBS2.Visual;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
            InitializeComponent();
            Update();
            MainScreen.DrawingLoop.Subscribe(Update);
        }

        public void Customer_Select(object sender, MouseButtonEventArgs e)
        {
            //Empty info tab
            Screen.TabItemInfo.Content = null;

            //Add info about this customer
            var ci = new CustomerInfoUserControl(customer);
            Screen.TabItemInfo.Content = ci;

            //Open the info tab of selected customer
            Screen.TabItemInfo.IsSelected = true;
        }

        public void Update()
        {
            var zoom = Screen.Zoom;
            Margin = new Thickness(customer.Location.X * zoom, customer.Location.Y * zoom, 0, 0);
            Height = 20 * zoom;
            Width = 20 * zoom;
            EllipseCustomer.Height = 8 * zoom;
            EllipseCustomer.Width = 8 * zoom;
        }
    }
}
