using System.Collections.Generic;

namespace KBS2.Console
{
    public interface ICommand
    {
        IEnumerable<char> Run(params string[] args);
    }
}
