using KBS2.Car.Sensors;

namespace KBS2.CarSystem.Sensors
{
    public abstract class Sensor
    {
        public double Range { get; set; }
        public Direction SensorDirection { get; set; }
        public CarSystem.Car Car { get; set; }
        public SensorController Controller { get; set; }
    }
}