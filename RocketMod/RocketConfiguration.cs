using CommandHotkeys.Models;
using Rocket.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandHotkeys.RocketMod
{
    public class RocketConfiguration : IRocketPluginConfiguration
    {
        public EHotkeys Hotkey { get; set; }
        public void LoadDefaults()
        {
            Hotkey = EHotkeys.Sprint | EHotkeys.LeanLeft;
        }
    }
}
