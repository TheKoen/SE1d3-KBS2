using System.Windows;
using System.Xml;
using KBS2.CitySystem;
using NUnit.Framework;

namespace UnitTests.CitySystem
{
    [TestFixture]
    public class CityParserTest
    {
        [Test]
        public void MakeCity()
        {
            var file = new XmlDocument();
            file.LoadXml("<City>\n\n" +
                         "<Roads>\n<Road Start=\"3,10\" End=\"42,35\" Width=\"20\" MaxSpeed=\"50\"></Road>\n</Roads>\n\n" +
                         "<Buildings>\n<Building Location=\"12,42\" Size=\"5\"></Building>\n</Buildings>\n\n" +
                         "<Intersections>\n<Intersection Location =\"35,13\" Size=\"5\"></Intersection>\n</Intersections>\n\n" +
                         "</City>");

            var city = CityParser.MakeCity(file, "Test City");

            Assert.AreEqual(1, city.Roads.Count);
            Assert.AreEqual(1, city.Buildings.Count);

            Assert.AreEqual(new Vector(3, 10), city.Roads[0].Start);
            Assert.AreEqual(new Vector(42, 35), city.Roads[0].End);
            Assert.AreEqual(20, city.Roads[0].Width);
            Assert.AreEqual(50, city.Roads[0].MaxSpeed);

            Assert.AreEqual(new Vector(12, 42), city.Buildings[0].Location);
            Assert.AreEqual(5, city.Buildings[0].Size);
        }
    }
}
