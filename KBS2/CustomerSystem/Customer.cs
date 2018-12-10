using System.Collections.Generic;
using System.Windows;
using KBS2.CitySystem;
using KBS2.Visual.Controls;

namespace KBS2.CustomerSystem
{
    public class Customer : IEntity
    {
        public string Name { get; set; }
        public string Gender { get; set; }
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
            Group = group;
            Mood = CustomerSystem.Moral.Happy;
            
            Controller = new CustomerController(this);

            MainScreen.AILoop.Subscribe(Controller.Update);
        }

        public Vector GetLocation()
        {
            return Location;
        }

        public List<Vector> GetPoints()
        {
            return new List<Vector>
            {
                Location
            };

        }
    }
}
