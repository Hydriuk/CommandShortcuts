using CommandHotkeys.API;
using CommandHotkeys.Models;
using Hydriuk.UnturnedModules.Adapters;
using Hydriuk.UnturnedModules.Extensions;
#if OPENMOD
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
#endif
using SDG.Unturned;
using System;
using UnityEngine;

namespace CommandHotkeys.Services
{
#if OPENMOD
    [PluginServiceImplementation(Lifetime = ServiceLifetime.Singleton)]
#endif
    public class CommandController : ICommandController
    {
        // Configuration
        private readonly float _maxHotkeyDelay;

        private readonly ICommandAdapter _commandAdapter;
        private readonly ICooldownProvider _cooldownProvider;
        private readonly IThreadAdapter _threadAdapter;
        private readonly ITranslationAdapter _translationAdapter;

        public CommandController(
            IConfigurationAdapter<Configuration> configuration,
            ICommandAdapter commandAdapter,
            IThreadAdapter threadAdapter,
            ICooldownProvider cooldownProvider,
            ITranslationAdapter translationAdapter)
        {
            _commandAdapter = commandAdapter;
            _threadAdapter = threadAdapter;
            _cooldownProvider = cooldownProvider;
            _translationAdapter = translationAdapter;

            _maxHotkeyDelay = 2f;
        }

        public void TryExecuteCommand(Player player, Shortcut shortcut)
        {
            TimeSpan timeToCooldown = _cooldownProvider.TryUseCooldown(player, shortcut);

            string parsedCommand = shortcut.Command
                .Replace("{PlayerID}", player.GetSteamID().ToString())
                .Replace("{PlayerName}", player.GetSteamPlayer().playerID.playerName)
                .Replace("{PlayerCharName}", player.GetSteamPlayer().playerID.characterName);

            if (timeToCooldown <= TimeSpan.Zero)
            {
                if(shortcut.ExecuteAsConsole)
                    _commandAdapter.Execute(parsedCommand);
                else
                    _commandAdapter.Execute(player, parsedCommand);
            }
            else
            {
                _threadAdapter.RunOnMainThread(() =>
                {
                    ChatManager.serverSendMessage(
                        _translationAdapter["CoolingDown", new { Seconds = Math.Ceiling(timeToCooldown.TotalSeconds) }],
                        Color.yellow,
                        toPlayer: player.GetSteamPlayer()
                    );
                });
            }
        }
    }
}