using System;
using System.Runtime.Serialization;

namespace KBS2.Utilities
{
    public class UnknownCommandException : CommandException
    {
        public UnknownCommandException()
        {
        }

        public UnknownCommandException(string message) : base(message)
        {
        }
    }
}