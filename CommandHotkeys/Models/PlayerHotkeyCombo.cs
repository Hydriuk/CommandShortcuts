using System;
using System.Collections.Generic;
using System.Text;

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
