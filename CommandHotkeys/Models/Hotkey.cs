using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace CommandHotkeys.Models
{
    public struct Hotkey
    {
        public EHotkeys Hotkeys;
        public EHotkeyAction Action;

        public override bool Equals(object obj)
        {
            if (obj is not Hotkey other)
                return false;

            return Hotkeys == other.Hotkeys;
        }

        public override int GetHashCode()
        {
            return Hotkeys.GetHashCode();
        }
    }
}
