using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace KBS2.Customer
{
    class Customer
    {
        public Vector Location { get; set; }
        public int Moral { get; set; }
        public CustomerController Controller { get; set; }
        public int Age { get; set; }
    }
}
