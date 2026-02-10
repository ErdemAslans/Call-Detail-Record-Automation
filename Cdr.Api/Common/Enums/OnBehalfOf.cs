using System.ComponentModel;

namespace Common.Enums;

public enum OnBehalfOfCode
{
    [Description("Unknown")]
    Unknown = 0,

    [Description("CctiLine")]
    CctiLine = 1,

    [Description("Unicast Shared Resource Provider")]
    UnicastSharedResourceProvider = 2,

    [Description("Call Park")]
    CallPark = 3,

    [Description("Conference")]
    Conference = 4,

    [Description("Call Forward")]
    CallForward = 5,

    [Description("Meet-Me Conference")]
    MeetMeConference = 6,

    [Description("Meet-Me Conference Intercepts")]
    MeetMeConferenceIntercepts = 7,

    [Description("Message Waiting")]
    MessageWaiting = 8,

    [Description("Multicast Shared Resource Provider")]
    MulticastSharedResourceProvider = 9,

    [Description("Transfer")]
    Transfer = 10,

    [Description("SSAPI Manager")]
    SSAPIManager = 11,

    [Description("Device")]
    Device = 12,

    [Description("Call Control")]
    CallControl = 13,

    [Description("Immediate Divert")]
    ImmediateDivert = 14,

    [Description("Barge")]
    Barge = 15,

    [Description("Pickup")]
    Pickup = 16,

    [Description("Refer")]
    Refer = 17,

    [Description("Replaces")]
    Replaces = 18,

    [Description("Redirection")]
    Redirection = 19,

    [Description("Callback")]
    Callback = 20,

    [Description("Path Replacement")]
    PathReplacement = 21,

    [Description("FacCmc Manager")]
    FacCmcManager = 22,

    [Description("Malicious Call")]
    MaliciousCall = 23,

    [Description("Mobility")]
    Mobility = 24,

    [Description("Aar")]
    Aar = 25,

    [Description("Directed Call Park")]
    DirectedCallPark = 26,

    [Description("Recording")]
    Recording = 27,

    [Description("Monitoring")]
    Monitoring = 28,

    [Description("CCDRequestingService")]
    CCDRequestingService = 29,

    [Description("Intercompany Media Engine")]
    IntercompanyMediaEngine = 30,

    [Description("FallBack Manager")]
    FallBackManager = 31,

    [Description("Presence Enabled Routing")]
    PresenceEnabledRouting = 32,

    [Description("AgentGreeting")]
    AgentGreeting = 33,

    [Description("NativeCallQueuing")]
    NativeCallQueuing = 34,

    [Description("MobileCallType")]
    MobileCallType = 35
}