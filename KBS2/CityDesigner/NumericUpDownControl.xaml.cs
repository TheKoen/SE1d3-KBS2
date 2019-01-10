using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace KBS2.CityDesigner
{
    /// <summary>
    /// Interaction logic for NumericUpDownControl.xaml
    /// </summary>
    public partial class NumericUpDownControl
    {
        public delegate void ValueChangedEventHandler(object source, PropertyChangedEventArgs e);

        #region Properties & Fields
        
        public event ValueChangedEventHandler ValueChanged;

        private int _value;
        private int _minValue = int.MinValue;
        private int _maxValue = int.MaxValue;

        public int Value
        {
            get => _value;
            set
            {
                if (value < MinValue) _value = MinValue;
                else if (value > MaxValue) _value = MaxValue;
                else _value = value;

                TextBoxValue.Text = _value.ToString();
            }
        }

        public int MinValue
        {
            get => _minValue;
            set
            {
                _minValue = value;
                if (Value < value) Value = value;
            }
        }

        public int MaxValue
        {
            get => _maxValue;
            set
            {
                _maxValue = value;
                if (Value > value) Value = value;
            }
        }

        public int StepSize { get; set; } = 1;

        #endregion
        

        public NumericUpDownControl()
        {
            InitializeComponent();
        }

        #region Private Methods
        
        private int VerifyValue(string value) =>
            int.TryParse(value, out var output) ? output : Value;


        private void TextBoxValue_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            Value = VerifyValue(((TextBox)sender).Text);
            ValueChanged?.Invoke(this, new PropertyChangedEventArgs("Value"));
        }

        private void ButtonUp_OnClick(object sender, RoutedEventArgs e)
        {
            if (Value + StepSize > MaxValue) return;
            Value += StepSize;
            ValueChanged?.Invoke(this, new PropertyChangedEventArgs("Value"));
        }

        private void ButtonDown_OnClick(object sender, RoutedEventArgs e)
        {
            if (Value - StepSize < MinValue) return;
            Value -= StepSize;
            ValueChanged?.Invoke(this, new PropertyChangedEventArgs("Value"));
        }
        
        #endregion
    }
}
