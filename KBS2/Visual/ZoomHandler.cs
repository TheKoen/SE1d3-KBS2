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
        private bool changing;
        private int time;

        public ZoomHandler(MainScreen screen)
        {
            Screen = screen;

            // Subscribe to the command loop to make sure we run all the time.
            MainScreen.CommandLoop.Subscribe(Update);
        }

        /// <summary>
        /// Bound to the CommandLoop Update event.
        /// Moves the canvas around when the player clicks and drags.
        /// </summary>
        private void Update()
        {
            // Check if the left mouse button is being held down.
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                // Get the current position of the mouse relative to the dropdown box.
                var current = Mouse.GetPosition(Screen.ZoomBox);

                // Check if the mouse origin has been set yet, if not, set it to the current position.
                if (Math.Abs(mouseOrigin.X + 1) < 0.01)
                {
                    mouseOrigin = new Vector(current.X, current.Y);
                    return;
                }

                // Scroll the canvas with the delta between the current position and the origin.
                var scroll = Screen.CanvasScroll;
                Screen.CanvasScroll.ScrollToHorizontalOffset(scroll.HorizontalOffset + mouseOrigin.X - current.X);
                Screen.CanvasScroll.ScrollToVerticalOffset(scroll.VerticalOffset + mouseOrigin.Y - current.Y);
                mouseOrigin = new Vector(current.X, current.Y);

                // If the mouse is being held down for more than 10 ticks, change the cursor.
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
                // If the mouse isn't being held down, change the cursor back to normal and reset the mouse origin.
                time = 0;
                Screen.Cursor = Cursors.Arrow;
                mouseOrigin.X = -1;
            }
        }

        /// <summary>
        /// Bound to the SelectionChanged event for the zoom dropdown box.
        /// Updates the zoom level to the selected amount.
        /// </summary>
        public void ZoomBoxChanged()
        {
            // Make sure a value has actually been selected.
            if (!(Screen.ZoomBox.SelectedValue is ComboBoxItem input)) return;

            // If we're changing the zoom level already, don't change it again.
            if (changing)
            {
                changing = false;
                return;
            }

            // Parse the value and set the zoom level.
            if (int.TryParse(input.Content.ToString().Replace("%", ""), out var value))
            {
                Screen.Zoom = value / 100F;
            }
        }

        /// <summary>
        /// Bound to the MouseWheel event for the main screen.
        /// Updates the zoom level depending on the scoll.
        /// </summary>
        public void Scroll(object source, MouseWheelEventArgs args)
        {
            // Clamp the scroll amount to be above 0
            var value = Math.Max(args.Delta / 6F + 180F, 0F) / 180F;
            // Multiply the zoom level by the value.
            Screen.Zoom *= value;

            // Update the amount shown in the dropdown box.
            changing = true;
            Screen.ZoomBox.SelectedIndex = Screen.ZoomBox.Items.Count - 1;
            ((ComboBoxItem) Screen.ZoomBox.SelectedItem).Content = $"{Math.Round(Screen.Zoom * 100)}%";
            changing = false;

            // Tell WPF that we handled the event to prevent the canvas from scrolling.
            args.Handled = true;
        }
    }
}