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
    public interface IEffectProvider : IDisposable
    {
        void SendValidatedEffect(Player player);
    }
}
