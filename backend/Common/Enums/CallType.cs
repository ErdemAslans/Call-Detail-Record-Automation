using System.ComponentModel;

namespace Common.Enums;

public enum CallType
{
    [Description("Answered call")]
    AnsweredCall = 1,

    [Description("Missed call")]
    MissedCall,
}