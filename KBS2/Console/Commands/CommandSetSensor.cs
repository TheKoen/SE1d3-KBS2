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
            if (args.Length < 4) { throw new InvalidParametersException(); };

            if (args[0] == "set") { throw new InvalidParametersException(); };

            var model = stringToCarModel(args[1]);
            var side = stringToDirection(args[2]);
            var sensor = args[3];

            var carWithModels = CitySystem.City.Instance.Cars.FindAll(car =>
            {
                if (car.Model == model)
                {
                    return true;
                }
                else
                {
                    return false;
                };
            });
            
            foreach(var car in carWithModels)
            {
                car.Sensors.RemoveAll(s => s.Direction == side && s.);

                car.Sensors.Add()
            }

            return "";
        }

        /// <summary>
        /// Convert string to CarModel
        /// </summary>
        /// <param name="carModel">string with carModel</param>
        /// <returns>Carmodel from string</returns>
        private static CarModel stringToCarModel(string carModel)
        {
            try
            {
                return (CarModel)Enum.Parse(typeof(CarModel), carModel);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
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
        /// <returns>the create sensor</returns>
        private static void CreateSensor(string sensorName, Car car, Direction direction, int range)
        {
            var sensorType = GetSensorsClasses()
                            .ToList()
                            .Find(type => type.Name == sensorName);

            foreach (var sensor in Sensor.SENSORS)
            {
                if (sensor.Type == sensorType)
                {
                    return sensor.Creator(car, direction, range);
                }
            }     

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
