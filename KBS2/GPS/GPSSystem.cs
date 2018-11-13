using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBS2.GPS
{
    public class GPSSystem
    {
        private static GPSSystem instance;
                

        public static GPSSystem Instance()
        {
            return instance;
        }

        /// <summary>
        /// returns a road located at this location
        /// </summary>
        /// <param name="location">location you want to check for a road</param>
        /// <returns></returns>
        public static Road GetRoad(Vector location)
        {
            var roads = City.Instance().Roads;

            foreach(var road in roads)
            {
                if(road.IsXRoad())
                {
                    
                }
                else
                {

                }
            }
        }
        

    }
}
