using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
            // Gets the command from the TextBox and invokes the SendCommand event
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
            var frame = new StackFrame(1, true);
            var sender = frame.GetMethod().DeclaringType;

            if (TextBlockOutput.Inlines.Count > 0)
                TextBlockOutput.Inlines.Add(new Run("\r\n"));
            TextBlockOutput.Inlines.Add(
                new Run(string.Format("[{0}] {1}", sender.FullName, string.Join("", text)))
                { Foreground = new SolidColorBrush(color) }
            );
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
