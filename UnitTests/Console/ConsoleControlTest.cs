using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls.Primitives;
using KBS2.Console;
using NUnit.Framework;

namespace UnitTests.Console
{
    [TestFixture, Apartment(ApartmentState.STA)]
    public class ConsoleControlTest
    {
        private ConsoleControl _console;
        private Regex _printMethodRegex;
        
        [SetUp]
        public void Init()
        {
            _console = new ConsoleControl();
            _printMethodRegex = new Regex(@"^\[(?<time>\d{2}:\d{2}:\d{2})\] (?<text>.*)$");
        }

        [TestCase("", "")]
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
        }

        [TestCase("", "")]
        [TestCase("Test1", "Test1")]
        [TestCase("Test2", "Test2")]
        [TestCase("Test3", "Test3")]
        [TestCase("Test4", "Test4")]
        [TestCase("Test5", "Test5")]
        public void PrintMethodTest(string input, string expected)
        {
            // Printing the input and getting the output from the console
            _console.Print(input);
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
    }
}