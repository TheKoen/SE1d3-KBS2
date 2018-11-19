using System.Windows;
using System.Windows.Media;
using KBS2.Console;
using KBS2.Console.Commands;
using KBS2.Util;
using KBS2.Utilities;

namespace KBS2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Registering commands
            CommandHandler.RegisterCommand("Set", new CommandSet());
            
            // Console logic
            MainConsole.SendCommand += (sender, args) =>
            {
                var input = args.Command;

                try
                {
                    var output = CommandHandler.HandleInput(input);
                    MainConsole.Print(output);
                }
                catch (CommandException exception)
                {
                    MainConsole.Print(exception.Message, Colors.Red);
                }
            };
        }
    }
}
