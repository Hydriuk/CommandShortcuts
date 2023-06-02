using CommandShortcuts.Models;
using System.Collections.Generic;

namespace CommandShortcuts
{
    public class Configuration
    {
        public List<Shortcut> Shortcuts { get; set; } = new List<Shortcut>();
    }
}