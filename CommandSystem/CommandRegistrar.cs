using System;
using System.Linq;

namespace CommandSystem
{
    public static class CommandRegistrar
    {
        /// <summary>
        /// Looks for classes with the <see cref="CommandMetadataAttribute"/> attribute and registers them if they are set to AutoRegister
        /// </summary>
        /// <param name="namespace">The namespace to scan for classes</param>
        public static void AutoRegisterCommands(string @namespace) =>
            // Getting all classes in this domain that implement ICommand and are in the given namespace
            AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(t => t.GetTypes())
                .Where(t => t.IsClass && t.Namespace == @namespace && typeof(ICommand).IsAssignableFrom(t))
                .Where(t => Attribute.IsDefined(t, typeof(CommandMetadataAttribute)))
                .Where(t =>
                    ((CommandMetadataAttribute) Attribute.GetCustomAttribute(t, typeof(CommandMetadataAttribute)))
                    .AutoRegister)
                .ToList().ForEach(CommandHandler.RegisterCommand);
    }
}