using System.Windows;

namespace KBS2.Util
{
    public class TextBoxProperties
    {
        public static readonly DependencyProperty GhostTextProperty =
            DependencyProperty.RegisterAttached("GhostText", typeof(object), typeof(TextBoxProperties), new PropertyMetadata("GhostText"));

        public static void SetGhostText(DependencyObject element, string value)
        {
            element.SetValue(GhostTextProperty, value);
        }

        public static object GetGhostText(DependencyObject element) =>
            element.GetValue(GhostTextProperty);
    }
}