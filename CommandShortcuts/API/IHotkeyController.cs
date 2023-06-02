#if OPENMOD
using OpenMod.API.Ioc;
#endif
using System;

namespace CommandShortcuts.API
{
#if OPENMOD
    [Service]
#endif
    public interface IHotkeyController : IDisposable
    {
    }
}
