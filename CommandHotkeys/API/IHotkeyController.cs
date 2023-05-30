#if OPENMOD
using OpenMod.API.Ioc;
#endif
using System;

namespace CommandHotkeys.API
{
#if OPENMOD
    [Service]
#endif
    public interface IHotkeyController : IDisposable
    {
    }
}
