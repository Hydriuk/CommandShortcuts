using CommandShortcuts.API;
using CommandShortcuts.Models;
using Hydriuk.UnturnedModules.Adapters;
using Hydriuk.UnturnedModules.Extensions;
using LiteDB;
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
        //private readonly Dictionary<Player, Dictionary<string, DateTime>> _executedCommands = new Dictionary<Player, Dictionary<string, DateTime>>();

        private readonly LiteDatabase _database;
        private readonly ILiteCollection<Cooldown> _cooldowns;

        public CooldownProvider(IEnvironmentAdapter environmentAdapter)
        {
            _database = new LiteDatabase($"{environmentAdapter.Directory}/cooldowns.db");
            _cooldowns = _database.GetCollection<Cooldown>();

            _cooldowns.EnsureIndex(cooldown => cooldown.PlayerID);
            _cooldowns.EnsureIndex(cooldown => cooldown.Permission);
        }

        public void Dispose()
        {
            _database.Dispose();
        }

        public TimeSpan TryUseCooldown(Player player, Shortcut shortcut)
        {
            Cooldown cooldown = _cooldowns.FindOne(cooldown => cooldown.PlayerID == player.GetSteamID().m_SteamID && cooldown.Permission == shortcut.Permission);

            if(cooldown == null)
            {
                cooldown = new Cooldown()
                {
                    PlayerID = player.GetSteamID().m_SteamID,
                    Permission = shortcut.Permission
                };
            }

            TimeSpan remainingCooldown = cooldown.CooledDownAt - DateTime.Now;

            if (remainingCooldown < TimeSpan.Zero)
            {
                cooldown.CooledDownAt = DateTime.Now.AddSeconds(shortcut.Cooldown);
                _cooldowns.Upsert(cooldown);
            }

            return remainingCooldown;
        }
    }
}