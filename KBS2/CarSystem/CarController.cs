using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using KBS2.CarSystem.Sensors;
using KBS2.CarSystem.Sensors.ActiveSensors;
using KBS2.CarSystem.Sensors.PassiveSensors;
using KBS2.GPS;
using KBS2.Util;

namespace KBS2.CarSystem
{
    public class CarController
    {
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

        // Maximum speed while driving normally.
        //private const double maxNormalSpeed = 1.0;

        // Maximum speed while turning.
        private const double maxTurningSpeed = 0.4;

        public Car Car { get; set; }

        private bool initialized;
        private bool braking;
        private bool obtainedNewTarget;
        private Vector newTarget;

        public CarController(Car car)
        {
            Car = car;
        }

        public void Init()
        {
            initialized = true;

            // If we have a CollisionSensor on the front, subscribe to it.
            if (HasSensors<CollisionSensor>(Direction.Front))
            {
                GetSensors<CollisionSensor>(Direction.Front).First().SubScribeSensorEvent(OnCollisionSensorDetect);
            }
        }

        private void OnCollisionSensorDetect(object source, SensorEventArgs args)
        {
            braking = true;
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

            // Update the current road with the road at our location.
            Car.CurrentRoad = GPSSystem.GetRoad(Car.Location);
            Car.CurrentIntersection = GPSSystem.FindIntersection(Car.Location);

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

            if (distanceToTarget < 20 && !closeToDestination)
            {
                if (!obtainedNewTarget)
                {
                    var intersection = GPSSystem.FindIntersection(GetClosestRoadPoint(Car.Location));
                    if (intersection == null)
                        return;

                    var target = GPSSystem.GetDirection(Car, intersection);
                    newTarget = target.Location;
                    App.Console.Print($"Obtained a new target: {newTarget}");
                    obtainedNewTarget = true;
                }
            }
            else
            {
                obtainedNewTarget = false;
            }

            if (Car.CurrentRoad == null || Car.CurrentIntersection != null)
            {
                if (newTarget.X > -1)
                {
                    Car.CurrentTarget = newTarget;
                    newTarget = new Vector(-1, -1);
                }
                HandleTurn(ref velocity, ref yaw, ref addedRotation);
            }
            else
            {
                // Check how far we are from our destination.
                if (!closeToDestination)
                {
                    // Call the handle functions to stay in the lane and accelerate/deccelerate.
                    HandleStayInLane(ref velocity, ref yaw, ref addedRotation);
                    HandleAccelerate(ref velocity, ref distanceToTarget);
                }
                else
                {
                    // Call the handle function to approach the target.
                    HandleApproachTarget(ref velocity, ref yaw, ref addedRotation, ref distanceToDestination);
                }
            }

            // Update the car's velocity with the result of the handle functions.
            velocity = MathUtil.RotateVector(velocity, -addedRotation);
            Car.Velocity = velocity;
            var rotation = new Vector(velocity.X, velocity.Y);
            rotation.Normalize();
            if (rotation.Length < 0.9 || double.IsNaN(rotation.Length) || velocity.Length < 0.1)
            {
                rotation = Car.Direction.GetVector();
            }

            Car.Rotation = rotation;
            Car.Direction = DirectionCarMethods.Parse(rotation);

            // Update the car's location with the velocity.
            Car.Location = Vector.Add(Car.Location, Car.Velocity);
            Car.DistanceTraveled += Car.Velocity.Length;
        }

        public void HandleTurn(ref Vector velocity, ref double yaw, ref double addedRotation)
        {
            // Get the current speed and rotation of the car.
            var speed = velocity.Length;
            var rotation = MathUtil.VelocityToRotation(velocity);

            // If we're going too fast to turn, slow down.
            if (speed > maxTurningSpeed)
            {
                velocity = Vector.Add(velocity, CalculateDeccelerationVector(velocity));
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
            if (angle > 225)
            {
                addedRotation += rotationSpeed * 0.8;
            }
            else if (angle > 135)
            {
                addedRotation += rotationSpeed * 90.0;
            }
            // If the angle is more than 45 degrees (90 - 45), rotate to the right.
            else if (angle > 45)
            {
                addedRotation -= rotationSpeed * 6;
            }
            // Otherwise, keep going straight.
            else
            {
                addedRotation = yaw < 0 ? rotationSpeed : -rotationSpeed;
            }
        }

        // TODO: Rework target approaching.
        public void HandleApproachTarget(ref Vector velocity, ref double yaw, ref double addedRotation, ref double distanceToDestination)
        {
            var speed = velocity.Length;
            var rotation = MathUtil.VelocityToRotation(velocity);
            
            if (speed > maxTurningSpeed / 2d || distanceToDestination < 20)
            {
                velocity = speed > 0.1
                    ? Vector.Add(velocity, CalculateDeccelerationVector(velocity))
                    : new Vector();
            }

            var destination = Car.Destination.Location;
            var location = Car.Location;

            var sub = new Vector(
                destination.X - location.X,
                destination.Y - location.Y
            );
            sub.Normalize();
            var angle = Vector.AngleBetween(rotation, sub);

            if (angle > 0)
            {
                if (yaw > -maxInLaneRotation)
                {
                    addedRotation -= rotationSpeed;
                }
            }
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

            return distanceToDestination < 20 && MathUtil.Distance(target, destination.Location) < 10;
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
                ? road.Start : road.End;
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