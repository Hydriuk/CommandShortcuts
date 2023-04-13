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
            Console.WriteLine(index);

            // If index exceeds the  length of the command
            if(index >= commandCandidate.Command.Hotkeys.Count)
            {
                // The command is invalid
                return false;
            }
            // If some part of the hotkey is not included in the current command
            if (hotkeys != EHotkeys.None && (commandCandidate.Command.Hotkeys[index] & hotkeys) != hotkeys)
            {
                Console.WriteLine($"{hotkeys} : invalid");
                // The command is invalid
                return false;
            }
            // If the full hotkey is the current command
            else if (commandCandidate.Command.Hotkeys[index] == hotkeys)
            {
                Console.WriteLine($"{hotkeys} : completed");
                // Continue to the next hotkey
                commandCandidate.ValidatingIndex++;
                Console.WriteLine(commandCandidate.ValidatingIndex);
                return true;
            }
            // If the full key does not correspond to the current command
            else
            {
                Console.WriteLine($"{hotkeys} : continued");
                // Wait for the next validate action
                return true;
            }
        }
    }
}
