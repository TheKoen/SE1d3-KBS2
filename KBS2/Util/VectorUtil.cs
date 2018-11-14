using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace KBS2.Util
{
    class VectorUtil
    {
        /// <summary>
        /// Calculates the distance between two coordinates.
        /// </summary>
        /// <param name="FirstPoint"></param>
        /// <param name="SecondPoint"></param>
        /// <returns></returns>
        public static double Distance(Vector FirstPoint, Vector SecondPoint)
        {
            return Math.Sqrt(Math.Pow(FirstPoint.X - SecondPoint.X, 2) + Math.Pow(FirstPoint.Y - SecondPoint.Y, 2));
        }
    }
}
