using System;
using System.Runtime.Serialization;

namespace KBS2.Utilities
{
    public abstract class CommandException : Exception, ISerializable
    {
        protected CommandException()
        {
        }

        protected CommandException(string message) : base(message)
        {
        }
    }
}