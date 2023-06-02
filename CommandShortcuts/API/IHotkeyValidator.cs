using CommandShortcuts.Models;
#if OPENMOD
using OpenMod.API.Ioc;
#endif
using SDG.Unturned;
using System;
using System.Collections.Generic;

namespace CommandShortcuts.API
{
#if OPENMOD
    [Service]
#endif
    public interface IHotkeyValidator : IDisposable
    {
        void Validate(Player player, IEnumerable<CommandCandidate> commandCandidates, EHotkeys hotkeys);
        void ValidateKey(Player player, CommandCandidate commandCandidate);

        event Action<Player, CommandCandidate> FinalKeyValidated;
    }
}
