using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBS2.Car.Sensors
{
    public delegate void SensorEventArgs();

    class ActiveSensor : Sensor
    {
        public event SensorEventArgs SensorEvent;


    }
}
