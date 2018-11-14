using KBS2.Util;
using System;
using System.Windows;

namespace KBS2.CustomerSystem

{
    public class CustomerController
    {
        public Customer Customer { get; set; }
        private static Random Random = new Random();
        private bool Walking = false;
        private Vector Direction;
        private int Delay;

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
            return VectorUtil.Distance(Customer.Group.Location, Customer.Location);
        }

        /// <summary>
        /// Moves the customer towards the give location.
        /// </summary>
        public bool MoveTowardsLocation(Vector location)
        {
            Vector cL = Customer.Location;

            Vector delta = new Vector(location.X - cL.X, location.Y - cL.X);
            delta.Normalize();
            var target = Vector.Add(Customer.Location, delta);
            
            foreach( var road in Customer.Group.RoadsNear)
            {
                if (road.IsOnRoad(target)) return false;
            }

            Customer.Location = target;
            return true;
        }

        public void Update() { 
            if(Delay > 0)
            {
                Delay--;
                return;
            }

            if(Walking && !Direction.Equals(Customer.Location))
            {
                if (!MoveTowardsLocation(Direction))
                {
                    Walking = false;
                }

                Walking = Direction.Equals(Customer.Location) ? true : false;
            }
            else if (WhereIsMyGroup() >= 40)
            {
                MoveTowardsLocation(Customer.Group.Location);
            }
            else
            {
                if(Random.Next(0, 5) == 1)
                {
                    Delay = Random.Next(50, 500);
                }
                else
                {
                    Walking = true;
                    var x = Random.Next(-10, 10);
                    var y = Random.Next(-10, 10);
                    Direction = new Vector(x, y);
                }
            }
        }
    }
}
