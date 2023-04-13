using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace CommandHotkeys.Models
{
    public class HotkeyedCommand
    {
        public string Name { get; set; } = string.Empty;
        public string Command { get; set; } = string.Empty;
        public List<EHotkeys> Hotkeys { get; set; } = new List<EHotkeys>();
    }
}
