using System.Collections.Generic;
using System.Runtime.Serialization;

namespace KBS2.Console.Commands
{
    public class CommandTest : ICommand
    {
        public IEnumerable<char> Run(params string[] args) =>
            args.Length == 0 ? "Ayyy!" : string.Join(" ", args);
    }
}