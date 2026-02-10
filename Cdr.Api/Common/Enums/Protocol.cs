using System.ComponentModel;

namespace Common.Enums;

public enum Protocol
{
    [Description("Unknown")]
    Unknown = 0,

    [Description("SIP")]
    SIP = 1,

    [Description("H323")]
    H323 = 2,

    [Description("CTI/JTAPI")]
    CTI_JTAPI = 3,

    [Description("Q931")]
    Q931 = 4,
}