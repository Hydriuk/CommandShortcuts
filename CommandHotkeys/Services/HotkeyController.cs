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
        private readonly Dictionary<Player, PlayerCommandCandidates> _playerHotkeyCombo = new Dictionary<Player, PlayerCommandCandidates>();
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
            _commandCandidatesAsset = configuration.Configuration.Commands.Select(command => new CommandCandidate(command));

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
                    command.Command.Permission != string.Empty &&
                    playerPermissions.Contains(command.Command.Permission)
                );

            _playerHotkeyCombo.Add(sPlayer.player, new PlayerCommandCandidates());
            _playerAllowedCommands.Add(sPlayer.player, allowedCommands);
        }
        private void ClearPlayer(SteamPlayer sPlayer)
        {
            _playerHotkeyCombo.Remove(sPlayer.player);
            _playerAllowedCommands.Remove(sPlayer.player);
        }

        private async void OnKeyStateChanged(Player player, EPlayerKey key, bool state)
        {
            float time = Time.realtimeSinceStartup;

            PlayerCommandCandidates playerCombo = _playerHotkeyCombo[player];

            // Clear current combo
            if (playerCombo.LastHotkeyTime != -1f && time - playerCombo.LastHotkeyTime > _maxDelay)
            {
                playerCombo.Reset();
            }

            IEnumerable<string> playerPermissions = await _permissionAdapter.GetPermissions(player.channel.owner.playerID.steamID);

            if (playerCombo.CommandCandidates == null)
            {
                playerCombo.CommandCandidates = _playerAllowedCommands[player].ToList();
            }

            // Get current hotkey
            EHotkeys hotkeys = ToHotkey(player.input.keys);

            // Filter commands by hotkey
            _hotkeyValidator.Validate(player, playerCombo, hotkeys);
        }

        private void OnCommandValidated(Player player, CommandCandidate commandCandidate)
        {
            _commandController.TryExecuteCommand(player, commandCandidate.Command);
            _playerHotkeyCombo[player].Reset();
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