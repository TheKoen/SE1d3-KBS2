using KBS2.CarSystem.Sensors;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using KBS2.CarSystem.Sensors.ActiveSensors;
using KBS2.CarSystem.Sensors.PassiveSensors;

namespace KBS2.CarSystem
{
    public struct SensorPrototype
    {
        public Direction Direction;
        public int Range;
        public CreateSensor Create;
    }

    public class CarModel
    {
        public static readonly CarModel TestModel = new CarModel(100, new List<SensorPrototype>
        {
            new SensorPrototype
            {
                Range = 40,
                Create = Sensor.SENSORS[typeof(EntityRadar)]
            },
            new SensorPrototype
            {
                Direction = Direction.Front,
                Range = 20,
                Create = Sensor.SENSORS[typeof(CollisionSensor)]
            },
            new SensorPrototype
            {
                Direction = Direction.Left,
                Create = Sensor.SENSORS[typeof(LineSensor)]
            },
            new SensorPrototype
            {
                Direction = Direction.Right,
                Create = Sensor.SENSORS[typeof(LineSensor)]
            }
        });

        public List<SensorPrototype> Sensors { get; }
        public double MaxSpeed { get; }

        public CarModel(double maxSpeed, List<SensorPrototype> sensors)
        {
            MaxSpeed = maxSpeed;
            Sensors = sensors;
        }

        public Car CreateCar(int id, Vector location, DirectionCar direction)
        {
            var car = new Car(id, this, location, new List<Sensor>(), direction, 10, 5);
            var sensors = Sensors
                .Select(prototype => prototype.Create(car, prototype.Direction, prototype.Range))
                .ToList();
            car.Sensors.AddRange(sensors);
            return car;
        }

        public static CarModel Get(string name)
        {
            return (CarModel)typeof(CarModel).GetField(name).GetValue(null);
        }
    }
}