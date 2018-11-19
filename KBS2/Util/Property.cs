using System;
using System.ComponentModel;
using KBS2.Console;

namespace KBS2.Util
{
    public class Property
    {
        public delegate void CustomPropertyChangedHandler(Property sender, CustomPropertyChangedArgs args);
        public event CustomPropertyChangedHandler PropertyChanged;
        
        private dynamic _value;
        public Type PropertyType { get; }
        public dynamic Value
        {
            get => _value;
            set
            {
                if (value.GetType() != PropertyType)
                    throw new TypeMismatchException($"Cannot assign value of type \"{value.GetType().Name}\" to Property of type\"{PropertyType.Name}\"");
                PropertyChanged?.Invoke(this, new CustomPropertyChangedArgs(_value, value));
                _value = value;
                
            }
        }

        public Property(dynamic value)
        {
            PropertyType = value.GetType();
            Value = value;
        }
        
        
        
        
    }
}
