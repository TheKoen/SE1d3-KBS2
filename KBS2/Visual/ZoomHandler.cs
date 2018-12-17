using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace KBS2.Visual
{
    public class ZoomHandler
    {
        private MainScreen Screen { get; }

        private Vector mouseOrigin;
        private int time;

        public ZoomHandler(MainScreen screen)
        {
            Screen = screen;

            MainScreen.CommandLoop.Subscribe(Update);
        }

        private void Update()
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                var current = Mouse.GetPosition(Screen.ZoomBox);

                if (Math.Abs(mouseOrigin.X + 1) < 0.01)
                {
                    mouseOrigin = new Vector(current.X, current.Y);
                    return;
                }

                var scroll = Screen.CanvasScroll;
                Screen.CanvasScroll.ScrollToHorizontalOffset(scroll.HorizontalOffset + mouseOrigin.X - current.X);
                Screen.CanvasScroll.ScrollToVerticalOffset(scroll.VerticalOffset + mouseOrigin.Y - current.Y);
                mouseOrigin = new Vector(current.X, current.Y);

                if (time < 10)
                {
                    time++;
                }
                else
                {
                    Screen.Cursor = Cursors.ScrollAll;
                }
            }
            else
            {
                time = 0;
                Screen.Cursor = Cursors.Arrow;
                mouseOrigin.X = -1;
            }
        }

        public void ZoomBoxChanged()
        {
            if (!(Screen.ZoomBox.SelectedValue is ComboBoxItem input)) return;
            if (int.TryParse(input.Content.ToString().Replace("%", ""), out var value))
            {
                Screen.Zoom = value / 100F;
            }
        }

        public void Scroll(object source, MouseWheelEventArgs args)
        {
            var value = Math.Max(args.Delta / 6F + 180F, 0F) / 180F;
            Screen.Zoom = Screen.Zoom * value;
            Screen.ZoomBox.SelectedIndex = Screen.ZoomBox.Items.Count - 1;
            ((ComboBoxItem) Screen.ZoomBox.SelectedItem).Content = $"{Math.Round(Screen.Zoom * 100)}%";
            args.Handled = true;
        }
    }
}