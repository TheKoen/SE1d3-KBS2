using System;
using System.Runtime.Serialization;

namespace KBS2.Exceptions
{
    public class TypeMismatchException : SystemException
    {
        public TypeMismatchException()
        {
        }

        public TypeMismatchException(string message) : base(message)
        {
        }

        public TypeMismatchException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected TypeMismatchException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
