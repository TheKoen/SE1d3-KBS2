using KBS2.CarSystem.Sensors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KBS2.Util
{
    public class StringConverters
    {
        /// <summary>
        /// Convert string to Direction
        /// </summary>
        /// <param name="directionSensor">string with direction</param>
        /// <returns>Direction from string</returns>
        public static Direction stringToDirection(string directionSensor)
        {
            try
            {
                return (Direction)Enum.Parse(typeof(Direction), directionSensor);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// Create a sensor out of a string
        /// </summary>
        /// <returns>the gettten sensor</returns>
        public static CreateSensor GetSensor(string sensorName)
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
