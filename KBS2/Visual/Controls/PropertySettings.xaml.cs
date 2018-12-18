using KBS2.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KBS2
{
    /// <summary>
    /// Interaction logic for PropertySettings.xaml
    /// </summary>
    public partial class PropertySettings : UserControl
    {
        public PropertySettings(string pn, string cv, bool isInt)
        {
            InitializeComponent();
            this.PropertyName = pn;
            this.CurrentValue = cv;

            if (isInt) TBCurrentValue.PreviewTextInput += NumberValidationTextBox;
            else TBCurrentValue.PreviewTextInput += TextValidationTextBox;
        }

        public string PropertyName
        {
            get => LabelPropertyName.Content.ToString();
            set => LabelPropertyName.Content = value;
        }

        public string CurrentValue
        {
            get => TBCurrentValue.Text.ToString();
            set => TBCurrentValue.Text = value;
        }

        public void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("^[^0-9,.]+$");
            e.Handled = regex.IsMatch(e.Text);
        }

        public void TextValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("^[^a-zA-Z0-9]+$");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
