using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
#if OPENMOD
using YamlDotNet.Serialization;
#endif

namespace CommandHotkeys.Models
{
    public class Shortcut
    {
        public string Permission { get; set; } = string.Empty;
        public string Command { get; set; } = string.Empty;
        public double Cooldown { get; set; }
        public bool ExecuteAsConsole { get; set; }

        [XmlArrayItem("Hotkey")]
        public List<string> Hotkeys { get; set; } = new List<string>();
        [XmlIgnore]
        public List<EHotkeys> HotkeyList { get => _hotkeys; }
        [XmlIgnore]
        private List<EHotkeys> _hotkeys = new List<EHotkeys>();


        public List<double> Casts { get; set; } = new List<double>();

        public void Validate()
        {
            _hotkeys = Hotkeys
                // Parse the string
                .Select(hotkeyString =>
                {
                    if (!Enum.TryParse(hotkeyString, out EHotkeys hotkeys))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Error.WriteLine($"[{DateTime.Now.ToLongTimeString()}][CommandHotkeys] - The following hotkey couldn't be parsed : '{hotkeyString}'. Please check your configuration file");
                        Console.ResetColor();

                        hotkeys = (EHotkeys)int.MaxValue;
                    }

                    return hotkeys;
                })

                // Bypass the double Sprint/HoldBreath requirement
                .Select(hotkey =>
                {
                    EHotkeys shiftFlags = hotkey & (EHotkeys.Sprint | EHotkeys.HoldBreath);
                    if (shiftFlags == EHotkeys.Sprint || shiftFlags == EHotkeys.HoldBreath)
                    {
                        hotkey |= EHotkeys.Sprint | EHotkeys.HoldBreath;
                    }

                    return hotkey;
                })
                .ToList();
        }
    }
}
