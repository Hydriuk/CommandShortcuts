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
        private static TriggerEffectParameters _validatedEffect = new TriggerEffectParameters(new Guid("bc41e0feaebe4e788a3612811b8722d3"));

        private readonly IThreadAdapter _threadAdapter;

        public HotkeyValidator(IThreadAdapter threadAdapter)
        {
            _threadAdapter = threadAdapter;
        }

        public void Validate(Player player, PlayerCommandCandidates commandCandidates, EHotkeys hotkeys) 
        {
            commandCandidates.CommandCandidates = commandCandidates.CommandCandidates
            .Where(commandCandidate =>
            {
                int initialIndex = commandCandidate.ValidatingIndex;
                bool success = TryValidate(player, commandCandidate, hotkeys);

                // Update last validated hotkey time
                if(commandCandidate.ValidatingIndex > initialIndex)
                {
                    commandCandidates.LastHotkeyTime = Time.realtimeSinceStartup;
                }

                return success;
            })
            .ToList();
        }

        public bool TryValidate(Player player, CommandCandidate commandCandidate, EHotkeys hotkeys)
        {
            int index = commandCandidate.ValidatingIndex;

            if (index >= commandCandidate.Command.HotkeyList.Count)
            {
                return true;
            }

            EHotkeys targetHotkey = commandCandidate.Command.HotkeyList[index];
            EHotkeys previousHotkeys = index > 0 ? commandCandidate.Command.HotkeyList[index - 1] : EHotkeys.None;

            // If pressing the hotkey
            if (hotkeys == targetHotkey)
            {
                SendEffect(player);
                commandCandidate.ValidatingIndex++;
                return true;
            }

            // The None hotkeys is softly forced to be exact.
            else if (targetHotkey == EHotkeys.None)
            {
                return (hotkeys & previousHotkeys) == hotkeys;
            }

            // Soft matching (target included in hotkey)
            else if (
                (hotkeys & targetHotkey) == targetHotkey)
            {
                SendEffect(player);
                commandCandidate.ValidatingIndex++;
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

        private void SendEffect(Player player)
        {
            var effect = _validatedEffect;
            effect.SetRelevantPlayer(player.GetTransportConnection());
            effect.position = player.transform.position;

            _threadAdapter.RunOnMainThread(() =>  EffectManager.triggerEffect(effect));
        }
    }
}
