using LiteDB;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommandShortcuts.Models
{
    public class Cooldown
    {
        public ObjectId Id { get; set; } = ObjectId.Empty;
        public ulong PlayerID { get; set; }
        public string Permission { get; set; } = string.Empty;
        public DateTime CooledDownAt { get; set; }
    }
}
