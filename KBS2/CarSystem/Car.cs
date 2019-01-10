using CommandSystem.PropertyManagement;
using KBS2.CarSystem.Sensors;
using KBS2.CitySystem;
using KBS2.CustomerSystem;
using KBS2.GPS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;

namespace KBS2.CarSystem
{
    public class Car : IEntity, INotifyPropertyChanged
    {
        public const double DefaultMaxSpeed = 1.0;

        #region Properties & Fields
        
        public int Id { get; set; }

        private readonly Property _location;
        public Vector Location
        {
            get => _location.Value;
            set
            {
                _location.Value = value;
                LocationString = $"{Location.X:F0}, {Location.Y:F0}";
            }
        }

        private string _locationString;

        public string LocationString
        {
            get => _locationString;
            private set
            {
                if (_locationString == value) return;

                _locationString = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("LocationString"));
            }
        }

        private Destination _destination = new Destination();
        public Destination Destination
        {
            get => _destination;
            set
            {
                _destination = value;
                if (_destination.Road == null) return;
                TargetLocationString = $"{Destination.Location.X:F0}, {Destination.Location.Y:F0}";
            }
        }

        private string _targetLocationString;

        public string TargetLocationString
        {
            get => _targetLocationString;
            private set
            {
                if (_targetLocationString == value) return;

                _targetLocationString = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TargetLocationString"));
            }
        }


        private readonly Property _direction;
        public DirectionCar Direction
        {
            get => _direction.Value;
            set => _direction.Value = value;
        }

        private readonly Property _rotation;
        public Vector Rotation
        {
            get => _rotation.Value;
            set => _rotation.Value = value;
        }

        private readonly Property _maxSpeed;
        public double MaxSpeed
        {
            get => _maxSpeed.Value;
            set => _maxSpeed.Value = value;
        }

        private double _distanceTraveled;
        public double DistanceTraveled
        {
            get => _distanceTraveled;
            set
            {
                _distanceTraveled = Math.Round(value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DistanceTraveled"));
            }
        }

        public double PassengersBoardDistance;

        private int _passengerCount;
        public int PassengerCount
        {
            get => _passengerCount;
            set
            {
                _passengerCount = Passengers.Count;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PassengerCount"));
            }
        }

        private readonly Property _model;

        public event PropertyChangedEventHandler PropertyChanged;

        public CarModel Model
        {
            get => _model.Value;
            set => _model.Value = value;
        }

        public Vector Velocity { get; set; } = new Vector(0, 0);

        public List<Sensor> Sensors { get; }

        public List<Customer> Passengers { get; }

        public List<Review> Reviews { get; set; }

        public Road CurrentRoad { get; set; }
        public Intersection CurrentIntersection { get; set; }
        public Vector CurrentTarget { get; set; }


        public Garage Garage { get; set; }

        public CarController Controller { get; }

        public int Width { get; set; }
        public int Length { get; set; }
        
        #endregion

        /// <summary>
        /// Create a <see cref="Car"/> 
        /// </summary>
        /// <param name="id">Unique Id for this <see cref="Car"/></param>
        /// <param name="model"><see cref="CarModel"/> of this <see cref="Car"/></param>
        /// <param name="location">Location of this <see cref="Car"/></param>
        /// <param name="sensors"><see cref="List{Sensor}"/> for this <see cref="Car"/></param>
        /// <param name="garage"><see cref="Garage"/> this <see cref="Car"/> belongs to</param>
        /// <param name="direction">Direction the <see cref="Car"/> is facing</param>
        /// <param name="width">Width of the <see cref="Car"/></param>
        /// <param name="length">Length of the <see cref="Car"/></param>
        public Car(int id, CarModel model, Vector location, List<Sensor> sensors, Garage garage, DirectionCar direction, int width, int length)
        {
            Id = id;
            Sensors = sensors;
            Garage = garage;
            Width = width;
            Length = length;

            Reviews = new List<Review>();
            Passengers = new List<Customer>();
            CurrentRoad = GPSSystem.GetRoad(location);

            Controller = new CarController(this);
            MainScreen.AILoop.Subscribe(Controller.Update);

            _location = new Property(location);
            //PropertyHandler.RegisterProperty($"car{id}.location", ref this.location);

            _direction = new Property(direction);
            //PropertyHandler.RegisterProperty($"car{id}.direction", ref this.direction);

            _rotation = new Property(direction.GetVector());
            //PropertyHandler.RegisterProperty($"car{id}.rotation", ref rotation);

            _maxSpeed = new Property(DefaultMaxSpeed);
            PropertyHandler.RegisterProperty($"car{id}.maxSpeed", ref _maxSpeed);

            _model = new Property(model);
            //PropertyHandler.RegisterProperty($"car{id}.model", ref this.model);
        }

        // TODO: Find appropriate summary
        public List<Vector> GetPoints()
        {
            if (Direction == DirectionCar.North || Direction == DirectionCar.South)
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
