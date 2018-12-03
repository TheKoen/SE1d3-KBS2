using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandSystem;
using CommandSystem.PropertyManagement;

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
            foreach (var prop in PropertyHandler.GetProperties().OrderBy(p => p.Key))
            {
                output.Append($"\n{prop.Key} ({prop.Value.Type.Name})");
            }

            return output.ToString();
        }
    }
}