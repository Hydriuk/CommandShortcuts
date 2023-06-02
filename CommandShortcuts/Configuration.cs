using CommandShortcuts.Models;
using System.Collections.Generic;

namespace CommandShortcuts
{
    public class Configuration
    {
        public string ValidatedEffectGUID { get; set; } = string.Empty;
        public string ChatIcon { get; set; } = string.Empty;

        public List<Shortcut> Shortcuts { get; set; } = new List<Shortcut>();
    }
}