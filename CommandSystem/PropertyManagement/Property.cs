using System;
using CommandSystem.Exceptions;

namespace CommandSystem.PropertyManagement
{
    public sealed class Property
    {
        public delegate void UserPropertyChangedHandler(Property sender, UserPropertyChangedArgs args);
        public event UserPropertyChangedHandler PropertyChanged;

        private dynamic _value;
        
        public Type Type { get; }

        public dynamic Value
        {
            get => _value;
            set
            {
                if (value.GetType() != Type)
                    throw new TypeMismatchException(
                        $"Cannot assign value of type \"{value.GetType().Name}\" to Property of type\"{Type.Name}\"");
                PropertyChanged?.Invoke(this, new UserPropertyChangedArgs(_value, value));
                _value = value;
            }
        }

        public Property(dynamic value)
        {
            if (value == null) throw new ArgumentNullException();
            Type = value.GetType();
            Value = value;
        }
    }
}