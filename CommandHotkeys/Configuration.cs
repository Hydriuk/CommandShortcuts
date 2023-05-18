using CommandHotkeys.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommandHotkeys
{
    public class Configuration
    {
        public List<HotkeyedCommand> Commands { get; set; } = new List<HotkeyedCommand>();
    }
}
