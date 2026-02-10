using System.ComponentModel;

namespace Common.Enums;

public enum RedirectReason
{
    [Description("Unknown")]
    Unknown = 0,

    [Description("Call Forward Busy")]
    CallForwardBusy = 1,

    [Description("Call Forward No Answer")]
    CallForwardNoAnswer = 2,

    [Description("Call Transfer")]
    CallTransfer = 4,

    [Description("Call Pickup")]
    CallPickup = 5,

    [Description("Call Park")]
    CallPark = 7,

    [Description("Call Park Pickup")]
    CallParkPickup = 8,

    [Description("CPE Out of Order")]
    CPEOutOfOrder = 9,

    [Description("Call Forward")]
    CallForward = 10,

    [Description("Call Park Reversion")]
    CallParkReversion = 11,

    [Description("Call Forward")]
    CallForwardAll = 15,

    [Description("Call Deflection")]
    CallDeflection = 18,

    [Description("Blind Transfer")]
    BlindTransfer = 34,

    [Description("Call Immediate Divert")]
    CallImmediateDivert = 50,

    [Description("Call Forward Alternate Party")]
    CallForwardAlternateParty = 66,

    [Description("Call Forward On Failure")]
    CallForwardOnFailure = 82,

    [Description("Conference")]
    Conference = 98,

    [Description("Barge")]
    Barge = 114,

    [Description("Aar")]
    Aar = 129,

    [Description("Refer")]
    Refer = 130,

    [Description("Replaces")]
    Replaces = 146,

    [Description("Redirection (3xx)")]
    Redirection3xx = 162,

    [Description("SIP-forward busy greeting")]
    SIPForwardBusyGreeting = 177,

    [Description("Call Forward Unregistered")]
    CallForwardUnregistered = 178,

    [Description("Follow Me (SIP-forward all greeting)")]
    FollowMeSIPForwardAllGreeting = 207,

    [Description("Out of Service (SIP-forward busy greeting)")]
    OutOfServiceSIPForwardBusyGreeting = 209,

    [Description("Time of Day (SIP-forward all greeting)")]
    TimeOfDaySIPForwardAllGreeting = 239,

    [Description("Do Not Disturb (SIP-forward no answer greeting)")]
    DoNotDisturbSIPForwardNoAnswerGreeting = 242,

    [Description("Unavailable (SIP-forward busy greeting)")]
    UnavailableSIPForwardBusyGreeting = 257,

    [Description("Away (SIP-forward no answer greeting)")]
    AwaySIPForwardNoAnswerGreeting = 274,

    [Description("Mobility HandIn")]
    MobilityHandIn = 303,

    [Description("Mobility HandOut")]
    MobilityHandOut = 319,

    [Description("Mobility Follow Me")]
    MobilityFollowMe = 335,

    [Description("Mobility Redial")]
    MobilityRedial = 351,

    [Description("Recording")]
    Recording = 354,

    [Description("Monitoring")]
    Monitoring = 370,

    [Description("Mobility IVR")]
    MobilityIVR = 399,

    [Description("Mobility DVOR")]
    MobilityDVOR = 401,

    [Description("Mobility EFA")]
    MobilityEFA = 402,

    [Description("Mobility Session Handoff")]
    MobilitySessionHandoff = 403,

    [Description("Mobility Cell Pickup")]
    MobilityCellPickup = 415,

    [Description("Click to Conference")]
    ClickToConference = 418,

    [Description("Forward No Retrieve")]
    ForwardNoRetrieve = 434,

    [Description("Forward No Retrieve Send Back to Parker")]
    ForwardNoRetrieveSendBackToParker = 450,

    [Description("Call Control Discovery (indicates that the call is redirected to a PSTN failover number)")]
    CallControlDiscovery = 464,

    [Description("Intercompany Media Engine (IME)")]
    IME = 480,

    [Description("IME Connection Timed Out")]
    IMEConnectionTimedOut = 496,

    [Description("IME Not Enrolled")]
    IMENotEnrolled = 512,

    [Description("IME Socket Error")]
    IMESocketError = 528,

    [Description("IME Domain Blacklisted")]
    IMEDomainBlacklisted = 544,

    [Description("IME Prefix Blacklisted")]
    IMEPrefixBlacklisted = 560,

    [Description("IME Expired Ticket")]
    IMEExpiredTicket = 576,

    [Description("IME Remote No Matching Route")]
    IMERemoteNoMatchingRoute = 592,

    [Description("IME Remote Unregistered")]
    IMERemoteUnregistered = 608,

    [Description("IME Remote IME Disabled")]
    IMERemoteIMEDisabled = 624,

    [Description("IME Remote Invalid IME Trunk URI")]
    IMERemoteInvalidIMETrunkURI = 640,

    [Description("IME Remote URI not E164")]
    IMERemoteURINotE164 = 656,

    [Description("IME Remote Called Number Not Available")]
    IMERemoteCalledNumberNotAvailable = 672,

    [Description("IME Invalid Ticket")]
    IMEInvalidTicket = 688,

    [Description("IME Unknown")]
    IMEUnknown = 704,

    [Description("IME PSTN Fallback")]
    IMEPSTNFallback = 720,

    [Description("Presence Enabled Routing")]
    PresenceEnabledRouting = 738,

    [Description("Agent Greeting")]
    AgentGreeting = 752,

    [Description("NuRD")]
    NuRD = 783,

    [Description("Native Call Queuing, queue a call")]
    NativeCallQueuingQueueCall = 786,

    [Description("Native Call Queuing, de-queue a call")]
    NativeCallQueuingDequeueCall = 802,

    [Description("Native Call Queuing, redirect to the second destination when no agent is logged in")]
    NativeCallQueuingRedirectNoAgent = 818,

    [Description("Native Call Queuing, redirect to the second destination when the queue is full")]
    NativeCallQueuingRedirectQueueFull = 834,

    [Description("Native Call Queuing, redirect to the second destination when the maximum wait time in queue is reached")]
    NativeCallQueuingRedirectMaxWait = 850
}
