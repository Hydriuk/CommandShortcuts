#if OPENMOD
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
#endif
using CommandHotkeys.API;
using CommandHotkeys.Models;
using Hydriuk.UnturnedModules.Adapters;
using Hydriuk.UnturnedModules.PlayerKeys;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CommandHotkeys.Services
{
#if OPENMOD
    [PluginServiceImplementation(Lifetime = ServiceLifetime.Singleton)]
#endif
    public class HotkeyController : IHotkeyController
    {
        // Services
        private readonly ICommandController _commandController;
        private readonly IPlayerKeysController _playerKeyController;
        private readonly IPermissionAdapter _permissionAdapter;
        private readonly IHotkeyValidator _hotkeyValidator;

        // Configuration
        private readonly IEnumerable<CommandCandidate> _commandCandidatesAsset;
        private readonly float _maxDelay;

        // Data
        private readonly Dictionary<Player, IEnumerable<CommandCandidate>> _commandCandidates = new Dictionary<Player, IEnumerable<CommandCandidate>>();
        private readonly Dictionary<Player, IEnumerable<CommandCandidate>> _playerAllowedCommands = new Dictionary<Player, IEnumerable<CommandCandidate>>();

        public HotkeyController(
            IConfigurationAdapter<Configuration> configuration,
            ICommandController commandController,
            IPermissionAdapter permissionAdapter,
            IPlayerKeysController playerKeysController,
            IHotkeyValidator hotkeyValidator)
        {
            _commandController = commandController;
            _permissionAdapter = permissionAdapter;
            _playerKeyController = playerKeysController;
            _hotkeyValidator = hotkeyValidator;
            _commandCandidatesAsset = configuration.Configuration.Shortcuts.Select(command => new CommandCandidate(command));

            _maxDelay = 1f;

            _hotkeyValidator.FinalKeyValidated += OnCommandValidated;
            PlayerKeysListener.KeyStateChanged += OnKeyStateChanged;
            Provider.onEnemyConnected += InitPlayer;
            Provider.onEnemyDisconnected += ClearPlayer;

            foreach (SteamPlayer sPlayer in Provider.clients)
            {
                InitPlayer(sPlayer);
            }
        }

        public void Dispose()
        {
            _hotkeyValidator.FinalKeyValidated -= OnCommandValidated;
            PlayerKeysListener.KeyStateChanged -= OnKeyStateChanged;
            Provider.onEnemyConnected -= InitPlayer;
            Provider.onEnemyDisconnected -= ClearPlayer;

            _playerKeyController.Dispose();
        }

        private async void InitPlayer(SteamPlayer sPlayer)
        {
            IEnumerable<string> playerPermissions = await _permissionAdapter.GetPermissions(sPlayer.playerID.steamID);
            IEnumerable<CommandCandidate> allowedCommands =
                _commandCandidatesAsset
                .Where(command =>
                    command.Shortcut.Permission != string.Empty &&
                    playerPermissions.Contains(command.Shortcut.Permission)
                );

            _commandCandidates.Add(sPlayer.player, new List<CommandCandidate>());
            _playerAllowedCommands.Add(sPlayer.player, allowedCommands);
        }
        private void ClearPlayer(SteamPlayer sPlayer)
        {
            _commandCandidates.Remove(sPlayer.player);
            _playerAllowedCommands.Remove(sPlayer.player);
        }

        private void OnKeyStateChanged(Player player, EPlayerKey key, bool state)
        {
            float time = Time.realtimeSinceStartup;

            IEnumerable<CommandCandidate> commandCandidates = _commandCandidates[player];

            // Remove expired candidates
            commandCandidates = commandCandidates.Where(candidate => candidate.LastHotkeyTime == -1 || time - candidate.LastHotkeyTime <= _maxDelay);

            if (commandCandidates.Count() == 0)
            {
                commandCandidates = _playerAllowedCommands[player].ToList();
                _commandCandidates[player] = commandCandidates;
            }

            // Get current hotkey
            EHotkeys hotkeys = ToHotkey(player.input.keys);

            // Filter commands by hotkey
            _hotkeyValidator.Validate(player, commandCandidates, hotkeys);
        }

        private void OnCommandValidated(Player player, CommandCandidate commandCandidate)
        {
            _commandController.TryExecuteCommand(player, commandCandidate.Shortcut);
            _commandCandidates[player] = _playerAllowedCommands[player].ToList();
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