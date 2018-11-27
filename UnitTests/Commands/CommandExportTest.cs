using System;
using System.IO;
using System.Threading;
using KBS2;
using KBS2.Console;
using KBS2.Console.Commands;
using KBS2.Exceptions;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace UnitTests.Commands
{
    [TestFixture, Apartment(ApartmentState.STA)]
    public class CommandExportTest
    {
        private readonly ICommand _command = new CommandExport();
        private const string Filename1 = "_test_fileexists";
        private const string Filename2Win = "C:\\Windows\\System32\\_test_noperm";
        private const string Filename2Unix = "/_test_noperm";
        private const string Filename3Unix = "/dev/null";
        private readonly string _filename4 = new string('a', 256);
        private const string Filename5 = "_test_success";

        private string _filename2 = string.Empty;
        
        [SetUp]
        public void Init()
        {
            typeof(MainWindow).GetProperty("Console")?.SetValue(null, new ConsoleControl());
            
            File.Create(Filename1).Close();

            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Win32NT:
                    _filename2 = Filename2Win;
                    break;
                case PlatformID.MacOSX:
                case PlatformID.Unix:
                    _filename2 = Filename2Unix;
                    break;
                default:
                    Assert.Fail($"Why the fuck are you running this on {Enum.GetName(typeof(PlatformID), Environment.OSVersion.Platform)}?");
                    break;
            }
        }

        [Test]
        public void ExportFileExists()
        {
            // On file exists
            Assert.Throws(typeof(InvalidParametersException), () => _command.Run(Filename1));
        }

        [Test]
        public void ExportNoPermission()
        {
            // On no permission to write file
            Assert.Throws(typeof(InvalidParametersException), () => _command.Run(_filename2));
        }

        [Test]
        public void ExportSystemDevice()
        {
            // Ignore when not Unix or MacOSX
            if (Environment.OSVersion.Platform != PlatformID.Unix &&
                Environment.OSVersion.Platform != PlatformID.MacOSX)
            {
                Assert.Ignore();
                return;
            }
            // On export to system device
            Assert.Throws(typeof(InvalidParametersException), () => _command.Run(Filename3Unix));
        }

        [Test]
        public void ExportPathTooLong()
        {
            // On path too long
            Assert.Throws(typeof(InvalidParametersException), () => _command.Run(_filename4));
        }

        [Test]
        public void ExportSuccess()
        {
            // On successful export
            Assert.DoesNotThrow(() => _command.Run(Filename5));
            File.Delete(Filename5);
            Assert.AreEqual($"Exported to \"{Filename5}\"", _command.Run(Filename5));
        }

        [TearDown]
        public void Cleanup()
        {
            if (File.Exists(Filename1)) File.Delete(Filename1);
            if (File.Exists(Filename5)) File.Delete(Filename5);
            
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Win32NT:
                    try { File.Delete(Filename2Win); } catch (Exception) { /* ignored */ }
                    break;
                case PlatformID.MacOSX:
                case PlatformID.Unix:
                    try { File.Delete(Filename2Unix); } catch (Exception) { /* ignored */ }
                    break;
            }
        }
    }
}