using CommandHotkeys.API;
using CommandHotkeys.Models;
#if OPENMOD
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
#endif
using SDG.Unturned;
using System.Linq;
using UnityEngine;

namespace CommandHotkeys.Services
{
#if OPENMOD
    [PluginServiceImplementation(Lifetime = ServiceLifetime.Singleton)]
#endif
    public class HotkeyValidator : IHotkeyValidator
    {
        private readonly ICastingProvider _castingProvider;
        private readonly IEffectProvider _effectProvider;

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

        public void Validate(Player player, PlayerCommandCandidates commandCandidates, EHotkeys hotkeys)
        {
            commandCandidates.CommandCandidates = commandCandidates.CommandCandidates
            .Where(commandCandidate =>
            {
                int initialIndex = commandCandidate.ValidatingIndex;
                bool success = TryValidate(player, commandCandidate, hotkeys);

                // Update last validated hotkey time
                if (commandCandidate.ValidatingIndex > initialIndex)
                {
                    commandCandidates.LastHotkeyTime = Time.realtimeSinceStartup;
                }

                return success;
            })
            // To list needed, or there will be a reference issue, duplicating the where calls, while still having the same count of commands
            .ToList();
        }

        public bool TryValidate(Player player, CommandCandidate commandCandidate, EHotkeys hotkeys)
        {
            int index = commandCandidate.ValidatingIndex;

            if (index >= commandCandidate.Command.HotkeyList.Count)
                return true;

            EHotkeys targetHotkey = commandCandidate.Command.HotkeyList[index];
            EHotkeys previousHotkeys = index > 0 ? commandCandidate.Command.HotkeyList[index - 1] : EHotkeys.None;

            if (_castingProvider.IsCasting(player, commandCandidate.Command.Permission))
            {
                if ((hotkeys & targetHotkey) == targetHotkey)
                {
                    return true;
                }
                else
                {
                    _castingProvider.AbortCast(player, commandCandidate.Command.Permission);
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

        private void ValidateKey(Player player, CommandCandidate commandCandidate)
        {
            _effectProvider.SendValidatedEffect(player);
            commandCandidate.ValidatingIndex++;
        }
    }
}