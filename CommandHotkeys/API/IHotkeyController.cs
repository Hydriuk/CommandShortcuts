#if OPENMOD
using OpenMod.API.Ioc;
#endif
using System;
using System.Collections.Generic;
using System.Text;

namespace CommandHotkeys.API
{
#if OPENMOD
    [Service]
#endif
    public interface IHotkeyController : IDisposable
    {
    }
}
