using System;

namespace KBS2.Console.Commands
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CommandMetadata : Attribute
    {
        public string Key { get; }
        public string[] Aliases { get; set; } = Array.Empty<string>();
        public string Description { get; set; } = string.Empty;
        public string[] Usages { get; set; } = Array.Empty<string>();
        public bool AutoRegister { get; set; } = false;

        public CommandMetadata(string key)
        {
            Key = key;
        }
    }
}