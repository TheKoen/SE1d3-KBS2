using System;
using CommandSystem.Exceptions;

namespace CommandSystem.PropertyManagement
{
    public sealed class Property
    {
        public delegate void UserPropertyChangedHandler(Property sender, UserPropertyChangedArgs args);
        public event UserPropertyChangedHandler PropertyChanged;

        private dynamic _value;
        private dynamic _firstValue;
        
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
                if (_firstValue == null) _firstValue = value;
            }
        }

        public Property(dynamic value)
        {
            if (value == null) throw new ArgumentNullException();
            Type = value.GetType();
            Value = value;
        }

        /// <summary>
        /// Resets the current value to the first value of this <see cref="Property"/> if they are not equal
        /// </summary>
        /// <returns>True if <see cref="Value"/> was changed</returns>
        public bool ResetToFirstValue()
        {
            var changed = _value != _firstValue;
            if (changed) Value = _firstValue;
            return changed;
        }
    }
}