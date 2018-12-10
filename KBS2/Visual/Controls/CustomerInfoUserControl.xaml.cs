﻿using KBS2.CustomerSystem;
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
        public Customer Customer { get; set; }
        private Moral LastMood = Moral.Happy;
        public CustomerInfoUserControl(Customer customer)
        {
            InitializeComponent();
            Customer = customer;
            //All the information of the customer will be added.
            //Location and Destination will be updated if changed.
            LabelInfoName.Content = customer.Name;
            LabelInfoAge.Content = customer.Age;
            LabelInfoGender.Content = customer.Gender;
            LabelInfoLocation.DataContext = customer;
            LabelInfoDestination.DataContext = customer;

            //Depending on the gender of our customer, a profile picture will be selected to represent it.
            Uri profilePicturePath = customer.Gender == "Female"
                ? new Uri(@" /KBS2;component/Images/Female_Profile_Picture.jpg", UriKind.Relative)
                : new Uri(@" / KBS2;component/Images/Male_Profile_Picture.jpg", UriKind.Relative);


            
            //Switch case for moral and change picture depending on moral (:
            MoralImage.Source = new BitmapImage(new Uri(@"/KBS2;component/Images/happy.png", UriKind.Relative));
            ProfilePicture.Source = new BitmapImage(profilePicturePath);

            MainScreen.WPFLoop.Subscribe(Update);
        }

        private void Update()
        {
            if (Customer.Mood == LastMood) return;
            LastMood = Customer.Mood;

            switch (Customer.Mood)
            {
                case Moral.Happy:
                    MoralImage.Source = new BitmapImage(new Uri(@"/KBS2;component/Images/Happy.png", UriKind.Relative));
                    return;

                case Moral.Neutral:
                    MoralImage.Source = new BitmapImage(new Uri(@"/KBS2;component/Images/Neutral.png", UriKind.Relative));
                    return;

                case Moral.Annoyed:
                    MoralImage.Source = new BitmapImage(new Uri(@"/KBS2;component/Images/Annoyed.png", UriKind.Relative));
                    return;

                case Moral.Sad:
                    MoralImage.Source = new BitmapImage(new Uri(@"/KBS2;component/Images/Sad.png", UriKind.Relative));
                    return;

                case Moral.Mad:
                    MoralImage.Source = new BitmapImage(new Uri(@"/KBS2;component/Images/Mad.png", UriKind.Relative));
                    return;
            }
        }
    }
}