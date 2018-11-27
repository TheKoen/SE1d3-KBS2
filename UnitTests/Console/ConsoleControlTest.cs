using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using KBS2.Console;
using NUnit.Framework;

namespace UnitTests.Console
{
    [TestFixture, Apartment(ApartmentState.STA)]
    public class ConsoleControlTest
    {
        private ConsoleControl _console;
        private Regex _printMethodRegex;

        private bool _sendCommandEventTestPassed;
        private bool _inputHistoryIndexTestPassed;
        
        [SetUp]
        public void Init()
        {
            _console = new ConsoleControl();
            _printMethodRegex = new Regex(@"^\[(?<time>\d{2}:\d{2}:\d{2})\] (?<text>.*)$");
        }
        
        [TestCase("", ""), Order(1)]
        [TestCase("Test", "Test")]
        public void SendCommandEventTest(string input, string expected)
        {
            var result = string.Empty;

            var handler = new ConsoleControl.SendCommandHandler(
                (sender, args) => result = args.Command
            );

            // Setting the event handler
            _console.SendCommand += handler;
            _console.TextBoxInput.Text = input;
            
            // Invoking the Click event on the Send button
            _console.ButtonSend.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            Assert.AreEqual(expected, result);

            // Removing the event handler
            _console.SendCommand -= handler;

            _sendCommandEventTestPassed = true;
        }

        [TestCase("", ""), Order(1)]
        [TestCase("Test1", "Test1")]
        [TestCase("Test2", "Test2")]
        [TestCase("Test3", "Test3")]
        [TestCase("Test4", "Test4")]
        [TestCase("Test5", "Test5")]
        public void PrintMethodTest(string input, string expected)
        {
            // Printing the input and getting the output from the console
            _console.Print(input);
            _console.PrintQueue();
            var output = _console.GetOutputHistory().Last();
            
            // Checking if the format of the output is correct
            Assert.True(_printMethodRegex.IsMatch(output));

            // Getting data from the Regex
            var match = _printMethodRegex.Match(output);
            var time = DateTime.ParseExact(match.Groups["time"].Value, "HH:mm:ss", CultureInfo.CurrentCulture);
            var text = match.Groups["text"].Value;

            // Checking if the time in the output matches the current time
            // (Using current time because messages get sent way fast)
            var minDeviation = DateTime.Now.Subtract(new TimeSpan(0, 0, 1));
            var maxDeviation = DateTime.Now.Add(new TimeSpan(0, 0, 1));
            
            Assert.True(time > minDeviation && time < maxDeviation);
            
            Assert.AreEqual(expected, text);
        }

        [Test, Order(2)]
        public void InputHistoryIndexTest()
        {
            if (!_sendCommandEventTestPassed)
                Assert.Inconclusive("SendCommandEvent test did not pass/run, but is required for this test to work");

            // Resetting the Console
            _console = new ConsoleControl();
            
            // Verifying that the TextBox stays empty when there's no history
            RaiseKeyUpEvent(ref _console);
            Assert.AreEqual(string.Empty, _console.TextBoxInput.Text);
            RaiseKeyDownEvent(ref _console);
            Assert.AreEqual(string.Empty, _console.TextBoxInput.Text);
            
            // Writing history ( ͡° ͜ʖ ͡°)
            _console.TextBoxInput.Text = "Test1";
            _console.ButtonSend.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            _console.TextBoxInput.Text = "Test2";
            _console.ButtonSend.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            // Testing if the history contains what it should contain
            RaiseKeyUpEvent(ref _console);
            Assert.AreEqual("Test2", _console.TextBoxInput.Text);
            RaiseKeyUpEvent(ref _console);
            Assert.AreEqual("Test1", _console.TextBoxInput.Text);
            RaiseKeyDownEvent(ref _console);
            Assert.AreEqual("Test2", _console.TextBoxInput.Text);
            RaiseKeyDownEvent(ref _console);
            Assert.AreEqual(string.Empty, _console.TextBoxInput.Text);

            _inputHistoryIndexTestPassed = true;
        }

        [Test, Order(3)]
        public void InputHistoryCapacityTest()
        {
            if (!_sendCommandEventTestPassed)
                Assert.Inconclusive("SendCommandEvent test did not pass/run, but is required for this test to work");
            if (!_inputHistoryIndexTestPassed)
                Assert.Inconclusive("InputHistoryIndex test did not pass/run, but is required for this test to work");
            
            // Resetting the Console
            _console = new ConsoleControl();
            
            _console.InputHistoryCapacity = 1;
            
            // Writing more history (˵ ͡~ ͜ʖ ͡°˵)ﾉ⌒♡*:･。.
            _console.TextBoxInput.Text = "Test1";
            _console.ButtonSend.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            _console.TextBoxInput.Text = "Test2";
            _console.ButtonSend.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            
            // Checking if the history is really only one entry, and if that entry is what it should be
            RaiseKeyUpEvent(ref _console);
            Assert.AreEqual("Test2", _console.TextBoxInput.Text);
            RaiseKeyUpEvent(ref _console);
            Assert.AreEqual("Test2", _console.TextBoxInput.Text);
        }


        private void RaiseKeyUpEvent(ref ConsoleControl console)
        {
            console.TextBoxInput.RaiseEvent(
                new KeyEventArgs(
                        Keyboard.PrimaryDevice,
                        new HwndSource(0, 0, 0, 0, 0, "", IntPtr.Zero), 
                        0,
                        Key.Up)
                    { RoutedEvent = UIElement.PreviewKeyDownEvent }
            );
        }

        private void RaiseKeyDownEvent(ref ConsoleControl console)
        {
            console.TextBoxInput.RaiseEvent(
                new KeyEventArgs(
                        Keyboard.PrimaryDevice,
                        new HwndSource(0, 0, 0, 0, 0, "", IntPtr.Zero), 
                        0,
                        Key.Down)
                    { RoutedEvent = UIElement.PreviewKeyDownEvent }
            );
        }
    }
}