using CommandHotkeys.Models;
#if OPENMOD
using OpenMod.API.Ioc;
#endif
using SDG.Unturned;

namespace CommandHotkeys.API
{
#if OPENMOD
    [Service]
#endif
    public interface ICommandController
    {
        void TryExecuteCommand(Player player, HotkeyedCommand command);
    }
}
