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

        public double WhereIsMyGroup()
        {
            return VectorUtil.Distance(Customer.Group.Location, Customer.Location);
        }

        public void Update() {
            var gL = Customer.Group.Location;
            var cL = Customer.Location;

            if (WhereIsMyGroup() > 40)
            {
                Vector delta = new Vector(gL.X - cL.X, gL.Y - cL.X);
                delta.Normalize();
                Customer.Location = Vector.Add(Customer.Location, delta);
            };
        }
    }
}
