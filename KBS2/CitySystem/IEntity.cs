using System.Collections.Generic;
using System.Windows;

namespace KBS2.CitySystem
{
    public interface IEntity   
    {
        /// <summary>
        /// get Location of a Entity
        /// </summary>
        /// <returns></returns>
        Vector GetLocation();

        /// <summary>
        /// get points of of an Entity
        /// </summary>
        /// <returns></returns>
        List<Vector> GetPoints();
    }
}
