using KBS2.CarSystem;
using KBS2.CarSystem.Sensors;
using KBS2.Exceptions;
using KBS2.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace KBS2.Console.Commands
{
    class CommandsSensors : ICommand
    {
        private List<SensorPrototype> listPotentialRemoved;

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
        
        /// <summary>
        /// Removes the Sensor
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private IEnumerable<char> RemoveSensor(params string[] args)
        {
            if (args.Length < 3 && args.Length != 2) { throw new InvalidParametersException("Invalid command usage, should be : Sensor Remove {model} [side] [sensor]"); };

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
            listPotentialRemoved = model.Sensors;
            var countSensorsModel = model.Sensors.Count;
            if (args.Length == 2)
            {
                printListPotentialRemoved();
                MainWindow.Console.SendCommand += removeSensors;
                return "";
            }
            else
            {
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
                .FindAll(sensor => sensor.Create.Equals(sensorFactory) && side == sensor.Direction)
                .Count;

                model.Sensors.RemoveAll(sensor => sensor.Create.Equals(sensorFactory) && side == sensor.Direction);
                return $"Removed {count} {args[3]} sensors.";
            }
        }

        private void removeSensors(ConsoleControl sender, SendCommandArgs args)
        {
            try
            {
                var i = int.Parse(args.Command);
                if(i == 0)
                {
                    listPotentialRemoved.Clear();
                    MainWindow.Console.Print("Sensors removed");
                }
                else if(i <= listPotentialRemoved.Count)
                {
                    listPotentialRemoved.RemoveAt(i-1);
                    MainWindow.Console.Print($"Sensor {i} removed");
                }
                else
                {
                    MainWindow.Console.SendCommand -= removeSensors;
                    MainWindow.Console.Print("Please give a digit, given", Colors.Red);
                }
                MainWindow.Console.SendCommand -= removeSensors;
            }
            catch(Exception e)
            {
                MainWindow.Console.SendCommand -= removeSensors;
                MainWindow.Console.Print("Could not parse: Please give a digit", Colors.Red);
            }
        }

        private void printListPotentialRemoved()
        {
            for (int i = 0; i < listPotentialRemoved.Count; i++)
            {
                var name = Sensor.SENSORS.FirstOrDefault(sensor => sensor.Value.Equals(listPotentialRemoved[i].Create)).Key.Name;
                MainWindow.Console.Print($"{i+1}. Name: {name}, Side: {listPotentialRemoved[i].Direction.ToString()}");
            }
            MainWindow.Console.Print("0. remove all.");
        }

        /// <summary>
        /// Add's a sensor
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
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
