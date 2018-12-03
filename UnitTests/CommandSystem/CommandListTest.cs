using System;
using System.Collections.Generic;
using System.Linq;
using CommandSystem;
using CommandSystem.Exceptions;
using KBS2.Exceptions;
using NUnit.Framework;

namespace UnitTests.CommandSystem
{
    [TestFixture]
    public class CommandListTest
    {
        private bool _registerCommandTestRan;
        
        [Test, Order(1)]
        public void RegisterCommand()
        {
            // Checking if the command gets registered correctly
            CommandHandler.RegisterCommand(typeof(UnitTestCommand));
            Assert.Contains("unittest", CommandHandler.GetCommandNames().ToArray());
            Assert.Throws(typeof(CommandHandlerException),
                () => CommandHandler.RegisterCommand(typeof(UnitTestCommand)));
            _registerCommandTestRan = true;
        }

        [Test, Order(2)]
        public void RunCommand()
        {
            if (!_registerCommandTestRan)
                Assert.Inconclusive("RegisterCommand did not run yet.\nMake sure to test on CommandListTest instead of RunCommand.");
            
            // Checking if the command runs successfully and throws properly
            IEnumerable<char> result = string.Empty;
            Assert.Throws(typeof(CommandInputException), () => CommandHandler.HandleInput("   "));
            Assert.Throws(typeof(CommandInputException), () => CommandHandler.HandleInput("Bla bla"));
            Assert.DoesNotThrow(() => result = CommandHandler.HandleInput("UnitTest Test result"));
            Assert.DoesNotThrow(() => result = CommandHandler.HandleInput("unittest Test result"));
            Assert.AreEqual("Test result", result);
        }
    }

    
    
    [CommandMetadata("UnitTest")]
    internal class UnitTestCommand : ICommand
    {
        public IEnumerable<char> Run(params string[] args)
        {
            return string.Join(" ", args);
        }
    }
}