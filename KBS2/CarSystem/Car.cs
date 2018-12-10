using System.Collections.Generic;
using System.Windows;
using CommandSystem.PropertyManagement;
using KBS2.CarSystem.Sensors;
using KBS2.CitySystem;
using KBS2.CustomerSystem;
using KBS2.GPS;

namespace KBS2.CarSystem
{
    public class Car : IEntity
    {
        public const double DefaultMaxSpeed = 1.0;

        public int Id { get; set; }

        private Property location;
        public Vector Location
        {
            get => location.Value;
            set => location.Value = value;
        }

        private Property direction;
        public DirectionCar Direction
        {
            get => direction.Value;
            set => direction.Value = value;
        }

        private Property rotation;
        public Vector Rotation
        {
            get => rotation.Value;
            set => rotation.Value = value;
        }

        private Property maxSpeed;
        public double MaxSpeed
        {
            get => maxSpeed.Value;
            set => maxSpeed.Value = value;
        }

        private Property model;
        public CarModel Model
        {
            get => model.Value;
            set
            {
                model.Value = value;
            }
        }

        public Vector Velocity { get; set; } = new Vector(0, 0);

        public List<Sensor> Sensors { get; }
        public List<Customer> Passengers { get; }

        public Road CurrentRoad { get; set; }
        public Intersection CurrentIntersection { get; set; }
        public Vector CurrentTarget { get; set; }
        public Destination Destination { get; set; }

        public CarController Controller { get; }

        public int Width { get; set; }
        public int Length { get; set; }

        /// <summary>
        /// Create a car 
        /// </summary>
        /// <param name="id">Unique Id for this car</param>
        /// <param name="model">The model of this car</param>
        /// <param name="location">Location of this car</param>
        /// <param name="sensors">List with sensors for this car</param>
        /// <param name="direction">Direction the car is facing</param>
        /// <param name="width"></param>
        /// <param name="length"></param>
        public Car(int id, CarModel model, Vector location, List<Sensor> sensors, DirectionCar direction, int width, int length)
        {
            Id = id;
            Sensors = sensors;
            Width = width;
            Length = length;

            Passengers = new List<Customer>();
            CurrentRoad = GPSSystem.GetRoad(location);
            
            Controller = new CarController(this);
            MainScreen.AILoop.Subscribe(Controller.Update);

            this.location = new Property(location);
            //PropertyHandler.RegisterProperty($"car{id}.location", ref this.location);

            this.direction = new Property(direction);
            //PropertyHandler.RegisterProperty($"car{id}.direction", ref this.direction);

            rotation = new Property(direction.GetVector());
            //PropertyHandler.RegisterProperty($"car{id}.rotation", ref rotation);

            maxSpeed = new Property(DefaultMaxSpeed);
            PropertyHandler.RegisterProperty($"car{id}.maxSpeed", ref maxSpeed);

            this.model = new Property(model);
            //PropertyHandler.RegisterProperty($"car{id}.model", ref this.model);
        }

        public Vector GetLocation()
        {
            return Location;
        }

        public List<Vector> GetPoints()
        {
            if(Direction == DirectionCar.North || Direction == DirectionCar.South)
            {
                return new List<Vector>
                {
                    new Vector(Location.X + Width / 2.0, Location.Y),
                    new Vector(Location.X - Width / 2.0, Location.Y),
                    new Vector(Location.X, Location.Y + Length / 2.0),
                    new Vector(Location.X, Location.Y - Length / 2.0)
                };
            }

            return new List<Vector>
            {
                new Vector(Location.X + Length / 2.0, Location.Y),
                new Vector(Location.X - Length / 2.0, Location.Y),
                new Vector(Location.X, Location.Y + Width / 2.0),
                new Vector(Location.X, Location.Y - Width / 2.0)
            };
        }
    }
}
