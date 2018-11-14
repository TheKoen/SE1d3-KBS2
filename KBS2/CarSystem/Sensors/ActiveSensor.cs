namespace KBS2.CarSystem.Sensors
{
    public delegate void SensorEventArgs();

    class ActiveSensor : Sensor
    {
        public event SensorEventArgs SensorEvent;


    }
}
