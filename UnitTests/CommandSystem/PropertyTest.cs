using System;
using CommandSystem.Exceptions;
using CommandSystem.PropertyManagement;
using NUnit.Framework;

namespace UnitTests.CommandSystem
{
    [TestFixture]
    public class PropertyTest
    {
        private bool _valueAndTypeStoredTestPassed = false;
        private bool _eventCalledTestPassed = false;
        
        [Order(0)]
        [TestCase(int.MaxValue, typeof(int))]
        [TestCase(short.MaxValue, typeof(short))]
        [TestCase(long.MaxValue, typeof(long))]
        [TestCase(float.MaxValue, typeof(float))]
        [TestCase(double.MaxValue, typeof(double))]
        [TestCase(new int[] {}, typeof(int[]))]
        [TestCase("", typeof(string))]
        [TestCase(new char[] {}, typeof(char[]))]
        public void ShouldStoreValueAndTypeCorrectly_WhenConstructorIsCalled(object input, Type expectedType)
        {
            var actual = new Property(input);
            
            Assert.AreEqual(input, actual.Value);
            Assert.AreEqual(expectedType, actual.Type);

            _valueAndTypeStoredTestPassed = true;
        }

        [Test, Order(1)]
        public void ShouldStoreProperty_WhenPropertyIsGiven()
        {
            if (!_valueAndTypeStoredTestPassed)
                Assert.Inconclusive("Required test \"ShouldStoreValueAndTypeCorrectly_WhenConstructorIsCalled\" did not pass");
            
            var innerProperty = new Property(0);
            
            var actual = new Property(innerProperty);
            
            Assert.AreEqual(innerProperty, actual.Value);
            Assert.AreEqual(typeof(Property), actual.Type);
        }

        [Test]
        public void ShouldThrowArgumentNullException_WhenConstructorIsCalledWithNull()
        {
            Assert.Throws<ArgumentNullException>(() => new Property(null));
        }

        [Test, Order(1)]
        public void ShouldInvokePropertyChangedEvent_WhenValueIsChanged()
        {
            if (!_valueAndTypeStoredTestPassed)
                Assert.Inconclusive("Required test \"ShouldStoreValueAndTypeCorrectly_WhenConstructorIsCalled\" did not pass");
            
            const double previousValue = 2d;
            const double newValue = 8d;
            var testProperty = new Property(previousValue);
            var propertyChanged = false;
            UserPropertyChangedArgs eventArgs = null;

            testProperty.PropertyChanged += (sender, args) =>
            {
                propertyChanged = true;
                eventArgs = args;
            };

            testProperty.Value = newValue;
            
            Assert.IsTrue(propertyChanged);
            Assert.AreEqual(previousValue, eventArgs.ValueBefore);
            Assert.AreEqual(newValue, eventArgs.ValueAfter);

            _eventCalledTestPassed = true;
        }

        [Test, Order(1)]
        public void ShouldThrowTypeMismatchException_WhenNewTypeDoesNotMatchPreviousType()
        {
            if (!_valueAndTypeStoredTestPassed)
                Assert.Inconclusive("Required test \"ShouldStoreValueAndTypeCorrectly_WhenConstructorIsCalled\" did not pass");
            
            var testProperty = new Property("Test");

            Assert.Throws<TypeMismatchException>(() => testProperty.Value = 12f);
        }

        [Order(2)]
        [TestCase(0, 1)]
        [TestCase((short) 0, (short) 1)]
        [TestCase(0L, 1L)]
        [TestCase(0f, 1f)]
        [TestCase(0d, 1d)]
        [TestCase(new[] { 0 }, new[] { 1 })]
        [TestCase("0", "1")]
        [TestCase(new [] {'0'}, new [] {'1'})]
        public void ShouldResetValueCorrectly_WhenResetToFirstValueIsCalled(object firstValue, object newValue)
        {
            if (!_eventCalledTestPassed)
                Assert.Inconclusive("Required test \"ShouldInvokePropertyChangedEvent_WhenValueIsChanged\" did not pass");
            
            var testProperty = new Property(firstValue);

            testProperty.Value = newValue;
            var actual = testProperty.ResetToFirstValue();
            
            Assert.AreEqual(firstValue, testProperty.Value);
            Assert.AreEqual(true, actual);
        }
    }
}