namespace KBS2.CarSystem.Sensors
{
    public abstract class PassiveSensor : Sensor {

        public PassiveSensor(Car car, Direction direction) : base(car, direction) { }
    }
}