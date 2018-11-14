using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBS2.Car.Sensors
{
    public abstract class SensorController
    {
        /// <summary>
        /// Updates the distance to a line of a lane
        /// </summary>
        public abstract void Update();
    }
}
