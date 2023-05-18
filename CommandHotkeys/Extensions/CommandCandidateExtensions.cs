using CommandHotkeys.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommandHotkeys.Extensions
{
    public static class CommandCandidateExtensions
    {
        // Soflty is refered as allowing keys from previous hotkey to still be pressed to validate the condition
        public static bool TryValidate(this CommandCandidate commandCandidate, EHotkeys hotkeys)
        {
            int index = commandCandidate.ValidatingIndex;

            if (index >= commandCandidate.Command.Hotkeys.Count)
            {
                return true;
            }

            EHotkeys targetHotkey = commandCandidate.Command.HotkeyList[index];
            EHotkeys previousHotkeys = index > 0 ? commandCandidate.Command.HotkeyList[index - 1] : EHotkeys.None;

            // If pressing the hotkey
            if (hotkeys == targetHotkey)
            {
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
    }
}
