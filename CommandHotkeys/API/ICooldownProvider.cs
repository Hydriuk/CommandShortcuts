using CommandHotkeys.Models;
#if OPENMOD
using OpenMod.API.Ioc;
#endif
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommandHotkeys.API
{
#if OPENMOD
    [Service]
#endif
    public interface ICooldownProvider : IDisposable
    {
        TimeSpan TryUseCooldown(Player player, HotkeyedCommand command);
    }
}
