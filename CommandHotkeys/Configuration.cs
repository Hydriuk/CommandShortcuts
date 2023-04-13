using CommandHotkeys.Models;
using Hydriuk.Unturned.SharedModules.Adapters;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommandHotkeys
{
    public class Configuration : IPluginConfiguration
    {
        public List<HotkeyedCommand> Commands { get; set; } = new List<HotkeyedCommand>();
    }
}
