#if OPENMOD
using CommandHotkeys.Models;
using OpenMod.API.Ioc;
using SDG.Unturned;
#endif
using System;
using System.Collections.Generic;
using System.Text;

namespace CommandHotkeys.API
{
#if OPENMOD
    [Service]
#endif
    public interface IHotkeyValidator
    {
        void Validate(Player player, PlayerCommandCandidates commandCandidates, EHotkeys hotkeys);
    }
}
