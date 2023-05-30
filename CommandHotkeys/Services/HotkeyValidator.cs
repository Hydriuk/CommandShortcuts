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
using System.Linq;
using UnityEngine;

namespace CommandHotkeys.Services
{
#if OPENMOD
    [PluginServiceImplementation(Lifetime = ServiceLifetime.Singleton)]
#endif
    public class HotkeyValidator : IHotkeyValidator
    {
        private static TriggerEffectParameters _validatedEffect;

        private readonly IThreadAdapter _threadAdapter;
        private readonly ICastingProvider _castingProvider;

        public HotkeyValidator(IThreadAdapter threadAdapter, ICastingProvider castingProvider)
        {
            _threadAdapter = threadAdapter;
            _castingProvider = castingProvider;

            _castingProvider.Casted += ValidateKey;

            if (Level.isLoaded)
                InitEffect();
            else
                Level.onPostLevelLoaded += OnLevelLoaded;
        }

        public void Dispose()
        {
            _castingProvider.Casted -= ValidateKey;
            Level.onPostLevelLoaded -= OnLevelLoaded;
        }

        private void OnLevelLoaded(int level) => InitEffect();
        private void InitEffect()
        {
            _validatedEffect = new TriggerEffectParameters(new Guid("bc41e0feaebe4e788a3612811b8722d3"));
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
            SendEffect(player);
            commandCandidate.ValidatingIndex++;
        }

        private void SendEffect(Player player)
        {
            var effect = _validatedEffect;
            effect.SetRelevantPlayer(player.GetTransportConnection());
            effect.position = player.transform.position;

            _threadAdapter.RunOnMainThread(() => EffectManager.triggerEffect(effect));
        }
    }
}