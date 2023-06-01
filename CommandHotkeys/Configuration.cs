using CommandHotkeys.Models;
using System.Collections.Generic;

namespace CommandHotkeys
{
    public class Configuration
    {
        public List<Shortcut> Shortcuts { get; set; } = new List<Shortcut>();
    }
}