using System;
using System.Collections.Generic;
using System.Text;

namespace CommandShortcuts.Models
{
    public class Cooldown
    {
        public ulong PlayerID { get; set; }
        public string Permission { get; set; } = string.Empty;
        public DateTime CooledDownAt { get; set; }
    }
}
