namespace CommandShortcuts.Models
{
    public class CommandCandidate
    {
        public Shortcut Shortcut { get; set; }
        public int ValidatingIndex { get; set; }
        public float LastHotkeyTime { get; set; }

        public CommandCandidate(Shortcut shortcut)
        {
            Shortcut = shortcut;
            ValidatingIndex = 0;
            LastHotkeyTime = -1;
        }
    }
}