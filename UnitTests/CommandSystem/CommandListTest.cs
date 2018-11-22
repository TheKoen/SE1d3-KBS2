using System.Collections.Generic;
using KBS2.Console;
using KBS2.Utilities;
using NUnit.Framework;
using ICommand = KBS2.Console.ICommand;

namespace UnitTests.CommandSystem
{
    [TestFixture]
    public class CommandListTest
    {
        [Test, Order(1)]
        public void RegisterCommand()
        {
            // Checking if the command gets registered correctly
            CommandHandler.RegisterCommand("UnitTest", new UnitTestCommand());
            Assert.Contains("unittest", CommandHandler.GetCommandNames());
            Assert.Throws(typeof(KeyExistsException),
                () => CommandHandler.RegisterCommand("UnitTest", new UnitTestCommand()));
        }

        [Test, Order(2)]
        public void RunCommand()
        {
            // Checking if the command runs successfully and throws properly
            IEnumerable<char> result = string.Empty;
            Assert.Throws(typeof(EmptyCommandException), () => CommandHandler.HandleInput("   "));
            Assert.Throws(typeof(UnknownCommandException), () => CommandHandler.HandleInput("Bla bla"));
            Assert.DoesNotThrow(() => result = CommandHandler.HandleInput("UnitTest Test result"));
            Assert.DoesNotThrow(() => result = CommandHandler.HandleInput("unittest Test result"));
            Assert.AreEqual("Test result", result);
        }
    }

    
    
    internal class UnitTestCommand : ICommand
    {
        public IEnumerable<char> Run(params string[] args)
        {
            return string.Join(" ", args);
        }
    }
}