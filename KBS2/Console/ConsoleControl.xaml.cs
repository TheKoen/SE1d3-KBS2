using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace KBS2.Console
{
    /// <summary>
    /// Interaction logic for ConsoleControl.xaml
    /// </summary>
    public partial class ConsoleControl : UserControl
    {

        public delegate void SendCommandHandler(ConsoleControl sender, SendCommandArgs args);

        // Gets invoked when a command is sent by the user
        public event SendCommandHandler SendCommand;

        public ConsoleControl()
        {
            InitializeComponent();

            ButtonSend.Click += HandleSendButton;
        }


        private void HandleSendButton(object sender, RoutedEventArgs args)
        {
            // Getting the command from the TextBox and invoking the SendCommand event
            var command = TextBoxInput.Text.Trim();
            SendCommand(this, new SendCommandArgs(command));
        }

        /// <summary>
        /// Prints some text to the output of the <see cref="ConsoleControl"/>
        /// </summary>
        /// <param name="text">The text to display</param>
        /// <param name="color">The color to print the text with</param>
        public void Print(IEnumerable<char> text, Color color)
        {
            StackFrame frame = null;
            Type caller = null;
            int layer = 1;
            // Finding first calling Type that is not this one
            while (caller == null || caller == typeof(ConsoleControl))
            {
                // Getting info on the n-th caller on the stack
                frame = new StackFrame(layer++, true);
                caller = frame.GetMethod().DeclaringType;
            }

            // Adding newline at the beginning when there already is input in the TextBlock
            if (TextBlockOutput.Inlines.Count > 0)
                TextBlockOutput.Inlines.Add(new Run("\r\n"));
            // Adding a piece of text with a color into the TextBlock
            TextBlockOutput.Inlines.Add(
                new Run(string.Format("[{0}] {1}", caller.FullName, string.Join("", text)))
                { Foreground = new SolidColorBrush(color) }
            );
            // Scrolling to the bottom of the ScrollViewer so that the user always sees the new text
            ScrollViewerOutput.ScrollToBottom();
        }

        /// <summary>
        /// Prints some text to the output of the <see cref="ConsoleControl"/>
        /// </summary>
        /// <param name="text">The text to display</param>
        public void Print(IEnumerable<char> text)
        {
            Print(text, Colors.White);
        }

        private void HandleInputKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;

            // Same functionality as a Click event from ButtonSend
            HandleSendButton(sender, new RoutedEventArgs());
        }
    }
}
