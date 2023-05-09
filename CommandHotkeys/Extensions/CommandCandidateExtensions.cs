using CommandHotkeys.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommandHotkeys.Extensions
{
    public static class CommandCandidateExtensions
    {
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

            // The None hotkeys is forced to be exact.
            else if (targetHotkey == EHotkeys.None)
            {
                return false;
            }

            // Soft mathcing (target included in hotkey)
            else if (
                (hotkeys & targetHotkey) == targetHotkey)
            {
                commandCandidate.ValidatingIndex++;
                return true;
            }

            // Unvalidate command if an unknow key is pressed
            else if ((hotkeys ^ targetHotkey ^ previousHotkeys) != EHotkeys.None)
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
