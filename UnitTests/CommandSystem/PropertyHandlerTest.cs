using CommandSystem.Exceptions;
using CommandSystem.PropertyManagement;
using NUnit.Framework;

namespace UnitTests.CommandSystem
{
    [TestFixture]
    public class PropertyHandlerTest
    {
        private bool _registerPropertyTestRan;
        
        private Property testProp = new Property("Test");
        
        [Test, Order(1)]
        public void RegisterProperty()
        {
            // Checking if the property gets registered correctly
            PropertyHandler.RegisterProperty("UnitTest", ref testProp);
            Assert.Contains("UnitTest", PropertyHandler.GetProperties().Keys);
            Assert.Throws(typeof(PropertyHandlerException),
                () => PropertyHandler.RegisterProperty("UnitTest", ref testProp));
            _registerPropertyTestRan = true;
        }

        [Test, Order(2)]
        public void ModifyProperty()
        {
            if (!_registerPropertyTestRan)
                Assert.Inconclusive("RegisterProperty did not run yet.\nMake sure to test on PropertyTest instead of ModifyProperty.");
            
            // Checking if the property gets edited correctly and throws properly
            Assert.Throws(typeof(PropertyHandlerException), () => PropertyHandler.ModifyProperty("Bla", "New Value"));
            Assert.Throws(typeof(TypeMismatchException), () => PropertyHandler.ModifyProperty("UnitTest", 123));
            Assert.DoesNotThrow(() => PropertyHandler.ModifyProperty("UnitTest", "New Value"));
            Assert.AreEqual("New Value", testProp.Value);
        }
    }
}