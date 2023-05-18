#if OPENMOD
using CommandHotkeys.API;
using CommandHotkeys.Models;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
using SDG.Unturned;
#endif
using System;
using System.Collections.Generic;
using System.IdentityModel.Protocols.WSTrust;
using System.Text;

namespace CommandHotkeys.Services
{
#if OPENMOD
    [PluginServiceImplementation(Lifetime = ServiceLifetime.Singleton)]
#endif
    public class CooldownProvider : ICooldownProvider
    {
        private readonly Dictionary<Player, Dictionary<string, DateTime>> _executedCommands = new Dictionary<Player, Dictionary<string, DateTime>>();

        public CooldownProvider() 
        {
            Provider.onEnemyConnected += InitPlayer;

            foreach (var sPlayer in Provider.clients)
            {
                InitPlayer(sPlayer);
            }
        }

        public void Dispose()
        {
            Provider.onEnemyConnected -= InitPlayer;
        }

        private void InitPlayer(SteamPlayer sPlayer)
        {
            _executedCommands.Add(sPlayer.player, new Dictionary<string, DateTime>());
        }

        public TimeSpan TryUseCooldown(Player player, HotkeyedCommand command)
        {
            Dictionary<string, DateTime> executedCommands = _executedCommands[player];

            if(!executedCommands.TryGetValue(command.Name, out DateTime lastExecution))
            {
                executedCommands.Add(command.Name, DateTime.MinValue);
                lastExecution = DateTime.MinValue;
            }

            TimeSpan remainingCooldown = lastExecution.AddSeconds(command.Cooldown) - DateTime.Now;

            if(remainingCooldown < TimeSpan.Zero)
            {
                executedCommands[command.Name] = DateTime.Now;
            }

            return remainingCooldown;
        }
    }
}
