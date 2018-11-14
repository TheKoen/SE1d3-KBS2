using KBS2.Util;
using System.Windows;

namespace KBS2.CustomerSystem

{
    public class CustomerController
    {
        public Customer Customer { get; set; }

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
        /// Moves the customer towards the group.
        /// </summary>
        public void MoveTowardsGroup()
        {
            Vector gL = Customer.Group.Location;
            Vector cL = Customer.Location;

            Vector delta = new Vector(gL.X - cL.X, gL.Y - cL.X);
            delta.Normalize();
            Customer.Location = Vector.Add(Customer.Location, delta);
        }

        public void Update() {
            if (WhereIsMyGroup() > 40)
            {
                MoveTowardsGroup();
            };
        }
    }
}
