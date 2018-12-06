using System;
using System.Collections.Generic;
using System.IO;
using System.Security;
using CommandSystem;
using CommandSystem.Exceptions;

namespace KBS2.Console.Commands
{
    [CommandMetadata("export",
        Description = "Exports the contents of the console to a file",
        Usages = new [] { "export <file>" },
        AutoRegister = true)]
    public class CommandExport : ICommand
    {
        public IEnumerable<char> Run(params string[] args)
        {
            if (args.Length < 1) throw new CommandInputException("Invalid parameters");

            var filename = string.Join(" ", args);
            if (File.Exists(filename))
            {
                throw new CommandInputException($"File \"{filename}\" already exists");
            }

            // Trying to write to a file
            try
            {
                using (var sw = File.CreateText(filename))
                {
                    foreach (var line in App.Console.GetOutputHistory())
                    {
                        sw.WriteLine(line);
                    }
                }
            }
            catch (SystemException se)
                when(se.GetType() == typeof(UnauthorizedAccessException) ||
                     se.GetType() == typeof(SecurityException))
            {
                throw new CommandInputException($"Accessn't file \"{filename}\"");
            }
            catch (ArgumentException)
            {
                throw new CommandInputException("Can't write to a system device");
            }
            catch (IOException ioe)
                when (ioe.GetType() == typeof(PathTooLongException) ||
                      ioe.GetType() == typeof(DirectoryNotFoundException))
            {
                throw new CommandInputException($"Invalid path \"{filename}\"");
            }

            return $"Exported to \"{filename}\"";
        }
    }
}