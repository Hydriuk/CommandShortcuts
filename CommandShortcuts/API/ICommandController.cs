using CommandShortcuts.Models;
#if OPENMOD
using OpenMod.API.Ioc;
#endif
using SDG.Unturned;

namespace CommandShortcuts.API
{
#if OPENMOD
    [Service]
#endif
    public interface ICommandController
    {
        void TryExecuteCommand(Player player, Shortcut shortcut);
    }
}
