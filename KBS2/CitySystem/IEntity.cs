using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace KBS2.CitySystem
{
    public interface IEntity   
    {
        Vector GetLocation();

        List<Vector> GetPoints();
    }
}
