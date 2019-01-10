using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using KBS2.CarSystem.Sensors;
using KBS2.CarSystem.Sensors.PassiveSensors;
using KBS2.Database;
using KBS2.GPS;
using KBS2.Util;
using Vector = System.Windows.Vector;

namespace KBS2.CarSystem
{
    public delegate void TripEndEvent(object source, TripEventArgs args);

    public class CarController
    {
        #region Constants
        
        /*
         * Do not change these constants unless you really know what you're doing.
         * They significantly affect the way the car AI operates and are an easy
         * way to break everything.
         */

        // Acceleration is calculated by 1 / accelerationDivider
        private const double AccelerationDivider = 40.0;

        // Decceleration is calculated by acceleration * brakingMultiplier
        private const double BrakingMultiplier = 1.2;

        // Maximum angle the car can rotate in a lane (in degrees).
        private const double MaxInLaneRotation = 10.0;

        // Maximum amount the car can deviate from the lane center.
        private const double MaxLaneDeviation = 0.5;

        // Speed at which the car will rotate (in degrees per tick).
        private const double RotationSpeed = 2.0;

        // Distance the car needs to brake.
        private const double BrakingDistance = 40.0;

        // Maximum speed while turning.
        private const double MaxTurningSpeed = 0.3;
        
        #endregion
        
        #region Properties & Fields
        
        public static event TripEndEvent TripEnd;

        public Car Car { get; }

        [Obsolete]
        private bool _initialized;
        [Obsolete]
        private bool _braking;
        private bool _obtainedNewTarget;
        private int _turning;
        private Vector _newTarget;
        private bool _flipping;
        private bool _approach;
        private bool _resetTarget;
        
        #endregion

        public CarController(Car car)
        {
            Car = car;
        }

        #region Public Methods
        
        public void Init()
        {
            _initialized = true;
        }

        /// <summary>
        /// Gets all the <see cref="Sensor"/>s of type <see cref="T"/> on a specific side.
        /// </summary>
        /// <typeparam name="T">Type of the <see cref="Sensor"/>; must extend <see cref="Sensor"/></typeparam>
        /// <param name="side">Side on which the <see cref="Sensor"/>s must be installed</param>
        /// <returns>A <see cref="List{T}"/> of <see cref="Sensor"/>s</returns>
        public List<T> GetSensors<T>(Direction side = Direction.Global) where T : Sensor
        {
            return Car.Sensors
                .FindAll(sensor => sensor.GetType() == typeof(T) && sensor.Direction.Equals(side))
                .ConvertAll(sensor => (T) sensor)
                .ToList();
        }

        /// <summary>
        /// Checks if there are any <see cref="Sensor"/>s of type <see cref="T"/> on a specific side.
        /// </summary>
        /// <typeparam name="T">Type of the <see cref="Sensor"/>; must extend <see cref="Sensor"/></typeparam>
        /// <param name="side">Side on which the <see cref="Sensor"/>s must be installed</param>
        /// <returns>True if there are any <see cref="Sensor"/>s of type <see cref="T"/> on the specified side</returns>
        public bool HasSensors<T>(Direction side = Direction.Global) where T : Sensor
        {
            return Car.Sensors
                .Any(sensor => sensor.GetType() == typeof(T) && sensor.Direction.Equals(side));
        }

        /// <summary>
        /// Sets properties for when a <see cref="CustomerGroup"/> has entered the <see cref="Car"/>
        /// </summary>
        public void PassengersReady()
        {
            Car.PassengersBoardDistance = Car.DistanceTraveled;
        }

        /// <summary>
        /// Runs the main car logic. Needs to be subscribed to the MainLoop.
        /// </summary>
        public void Update()
        {
            // If we're not initialized, initialize!
            if (!_initialized)
            {
                Init();
            }

            // Update the passenger count.
            Car.PassengerCount = Car.Passengers.Count;

            // Update the current road with the road at our location.
            Car.CurrentRoad = GPSSystem.GetRoad(Car.Location);
            Car.CurrentIntersection = GPSSystem.FindIntersection(Car.Location);

            if (Car.CurrentRoad == null && Car.CurrentIntersection == null)
            {
                if (!_resetTarget)
                {
                    _resetTarget = true;
                    var closestRoad = GPSSystem.NearestRoad(Car.Location);
                    if (closestRoad != null && Car != null)
                    {
                        Car.CurrentTarget =
                            MathUtil.Distance(closestRoad.End, Car.Location) >
                            MathUtil.Distance(closestRoad.Start, Car.Location)
                                ? closestRoad.Start
                                : closestRoad.End;
                        App.Console.Print($"[C{Car.Id}] Lost road, trying to get back on, targeting {Car.CurrentTarget}", Colors.Blue);
                    }
                }
            }
            else if (_resetTarget)
            {
                App.Console.Print($"[C{Car.Id}] Road found again", Colors.Blue);
                _resetTarget = false;
            }

            // Calculate the distance to the local target (usually the next intersection).
            var distanceToTarget = MathUtil.Distance(Car.Location, Car.CurrentTarget);
            // Calculate the distance to the destination.
            var distanceToDestination = MathUtil.Distance(Car.Location, Car.Destination.Location);
            // Calculate the relative yaw (in degrees).
            var yaw = MathUtil.VectorToAngle(Car.Rotation, Car.Direction);
            // Get the current velocity of the car.
            var velocity = Car.Velocity;
            // Create a variable to store the added rotation in this update call.
            var addedRotation = 0.0;
            var closeToDestination = CloseToDestination();

            // Check if we're close to our target but not the destination.
            if (distanceToTarget < 20 && !closeToDestination && !_resetTarget)
            {
                // Check if we've not obtained a new target yet.
                if (!_obtainedNewTarget)
                {
                    // Find the nearest intersection.
                    var intersection = GPSSystem.FindIntersection(GetClosestRoadPoint(Car.Location));
                    if (intersection == null) return;

                    App.Console.Print($"[C{Car.Id}] Requesting new target from intersection {intersection.Location}...", Colors.Blue);

                    // Request the next target from the GPSSystem.
                    var target = GPSSystem.GetDirection(Car, intersection);

                    // Update our target.
                    _newTarget = target.Location;
                    _obtainedNewTarget = true;

                    var distance = Math.Round(MathUtil.Distance(_newTarget, Car.Location));
                    App.Console.Print($"[C{Car.Id}] Obtained a new target {_newTarget} ({distance}m away)", Colors.Blue);
                }
            }
            else
            {
                _obtainedNewTarget = false;
            }

            // Check if we are turning.
            if (_turning > 0)
            {
                _turning--;

                // Check if we locked on the new target yet.
                if (_newTarget.X > -1)
                {
                    // Lock on the new target and reset newTarget.
                    Car.CurrentTarget = _newTarget;
                    _newTarget = new Vector(-1, -1);
                    App.Console.Print($"[C{Car.Id}] Locked on to target", Colors.Blue);
                }

                // Call the handle function to turn.
                HandleTurn(ref velocity, ref yaw, ref addedRotation);
            }
            // Check if we're still turning around.
            else if (_flipping)
            {
                // Stop turning around.
                _flipping = false;
                App.Console.Print($"[C{Car.Id}] Turn-around done", Colors.Blue);
            }

            // Check if we're on an intersection.
            if (Car.CurrentIntersection != null)
            {
                // Reset our turn timer.
                _turning = 20;
            }
            else
            {
                // Check if we're not close to the destination.
                if (!closeToDestination)
                {
                    // If we're still approaching, stop approaching.
                    if (_approach)
                    {
                        _approach = false;
                        App.Console.Print($"[C{Car.Id}] Approach done", Colors.Blue);
                    }

                    // Call the handle functions to stay in the lane and accelerate/deccelerate.
                    HandleStayInLane(ref yaw, ref addedRotation);
                    HandleAccelerate(ref velocity, ref distanceToTarget);
                }
                else
                {
                    // If we're not approaching, start approaching.
                    if (!_approach)
                    {
                        _approach = true;
                        App.Console.Print($"[C{Car.Id}] Initiating approach", Colors.Blue);
                    }

                    // Call the handle function to approach the target.
                    HandleApproachTarget(ref velocity, ref yaw, ref addedRotation, ref distanceToDestination);
                }
            }

            // Update the car's velocity with the result of the handle functions:
            // Temporarily store the current speed.
            var speed = velocity.Length;
            // Rotate the velocity based on the addedRotation this tick.
            velocity = MathUtil.RotateVector(velocity, -addedRotation);
            // Normalize the velocity and multiply with the speed to make sure it stays the same.
            velocity.Normalize();
            velocity = Vector.Multiply(velocity, speed);
            // Actually update the car's velocity.
            Car.Velocity = velocity;

            // Calculate the rotation by normalizing the velocity.
            var rotation = new Vector(velocity.X, velocity.Y);
            rotation.Normalize();

            // If the rotation vector is invalid or the car is not moving, set the rotation to the absolute direction.
            if (rotation.Length < 0.9 || double.IsNaN(rotation.Length) || velocity.Length < 0.1)
            {
                rotation = Car.Direction.GetVector();
            }

            // Actually update the rotation and update the absolute direction.
            Car.Rotation = rotation;
            Car.Direction = DirectionCarMethods.Parse(rotation);

            // Update the car's location by adding the velocity to it.
            Car.Location = Vector.Add(Car.Location, Car.Velocity);
            Car.DistanceTraveled += Car.Velocity.Length;
        }

        /*
         * All the Handle functions take references to variable from the Update function.
         * They will modify these variables to change behaviour of the car. The value of
         * these variables shouldn't be overwritten, but only changed (adding, multiplying, etc)
         */
        
        /// <summary>
        /// Makes the <see cref="Car"/> turn on an intersection towards the target.
        /// </summary>
        /// <param name="velocity">Current velocity of the <see cref="Car"/></param>
        /// <param name="yaw">Current relative yaw of the <see cref="Car"/></param>
        /// <param name="addedRotation">Amount of rotation (in degrees) to add</param>
        public void HandleTurn(ref Vector velocity, ref double yaw, ref double addedRotation)
        {
            // Get the current speed and rotation of the car.
            var speed = velocity.Length;
            var rotation = MathUtil.VelocityToRotation(velocity);
            var intersection = Car.CurrentIntersection;

            // If we're going too fast to turn, slow down.
            if (speed > MaxTurningSpeed)
            {
                velocity = Vector.Add(velocity, CalculateDeccelerationVector(velocity));
            }

            // If we're not on the intersection yet, just keep driving straight.
            if (intersection == null)
            {
                HandleStayInLane(ref yaw, ref addedRotation);
                return;
            }

            // Get the target and current location.
            var target = Car.CurrentTarget;
            var location = Car.Location;

            // Calculate the angle between the car and the target.
            var targetAngle = new Vector(
                target.X - location.X,
                target.Y - location.Y
            );
            var angle = Vector.AngleBetween(rotation, targetAngle);
            if (angle < 0) angle += 360;

            // If the angle is more than 225 degrees (-90 - 45), rotate to the left.
            if (angle > 225 && angle < 315)
            {
                // If we're not turning around, slowly turn left.
                if (!_flipping)
                {
                    addedRotation += RotationSpeed * 0.8;
                }
                // If we are turning around, keep steering steeply left.
                else
                {
                    addedRotation += RotationSpeed * 6;
                }
            }
            // If the angle is more than 135 degrees (180 - 45), turn around.
            else if (angle > 135 && angle < 315)
            {
                if (!_flipping)
                {
                    _flipping = true;
                    App.Console.Print($"[C{Car.Id}] Initiating turn-around", Colors.Blue);
                }

                addedRotation += RotationSpeed * 6;
            }
            // If the angle is more than 45 degrees (90 - 45), rotate to the right.
            else if (angle > 45 && angle < 315)
            {
                addedRotation -= RotationSpeed * 6;
            }
            // Otherwise, keep going straight.
            else
            {
                addedRotation = yaw < 0 ? RotationSpeed : -RotationSpeed;
            }
        }

        /// <summary>
        /// Makes the <see cref="Car"/> approach the target and slow down.
        /// </summary>
        /// <param name="velocity">Current velocity of the <see cref="Car"/></param>
        /// <param name="yaw">Current relative yaw of the <see cref="Car"/></param>
        /// <param name="addedRotation">Amount of rotation (in degrees) to add</param>
        /// <param name="distanceToDestination">Distance to the destination</param>
        public void HandleApproachTarget(ref Vector velocity, ref double yaw, ref double addedRotation, ref double distanceToDestination)
        {
            // Get the current speed and rotation of the car.
            var speed = velocity.Length;
            var rotation = MathUtil.VelocityToRotation(velocity);

            // Check if we're going too fast, have passengers or are near the destination.
            if (speed > MaxTurningSpeed / 2d || Car.PassengerCount > 0 || distanceToDestination < 20)
            {
                // Check if we need to drop passengers off.
                if (Car.PassengerCount > 0 && /*speed < 0.05 && */speed > 0)
                {
                    var distance = Car.DistanceTraveled - Car.PassengersBoardDistance;
                    var price = GPSSystem.CalculatePrice(distance);
                    App.Console.Print($"[C{Car.Id}] Charged €{price:F2} for {distance:F1} units", Colors.Blue);
                    
                    App.Console.Print($"[C{Car.Id}] Dropping customers", Colors.Blue);
                    var passenger = Car.Passengers[0];
                    TripEnd?.Invoke(this, new TripEventArgs(passenger.Group, passenger.Building.Location, Car.Location, Car));
                    Car.Passengers.Clear();
                    MainScreen.AILoop.Unsubscribe(Update);
                }

                // Slow down the car.
                velocity = speed > 0.05
                    ? Vector.Add(velocity, CalculateDeccelerationVector(velocity))
                    : new Vector();
            }

            // Get the target and current location.
            var destination = Car.Destination.Location;
            var location = Car.Location;

            // Calculate the angle between the car and the target.
            var sub = new Vector(
                destination.X - location.X,
                destination.Y - location.Y
            );
            sub.Normalize();
            var angle = Vector.AngleBetween(rotation, sub);

            // If we need to go to the right and are able to, do so.
            if (angle > 0)
            {
                if (yaw > -MaxInLaneRotation)
                {
                    addedRotation -= RotationSpeed;
                }
            }
            // If we need to go to the left and are able to, do so.
            else
            {
                if (yaw < MaxInLaneRotation)
                {
                    addedRotation += RotationSpeed;
                }
            }
        }

        /// <summary>
        /// Rotates the <see cref="Car"/> in the right direction to stay in the lane.
        /// </summary>
        /// <param name="yaw">Current relative yaw of the <see cref="Car"/></param>
        /// <param name="addedRotation">Amount of rotation (in degrees) to add</param>
        public void HandleStayInLane(ref double yaw, ref double addedRotation)
        {
            // Check if the car has LineSensors on the left and right side.
            if (!HasSensors<LineSensor>(Direction.Left) || !HasSensors<LineSensor>(Direction.Right)) return;
            // Request the distance to the left and right side from the sensors.
            var distanceToLeft = GetSensors<LineSensor>(Direction.Left).First().Distance;
            var distanceToRight = GetSensors<LineSensor>(Direction.Right).First().Distance;

            // Check if we're too far to the right side of the lane.
            if (distanceToLeft - distanceToRight >= MaxLaneDeviation)
            {
                //Rotate to the left if possible.
                if (yaw < MaxInLaneRotation)
                {
                    addedRotation += RotationSpeed;
                }
            }
            // Check if we're too far to the left side of the lane.
            else if (distanceToRight - distanceToLeft >= MaxLaneDeviation)
            {
                //Rotate to the right if possible.
                if (yaw > -MaxInLaneRotation)
                {
                    addedRotation -= RotationSpeed;
                }
            }
            // If we're centered on the lane again, rotate the car straight.
            else if (Math.Abs(yaw) > 0.01)
            {
                addedRotation = yaw < 0 ? RotationSpeed : -RotationSpeed;
            }
        }

        /// <summary>
        /// Changing the <see cref="Car"/>'s velocity depending on the surroundings.
        /// </summary>
        /// <param name="velocity">Current velocity of the <see cref="Car"/></param>
        /// <param name="distanceToTarget">Distance to the current local target</param>
        public void HandleAccelerate(ref Vector velocity, ref double distanceToTarget)
        {
            // Get the current speed of the car.
            var speed = velocity.Length;

            // Check if we're far away enough from the target
            if (distanceToTarget > BrakingDistance * Car.MaxSpeed / 2d)
            {
                if (_braking) return;
                // If we're not braking, accelerate the car.
                if (Math.Abs(speed) < 0.001)
                {
                    velocity = Vector.Divide(Car.Direction.GetVector(), AccelerationDivider);
                }
                else if (speed < Car.MaxSpeed)
                {
                    velocity = Vector.Add(velocity, CalculateAccelerationVector(velocity));
                }
            }
            else if (_braking)
            {
                // If we are braking, deccelerate the car.
                velocity = speed > 0.1
                    ? Vector.Add(velocity, CalculateDeccelerationVector(velocity))
                    : new Vector();
            }
            else
            {
                // Otherwise we're in a turn, so slow down a little.
                if (speed > MaxTurningSpeed)
                {
                    velocity = Vector.Add(velocity, CalculateDeccelerationVector(velocity));
                }
            }
        }
        
        #endregion

        #region Private Methods
        
        /// <summary>
        /// Checks if the <see cref="Car"/> is close to it's destination.
        /// </summary>
        /// <returns>True if the <see cref="Car"/> is close</returns>
        private bool CloseToDestination() =>
            MathUtil.Distance(Car.Location, Car.Destination.Location) < 45;

        /// <summary>
        /// Gets the closest <see cref="Vector"/> of a road using <see cref="GPSSystem"/>.
        /// </summary>
        /// <param name="location">Current location of the <see cref="Car"/></param>
        /// <returns>The <see cref="Vector"/> of the start- or endpoint of a road</returns>
        private static Vector GetClosestRoadPoint(Vector location)
        {
            var road = GPSSystem.GetRoad(location);
            if (road == null)
            {
                throw new ArgumentException("Location is not on a road!");
            }

            return GPSSystem.CalculateDistance(location, road.Start) <
                   GPSSystem.CalculateDistance(location, road.End)
                ? road.Start
                : road.End;
        }

        /// <summary>
        /// Calculates the <see cref="Vector"/> used for acceleration using the current velocity.
        /// The current velocity is only used to detarmaine the direction.
        /// </summary>
        /// <param name="velocity">Current velocity of the car</param>
        /// <returns>Acceleration <see cref="Vector"/></returns>
        private static Vector CalculateAccelerationVector(Vector velocity)
        {
            var clone = new Vector(velocity.X, velocity.Y);
            clone.Normalize();
            return Vector.Divide(clone, AccelerationDivider);
        }

        /// <summary>
        /// Calculates the <see cref="Vector"/> used for deceleration using the current velocity.
        /// The current velocity is only used to detarmaine the direction.
        /// </summary>
        /// <param name="velocity">Current velocity of the <see cref="Car"/></param>
        /// <returns>Decceleration <see cref="Vector"/></returns>
        private static Vector CalculateDeccelerationVector(Vector velocity)
        {
            var acceleration = CalculateAccelerationVector(velocity);
            acceleration.Negate();
            return Vector.Multiply(acceleration, BrakingMultiplier);
        }
        
        #endregion
    }
}