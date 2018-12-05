using System.Collections.Generic;
using KBS2.CarSystem;
using KBS2.CarSystem.Sensors;
using KBS2.CarSystem.Sensors.ActiveSensors;
using KBS2.CarSystem.Sensors.PassiveSensors;
using KBS2.ModelDesigner;
using Moq;
using NUnit.Framework;

namespace UnitTests.ModelDesigner
{
    [TestFixture]
    public class CarDesignTest
    {
        [Test]
        public void GetSensors()
        {
            var sensorList = new List<SensorPrototype>
            {
                new SensorPrototype
                {
                    Direction = Direction.Front,
                    Range = 30,
                    Create = Sensor.Sensors[typeof(CollisionSensor)]
                },
                new SensorPrototype
                {
                    Direction = Direction.Global,
                    Range = 45,
                    Create = Sensor.Sensors[typeof(EntityRadar)]
                }
            };
            
            var mockedDesign = new Mock<ICarDesign>();
            mockedDesign.Setup(x => x.SensorList)
                .Returns(sensorList);
            var design = mockedDesign.Object;

            var actual = design.SensorList;
            
            Assert.AreEqual(sensorList, actual);
        }
    }
}