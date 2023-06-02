using CommandShortcuts.API;
using CommandShortcuts.Models;
#if OPENMOD
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
#endif
using SDG.Unturned;
using System;
using System.Collections.Generic;

namespace CommandShortcuts.Services
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

        public TimeSpan TryUseCooldown(Player player, Shortcut shortcut)
        {
            Dictionary<string, DateTime> executedCommands = _executedCommands[player];

            if (!executedCommands.TryGetValue(shortcut.Permission, out DateTime lastExecution))
            {
                executedCommands.Add(shortcut.Permission, DateTime.MinValue);
                lastExecution = DateTime.MinValue;
            }

            TimeSpan remainingCooldown = lastExecution.AddSeconds(shortcut.Cooldown) - DateTime.Now;

            if (remainingCooldown < TimeSpan.Zero)
            {
                executedCommands[shortcut.Permission] = DateTime.Now;
            }

            return remainingCooldown;
        }
    }
}