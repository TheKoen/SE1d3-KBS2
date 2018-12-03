using System;
using KBS2.CarSystem.Sensors;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows;
using KBS2.CarSystem.Sensors.ActiveSensors;
using KBS2.CarSystem.Sensors.PassiveSensors;
using KBS2.Exceptions;

namespace KBS2.CarSystem
{
    [Serializable]
    public struct SensorPrototype : ISerializable
    {
        public Direction Direction;
        public int Range;
        public CreateSensor Create;

        public SensorPrototype(SerializationInfo info, StreamingContext context)
        {
            Direction = (Direction) info.GetValue("direction", typeof(Direction));
            Range = (int) info.GetValue("range", typeof(int));
            Create = (CreateSensor) info.GetValue("create", typeof(CreateSensor));
        }
        
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("direction", Direction, typeof(Direction));
            info.AddValue("range", Range, typeof(int));
            info.AddValue("create", Create, typeof(CreateSensor));
        }
    }

    public class CarModel
    {
        
        private static readonly List<CarModel> CarModelList = new List<CarModel>
        {
            new CarModel(100, new List<SensorPrototype>
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
            }, "TestModel")
        };

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
            if (!Contains(name))
                throw new KeyNotFoundException($"Car model \"{name}\" does not exist");
            return CarModelList.Last(m => m.Name.Equals(name));
        }

        public static void Set(CarModel model)
        {
            if (Contains(model.Name))
                throw new KeyExistsException($"Car model \"{model.Name}\" already exists");
            CarModelList.Add(model);
        }

        public static bool Contains(string name)
        {
            return CarModelList.Select(m => m.Name).Contains(name);
        }

        public static void Remove(string name)
        {
            if (!Contains(name))
                throw new KeyNotFoundException($"Car model \"{name}\" does not exist");
            CarModelList.RemoveAll(m => m.Name == name);
        }
    }
}