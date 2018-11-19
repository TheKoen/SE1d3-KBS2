﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using KBS2.CarSystem;
using KBS2.CarSystem.Sensors;

namespace UnitTests.Util
{
    public class CarBuilder
    {
        public static int ID = 0;

        private Vector location;
        private List<Sensor> sensors = new List<Sensor>();
        private DirectionCar direction;
        private CarModel model;

        public CarBuilder Location(Vector location)
        {
            this.location = location;
            return this;
        }

        public CarBuilder Sensor(Sensor sensor)
        {
            sensors.Add(sensor);
            return this;
        }

        public CarBuilder Direction(DirectionCar direction)
        {
            this.direction = direction;
            return this;
        }

        public CarBuilder Model(CarModel model)
        {
            this.model = model;
            return this;
        }

        public Car Build()
        {
            if (location == null)
            {
                throw new ArgumentException("Location cannot be null");
            }
            return new Car(ID++, model, location, sensors, direction);
        }
    }
}
