using System.Collections.Generic;
using CommandSystem.Exceptions;

namespace CommandSystem.PropertyManagement
{
    public static class PropertyHandler
    {
        // List of properties that can be modified using console commands
        private static readonly Dictionary<string, Property> PropertyList = new Dictionary<string, Property>();
        
        /// <summary>
        /// Registers a <see cref="Property"/> to the <see cref="CommandHandler"/> for future use
        /// </summary>
        /// <param name="name">Name of the <see cref="Property"/></param>
        /// <param name="property"><see cref="Property"/> to be stored</param>
        public static void RegisterProperty(string name, ref Property property)
        {
            if (PropertyList.ContainsKey(name))
                throw new PropertyHandlerException($"Property \"{name}\" already exists");

            PropertyList.Add(name, property);
        }
        
        /// <summary>
        /// Gets the name of every registered <see cref="Property"/>
        /// </summary>
        /// <returns><see cref="Dictionary{String,Property}"/> of properties</returns>
        public static Dictionary<string, Property> GetProperties() => PropertyList;
        
        /// <summary>
        /// Modifies a <see cref="Property"/>
        /// </summary>
        /// <param name="name">Name of the <see cref="Property"/> to modify</param>
        /// <param name="value">New value for the <see cref="Property"/></param>
        public static void ModifyProperty(string name, object value)
        {
            if (!PropertyList.ContainsKey(name))
                throw new PropertyHandlerException($"Property \"{name}\" is not registered");
            
            PropertyList[name].Value = value;
        }
        
        /// <summary>
        /// Resets the list of <see cref="Property"/> objects
        /// </summary>
        public static void ResetProperties() => PropertyList.Clear();
    }
}