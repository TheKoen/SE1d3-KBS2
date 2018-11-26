using System;
using System.Runtime.Serialization;

namespace KBS2.Exceptions
{
    public class CommandException : Exception, ISerializable
    {
        public CommandException()
        {
        }

        public CommandException(string message) : base(message)
        {
        }
    }
}