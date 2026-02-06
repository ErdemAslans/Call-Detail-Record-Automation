using System.ComponentModel;

namespace Common.Enums;

public enum CallEndedReason
{
    [Description("Unknown reason")]
    Unknown = 1, // Dökümanda Unknown olarak işaretlenmiş

    [Description("Normal termination")]
    NormalTermination,

    [Description("Busy")]
    Busy,

    [Description("Technical issue")]
    TechnicalIssue
}