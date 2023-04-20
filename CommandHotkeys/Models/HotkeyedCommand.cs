using CommandHotkeys.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using YamlDotNet.Serialization;

namespace CommandHotkeys.Models
{
    public class HotkeyedCommand
    {
        public string Name { get; set; } = string.Empty;
        public string Command { get; set; } = string.Empty;
        public string Permission { get; set; } = string.Empty;

        [YamlIgnore]
        public List<EHotkeys> HotkeyList { get; private set; } = new List<EHotkeys>();

        [YamlMember(Alias = "Hotkeys")]
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
                    if (shiftFlags != (EHotkeys.Sprint | EHotkeys.HoldBreath))
                    {
                        hotkey |= EHotkeys.Sprint | EHotkeys.HoldBreath;
                    }

                    return hotkey;
                })
                .ToList();
        }
    }
}
