using System.Collections.Generic;
using System.Windows;
using KBS2.CarSystem.Sensors;
using KBS2.CitySystem;
using KBS2.Console;
using KBS2.CustomerSystem;
using KBS2.GPS;
using KBS2.Utilities;

namespace KBS2.CarSystem
{
    public class Car : IEntity
    {
        public const double DEFAULT_MAX_SPEED = 60;

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
            set => model.Value = value;
        }

        public Vector Velocity { get; set; } = new Vector(0, 0);

        public List<Sensor> Sensors { get; }
        public List<Customer> Passengers { get; }

        public Road CurrentRoad { get; set; }
        public Vector CurrentTarget { get; set; }

        public CarController Controller { get; }

        /// <summary>
        /// Create a car 
        /// </summary>
        /// <param name="id">Unique Id for this car</param>
        /// <param name="model">The model of this car</param>
        /// <param name="location">Location of this car</param>
        /// <param name="sensors">List with sensors for this car</param>
        /// <param name="direction">Direction the car is facing</param>
        public Car(int id, CarModel model, Vector location, List<Sensor> sensors, DirectionCar direction)
        {
            Id = id;
            Sensors = sensors;
            Passengers = new List<Customer>();
            CurrentRoad = GPSSystem.GetRoad(location);
            
            Controller = new CarController(this);
            MainWindow.Loop.Subscribe(Controller.Update);

            this.location = new Property(location);
            CommandHandler.RegisterProperty($"car{id}.location", ref this.location);

            this.direction = new Property(direction);
            CommandHandler.RegisterProperty($"car{id}.direction", ref this.direction);

            maxSpeed = new Property(DEFAULT_MAX_SPEED);
            CommandHandler.RegisterProperty($"car{id}.maxSpeed", ref maxSpeed);

            this.model = new Property(model);
            CommandHandler.RegisterProperty($"car{id}.model", ref this.model);
        }

        public Vector GetLocation()
        {
            return Location;
        }
    }
}
