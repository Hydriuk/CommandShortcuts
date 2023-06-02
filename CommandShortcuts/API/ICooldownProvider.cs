using CommandShortcuts.Models;
#if OPENMOD
using OpenMod.API.Ioc;
#endif
using SDG.Unturned;
using System;

namespace CommandShortcuts.API
{
#if OPENMOD
    [Service]
#endif
    public interface ICooldownProvider : IDisposable
    {
        TimeSpan TryUseCooldown(Player player, Shortcut shortcut);
    }
}
