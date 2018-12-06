using System;
using System.Linq;

namespace CommandSystem
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class CommandMetadataAttribute : Attribute
    {
        private string[] _aliases = Array.Empty<string>();
        
        public string Key { get; }
        
        public string[] Aliases
        {
            get => _aliases;
            set => _aliases = value.Select(s => s.ToLowerInvariant()).ToArray();
        }

        public string Description { get; set; } = string.Empty;
        
        public string[] Usages { get; set; }
        
        public bool AutoRegister { get; set; }

        public CommandMetadataAttribute(string key)
        {
            Key = key.ToLowerInvariant();
            Usages = new[] {Key};
        }
    }
}