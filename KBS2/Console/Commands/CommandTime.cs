using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KBS2.CitySystem;
using KBS2.Exceptions;
using KBS2.Utilities;

namespace KBS2.Console.Commands
{
    [CommandMetadata("time",
        Description = "Allows you to pause, start and reset the simulation",
        Usages = new[] { "time <pause/start/reset>" },
        AutoRegister = true)]
    class CommandTime : ICommand
    {
        public IEnumerable<char> Run(params string[] args)
        {
            if (args.Length == 0)
            {
                throw new InvalidParametersException("Command usage: \"time <pause/start/reset>\"");
            }

            var loop = MainWindow.Loop;
            var subcommand = args[0].ToLower();
            switch (subcommand)
            {
                case "start":
                    if (loop.IsRunning())
                    {
                        throw new CommandException("Simulation is already running!");
                    }
                    loop.Start();
                    return "Simulation started.";
                case "pause":
                    if (!loop.IsRunning())
                    {
                        throw new CommandException("Simulation is not running!");
                    }
                    loop.Stop();
                    return "Simulation paused.";
                case "reset":
                    loop.Stop();
                    City.Instance.Controller.Reset();
                    return "Simulation reset. Type \"time start\" to start it again.";
                default:
                    throw new InvalidParametersException($"Unkown subcommand \'{subcommand}\'");
            }
        }
    }
}
