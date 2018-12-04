using KBS2.CitySystem;
using KBS2.CustomerSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace KBS2.GPS.TSP.CustomerOrder
{
    public abstract class CustomerOrderAlgoritme
    {
        protected abstract List<CustomerGroup> CalculateOrder(Vector start, List<CustomerGroup> customers);
    }
}
