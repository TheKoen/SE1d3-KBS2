using System.Collections.Generic;
using System.Windows;

namespace KBS2.CitySystem
{
    public interface IEntity   
    {
        Vector GetLocation();

        List<Vector> GetPoints();
    }
}
