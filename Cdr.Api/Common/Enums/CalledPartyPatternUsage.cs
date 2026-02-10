using System.ComponentModel;

namespace YourNamespace
{
    public enum CalledPartyPatternUsage
    {
        [Description("PATTERN_CALL_PARK")]
        CallPark = 0,

        [Description("PATTERN_CONF")]
        Conference = 1,

        [Description("PATTERN_DEVICE")]
        Device = 2,

        [Description("PATTERN_TRANSLATION")]
        Translation = 3,

        [Description("PATTERN_CALL_PICK_UP_GROUP")]
        CallPickUpGroup = 4,

        [Description("PATTERN_ROUTE")]
        Route = 5,

        [Description("PATTERN_MESSAGE_WAITING")]
        MessageWaiting = 6,

        [Description("PATTERN_HUNT_PILOT")]
        HuntPilot = 7,

        [Description("PATTERN_VOICE_MAIL_PORT")]
        VoiceMailPort = 8,

        [Description("PATTERN_ROUTE_DOMAIN")]
        DomainRouting = 9,

        [Description("PATTERN_ROUTE_IPNET")]
        IPAddressRouting = 10,

        [Description("PATTERN_DEVICE_TEMPLATE")]
        DeviceTemplate = 11,

        [Description("PATTERN_DIRECTED_CALL_PARK")]
        DirectedCallPark = 12,

        [Description("PATTERN_DEVICE_INTERCOM")]
        DeviceIntercom = 13,

        [Description("PATTERN_TRANSLATION_INTERCOM")]
        TranslationIntercom = 14,

        [Description("PATTERN_TRANSLATION_CALLING_PARTY_NUMBER")]
        TranslationCallingPartyNumber = 15,

        [Description("PATTERN_MOBILITY_HANDOFF")]
        MobilityHandoff = 16,

        [Description("PATTERN_MOBILITY_DTMF")]
        MobilityDTMF = 17,

        [Description("PATTERN_MOBILITY_IVR")]
        MobilityIVR = 18,

        [Description("PATTERN_DEVICE_INTERCOM_TEMPLATE")]
        DeviceIntercomTemplate = 19
    }
}