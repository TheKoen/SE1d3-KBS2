using System;

namespace CommandSystem.Exceptions
{
    public class PropertyHandlerException : Exception
    {
        public PropertyHandlerException()
        {
        }

        public PropertyHandlerException(string message) : base(message)
        {
        }
    }
}