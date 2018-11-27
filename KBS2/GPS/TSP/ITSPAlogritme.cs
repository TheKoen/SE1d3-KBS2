using KBS2.CitySystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace KBS2.GPS.TSP
{
    interface ITSPAlogritme
    {
        List<Road> CalculatePath(Vector start, Vector end)
    }
}
