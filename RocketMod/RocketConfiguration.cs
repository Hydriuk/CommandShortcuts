using CommandShortcuts.Models;
using Hydriuk.UnturnedModules.Adapters;
using Rocket.API;
using System.Collections.Generic;
using System.Xml.Serialization;
using static System.Net.Mime.MediaTypeNames;

namespace CommandShortcuts.RocketMod
{
    public class RocketConfiguration : Configuration, IConfigurationAdapter<Configuration>, IRocketPluginConfiguration
    {
        [XmlIgnore]
        public Configuration Configuration => this;

        public void LoadDefaults()
        {
            ValidatedEffectGUID = "bc41e0feaebe4e788a3612811b8722d3";
            ChatIcon = "https://i.imgur.com/nD9DLyu.png";
            Shortcuts = new List<Shortcut>()
            {
                new Shortcut()
                {
                    Permission = "cmdsc.healing.healing",
                    Command = "/heal {PlayerID}",
                    ExecuteAsConsole = true,
                    Cooldown = 3600,
                    Hotkeys = new List<string>() { "Prone, LeanLeft", "Prone", "Prone, LeanRight" },
                    Casts = new List<double>() { 2, 0 , 2 },
                },
                new Shortcut()
                {
                    Permission = "cmdsc.healing.vip",
                    Command = "/heal {PlayerID}",
                    ExecuteAsConsole = true,
                    Cooldown = 600,
                    Hotkeys = new List<string>() { "Plugin1" },
                    Casts = new List<double>()
                }
            };
        }
    }
}