using System;
using KBS2.CarSystem.Sensors;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows;
using KBS2.CarSystem.Sensors.ActiveSensors;
using KBS2.CarSystem.Sensors.PassiveSensors;
using KBS2.CitySystem;
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
        
        #region Properties & Fields
        
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
        
        #endregion

        public CarModel(double maxSpeed, List<SensorPrototype> sensors, string name)
        {
            MaxSpeed = maxSpeed;
            Sensors = sensors;
            Name = name;
        }

        #region Public Methods
        
        /// <summary>
        /// Creates a <see cref="Car"/> from this <see cref="CarModel"/>
        /// </summary>
        /// <param name="id">Id of the new <see cref="Car"/></param>
        /// <param name="location">Location of the new <see cref="Car"/></param>
        /// <param name="garage"><see cref="Garage"/> the new <see cref="Car"/> will belong to</param>
        /// <param name="direction"><see cref="DirectionCar"/> it will face</param>
        /// <returns>The new <see cref="Car"/></returns>
        public Car CreateCar(int id, Vector location, Garage garage, DirectionCar direction)
        {
            var car = new Car(id, this, location, new List<Sensor>(), garage, direction, 5, 10);
            var sensors = Sensors
                .Select(prototype => prototype.Create(car, prototype.Direction, prototype.Range))
                .ToList();
            car.Sensors.AddRange(sensors);
            return car;
        }

        /// <summary>
        /// Gets a <see cref="CarModel"/> with the specified name
        /// </summary>
        /// <param name="name">Name of the <see cref="CarModel"/></param>
        /// <returns>The <see cref="CarModel"/> matching the name</returns>
        public static CarModel GetModel(string name)
        {
            if (!ModelExists(name))
                throw new KeyNotFoundException($"Car model \"{name}\" does not exist");
            return CarModelList.Last(m => m.Name.Equals(name));
        }

        public static IEnumerable<CarModel> GetAll() =>
            new List<CarModel>(CarModelList);

        public static void AddModel(CarModel model)
        {
            if (ModelExists(model.Name))
                throw new KeyExistsException($"Car model \"{model.Name}\" already exists");
            CarModelList.Add(model);
        }

        public static bool ModelExists(string name) =>
            CarModelList.Select(m => m.Name).Contains(name);
        
        #endregion
    }
}