using System;

namespace CommandSystem.Exceptions
{
    public sealed class TypeMismatchException : Exception
    {
        public TypeMismatchException()
        {
        }

        public TypeMismatchException(string message) : base(message)
        {
        }
    }
}