using KBS2.CarSystem;
using KBS2.CarSystem.Sensors;
using KBS2.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using CommandSystem;
using CommandSystem.Exceptions;

namespace KBS2.Console.Commands
{
    [CommandMetadata("sensor",
        Description = "Manages sensors",
        Usages = new []
        {
            "sensor remove <model> [side] [sensor]",
            "sensor add <model> <side> <sensor> [range]",
            "sensor types"
        },
        AutoRegister = true)]
    class CommandsSensors : ICommand
    {
        private List<SensorPrototype> listPotentialRemoved;

        public IEnumerable<char> Run(params string[] args)
        {
            if(args.Length == 0)
            {
                throw new CommandInputException("Invalid command usage, you can use remove, add and types");
            }
            if(args[0] == "add")
            {
                return AddSensor(args);
            }
            else if(args[0] == "remove")
            {
                return RemoveSensor(args);
            }
            else if(args[0] == "types")
            {
                return printTypes(args);
            }
            else
            {
                throw new CommandInputException("Invalid command usage, you can use remove, add types");
            }           
        }
        
        /// <summary>
        /// Removes the Sensor
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private IEnumerable<char> RemoveSensor(params string[] args)
        {
            if (args.Length < 3 && args.Length != 2) { throw new CommandInputException("Invalid command usage, should be : sensor remove <model> [side] [sensor] [sensor]"); };

            CarModel model;
            Direction side;
            CreateSensor sensorFactory;
            try
            {
                model = CarModel.GetModel(args[1]);
            }
            catch (Exception e)
            {
                throw new CommandInputException("Invalid command: Model doesn't exist.");
            }
            listPotentialRemoved = model.Sensors;
            var countSensorsModel = model.Sensors.Count;
            if (args.Length == 2)
            {
                printListPotentialRemoved();
                App.Console.SendCommand += removeSensors;
                return null;
            }
            else
            {
                try
                {
                    side = StringConverters.StringToDirection(args[2]);
                }
                catch (Exception e)
                {
                    throw new CommandInputException("Invalid command: Side is wrong used.");
                }
                try
                {
                    sensorFactory = StringConverters.GetSensor(args[3]);
                }
                catch (Exception e)
                {
                    throw new CommandInputException("Invalid command: Could not create sensor.");
                }
                var count = model.Sensors
                .FindAll(sensor => sensor.Create.Equals(sensorFactory) && side == sensor.Direction)
                .Count;

                model.Sensors.RemoveAll(sensor => sensor.Create.Equals(sensorFactory) && side == sensor.Direction);
                return $"Removed {count} {args[3]} sensors.";
            }
        }
        /// <summary>
        /// prints all types of sensors
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private string printTypes(string[] args)
        {
            var items = StringConverters.GetSensorNames();
            foreach(var item in items)
            {
                App.Console.Print(item);
            }

            return null;
        }

        /// <summary>
        /// removes the sensor by the event SendCommand
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args">command given</param>
        private void removeSensors(ConsoleControl sender, SendCommandArgs args)
        {
            try
            {
                var i = int.Parse(args.Command);
                if(i == 0)
                {
                    listPotentialRemoved.Clear();
                    App.Console.Print("Sensors removed");
                }
                else if(i <= listPotentialRemoved.Count)
                {
                    listPotentialRemoved.RemoveAt(i-1);
                    App.Console.Print($"Sensor {i} removed");
                }
                else
                {
                    App.Console.SendCommand -= removeSensors;
                    App.Console.Print("Please give a digit, given", Colors.Red);
                }
                App.Console.SendCommand -= removeSensors;
            }
            catch(Exception e)
            {
                App.Console.SendCommand -= removeSensors;
                App.Console.Print("Could not parse: Please give a digit", Colors.Red);
            }
        }

        /// <summary>
        /// prints the options you can remove from a car model
        /// </summary>
        private void printListPotentialRemoved()
        {
            for (int i = 0; i < listPotentialRemoved.Count; i++)
            {
                var name = Sensor.Sensors.FirstOrDefault(sensor => sensor.Value.Equals(listPotentialRemoved[i].Create)).Key.Name;
                App.Console.Print($"{i+1}. Name: {name}, Side: {listPotentialRemoved[i].Direction.ToString()}");
            }
            App.Console.Print("0. remove all.");
        }

        /// <summary>
        /// Add's a sensor
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private IEnumerable<char> AddSensor(params string[] args)
        {
            if (args.Length < 4) { throw new CommandInputException("Invalid command usage, should be : sensor add <model> <side> <sensor> [range]"); };

            if (args[0] != "add") { throw new CommandInputException("Invalid command useage, should be : sensor add <model> <side> <sensor> [range]"); };

            CarModel model;
            Direction side;
            CreateSensor sensorFactory;

            try
            {
                model = CarModel.GetModel(args[1]);
            }
            catch (Exception)
            {
                throw new CommandInputException("Invalid command: Model doesn't exist.");
            }
            try
            {
                side = StringConverters.StringToDirection(args[2]);
            }
            catch (Exception)
            {
                throw new CommandInputException("Invalid command: Side is wrong used.");
            }
            try
            {
                sensorFactory = StringConverters.GetSensor(args[3]);
            }
            catch (Exception)
            {
                throw new CommandInputException("Invalid command: Could not create sensor.");
            }
            var range = 0;
            if (args.Length == 5)
            {
                try
                {
                    range = int.Parse(args[4]);
                }
                catch (Exception)
                {
                    throw new CommandInputException("Invalid command: Could not parse range.");
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
