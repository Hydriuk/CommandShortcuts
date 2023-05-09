#if OPENMOD
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
#endif
using CommandHotkeys.Models;
using Hydriuk.Unturned.SharedModules.PlayerKeyModule.Components;
using Hydriuk.Unturned.SharedModules.PlayerKeyModule.Models;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Linq;
using Hydriuk.Unturned.SharedModules.Adapters;
using CommandHotkeys.API;
using CommandHotkeys.Extensions;
using Hydriuk.Unturned.SharedModules.PlayerKeyModule.API;
using Hydriuk.Unturned.SharedModules.PlayerKeyModule.Services;

namespace CommandHotkeys.Services
{
#if OPENMOD
    [PluginServiceImplementation(Lifetime = ServiceLifetime.Singleton)]
#endif
    public class HotkeyController : IHotkeyController
    {
        // Services
        private readonly ICommandController _commandController;
        private readonly IPlayerKeyController _playerKeyController;
        private readonly IPermissionsAdapter _permissionsAdapter;

        // Configuration
        private readonly IEnumerable<CommandCandidate> _commandCandidatesAsset;
        private readonly float _maxDelay;

        // Data
        private readonly Dictionary<Player, PlayerCommandCandidates> _playerHotkeyCombo = new Dictionary<Player, PlayerCommandCandidates>();

        public HotkeyController(IConfigurationAdapter<Configuration> configuration, ICommandController commandController, IPermissionsAdapter permissionsAdapter)
        {
            _commandController = commandController;
            _permissionsAdapter = permissionsAdapter;
            _commandCandidatesAsset = configuration.Configuration.Commands.Select(command => new CommandCandidate(command));
            
            _maxDelay = 1f;

            _playerKeyController = new PlayerKeyController();

            PlayerKeyListener.KeyStateChanged += OnKeyStateChanged;

            Provider.onEnemyConnected += InitPlayer;
            Provider.onEnemyDisconnected += ClearPlayer;

            foreach (SteamPlayer sPlayer in Provider.clients)
            {
                InitPlayer(sPlayer);
            }
        }

        public void Dispose()
        {
            PlayerKeyListener.KeyStateChanged -= OnKeyStateChanged;

            Provider.onEnemyConnected -= InitPlayer;
            Provider.onEnemyDisconnected -= ClearPlayer;

            _playerKeyController.Dispose();
        }

        private void InitPlayer(SteamPlayer sPlayer) => _playerHotkeyCombo.Add(sPlayer.player, new PlayerCommandCandidates());
        private void ClearPlayer(SteamPlayer sPlayer) => _playerHotkeyCombo.Remove(sPlayer.player);

        private async void OnKeyStateChanged(Player player, EPlayerKey key, bool state)
        {
            if (!state)
                return;

            float time = Time.realtimeSinceStartup;

            PlayerCommandCandidates playerCombo = _playerHotkeyCombo[player];

            // Clear current combo
            if (time - playerCombo.LastHotkeyTime > _maxDelay)
            {
                playerCombo.Reset();
            }

            IEnumerable<string> playerPermissions = await _permissionsAdapter.GetPermissions(player.channel.owner.playerID.steamID);

            IEnumerable<CommandCandidate> commandCandidates = playerCombo.CommandCandidates ?? _commandCandidatesAsset
                .Where(command => playerPermissions.Contains(command.Command.Permission));

            // Get current hotkey
            EHotkeys hotkeys = ToHotkey(player.input.keys);

            // Filter commands by current hotkey pressed
            commandCandidates = commandCandidates
                .Where(commandCandidate => commandCandidate.TryValidate(hotkeys))
                .ToList();

            // Reset candidates if no more candidates are available
            if (commandCandidates.Count() == 0)
            {
                playerCombo.Reset();
                return;
            }

            playerCombo.CommandCandidates = commandCandidates;

            CommandCandidate validCommand = commandCandidates
                .Where(commandCandidate => commandCandidate.ValidatingIndex >= commandCandidate.Command.HotkeyList.Count)
                .FirstOrDefault();

            if (validCommand != null)
            {
                // Only execute the command if the index match the count
                if(validCommand.ValidatingIndex == validCommand.Command.HotkeyList.Count)
                {
                    _commandController.TryExecuteCommand(player, validCommand.Command);
                    playerCombo.Reset();
                }
            }

            // Update last hotkey pressed time
            playerCombo.LastHotkeyTime = time;
        }

        private EHotkeys ToHotkey(bool[] keys)
        {
            int hotkey = 0;

            for (int i = 0; i < keys.Length; i++)
            {
                if (keys[i])
                {
                    hotkey |= 1 << i;
                }
            }

            return (EHotkeys)hotkey;
        }
    }
}
