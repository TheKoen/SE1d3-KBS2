using System.Collections.Generic;
using System.Linq;
using CommandSystem;
using CommandSystem.Exceptions;
using CommandSystem.PropertyManagement;

namespace KBS2.Console.Commands
{
    [CommandMetadata("get",
        Description = "Gets the value of a Property",
        Usages = new [] { "get <property>" },
        AutoRegister = true)]
    public class CommandGet : ICommand
    {
        public IEnumerable<char> Run(params string[] args)
        {
            if (args.Length < 1) throw new CommandInputException("Invalid parameters");

            var propName = args[0];
            
            // Getting property
            var properties = PropertyHandler.GetProperties().Where(p => p.Key == propName).ToArray();
            if (properties.Length == 0) throw new CommandInputException($"Unknown property \"{propName}\"");
            var property = properties[0].Value;
            
            // Returning the property value
            return $"{propName} = {property.Value} ({property.Type.Name})";
        }
    }
}