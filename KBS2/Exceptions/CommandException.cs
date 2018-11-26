using System;
using System.Runtime.Serialization;

namespace KBS2.Utilities
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