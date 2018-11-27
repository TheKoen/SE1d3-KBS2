using System;

namespace KBS2.Exceptions
{
    public class CommandException : Exception
    {
        public CommandException()
        {
        }

        public CommandException(string message) : base(message)
        {
        }
    }
}