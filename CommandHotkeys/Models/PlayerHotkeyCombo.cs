using System.Collections.Generic;

namespace CommandHotkeys.Models
{
    public class PlayerCommandCandidates
    {
        public IEnumerable<CommandCandidate>? CommandCandidates { get; set; } = null;
        public float LastHotkeyTime { get; set; } = -1f;

        public void Reset()
        {
            CommandCandidates = null;
            LastHotkeyTime = -1f;
        }
    }
}