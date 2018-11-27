using KBS2.CitySystem;
using KBS2.CustomerSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace KBS2.GPS.TSP
{
    public abstract class TSPAlgoritme
    {
        protected abstract List<Road> CalculatePath(Vector start, Vector end, List<CustomerGroup> customers);
    }
}
