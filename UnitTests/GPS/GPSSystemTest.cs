﻿using KBS2.CarSystem;
using KBS2.CarSystem.Sensors;
using KBS2.CarSystem.Sensors.PassiveSensors;
using KBS2.CitySystem;
using KBS2.CustomerSystem;
using KBS2.GPS;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using UnitTests.Util;

namespace UnitTests.GPS
{
    [TestFixture]
    class GPSSystemTest
    {
        [TestCase(0, 0, 100, 0, 100, 100, 200, 100, 0, 50)] // get road location
        public void GetRoadTest(int x1b, int y1b, int x1e, int y1e, int x2b, int y2b, int x2e, int y2e, int sx, int sy)
        {
            var road1 = new Road(new Vector(x1b, y1b), new Vector(x1e, y1e), 20, 100);
            var road2 = new Road(new Vector(x2b, y2b), new Vector(x2e, y2e), 20, 100);

            var city = new CityBuilder()
                .Road(road1)
                .Road(road2)
                .Build();

            if (GPSSystem.GetRoad(new Vector(x1b, y1b)).Equals(road1))
            {
                Assert.Pass();
            }
            else
            {
                Assert.Fail();
            }  
        }
        
        [TestCase(0, 0, 0, 100, 100, 0, 100, 100, 0, 0)] // road 1 is nearest road
        [TestCase(0, 0, 0, 100, 0, 150, 0, 250, 0, 100)] //
        public void NearestRoadTest(int x1b, int y1b, int x1e, int y1e, int x2b, int y2b, int x2e, int y2e, int ex, int ey)
        {
            var road1 = new Road(new Vector(x1b, y1b), new Vector(x1e, y1e), 20, 100);
            var road2 = new Road(new Vector(x2b, y2b), new Vector(x2e, y2e), 20, 100);

            var city = new CityBuilder()
                .Road(road1)
                .Road(road2)
                .Build();

            var road = GPSSystem.NearestRoad(new Vector(ex, ey));

            Assert.AreEqual(road, road1);
        }

        [TestCase(20, 20, 20, 52.5, DirectionCar.East)]
        [TestCase(80, 80, 80, 47.5, DirectionCar.East)]
        public void RequestCarTest(double gX, double gY, double eX, double eY, DirectionCar direction)
        {
            var road = new Road(new Vector(0, 50), new Vector(100, 50), 10, 100);
            var garage1 = new Garage(new Vector(80, 20), 10, DirectionCar.East);
            var garage2 = new Garage(new Vector(20, 80), 10, DirectionCar.East);
            var building = new Building(new Vector(gX, gY), 1);

            var city = new CityBuilder()
                .Road(road)
                .Building(garage1)
                .Building(garage2)
                .Building(building)
                .Build();

            var group = new CustomerGroup(1, building, building);

            GPSSystem.RequestCar(new Destination { Road = road, Location = building.Location }, group);

            Assert.IsNotEmpty(city.Cars);

            var car = city.Cars[0];
            Assert.AreEqual(new Vector(eX, eY), car.Location);
            Assert.AreEqual(direction, car.Direction);
        }

        [TestCase(35, 85, DirectionCar.North, 90, 60, 2, 3, 0, 90, 75)]
        [TestCase(45, 5, DirectionCar.West, 45, 90, 5, 0, 3, 35, 70)] 
        [TestCase(35, 85, DirectionCar.North, 90, 60, 2, 2, 0, 102.5, 60)]
        public void GetDirection(double cX, double cY, DirectionCar direction, double dX, double dY, int road, int destRoad, int intersection, double eX, double eY)
        {
            var roads = new Road[]{
             new Road(new Vector(35, 80), new Vector( 35, 100), 10, 0),
             new Road(new Vector(0, 75), new Vector(30, 75), 10, 0),
             new Road(new Vector(40, 75), new Vector(90, 75), 10, 0),
             new Road(new Vector(95, 70), new Vector(95, 10), 10, 0),
             new Road(new Vector(40, 5), new Vector(90, 5), 10, 0),
             new Road(new Vector(35, 10), new Vector(35, 70), 10, 0)
            };

            var intersections = new Intersection[]
            {
                new Intersection(new Vector(35, 75), 10),
                new Intersection(new Vector(95, 75), 10),
                new Intersection(new Vector(95, 5), 10),
                new Intersection(new Vector(35, 5), 10)
            };
            
            var city = new CityBuilder()
                .Road(roads[0])
                .Road(roads[1])
                .Road(roads[2])
                .Road(roads[3])
                .Road(roads[4])
                .Road(roads[5])
                .Intersection(intersections[0])
                .Intersection(intersections[1])
                .Intersection(intersections[2])
                .Intersection(intersections[3])
                .Build();

            var car = new Car(1, CarModel.TestModel, new Vector(cX, cY), new List<Sensor>(), direction, 5, 5);
            city.Cars.Add(car);

            car.Destination = new Destination { Location = new Vector(dX, dY), Road = roads[destRoad] };

            var carDestination = GPSSystem.GetDirection(car, intersections[intersection]);
            
            Assert.AreEqual(eX, carDestination.Location.X, 0.01);
            Assert.AreEqual(eY, carDestination.Location.Y, 0.01);
            Assert.AreEqual(roads[road], carDestination.Road);
        }
    }
}