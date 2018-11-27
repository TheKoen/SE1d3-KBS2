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
                Direction = Direction.Global,
                Range = 40,
                Create = Sensor.Sensors[typeof(EntityRadar)]
            },
            new SensorPrototype
            {
                Direction = Direction.Front,
                Range = 20,
                Create = Sensor.Sensors[typeof(CollisionSensor)]
            },
            new SensorPrototype
            {
                Direction = Direction.Left,
                Create = Sensor.Sensors[typeof(LineSensor)]
            },
            new SensorPrototype
            {
                Direction = Direction.Right,
                Create = Sensor.Sensors[typeof(LineSensor)]
            }
        }, "TestModel");

        public List<SensorPrototype> Sensors { get; }
        public double MaxSpeed { get; }
        public string Name { get; }

        public CarModel(double maxSpeed, List<SensorPrototype> sensors, string name)
        {
            MaxSpeed = maxSpeed;
            Sensors = sensors;
            Name = name;
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