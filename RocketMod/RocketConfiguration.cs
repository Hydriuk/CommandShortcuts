using CommandHotkeys.Models;
using Hydriuk.UnturnedModules.Adapters;
using Rocket.API;
using System.Collections.Generic;

namespace CommandHotkeys.RocketMod
{
    public class RocketConfiguration : Configuration, IConfigurationAdapter<Configuration>, IRocketPluginConfiguration
    {
        public Configuration Configuration => this;

        public void LoadDefaults()
        {
            Shortcuts = new List<HotkeyedCommand>()
            {
                new HotkeyedCommand()
                {
                    Permission = "cmdsc.healing.healing",
                    Command = "/heal",
                    Cooldown = 3600,
                    Hotkeys = new List<string>() { "Sprint, LeanLeft", "None", "Sprint, LeanRight" },
                    Casts = new List<double>() { 2, 0 , 2 }
                },
                new HotkeyedCommand()
                {
                    Permission = "cmdsc.healing.vip",
                    Command = "/heal",
                    Cooldown = 600,
                    Hotkeys = new List<string>() { "Plugin1" },
                    Casts = new List<double>()
                }
            };
        }
    }
}