﻿using KBS2.CitySystem;
using System.Collections.Generic;

namespace KBS2.CarSystem.Sensors.ActiveSensors
{
    public class CollisionSensor : ActiveSensor
    {
        public CollisionSensor(Direction direction, double range)
        {
            Range = range;
            SensorDirection = direction;

            Controller = new CollisionSensorController(this);
        }
    }
    
    class CollisionSensorController : SensorController
    {
        public CollisionSensor Sensor { get; set; }

        public CollisionSensorController(CollisionSensor sensor)
        {
            Sensor = sensor;
            


        }

        public override void Update()
        {
            var carDir = Sensor.Car.Direction;
            var sensorDir = CheckDirAuto(carDir);
            Sensor.Entities = (List<IEntity>)CheckEntitiesInRange(sensorDir);

            if(CheckEntitiesInRange(sensorDir).Count == 0)
            {
                return;
            }
            else
            {
                Sensor.DetectedEntities();
            }


        }

        /// <summary>
        /// Checks if there are any cars in the range of the sensor with the sensors direction
        /// </summary>
        /// <param name="sensorDir">the Direction of a sensor</param>
        /// <returns>a list of cars in range</returns>
        private List<IEntity> CheckEntitiesInRange(DirectionCar sensorDir)
        {
            switch(sensorDir)
            {
                case DirectionCar.North:
                    return City.Instance().Cars
                        .FindAll(car => car.Location.Y > Sensor.Car.Location.Y && (car.Location.Y - Sensor.Car.Location.Y) < Sensor.Range)
                        .ConvertAll(car => (IEntity)car);
                  
                case DirectionCar.South:
                    return City.Instance().Cars
                        .FindAll(car => car.Location.Y < Sensor.Car.Location.Y && (Sensor.Car.Location.Y - car.Location.Y) < Sensor.Range)
                        .ConvertAll(car => (IEntity)car);

                case DirectionCar.East:
                    return City.Instance().Cars
                        .FindAll(car => car.Location.X > Sensor.Car.Location.X && (car.Location.X - Sensor.Car.Location.X) < Sensor.Range)
                        .ConvertAll(car => (IEntity)car);

                case DirectionCar.West:
                    return City.Instance().Cars
                        .FindAll(car => car.Location.X < Sensor.Car.Location.X && (Sensor.Car.Location.X - car.Location.X) < Sensor.Range)
                        .ConvertAll(car => (IEntity)car);
                default:
                    throw new System.Exception("Error Check Entities in range");
            }
        }

        /// <summary>
        /// Converts the direction of the car into a direction of the sensor
        /// </summary>
        /// <param name="carDir">car direction</param>
        /// <returns>direction for the sensor</returns>
        private DirectionCar CheckDirAuto(DirectionCar carDir)
        {
            if (Sensor.SensorDirection == Direction.Front)
            {
                return carDir;
            }
            else if (Sensor.SensorDirection == Direction.Back)
            {
                if (carDir == DirectionCar.North)
                {
                    return DirectionCar.South;
                }
                if (carDir == DirectionCar.South)
                {
                    return DirectionCar.North;
                }
                if (carDir == DirectionCar.West)
                {
                    return DirectionCar.East;
                }
                if (carDir == DirectionCar.East)
                {
                    return DirectionCar.West;
                }
            }
            else if (Sensor.SensorDirection == Direction.Left)
            {
                if (carDir == DirectionCar.North)
                {
                    return DirectionCar.West;
                }
                if (carDir == DirectionCar.South)
                {
                    return DirectionCar.East;
                }
                if (carDir == DirectionCar.West)
                {
                    return DirectionCar.South;
                }
                if (carDir == DirectionCar.East)
                {
                    return DirectionCar.North;
                }
            }
            else if (Sensor.SensorDirection == Direction.Right)
            {
                if (carDir == DirectionCar.North)
                {
                    return DirectionCar.East;
                }
                if (carDir == DirectionCar.South)
                {
                    return DirectionCar.West;
                }
                if (carDir == DirectionCar.West)
                {
                    return DirectionCar.North;
                }
                if (carDir == DirectionCar.East)
                {
                    return DirectionCar.South;
                }
            }
            throw new System.Exception("Error check direction auto");
        }

    }
}
