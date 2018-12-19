using System;

namespace CommandSystem.Exceptions
{
    public class CommandHandlerException : Exception
    {
        public CommandHandlerException()
        {
        }

        public CommandHandlerException(string message) : base(message)
        {
        }
    }
}