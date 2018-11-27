using System;
using System.Runtime.Serialization;

namespace KBS2.Exceptions
{
    public class KeyExistsException : SystemException
    {
        public KeyExistsException()
        {
        }

        public KeyExistsException(string message) : base(message)
        {
        }

        public KeyExistsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected KeyExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
