namespace CommandHotkeys.Models
{
    public class CommandCandidate
    {
        public HotkeyedCommand Command { get; set; }
        public int ValidatingIndex { get; set; }

        public CommandCandidate(HotkeyedCommand command)
        {
            Command = command;
            ValidatingIndex = 0;
        }
    }
}