using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using KBS2.Console;
using KBS2.Exceptions;

namespace KBS2
{
    /// <summary>
    /// Interaction logic for ConsoleWindow.xaml
    /// </summary>
    public partial class ConsoleWindow : Window
    {
        public ConsoleWindow()
        {
            InitializeComponent();
            App.Console = MainConsole;

            // Registering commands
            CommandRegistrar.AutoRegisterCommands("KBS2.Console.Commands");

            // Console logic

            MainConsole.SendCommand += (sender, args) =>
            {
                var input = args.Command;

                try
                {
                    var output = CommandHandler.HandleInput(input);
                    if (output == null) return;
                    MainConsole.Print(output);
                }
                catch (CommandException exception)
                {
                    MainConsole.Print(exception.Message, Colors.Red);
                }
            };
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }
    }
}
