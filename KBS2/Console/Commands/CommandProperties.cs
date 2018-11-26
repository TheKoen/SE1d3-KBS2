using System.Collections.Generic;
using System.Text;

namespace KBS2.Console.Commands
{
    [CommandMetadata("properties",
        Aliases = new[] { "props" },
        Description = "Shows a list of all currently registered Properties",
        AutoRegister = true)]
    public class CommandProperties : ICommand
    {
        public IEnumerable<char> Run(params string[] args)
        {
            var output = new StringBuilder("Properties:");
            foreach (var prop in CommandHandler.GetProperties())
            {
                output.Append($"\n{prop.Key} ({prop.Value.PropertyType.Name})");
            }

            return output.ToString();
        }
    }
}