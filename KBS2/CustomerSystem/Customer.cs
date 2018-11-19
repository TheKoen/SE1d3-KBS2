using System.Windows;
using KBS2.CitySystem;

namespace KBS2.CustomerSystem
{
    public class Customer
    {
        public string Name { get; set; }
        public Vector Location { get; set; }
        public double Moral { get; set; }
        public CustomerController Controller { get; set; }
        public int Age { get; set; }
        public Building Building { get; set; }
        public CustomerGroup Group { get; set; }
        public Moral Mood { get; set; }

        public Customer(Vector location, int age, Building building, CustomerGroup group)
        {
            Location = location;
            Age = age;
            Building = building;
            Moral = 25;
            Controller = new CustomerController(this);
            Group = group;
            Mood = CustomerSystem.Moral.Happy;
        }
    }
}
