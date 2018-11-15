using KBS2.Console;
using System;

namespace KBS2.Utilities
{
    public class Property
    {
        private object _value;
        public Type PropertyType { get; }
        public object Value
        {
            get => _value;
            set
            {
                if (value.GetType() != PropertyType)
                    throw new TypeMismatchException($"Cannot assign value of type \"{value.GetType().Name}\" to Property of type\"{PropertyType.Name}\"");
                _value = value;
            }
        }

        public Property(object value)
        {
            PropertyType = value.GetType();
            Value = value;
        }
    }
}
