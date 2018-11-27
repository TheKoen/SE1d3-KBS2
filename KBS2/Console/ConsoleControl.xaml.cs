using System;
using System.Collections.Generic;
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

        private readonly LinkedList<string> _inputHistory = new LinkedList<string>();
        private readonly Queue<string> _outputHistory = new Queue<string>();
        private int _inputHistoryCapacity = 32;
        private int _outputHistoryCapacity = 256;
        private int _inputHistoryIndex = -1;
        private Queue<Tuple<IEnumerable<char>, Color?>> _printQueue = new Queue<Tuple<IEnumerable<char>, Color?>>();

        public int InputHistoryCapacity
        {
            get => _inputHistoryCapacity;
            set
            {
                if (value < 1) throw new IndexOutOfRangeException("History capacity may not be less than 1");
                _inputHistoryCapacity = value;
                while (_inputHistory.Count > _inputHistoryCapacity)
                    _inputHistory.RemoveFirst();
                if (_inputHistoryIndex >= _inputHistoryCapacity)
                    _inputHistoryIndex = -1;
            }
        }

        public int OutputHistoryCapacity
        {
            get => _outputHistoryCapacity;
            set
            {
                if (value < 1) throw new IndexOutOfRangeException("History capacity may not be less than 1");
                _outputHistoryCapacity = value;
                while (_outputHistory.Count > _outputHistoryCapacity)
                    _outputHistory.Dequeue();
            }
        }
        
        public ConsoleControl()
        {
            InitializeComponent();
            
            ButtonSend.Click += HandleSend;
            MainWindow.CommandLoop.Subscribe(PrintQueue);
        }

        private void HandleSend(object sender, RoutedEventArgs args)
        {
            // Getting the command from the TextBox and invoking the SendCommand event
            var command = TextBoxInput.Text.Trim();
            SendCommand?.Invoke(this, new SendCommandArgs(command));
            if (_inputHistory.Count >= _inputHistoryCapacity)
                _inputHistory.RemoveFirst();
            _inputHistory.AddLast(command);
            TextBoxInput.Text = string.Empty;
            _inputHistoryIndex = -1;
        }

        /// <summary>
        /// Prints some text to the output of the <see cref="ConsoleControl"/>
        /// </summary>
        /// <param name="text">The text to display</param>
        /// <param name="color">The color to print the text with, is White by default</param>
        public void Print(IEnumerable<char> text, Color? color = null)
        {
            _printQueue.Enqueue(new Tuple<IEnumerable<char>, Color?>(text, color));
        }

        public void PrintQueue()
        {
            while (_printQueue.Count > 0)
            {
                var queued = _printQueue.Dequeue();
                
                var stringText = $"[{DateTime.Now:HH:mm:ss}] {string.Join("", queued.Item1)}";

                // Adding newline at the beginning when there already is input in the TextBlock
                if (TextBlockOutput.Inlines.Count > 0)
                    TextBlockOutput.Inlines.Add(new Run("\r\n"));
                // Adding a piece of text with a color into the TextBlock
                TextBlockOutput.Inlines.Add(
                    new Run(stringText)
                        { Foreground = new SolidColorBrush(queued.Item2 ?? Colors.White) }
                );
                // Scrolling to the bottom of the ScrollViewer so that the user always sees the new text
                ScrollViewerOutput.ScrollToBottom();
            
                // Adding the output to the output history
                if (_outputHistory.Count >= _outputHistoryCapacity)
                    _outputHistory.Dequeue();
                _outputHistory.Enqueue(stringText);
            }
        }

        /// <summary>
        /// Gets the input history as a <see cref="String"/> array
        /// </summary>
        /// <returns>The input history</returns>
        public IEnumerable<string> GetOutputHistory() => _outputHistory.ToArray();

        private void HandleInputKeyDown(object sender, KeyEventArgs e)
        {
            LinkedListNode<string> current;
            switch (e.Key)
            {
                case Key.Enter:
                    // Sending input
                    HandleSend(sender, new RoutedEventArgs());
                    break;
                case Key.Up:
                    // Getting the next value in the input history
                    if (_inputHistory.Count <= _inputHistoryIndex + 1) break;
                    ++_inputHistoryIndex;
                    
                    current = _inputHistory.Last;
                    // Finding the correct element in the input history
                    for (var i = 0; i < _inputHistoryIndex; ++i)
                        current = current.Previous;
                    TextBoxInput.Text = current.Value;
                    // Making sure the caret is in the correct position
                    TextBoxInput.CaretIndex = current.Value.Length;
                    break;
                case Key.Down:
                    // Getting the previous value in the input history
                    if (_inputHistoryIndex <= -1) break;
                    --_inputHistoryIndex;
                    
                    if (_inputHistoryIndex <= -1)
                    {
                        // Clearing the TextBox when we're back at index -1
                        TextBoxInput.Text = string.Empty;
                        break;
                    }
                    
                    current = _inputHistory.Last;
                    // Finding the correct element in the input history
                    for (var i = 0; i < _inputHistoryIndex; ++i)
                        current = current.Previous;
                    TextBoxInput.Text = current.Value;
                    // Making sure the caret is in the correct position
                    TextBoxInput.CaretIndex = current.Value.Length;
                    break;
            }
        }
    }
}
