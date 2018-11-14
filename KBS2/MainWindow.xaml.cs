using KBS2.CitySystem;
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
using System.Xml;

namespace KBS2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public City City { get; set; }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click_AddCity(object sender, RoutedEventArgs e)
        {
            XmlDocument xmlCity = new XmlDocument();
            xmlCity.Load("testcity.xml");
            City = CityParser.MakeCity(xmlCity);
        }

        private void Button_Click_Coffee(object sender, RoutedEventArgs e)
        {
            CoffeeLabel.Content = "m";
        }
    }
}
