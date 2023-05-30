using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
#if OPENMOD
using YamlDotNet.Serialization;
#endif

namespace CommandHotkeys.Models
{
    public class HotkeyedCommand
    {
        public string Command { get; set; } = string.Empty;
        public bool ExecuteAsConsole { get; set; }
        public string Permission { get; set; } = string.Empty;
        public double Cooldown { get; set; }

#if OPENMOD
        [YamlIgnore]
#endif
        [XmlIgnore]
        public List<EHotkeys> HotkeyList { get; private set; } = new List<EHotkeys>();

#if OPENMOD
        [YamlMember(Alias = "Hotkeys")]
#endif
        public List<string> Hotkeys
        {
            get => HotkeyList
                .Select(hotkey => hotkey.ToString())
                .ToList();

            set => HotkeyList = value
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

        public List<double> Casts { get; set; } = new List<double>();
    }
}
