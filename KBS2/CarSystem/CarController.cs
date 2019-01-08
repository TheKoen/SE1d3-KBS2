using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using KBS2.CarSystem.Sensors;
using KBS2.CarSystem.Sensors.ActiveSensors;
using KBS2.CarSystem.Sensors.PassiveSensors;
using KBS2.Database;
using KBS2.GPS;
using KBS2.Util;
using City = KBS2.CitySystem.City;
using Vector = System.Windows.Vector;

namespace KBS2.CarSystem
{
    public delegate void TripEndEvent(object source, TripEventArgs args);

    public class CarController
    {
        /*
         * Do not change these constants unless you really know what you're doing.
         * They significantly affect the way the car AI operates and are an easy
         * way to break everything.
         */

        // Acceleration is calculated by 1 / accelerationDivider
        private const double accelerationDivider = 40.0;

        // Decceleration is calculated by acceleration * brakingMultiplier
        private const double brakingMultiplier = 1.2;

        // Maximum angle the car can rotate in a lane (in degrees).
        private const double maxInLaneRotation = 10.0;

        // Maximum amount the car can deviate from the lane center.
        private const double maxLaneDeviation = 0.5;

        // Speed at which the car will rotate (in degrees per tick).
        private const double rotationSpeed = 2.0;

        // Distance the car needs to brake.
        private const double brakingDistance = 40.0;

        // Maximum speed while turning.
        private const double maxTurningSpeed = 0.3;
        
        public static event TripEndEvent TripEnd;

        public Car Car { get; set; }

        private bool initialized;
        private bool braking;
        private bool obtainedNewTarget;
        private int turning;
        private Vector newTarget;
        private bool flipping;
        private bool approach;
        private bool resetTarget;

        public CarController(Car car)
        {
            Car = car;
        }

        public void Init()
        {
            initialized = true;
        }

        /// <summary>
        /// Get all the sensors of type T on a specific side.
        /// </summary>
        /// <typeparam name="T">Type of the Sensor. Must extend Sensor</typeparam>
        /// <param name="side">Side on which the sensors must be installed</param>
        /// <returns>A list with Sensors of type T</returns>
        public List<T> GetSensors<T>(Direction side = Direction.Global) where T : Sensor
        {
            return Car.Sensors
                .FindAll(sensor => sensor.GetType() == typeof(T) && sensor.Direction.Equals(side))
                .ConvertAll(sensor => (T) sensor)
                .ToList();
        }

        /// <summary>
        /// Check if there are any sensors of type T on a specific side.
        /// </summary>
        /// <typeparam name="T">Type of the Sensor. Must extend Sensor</typeparam>
        /// <param name="side">Side on which the sensors must be installed</param>
        /// <returns>true if there are any Sensors of type T on the specified side</returns>
        public bool HasSensors<T>(Direction side = Direction.Global) where T : Sensor
        {
            return Car.Sensors
                .Any(sensor => sensor.GetType() == typeof(T) && sensor.Direction.Equals(side));
        }

        public void PassengersReady() { }

        /// <summary>
        /// Runs the main car logic. Needs to be subscribed to the MainLoop.
        /// </summary>
        public void Update()
        {
            // If we're not initialized, initialize!
            if (!initialized)
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
                if (!resetTarget)
                {
                    resetTarget = true;
                    var closestRoad = GPSSystem.NearestRoad(Car.Location);
                    if (closestRoad != null && Car != null)
                    {
                        Car.CurrentTarget =
                            MathUtil.Distance(closestRoad.End, Car.Location) >
                            MathUtil.Distance(closestRoad.Start, Car.Location)
                                ? closestRoad.Start
                                : closestRoad.End;
                        App.Console.Print($"[C{Car.Id}] Lost road, trying to get back on, targeting {Car.CurrentTarget}", Colors.Blue);
                    };
                }
            }
            else if (resetTarget)
            {
                App.Console.Print($"[C{Car.Id}] Road found again", Colors.Blue);
                resetTarget = false;
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
            if (distanceToTarget < 20 && !closeToDestination && !resetTarget)
            {
                // Check if we've not obtained a new target yet.
                if (!obtainedNewTarget)
                {
                    // Find the nearest intersection.
                    var intersection = GPSSystem.FindIntersection(GetClosestRoadPoint(Car.Location));
                    if (intersection == null) return;

                    App.Console.Print($"[C{Car.Id}] Requesting new target from intersection {intersection.Location}...", Colors.Blue);

                    // Request the next target from the GPSSystem.
                    var target = GPSSystem.GetDirection(Car, intersection);

                    // Update our target.
                    newTarget = target.Location;
                    obtainedNewTarget = true;

                    var distance = Math.Round(MathUtil.Distance(newTarget, Car.Location));
                    App.Console.Print($"[C{Car.Id}] Obtained a new target {newTarget} ({distance}m away)", Colors.Blue);
                }
            }
            else
            {
                obtainedNewTarget = false;
            }

            // Check if we are turning.
            if (turning > 0)
            {
                turning--;

                // Check if we locked on the new target yet.
                if (newTarget.X > -1)
                {
                    // Lock on the new target and reset newTarget.
                    Car.CurrentTarget = newTarget;
                    newTarget = new Vector(-1, -1);
                    App.Console.Print($"[C{Car.Id}] Locked on to target", Colors.Blue);
                }

                // Call the handle function to turn.
                HandleTurn(ref velocity, ref yaw, ref addedRotation);
            }
            // Check if we're still turning around.
            else if (flipping)
            {
                // Stop turning around.
                flipping = false;
                App.Console.Print($"[C{Car.Id}] Turn-around done", Colors.Blue);
            }

            // Check if we're on an intersection.
            if (Car.CurrentIntersection != null)
            {
                // Reset our turn timer.
                turning = 20;
            }
            else
            {
                // Check if we're not close to the destination.
                if (!closeToDestination)
                {
                    // If we're still approaching, stop approaching.
                    if (approach)
                    {
                        approach = false;
                        App.Console.Print($"[C{Car.Id}] Approach done", Colors.Blue);
                    }

                    // Call the handle functions to stay in the lane and accelerate/deccelerate.
                    HandleStayInLane(ref velocity, ref yaw, ref addedRotation);
                    HandleAccelerate(ref velocity, ref distanceToTarget);
                }
                else
                {
                    // If we're not approaching, start approaching.
                    if (!approach)
                    {
                        approach = true;
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
        /// Makes the car turn on an intersection towards the target.
        /// </summary>
        /// <param name="velocity">Current velocity of the car</param>
        /// <param name="yaw">Current relative yaw of the car</param>
        /// <param name="addedRotation">Amount of rotation (in degrees) to add</param>
        public void HandleTurn(ref Vector velocity, ref double yaw, ref double addedRotation)
        {
            // Get the current speed and rotation of the car.
            var speed = velocity.Length;
            var rotation = MathUtil.VelocityToRotation(velocity);
            var intersection = Car.CurrentIntersection;

            // If we're going too fast to turn, slow down.
            if (speed > maxTurningSpeed)
            {
                velocity = Vector.Add(velocity, CalculateDeccelerationVector(velocity));
            }

            // If we're not on the intersection yet, just keep driving straight.
            if (intersection == null)
            {
                HandleStayInLane(ref velocity, ref yaw, ref addedRotation);
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
                if (!flipping)
                {
                    addedRotation += rotationSpeed * 0.8;
                }
                // If we are turning around, keep steering steeply left.
                else
                {
                    addedRotation += rotationSpeed * 6;
                }
            }
            // If the angle is more than 135 degrees (180 - 45), turn around.
            else if (angle > 135 && angle < 315)
            {
                if (!flipping)
                {
                    flipping = true;
                    App.Console.Print($"[C{Car.Id}] Initiating turn-around", Colors.Blue);
                }

                addedRotation += rotationSpeed * 6;
            }
            // If the angle is more than 45 degrees (90 - 45), rotate to the right.
            else if (angle > 45 && angle < 315)
            {
                addedRotation -= rotationSpeed * 6;
            }
            // Otherwise, keep going straight.
            else
            {
                addedRotation = yaw < 0 ? rotationSpeed : -rotationSpeed;
            }
        }

        /// <summary>
        /// Makes the car approach the target and slow down.
        /// </summary>
        /// <param name="velocity">Current velocity of the car</param>
        /// <param name="yaw">Current relative yaw of the car</param>
        /// <param name="addedRotation">Amount of rotation (in degrees) to add</param>
        /// <param name="distanceToDestination">Distance to the destination</param>
        public void HandleApproachTarget(ref Vector velocity, ref double yaw, ref double addedRotation, ref double distanceToDestination)
        {
            // Get the current speed and rotation of the car.
            var speed = velocity.Length;
            var rotation = MathUtil.VelocityToRotation(velocity);

            // Check if we're going too fast, have passengers or are near the destination.
            if (speed > maxTurningSpeed / 2d || Car.PassengerCount > 0 || distanceToDestination < 20)
            {
                // Check if we need to drop passengers off.
                if (Car.PassengerCount > 0 && /*speed < 0.05 && */speed > 0)
                {
                    App.Console.Print($"[C{Car.Id}] Dropping customers", Colors.LimeGreen);
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
                if (yaw > -maxInLaneRotation)
                {
                    addedRotation -= rotationSpeed;
                }
            }
            // If we need to go to the left and are able to, do so.
            else
            {
                if (yaw < maxInLaneRotation)
                {
                    addedRotation += rotationSpeed;
                }
            }
        }

        /// <summary>
        /// Rotates the car in the right direction to stay in the lane.
        /// </summary>
        /// <param name="velocity">Current velocity of the car</param>
        /// <param name="yaw">Current relative yaw of the car</param>
        /// <param name="addedRotation">Amount of rotation (in degrees) to add</param>
        public void HandleStayInLane(ref Vector velocity, ref double yaw, ref double addedRotation)
        {
            // Check if the car has LineSensors on the left and right side.
            if (HasSensors<LineSensor>(Direction.Left) && HasSensors<LineSensor>(Direction.Right))
            {
                // Request the distance to the left and right side from the sensors.
                var distanceToLeft = GetSensors<LineSensor>(Direction.Left).First().Distance;
                var distanceToRight = GetSensors<LineSensor>(Direction.Right).First().Distance;

                // Check if we're too far to the right side of the lane.
                if (distanceToLeft - distanceToRight >= maxLaneDeviation)
                {
                    //Rotate to the left if possible.
                    if (yaw < maxInLaneRotation)
                    {
                        addedRotation += rotationSpeed;
                    }
                }
                // Check if we're too far to the left side of the lane.
                else if (distanceToRight - distanceToLeft >= maxLaneDeviation)
                {
                    //Rotate to the right if possible.
                    if (yaw > -maxInLaneRotation)
                    {
                        addedRotation -= rotationSpeed;
                    }
                }
                // If we're centered on the lane again, rotate the car straight.
                else if (Math.Abs(yaw) > 0.01)
                {
                    addedRotation = yaw < 0 ? rotationSpeed : -rotationSpeed;
                }
            }
        }

        /// <summary>
        /// Accelerates (or deccelerates) the car depending on the surroundings.
        /// </summary>
        /// <param name="velocity">Current velocity of the car</param>
        /// <param name="distanceToTarget">Distance to the current local target</param>
        public void HandleAccelerate(ref Vector velocity, ref double distanceToTarget)
        {
            // Get the current speed of the car.
            var speed = velocity.Length;

            // Check if we're far away enough from the target
            if (distanceToTarget > brakingDistance * Car.MaxSpeed / 2d)
            {
                if (!braking)
                {
                    // If we're not braking, accelerate the car.
                    if (Math.Abs(speed) < 0.001)
                    {
                        velocity = Vector.Divide(Car.Direction.GetVector(), accelerationDivider);
                    }
                    else if (speed < Car.MaxSpeed)
                    {
                        velocity = Vector.Add(velocity, CalculateAccelerationVector(velocity));
                    }
                }
            }
            else if (braking)
            {
                // If we are braking, deccelerate the car.
                velocity = speed > 0.1
                    ? Vector.Add(velocity, CalculateDeccelerationVector(velocity))
                    : new Vector();
            }
            else
            {
                // Otherwise we're in a turn, so slow down a little.
                if (speed > maxTurningSpeed)
                {
                    velocity = Vector.Add(velocity, CalculateDeccelerationVector(velocity));
                }
            }
        }

        private bool CloseToDestination()
        {
            var distanceToDestination = MathUtil.Distance(Car.Location, Car.Destination.Location);
            var target = Car.CurrentTarget;
            var destination = Car.Destination;

            return distanceToDestination < 45/* && MathUtil.Distance(target, destination.Location) < 10*/;
        }

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
        /// Calculates the vector used for acceleration using the current velocity.
        /// The current velocity is only used to detarmaine the direction.
        /// </summary>
        /// <param name="velocity">Current velocity of the car</param>
        /// <returns>Acceleration vector</returns>
        private static Vector CalculateAccelerationVector(Vector velocity)
        {
            var clone = new Vector(velocity.X, velocity.Y);
            clone.Normalize();
            return Vector.Divide(clone, accelerationDivider);
        }

        /// <summary>
        /// Calculates the vector used for decceleration using the current velocity.
        /// The current velocity is only used to detarmaine the direction.
        /// </summary>
        /// <param name="velocity">Current velocity of the car</param>
        /// <returns>Decceleration vector</returns>
        private static Vector CalculateDeccelerationVector(Vector velocity)
        {
            var acceleration = CalculateAccelerationVector(velocity);
            acceleration.Negate();
            return Vector.Multiply(acceleration, brakingMultiplier);
        }
    }
}