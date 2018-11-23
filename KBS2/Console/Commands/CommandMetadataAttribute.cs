using System;

namespace KBS2.Console.Commands
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CommandMetadataAttribute : Attribute
    {
        public string Key { get; }
        public string[] Aliases { get; set; } = Array.Empty<string>();
        public string Description { get; set; } = string.Empty;
        public string[] Usages { get; set; } = Array.Empty<string>();
        public bool AutoRegister { get; set; } = false;

        public CommandMetadataAttribute(string key)
        {
            Key = key;
        }
    }
}