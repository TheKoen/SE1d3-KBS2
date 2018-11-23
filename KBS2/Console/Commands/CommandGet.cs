using System.Collections.Generic;
using System.Linq;
using KBS2.Exceptions;

namespace KBS2.Console.Commands
{
    [CommandMetadata("Get",
        Description = "Gets the value of a Property",
        AutoRegister = true)]
    public class CommandGet : ICommand
    {
        public IEnumerable<char> Run(params string[] args)
        {
            if (args.Length < 1) throw new InvalidParametersException();

            var propName = args[0];
            
            // Getting property
            var properties = CommandHandler.GetProperties().Where(p => p.Key == propName).ToArray();
            if (properties.Length == 0) throw new InvalidParametersException($"Unknown property \"{propName}\"");
            var property = properties[0].Value;
            
            // Returning the property value
            return $"{propName} = {property.Value} ({property.PropertyType.Name})";
        }
    }
}