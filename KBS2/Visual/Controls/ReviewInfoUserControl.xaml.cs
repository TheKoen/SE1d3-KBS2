using KBS2.CustomerSystem;
using System;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace KBS2.Visual.Controls
{
    /// <summary>
    /// Interaction logic for ReviewInfoUserControl.xaml
    /// </summary>
    public partial class ReviewInfoUserControl : UserControl
    {
        public ReviewInfoUserControl(Review review, int number)
        {
            var customer = review.CustomerData;
            InitializeComponent();

            var FirstName = customer.Name.Split(' ')[0];
            var FirstLetter = FirstName.Substring(0, 1).ToUpper() + ".";
            var LastName = customer.Name.Split(' ')[1];

            LabelCustomerName.Content = FirstLetter + " " + LastName;
            LabelDate.Content = review.Date.Date.ToString("d");
            LabelReviewNr.Content = $"#{number}";


            switch (review.Rating)
            {
                case 1:
                    LabelRating.Content = "★";
                    break;
                case 2:
                    LabelRating.Content = "★★";
                    break;
                case 3:
                    LabelRating.Content = "★★★";
                    break;
                case 4:
                    LabelRating.Content = "★★★★";
                    break;
                case 5:
                    LabelRating.Content = "★★★★★";
                    break;
                default:
                    LabelRating.Foreground = System.Windows.Media.Brushes.Black;
                    LabelRating.Content = "No rating given.";
                    break;
            }

            
            TBContent.Text = review.Content;

            Uri profilePicturePath = null;

            if (customer.Gender == "Female" || customer.Gender == "Male")
            {
                profilePicturePath = customer.Gender == "Female"
                ? new Uri(@" /KBS2;component/Images/Female_Profile_Picture.jpg", UriKind.Relative)
                : new Uri(@" /KBS2;component/Images/Male_Profile_Picture.jpg", UriKind.Relative);
            }
            else
            {
                profilePicturePath = new Uri(@" /KBS2;component/Images/Other_Profile_Picture.png", UriKind.Relative);
            }

            ProfilePicture.Source = new BitmapImage(profilePicturePath);

        }
    }
}
