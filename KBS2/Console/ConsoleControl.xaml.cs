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
        
        private Queue<string> _outputHistory = new Queue<string>();

        public ConsoleControl()
        {
            InitializeComponent();

            ButtonSend.Click += HandleSendButton;
        }

        public void TestMap()
        {
            SendCommand?.Invoke(this, new SendCommandArgs("Map"));
        }

        private void HandleSendButton(object sender, RoutedEventArgs args)
        {
            // Getting the command from the TextBox and invoking the SendCommand event
            var command = TextBoxInput.Text.Trim();
            SendCommand?.Invoke(this, new SendCommandArgs(command));
            TextBoxInput.Text = string.Empty;
        }

        /// <summary>
        /// Prints some text to the output of the <see cref="ConsoleControl"/>
        /// </summary>
        /// <param name="text">The text to display</param>
        /// <param name="color">The color to print the text with, is White by default</param>
        public void Print(IEnumerable<char> text, Color? color = null)
        {
            var stringText = $"[{DateTime.Now:hh:mm:ss}] {string.Join("", text)}";

            // Adding newline at the beginning when there already is input in the TextBlock
            if (TextBlockOutput.Inlines.Count > 0)
                TextBlockOutput.Inlines.Add(new Run("\r\n"));
            // Adding a piece of text with a color into the TextBlock
            TextBlockOutput.Inlines.Add(
                new Run(stringText)
                { Foreground = new SolidColorBrush(color ?? Colors.White) }
            );
            // Scrolling to the bottom of the ScrollViewer so that the user always sees the new text
            ScrollViewerOutput.ScrollToBottom();
            
            // Adding the output to the output history
            _outputHistory.Enqueue(stringText);
        }

        /// <summary>
        /// Gets the input history as a <see cref="String"/> array
        /// </summary>
        /// <returns>The input history</returns>
        public IEnumerable<string> GetOutputHistory() => _outputHistory.ToArray();

        private void HandleInputKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;

            // Same functionality as a Click event from ButtonSend
            HandleSendButton(sender, new RoutedEventArgs());
        }
    }
}
