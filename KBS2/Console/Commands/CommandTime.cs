using System.Collections.Generic;
using CommandSystem;
using CommandSystem.Exceptions;
using KBS2.CitySystem;

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
                throw new CommandInputException("Command usage: \"time <pause/start/reset>\"");
            }

            var AILoop = MainScreen.AILoop;
            var WPFLoop = MainScreen.WPFLoop;
            var subcommand = args[0].ToLower();
            switch (subcommand)
            {
                case "start":
                    if (AILoop.IsRunning())
                    {
                        throw new CommandException("Simulation is already running!");
                    }
                    AILoop.Start();
                    WPFLoop.Start();
                    return "Simulation started.";
                case "pause":
                    if (!AILoop.IsRunning())
                    {
                        throw new CommandException("Simulation is not running!");
                    }
                    AILoop.Stop();
                    WPFLoop.Stop();
                    return "Simulation paused.";
                case "reset":
                    AILoop.Stop();
                    WPFLoop.Stop();
                    City.Instance.Controller.Reset();
                    return "Simulation reset. Type \"time start\" to start it again.";
                default:
                    throw new CommandInputException($"Unkown subcommand \'{subcommand}\'");
            }
        }
    }
}
