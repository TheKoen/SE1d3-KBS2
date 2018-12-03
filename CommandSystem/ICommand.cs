using System.Collections.Generic;

namespace CommandSystem
{
    public interface ICommand
    {
        IEnumerable<char> Run(params string[] args);
    }
}