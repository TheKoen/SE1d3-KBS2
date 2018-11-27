﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using KBS2.CitySystem;
using KBS2.Exceptions;

namespace KBS2.Console.Commands
{
    [CommandMetadata("load",
        Description = "Loads a city from an XML file",
        Usages = new[] { "load <file>" },
        AutoRegister = true)]
    public class CommandLoad : ICommand
    {
        public IEnumerable<char> Run(params string[] args)
        {
            if (args.Length == 0)
            {
                throw new InvalidParametersException("Command usage: \"load <file>\"");
            }

            if (!File.Exists(args[0]))
            {
                throw new InvalidParametersException($"Cannot find file \"{args[0]}\"");
            }
            
            try
            {
                var file = new XmlDocument();
                file.Load(args[0]);
                CityParser.MakeCity(file);
            }
            catch (Exception e)
            {
                throw new CommandException($"Error while loading city: {e}");
            }

            return "Loaded city.";
        }
    }
}