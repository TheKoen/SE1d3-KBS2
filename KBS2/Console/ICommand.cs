using System.Collections.Generic;
using System.Runtime.Serialization;

namespace KBS2.Console
{
    public interface ICommand
    {
        IEnumerable<char> Run(params string[] args);
    }
}
