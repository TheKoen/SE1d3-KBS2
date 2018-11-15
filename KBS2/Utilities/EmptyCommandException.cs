using System;
using System.Runtime.Serialization;

namespace KBS2.Utilities
{
    public class EmptyCommandException : CommandException
    {
        public EmptyCommandException() : this("Empty input")
        {
        }

        public EmptyCommandException(string message) : base(message)
        {
        }
    }
}