using KBS2.Util;
using System;
using System.Windows;

namespace KBS2.CustomerSystem

{
    public class CustomerController
    {
        private static readonly Random Random = new Random();

        public Customer Customer { get; set; }
        private bool walking;
        private Vector direction;
        private int delay;

        public CustomerController(Customer customer)
        {
            Customer = customer;            
        }

        /// <summary>
        /// Looks at the position of the group and checks the distance between self and group.
        /// </summary>
        /// <returns>Distance between group and own location.</returns>
        public double WhereIsMyGroup()
        {
            if (Customer.Group == null) return 0;
            var distance = MathUtil.Distance(Customer.Group.Location, Customer.Location);
            return distance;
        }

        /// <summary>
        /// Moves the customer towards the give location.
        /// </summary>
        public bool MoveTowardsLocation(Vector location)
        {
            if (Customer.Group == null) return false;
            var cL = Customer.Location;

            var delta = new Vector(location.X - cL.X, location.Y - cL.Y);
            delta.Normalize();
            var target = Vector.Add(Customer.Location, delta);
            
            foreach(var road in Customer.Group.RoadsNear)
            {
                if (road.IsOnRoad(target)) return false;
            }

            Customer.Location = target;
            return true;
        }

        public void MoodChange(double moral)
        {
            if (moral > 20)
                Customer.Mood = Moral.Happy;
            else if (moral <= 20 && moral >= 15)
                Customer.Mood = Moral.Neutral;
            else if (moral <= 14 && moral >= 10)
                Customer.Mood = Moral.Annoyed;
            else if (moral <= 9 && moral >= 5)
                Customer.Mood = Moral.Sad;
            else if (moral <= 4 && moral >= 1)
                Customer.Mood = Moral.Mad;
            else
            {
                // Cancel order
            }
        }

        public void Update() {
            // After x time (moral <0 in 1 minute at -0.01 per tick) the customer moral decreases
            MoodChange(Math.Round(Customer.Moral));
            Customer.Moral = Customer.Moral - 0.01;
            if (Customer.Moral < 0) Customer.Moral = 0;

            if (delay > 0)
            {
                delay--;
                return;
            }
            
            if(walking && !direction.Equals(Customer.Location))
            {
                if (!MoveTowardsLocation(direction))
                {
                    walking = false;
                }

                walking = direction.Equals(Customer.Location);
            }
            else if (WhereIsMyGroup() >= 10)
            {
                MoveTowardsLocation(Customer.Group.Location);
            }
            else
            {
                if(Random.Next(0, 5) == 1)
                {
                    delay = Random.Next(50, 500);
                }
                else
                {
                    walking = true;
                    var x = Random.Next(-10, 10);
                    var y = Random.Next(-10, 10);
                    direction = new Vector(x, y);
                }
            }
        }
    }
}
