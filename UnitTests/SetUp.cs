using System;
using System.IO.Packaging;
using System.Windows;
using NUnit.Framework;

namespace UnitTests
{
    [SetUpFixture]
    public class SetUp
    {
        [OneTimeSetUp]
        public void Init()
        {
            PackUriHelper.Create(new Uri("reliable://0"));
            Application.LoadComponent(
                new Uri("/KBS2;component/App.xaml", UriKind.Relative));
        }
    }
}