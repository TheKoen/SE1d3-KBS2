using System.Collections.Generic;
using System.Windows;

namespace KBS2.CitySystem
{
    public interface IEntity   
    {
        /// <summary>
        /// get points of of an Entity
        /// </summary>
        /// <returns></returns>
        List<Vector> GetPoints();
    }
}
