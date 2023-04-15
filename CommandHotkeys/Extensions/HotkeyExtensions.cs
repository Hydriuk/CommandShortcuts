using CommandHotkeys.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommandHotkeys.Extensions
{
    public static class HotkeyExtensions
    {
        public static Hotkey Parse(this string hotkeyString)
        {
            // Parse action
            EHotkeyAction hotkeyAction;
            if(hotkeyString.StartsWith(")") && hotkeyString.EndsWith("("))
            {
                hotkeyAction = EHotkeyAction.Hold;
                hotkeyString = hotkeyString.Trim(')', '(');
            }
            else if (hotkeyString.StartsWith("(") && hotkeyString.EndsWith(")"))
            {
                hotkeyAction = EHotkeyAction.Release;
                hotkeyString = hotkeyString.Trim('(', ')');
            }
            else
            {
                hotkeyAction = EHotkeyAction.Press;
            }

            // Parse hotkeys
            if (!Enum.TryParse(hotkeyString, out EHotkeys hotkeys))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine($"[{DateTime.Now.ToLongTimeString()}][CommandHotkeys] - The following hotkey couldn't be parsed : '{hotkeyString}'. Please check your configuration file");
                Console.ResetColor();

                hotkeys = (EHotkeys)int.MaxValue;
            }

            return new Hotkey()
            {
                Action = hotkeyAction,
                Hotkeys = hotkeys
            };
        }

        public static string Serialize(this Hotkey hotkey)
        {
            switch (hotkey.Action)
            {
                case EHotkeyAction.Hold:
                    return $"){hotkey.Hotkeys}(";

                case EHotkeyAction.Release:
                    return $"({hotkey.Hotkeys})";

                case EHotkeyAction.Press:
                default:
                    return $"{hotkey.Hotkeys}";
            }
        }
    }
}
