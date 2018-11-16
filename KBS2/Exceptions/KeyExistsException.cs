using System;
using System.Runtime.Serialization;

namespace KBS2.Console
{
    public class KeyExistsException : SystemException, ISerializable
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
