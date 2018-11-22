using KBS2.CarSystem;
using KBS2.CarSystem.Sensors;
using KBS2.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KBS2.Console.Commands
{
    class CommandSetSensor : ICommand
    {
        public IEnumerable<char> Run(params string[] args)
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
            catch(Exception e)
            {
                throw new InvalidParametersException("Invalid command: Model doesn't exist.");
            }
            try
            {
                side = stringToDirection(args[2]);
            }
            catch (Exception e)
            {
                throw new InvalidParametersException("Invalid command: Side is wrong used.");
            }
            try
            {
                sensorFactory = GetSensor(args[3]);
            }
            catch (Exception e)
            {
                throw new InvalidParametersException("Invalid command: Could not create sensor.");
            }
            var range = 0;
            if(args.Length == 5)
            {
                try
                {
                    range = int.Parse(args[4]);
                }
                catch(Exception e)
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

        /// <summary>
        /// Convert string to Direction
        /// </summary>
        /// <param name="directionSensor">string with direction</param>
        /// <returns>Direction from string</returns>
        private static Direction stringToDirection(string directionSensor)
        {
            try
            {
                return (Direction)Enum.Parse(typeof(Direction), directionSensor);
            }
            catch(Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// Create a sensor out of a string
        /// </summary>
        /// <returns>the gettten sensor</returns>
        private static CreateSensor GetSensor(string sensorName)
        {
            var sensorType = GetSensorsClasses()
                            .ToList()
                            .Find(type => type.Name == sensorName);

            return Sensor.SENSORS[sensorType];
        }

        
        /// <summary>
        /// Get all Sensors in Namespace ActiveSensors en PassiveSensors
        /// </summary>
        /// <returns>return all Types of Sensors</returns>
        private static Type[] GetSensorsClasses()
        {
            string nameSpaceSensors1 = "KBS2.CarSystem.Sensors.ActiveSensors";
            string nameSpaceSensors2 = "KBS2.CarSystem.Sensors.PassiveSensors";
            return 
                Assembly.GetExecutingAssembly().GetTypes()
                    .Where(t => String.Equals(t.Namespace, nameSpaceSensors1, StringComparison.Ordinal) || String.Equals(t.Namespace, nameSpaceSensors2, StringComparison.Ordinal))
                    .ToArray();
        }


    }
}
