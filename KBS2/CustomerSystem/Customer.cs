using System.Windows;
using KBS2.CitySystem;

namespace KBS2.CustomerSystem
{
    public class Customer
    {
        public string Name { get; set; }
        public Vector Location { get; set; }
        public int Moral { get; set; }
        public CustomerController Controller { get; set; }
        public int Age { get; set; }
        public Building Building { get; set; }

        public Customer(Vector location, int age, Building building)
        {
            Location = location;
            Age = age;
            Building = building;
            Moral = 10;
            Controller = new CustomerController();
        }
    }
}
