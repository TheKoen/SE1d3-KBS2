using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBS2.Customer
{
    class CustomerGroupController
    {
        public CustomerGroup Group { get; set; }

        public CustomerGroupController(CustomerGroup group)
        {
            Group = group;
        }

        public void Update() {

        }
    }
}
