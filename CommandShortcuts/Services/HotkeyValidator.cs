using CommandShortcuts.API;
using CommandShortcuts.Models;
#if OPENMOD
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
#endif
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CommandShortcuts.Services
{
#if OPENMOD
    [PluginServiceImplementation(Lifetime = ServiceLifetime.Singleton)]
#endif
    public class HotkeyValidator : IHotkeyValidator
    {
        private readonly ICastingProvider _castingProvider;
        private readonly IEffectProvider _effectProvider;

        public event Action<Player, CommandCandidate>? FinalKeyValidated;

        public HotkeyValidator(ICastingProvider castingProvider, IEffectProvider effectProvider)
        {
            _castingProvider = castingProvider;
            _effectProvider = effectProvider;

            _castingProvider.Casted += ValidateKey;
        }

        public void Dispose()
        {
            _castingProvider.Casted -= ValidateKey;
        }

        public void Validate(Player player, IEnumerable<CommandCandidate> commandCandidates, EHotkeys hotkeys)
        {
            commandCandidates = commandCandidates
            .Where(commandCandidate =>
            {
                int initialIndex = commandCandidate.ValidatingIndex;

                return TryValidate(player, commandCandidate, hotkeys);
            })
            // To list needed, or there will be a reference issue, duplicating the where calls, while still having the same count of commands
            .ToList();
        }

        public bool TryValidate(Player player, CommandCandidate commandCandidate, EHotkeys hotkeys)
        {
            int index = commandCandidate.ValidatingIndex;

            if (index >= commandCandidate.Shortcut.HotkeyList.Count)
                return true;

            EHotkeys targetHotkey = commandCandidate.Shortcut.HotkeyList[index];
            EHotkeys previousHotkeys = index > 0 ? commandCandidate.Shortcut.HotkeyList[index - 1] : EHotkeys.None;

            if (_castingProvider.IsCasting(player, commandCandidate.Shortcut.Permission))
            {
                if ((hotkeys & targetHotkey) == targetHotkey)
                {
                    return true;
                }
                else
                {
                    _castingProvider.AbortCast(player, commandCandidate.Shortcut.Permission);
                    return false;
                }
            }

            // The None hotkeys is forced to be exact. Previous key is softly accepted
            if (targetHotkey == EHotkeys.None)
            {
                if (hotkeys == EHotkeys.None)
                {
                    ValidateKey(player, commandCandidate);
                    return true;
                }
                else if ((hotkeys & previousHotkeys) == hotkeys)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            // Soft matching (target included in hotkey)
            else if ((hotkeys & targetHotkey) == targetHotkey)
            {
                if (!_castingProvider.TryStartCast(player, commandCandidate))
                {
                    ValidateKey(player, commandCandidate);
                }
                return true;
            }

            // Unvalidate command if a key that is nor in pervious hotkey nor in target hotkey is pressed
            else if ((hotkeys & (previousHotkeys | targetHotkey)) != hotkeys)
            {
                return false;
            }

            // Wait for the next hotkey pressed
            else
            {
                return true;
            }
        }

        public void ValidateKey(Player player, CommandCandidate commandCandidate)
        {
            _effectProvider.SendValidatedEffect(player);
            commandCandidate.ValidatingIndex++;
            commandCandidate.LastHotkeyTime = Time.realtimeSinceStartup;

            if (commandCandidate.ValidatingIndex == commandCandidate.Shortcut.HotkeyList.Count)
            {
                FinalKeyValidated?.Invoke(player, commandCandidate);
            }
        }
    }
}