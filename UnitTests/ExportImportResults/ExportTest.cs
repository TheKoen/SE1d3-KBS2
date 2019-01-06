using KBS2.Util;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitTests.Util;

namespace UnitTests.ExportImportResults
{
    [TestFixture]
    public class ExportTest
    {
        [TestCase()]
        public void ExportSimulation()
        {
            DataBaseUnitTest.FillUnitTestDatabaseWithTestData();

            ResultExport.ExportResult(1, "UnitTestDatabase");
        }
    }
}
