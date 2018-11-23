﻿using KBS2.CarSystem;
using KBS2.CarSystem.Sensors;
using KBS2.Exceptions;
using KBS2.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KBS2.Console.Commands
{
    [CommandMetadata("Sensors")]
    class CommandsSensors : ICommand
    {
        public IEnumerable<char> Run(params string[] args)
        {
            if(args.Length == 0)
            {
                throw new InvalidParametersException("Invalid command usage, you can use remove and add");
            }
            if(args[0] == "add")
            {
                return AddSensor(args);
            }
            else if(args[0] == "remove")
            {
                return RemoveSensor(args);
            }
            else
            {
                throw new InvalidParametersException("Invalid command usage, you can use remove and add");
            }           
        }
        
        private IEnumerable<char> RemoveSensor(params string[] args)
        {
            if (args.Length < 3) { throw new InvalidParametersException("Invalid command usage, should be : Sensor Remove {model} {side} {sensor}"); };

            CarModel model;
            Direction side;
            CreateSensor sensorFactory;
            try
            {
                model = CarModel.Get(args[1]);
            }
            catch (Exception e)
            {
                throw new InvalidParametersException("Invalid command: Model doesn't exist.");
            }
            try
            {
                side = StringConverters.stringToDirection(args[2]);
            }
            catch (Exception e)
            {
                throw new InvalidParametersException("Invalid command: Side is wrong used.");
            }
            try
            {
                sensorFactory = StringConverters.GetSensor(args[3]);
            }
            catch (Exception e)
            {
                throw new InvalidParametersException("Invalid command: Could not create sensor.");
            }
            var count = model.Sensors
                .FindAll(sensor => sensor.GetType().Equals(sensorFactory.GetType()) && sensor.Direction == side)
                .Count();
            model.Sensors.RemoveAll(sensor => sensor.GetType().Equals(sensorFactory.GetType()) && sensor.Direction == side);

            return $"Removed {count} sensors.";
        }

        private IEnumerable<char> AddSensor(params string[] args)
        {
            if (args.Length < 4) { throw new InvalidParametersException("Invalid command usage, should be : Sensor add {model} {side} {sensor} [range]"); };

            if (args[0] != "add") { throw new InvalidParametersException("Invalid command useage, should be : Sensor add {model} {side} {sensor} [range]"); };

            CarModel model;
            Direction side;
            CreateSensor sensorFactory;

            try
            {
                model = CarModel.Get(args[1]);
            }
            catch (Exception e)
            {
                throw new InvalidParametersException("Invalid command: Model doesn't exist.");
            }
            try
            {
                side = StringConverters.stringToDirection(args[2]);
            }
            catch (Exception e)
            {
                throw new InvalidParametersException("Invalid command: Side is wrong used.");
            }
            try
            {
                sensorFactory = StringConverters.GetSensor(args[3]);
            }
            catch (Exception e)
            {
                throw new InvalidParametersException("Invalid command: Could not create sensor.");
            }
            var range = 0;
            if (args.Length == 5)
            {
                try
                {
                    range = int.Parse(args[4]);
                }
                catch (Exception e)
                {
                    throw new InvalidParametersException("Invalid command: Could not parse range.");
                }
            }

            var sensor = new SensorPrototype
            {
                Direction = side,
                Range = range,
                Create = sensorFactory
            };

            model.Sensors.Add(sensor);

            CitySystem.City.Instance.Cars
                .FindAll(car => car.Model == model)
                .ForEach(car => car.Sensors.Add(sensor.Create(car, sensor.Direction, sensor.Range)));



            return $"Added sensor {args[3]} to {model.Name}";
        }

    }
}