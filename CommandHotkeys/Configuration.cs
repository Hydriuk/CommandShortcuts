using CommandHotkeys.Models;
using System.Collections.Generic;

namespace CommandHotkeys
{
    public class Configuration
    {
        public List<HotkeyedCommand> Shortcuts { get; set; } = new List<HotkeyedCommand>();
    }
}