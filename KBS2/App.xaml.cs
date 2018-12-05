using System.Windows;
using KBS2.Console;
using KBS2.Console.Commands;

namespace KBS2
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        public static ConsoleControl Console;

        public App()
        {
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            CommandMap.Stop();
        }
    }
}
