using KBS2.CarSystem;
using System.Windows;

namespace KBS2.Visual.Controls
{
    /// <summary>
    /// Interaction logic for ReviewListWindow.xaml
    /// </summary>
    public partial class ReviewListWindow : Window
    {

        public ReviewListWindow(Car car)
        {
            InitializeComponent();

            if (car.Reviews != null)
            {
                var i = 0;
                foreach (var review in car.Reviews)
                {

                    ReviewInfoUserControl uc = new ReviewInfoUserControl(review, i);

                    ReviewListStackPanel.Children.Add(uc);
                    i++;
                }
            }
        }


        private void Window_Deactivated(object sender, System.EventArgs e)
        {
            try
            {
                this.Close();
            }
            catch (System.InvalidOperationException)
            {
                return;
            }
        }
    }
}
