using System;
using System.Linq;

namespace KBS2.Console.Commands
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CommandMetadataAttribute : Attribute
    {
        private string[] _aliases = Array.Empty<string>();
        
        public string Key { get; }
        public string[] Aliases {
            get => _aliases;
            set => _aliases = value.Select(s => s.ToLowerInvariant()).ToArray();
        }
        public string Description { get; set; } = string.Empty;
        public string[] Usages { get; set; }
        public bool AutoRegister { get; set; } = false;

        public CommandMetadataAttribute(string key)
        {
            Key = key.ToLower();
            Usages = new [] { Key };
        }
    }
}