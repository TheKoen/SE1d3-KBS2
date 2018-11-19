using System.Collections.Generic;
using KBS2.Console;
using KBS2.Util;
using NUnit.Framework;

namespace UnitTests.CommandSystem
{
    [TestFixture]
    public class PropertyTest
    {
        private Property testProp = new Property("Test");
        
        [Test, Order(1)]
        public void RegisterProperty()
        {
            // Checking if the property gets registered correctly
            CommandHandler.RegisterProperty("UnitTest", ref testProp);
            Assert.Contains("UnitTest", CommandHandler.GetProperties().Keys);
            Assert.Throws(typeof(KeyExistsException),
                () => CommandHandler.RegisterProperty("UnitTest", ref testProp));
        }

        [Test, Order(2)]
        public void ModifyProperty()
        {
            // Checking if the property gets edited correctly and throws properly
            Assert.Throws(typeof(KeyNotFoundException), () => CommandHandler.ModifyProperty("Bla", "New Value"));
            Assert.Throws(typeof(TypeMismatchException), () => CommandHandler.ModifyProperty("UnitTest", 123));
            Assert.DoesNotThrow(() => CommandHandler.ModifyProperty("UnitTest", "New Value"));
            Assert.AreEqual("New Value", testProp.Value);
        }
    }
}