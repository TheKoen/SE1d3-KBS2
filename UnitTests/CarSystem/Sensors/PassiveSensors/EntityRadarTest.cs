﻿using KBS2.CarSystem;
using KBS2.CarSystem.Sensors.ActiveSensors;
using KBS2.CitySystem;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using UnitTests.Util;

namespace UnitTests.CarSystem.Sensors.PassiveSensors
{
    [TestFixture]
    class EntityRadarTest
    {
        [TestCase(0, 0, 50, 0, 100)] // in front
        [TestCase(0, 0, -50, 0, 100)] // behind
        [TestCase(0, 0, 0, 50, 100)] // left
        [TestCase(0, 0, 0, -50, 100)] // right
        public void TestEntityRadarInRange(double carX, double carY, double secCarX, double secCarY, double range)
        {
            var city = new CityBuilder()
                .Road(new Vector(0, 50), new Vector(10, 50), 10)
                .Road(new Vector(0, 20), new Vector(10, 20), 10)
                .Build();
            var sensor = new EntityRadar(range);
            city.Cars.Add(new CarBuilder()
                .Location(new Vector(carX, carY))
                .Direction(DirectionCar.East)
                .Sensor(sensor)
                .Build());
            var secCar = (new CarBuilder()
                .Location(new Vector(secCarX, secCarY))
                .Direction(DirectionCar.East)
                .Build());
            city.Cars.Add(secCar);

            sensor.Controller.Update();

            if(sensor.EntitiesInRange.Find(car => car.Equals((IEntity)secCar)).Equals((IEntity)secCar))
            {
                Assert.Pass();
            }
            else
            {
                Assert.Fail();
            }
            
        }

        [TestCase(0,0, 100, 0, 50)] // front out of range
        [TestCase(0, 0, -100, 0, 50)] // behind out of range
        [TestCase(0, 0, 0, 100, 50)] // left out of range
        [TestCase(0, 0, 0, -100, 50)] // right out of range
        public void TestEntityRadarOutOfRange(double carX, double carY, double secCarX, double secCarY, double range)
        {
            var city = new CityBuilder()
                .Road(new Vector(0, 50), new Vector(10, 50), 10)
                .Road(new Vector(0, 20), new Vector(10, 20), 10)
                .Build();
            var sensor = new EntityRadar(range);
            city.Cars.Add(new CarBuilder()
                .Location(new Vector(carX, carY))
                .Direction(DirectionCar.East)
                .Sensor(sensor)
                .Build());
            var secCar = (new CarBuilder()
                .Location(new Vector(secCarX, secCarY))
                .Direction(DirectionCar.East)
                .Build());
            city.Cars.Add(secCar);

            sensor.Controller.Update();

            if (sensor.EntitiesInRange.Count > 0)
            {
                Assert.Fail();
            }
            else
            {
                Assert.Pass();
            }

        }

    }
}
