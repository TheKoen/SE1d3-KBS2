using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBS2.Customer
{
    class CustomerGroup
    {
        public List<Customer> Customers { get; set; }
        public Building Destination { get; set; }
        public CustomerGroupController Controller { get; set; }
    }
}
