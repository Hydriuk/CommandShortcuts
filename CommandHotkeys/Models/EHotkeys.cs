using System;
using System.Collections.Generic;
using System.Text;

namespace CommandHotkeys.Models
{
    [Flags]
    public enum EHotkeys
    {
        None            = 0,
        Jump            = 1 << 0,
        Primary         = 1 << 1,
        Secondary       = 1 << 2,
        Crouch          = 1 << 3,
        Prone           = 1 << 4,
        Sprint          = 1 << 5,
        LeanLeft        = 1 << 6,
        LeanRight       = 1 << 7,
        ToggleTactical  = 1 << 8,
        HoldBreath      = 1 << 9,
        Plugin1         = 1 << 10,
        Plugin2         = 1 << 11,
        Plugin3         = 1 << 12,
        Plugin4         = 1 << 13,
        Plugin5         = 1 << 14,
    }
}
