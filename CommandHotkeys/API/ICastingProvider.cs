using CommandHotkeys.Models;
#if OPENMOD
using OpenMod.API.Ioc;
#endif
using SDG.Unturned;
using System;

namespace CommandHotkeys.API
{
#if OPENMOD
    [Service]
#endif
    public interface ICastingProvider : IDisposable
    {
        bool TryStartCast(Player player, CommandCandidate commandCandidate);
        bool IsCasting(Player player, string commandName);
        void AbortCast(Player player, string commandName);

        event Action<Player, CommandCandidate>? Casted;
    }
}
