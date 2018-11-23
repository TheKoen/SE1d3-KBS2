using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KBS2.Exceptions;

namespace KBS2.Console.Commands
{
    [CommandMetadata("help",
        Description = "Shows a list of commands or information about a given command",
        Usages = new [] { "help [command]" },
        AutoRegister = true)]
    public class CommandHelp : ICommand
    {
        public IEnumerable<char> Run(params string[] args)
        {
            var output = new StringBuilder();
            
            // Getting all commands registered in the CommandHandler
            var commands = CommandHandler.GetCommands()
                .Select(type => (CommandMetadataAttribute)Attribute.GetCustomAttribute(type, typeof(CommandMetadataAttribute)))
                .ToList();

            if (args.Length < 1)
            {
                // Showing a list of commands with their descriptions
                output.Append("Commands:");
                commands.ForEach(c => output.Append($"\n{c.Key} : {c.Description}"));
                return output.ToString();
            }

            // Finding the given command
            var selected = commands.Where(c => c.Aliases.Contains(args[0]) || c.Key == args[0]).ToList();
            if (selected.Count < 1)
                throw new InvalidParametersException($"Unknown command \"{args[0]}\"");
            if (selected.Count > 1)
                throw new Exception("There should never be multiple commands with the same key or aliases");
            var command = selected[0];

            // Showing information about the given command, such as the name, the description, aliases and usages.
            output.Append($"{command.Key}\n");
            output.Append($"{command.Description}\n\n");
            output.Append($"Aliases: {string.Join(", ", command.Aliases)}\n");
            output.Append("Usages:");
            foreach (var usage in command.Usages)
                output.Append($"\n\t- {usage}");

            return output.ToString();
        }
    }
}