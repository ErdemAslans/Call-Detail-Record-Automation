using System.ComponentModel;

namespace Common.Enums;

public enum PrecedenceLevel
{
    [Description("Flash/Executive Override")]
    FlashExecutiveOverride = 0,

    [Description("Flash")]
    Flash= 1,

    [Description("Immediate")]
    Immediate = 2,

    [Description("Priority")]
    Priority = 3,

    [Description("Routine")]
    Routine = 4,
}