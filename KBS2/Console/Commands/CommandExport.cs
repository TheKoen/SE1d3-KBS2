using System;
using System.Collections.Generic;
using System.IO;
using System.Security;
using KBS2.Exceptions;

namespace KBS2.Console.Commands
{
    [CommandMetadata("Export",
        Description = "Exports the contents of the console to a file")]
    public class CommandExport : ICommand
    {
        public IEnumerable<char> Run(params string[] args)
        {
            if (args.Length < 1) throw new InvalidParametersException();

            var filename = string.Join(" ", args);
            if (File.Exists(filename))
            {
                throw new InvalidParametersException($"File \"{filename}\" already exists");
            }

            // Trying to write to a file
            try
            {
                using (var sw = File.CreateText(filename))
                {
                    foreach (var line in MainWindow.Console.GetOutputHistory())
                    {
                        sw.WriteLine(line);
                    }
                }
            }
            catch (SystemException se)
                when(se.GetType() == typeof(UnauthorizedAccessException) ||
                     se.GetType() == typeof(SecurityException))
            {
                throw new InvalidParametersException($"Accessn't file \"{filename}\"");
            }
            catch (ArgumentException)
            {
                throw new InvalidParametersException("Can't write to a system device");
            }
            catch (IOException ioe)
                when (ioe.GetType() == typeof(PathTooLongException) ||
                      ioe.GetType() == typeof(DirectoryNotFoundException))
            {
                throw new InvalidParametersException($"Invalid path \"{filename}\"");
            }

            return $"Exported to \"{filename}\"";
        }
    }
}