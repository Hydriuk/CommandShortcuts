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
                return false;

            EHotkeys targetHotkey = commandCandidate.Command.HotkeyList[index];
            EHotkeys previousHotkeys = index > 0 ? commandCandidate.Command.HotkeyList[index - 1] : EHotkeys.None;

            // If pressing the hotkey
            if (hotkeys == targetHotkey)
            {
                commandCandidate.ValidatingIndex++;
                return true;
            }
           
            else if ((
                // If all keys common to current, previous and target hotkeys
                (hotkeys & previousHotkeys & targetHotkey) | 
                // Plus current keys that are in current but not in previous hotkeys, while part of the target 
                (hotkeys ^ previousHotkeys) & hotkeys & targetHotkey)
                // Equals the target
                == targetHotkey)
            {
                commandCandidate.ValidatingIndex++;
                return true;
            }

            else
            {
                return false;
            }
        }
    }
}
