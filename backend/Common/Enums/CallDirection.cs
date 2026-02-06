using System.ComponentModel;

public enum CallDirection
{
    [Description("Incoming")]
    Incoming = 1,

    [Description("Outgoing")]
    Outgoing = 2,

    [Description("Internal")]
    Internal = 3,
}