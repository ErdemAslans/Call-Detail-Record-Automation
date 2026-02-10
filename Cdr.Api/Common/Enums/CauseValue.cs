using System.ComponentModel;

namespace Common.Enums
{
    public enum CauseValue : long
    {
        [Description("No error")]
        NoError = 0,
        
        [Description("Unallocated (unassigned) number")]
        UnallocatedNumber = 1,
        
        [Description("No route to specified transit network (national use)")]
        NoRouteToTransitNetwork = 2,
        
        [Description("No route to destination")]
        NoRouteToDestination = 3,
        
        [Description("Send special information tone")]
        SendSpecialInformationTone = 4,
        
        [Description("Misdialed trunk prefix (national use)")]
        MisdialedTrunkPrefix = 5,
        
        [Description("Channel unacceptable")]
        ChannelUnacceptable = 6,
        
        [Description("Call awarded and being delivered in an established channel")]
        CallAwarded = 7,
        
        [Description("Preemption")]
        Preemption = 8,
        
        [Description("Preemption—circuit reserved for reuse")]
        PreemptionCircuitReserved = 9,
        
        [Description("Normal call clearing")]
        NormalCallClearing = 16,
        
        [Description("User busy")]
        UserBusy = 17,
        
        [Description("No user responding")]
        NoUserResponding = 18,
        
        [Description("No answer from user")]
        NoAnswerFromUser = 19,
        
        [Description("Subscriber absent")]
        SubscriberAbsent = 20,
        
        [Description("Call rejected")]
        CallRejected = 21,
        
        [Description("Number changed")]
        NumberChanged = 22,
        
        [Description("Natural Exchange Routing Error")]
        NaturalExchangeRoutingError = 25,
        
        [Description("Non-selected user clearing")]
        NonSelectedUserClearing = 26,
        
        [Description("Destination out of order")]
        DestinationOutOfOrder = 27,
        
        [Description("Invalid number format (address incomplete)")]
        InvalidNumberFormat = 28,
        
        [Description("Facility rejected")]
        FacilityRejected = 29,
        
        [Description("Response to STATUS ENQUIRY")]
        ResponseToStatusEnquiry = 30,
        
        [Description("Normal, unspecified")]
        NormalUnspecified = 31,
        
        [Description("No circuit/channel available")]
        NoCircuitChannelAvailable = 34,
        
        [Description("Network out of order")]
        NetworkOutOfOrder = 38,
        
        [Description("Permanent frame mode connection out of service")]
        PermanentFrameModeConnectionOutOfService = 39,
        
        [Description("Permanent frame mode connection operational")]
        PermanentFrameModeConnectionOperational = 40,
        
        [Description("Temporary failure")]
        TemporaryFailure = 41,
        
        [Description("Switching equipment congestion")]
        SwitchingEquipmentCongestion = 42,
        
        [Description("Access information discarded")]
        AccessInformationDiscarded = 43,
        
        [Description("Requested circuit/channel not available")]
        RequestedCircuitChannelNotAvailable = 44,
        
        [Description("Precedence call blocked")]
        PrecedenceCallBlocked = 46,
        
        [Description("Resource unavailable, unspecified")]
        ResourceUnavailableUnspecified = 47,
        
        [Description("Quality of Service not available")]
        QualityOfServiceNotAvailable = 49,
        
        [Description("Requested facility not subscribed")]
        RequestedFacilityNotSubscribed = 50,
        
        [Description("Service operation violated")]
        ServiceOperationViolated = 53,
        
        [Description("Incoming calls barred")]
        IncomingCallsBarred = 54,
        
        [Description("Incoming calls barred within Closed User Group (CUG)")]
        IncomingCallsBarredWithinCUG = 55,
        
        [Description("Bearer capability not authorized")]
        BearerCapabilityNotAuthorized = 57,
        
        [Description("Bearer capability not presently available")]
        BearerCapabilityNotPresentlyAvailable = 58,
        
        [Description("Inconsistency in designated outgoing access information and subscriber class")]
        InconsistencyInDesignatedOutgoingAccessInformation = 62,
        
        [Description("Service or option not available, unspecified")]
        ServiceOrOptionNotAvailableUnspecified = 63,
        
        [Description("Bearer capability not implemented")]
        BearerCapabilityNotImplemented = 65,
        
        [Description("Channel type not implemented")]
        ChannelTypeNotImplemented = 66,
        
        [Description("Requested facility not implemented")]
        RequestedFacilityNotImplemented = 69,
        
        [Description("Only restricted digital information bearer capability is available (national use)")]
        OnlyRestrictedDigitalInformationBearerCapabilityAvailable = 70,
        
        [Description("Service or option that is not implemented, unspecified")]
        ServiceOrOptionNotImplementedUnspecified = 79,
        
        [Description("Invalid call reference value")]
        InvalidCallReferenceValue = 81,
        
        [Description("Identified channel does not exist")]
        IdentifiedChannelDoesNotExist = 82,
        
        [Description("A suspended call exists, but this call identity does not")]
        SuspendedCallExistsButThisCallIdentityDoesNot = 83,
        
        [Description("Call identity in use")]
        CallIdentityInUse = 84,
        
        [Description("No call suspended")]
        NoCallSuspended = 85,
        
        [Description("Call having the requested call identity has been cleared")]
        CallHavingRequestedCallIdentityCleared = 86,
        
        [Description("User not member of CUG (Closed User Group)")]
        UserNotMemberOfCUG = 87,
        
        [Description("Incompatible destination")]
        IncompatibleDestination = 88,
        
        [Description("Destination number missing and DC not subscribed")]
        DestinationNumberMissingAndDCNotSubscribed = 90,
        
        [Description("Invalid transit network selection (national use)")]
        InvalidTransitNetworkSelection = 91,
        
        [Description("Invalid message, unspecified")]
        InvalidMessageUnspecified = 95,
        
        [Description("Mandatory information element is missing")]
        MandatoryInformationElementMissing = 96,
        
        [Description("Message type nonexistent or not implemented")]
        MessageTypeNonexistentOrNotImplemented = 97,
        
        [Description("Message is not compatible with the call state, or the message type is nonexistent or not implemented")]
        MessageNotCompatibleWithCallStateOrMessageTypeNonexistentOrNotImplemented = 98,
        
        [Description("An information element or parameter does not exist or is not implemented")]
        InformationElementOrParameterDoesNotExist = 99,
        
        [Description("Invalid information element contents")]
        InvalidInformationElementContents = 100,
        
        [Description("The message is not compatible with the call state")]
        MessageNotCompatibleWithCallState = 101,
        
        [Description("Call terminated when timer expired; a recovery routine that is executed to recover from the error")]
        CallTerminatedWhenTimerExpired = 102,
        
        [Description("Parameter nonexistent or not implemented - passed on (national use)")]
        ParameterNonexistentOrNotImplemented = 103,
        
        [Description("Message with unrecognized parameter discarded")]
        MessageWithUnrecognizedParameterDiscarded = 110,
        
        [Description("Protocol error, unspecified")]
        ProtocolErrorUnspecified = 111,
        
        [Description("Precedence Level Exceeded")]
        PrecedenceLevelExceeded = 122,
        
        [Description("Device not Preemptable")]
        DeviceNotPreemptable = 123,
        
        [Description("Out of bandwidth (Cisco specific)")]
        OutOfBandwidth = 125,
        
        [Description("Call split (Cisco specific)")]
        CallSplit = 126,
        
        [Description("Interworking, unspecified")]
        InterworkingUnspecified = 127,
        
        [Description("Precedence out of bandwidth")]
        PrecedenceOutOfBandwidth = 129,
        
        [Description("Natural Isolated Code")]
        NaturalIsolatedCode = 130,
        
        [Description("Call Control Discovery PSTN Failover (Cisco specific)")]
        CallControlDiscoveryPSTNFailover = 131,
        
        [Description("IME QOS Fallback (Cisco specific)")]
        IMEQOSFallback = 132,
        
        [Description("PSTN Fallback locate Call Error (Cisco specific)")]
        PSTNFallbackLocateCallError = 133,
        
        [Description("PSTN Fallback wait for DTMF Timeout (Cisco specific)")]
        PSTNFallbackWaitForDTMFTimeout = 134,
        
        [Description("IME Failed Connection Timed out (Cisco specific)")]
        IMEFailedConnectionTimedOut = 135,
        
        [Description("IME Failed not enrolled (Cisco specific)")]
        IMEFailedNotEnrolled = 136,
        
        [Description("IME Failed socket error (Cisco specific)")]
        IMEFailedSocketError = 137,
        
        [Description("IME Failed domain blocked (Cisco specific)")]
        IMEFailedDomainBlocked = 138,
        
        [Description("IME Failed prefix blocked (Cisco specific)")]
        IMEFailedPrefixBlocked = 139,
        
        [Description("IME Failed expired ticket (Cisco specific)")]
        IMEFailedExpiredTicket = 140,
        
        [Description("IME Failed remote no matching route (Cisco specific)")]
        IMEFailedRemoteNoMatchingRoute = 141,
        
        [Description("IME Failed remote unregistered (Cisco specific)")]
        IMEFailedRemoteUnregistered = 142,
        
        [Description("IME Failed remote IME disabled (Cisco specific)")]
        IMEFailedRemoteIMEDisabled = 143,
        
        [Description("IME Failed remote invalid IME trunk URI (Cisco specific)")]
        IMEFailedRemoteInvalidIMETrunkURI = 144,
        
        [Description("IME Failed remote URI not E164 (Cisco specific)")]
        IMEFailedRemoteURINotE164 = 145,
        
        [Description("IME Failed remote called number not available (Cisco specific)")]
        IMEFailedRemoteCalledNumberNotAvailable = 146,
        
        [Description("IME Failed Invalid Ticket (Cisco specific)")]
        IMEFailedInvalidTicket = 147,
        
        [Description("IME Failed unknown (Cisco specific)")]
        IMEFailedUnknown = 148,
        
        [Description("DCC Allowed Percentage Exceeded")]
        DCCAllowedPercentageExceeded = 155,

        [Description("Conference Full (was 124)")]
        ConferenceFull = 262144,

        [Description("Call split (was 126) This code applies when a call terminates during a transfer operation because it was split off and terminated (was not part of the final transferred call). This code can help you to determine which calls terminated as part of a feature operation.")]
        CallSplit126 = 393216,

        [Description("Conference drop any party/Conference drop last party (was 128)")]
        ConferenceDrop = 458752,

        [Description("CCM_SIP_400_BAD_REQUEST")]
        CCM_SIP_400_BAD_REQUEST = 16777257,

        [Description("CCM_SIP_401_UNAUTHORIZED")]
        CCM_SIP_401_UNAUTHORIZED = 33554453,

        [Description("CCM_SIP_402_PAYMENT_REQUIRED")]
        CCM_SIP_402_PAYMENT_REQUIRED = 50331669,

        [Description("CCM_SIP_403_FORBIDDEN")]
        CCM_SIP_403_FORBIDDEN = 67108885,

        [Description("CCM_SIP_404_NOT_FOUND")]
        CCM_SIP_404_NOT_FOUND = 83886081,

        [Description("CCM_SIP_405_METHOD_NOT_ALLOWED")]
        CCM_SIP_405_METHOD_NOT_ALLOWED = 100663359,

        [Description("CCM_SIP_406_NOT_ACCEPTABLE")]
        CCM_SIP_406_NOT_ACCEPTABLE = 117440591,

        [Description("CCM_SIP_407_PROXY_AUTHENTICATION_REQUIRED")]
        CCM_SIP_407_PROXY_AUTHENTICATION_REQUIRED = 134217749,

        [Description("CCM_SIP_408_REQUEST_TIMEOUT")]
        CCM_SIP_408_REQUEST_TIMEOUT = 150995046,

        [Description("CCM_SIP_410_GONE")]
        CCM_SIP_410_GONE = 184549398,

        [Description("CCM_SIP_411_LENGTH_REQUIRED")]
        CCM_SIP_411_LENGTH_REQUIRED = 201326719,

        [Description("CCM_SIP_413_REQUEST_ENTITY_TOO_LONG")]
        CCM_SIP_413_REQUEST_ENTITY_TOO_LONG = 234881151,

        [Description("CCM_SIP_414_REQUEST_URI_TOO_LONG")]
        CCM_SIP_414_REQUEST_URI_TOO_LONG = 251658367,

        [Description("CCM_SIP_415_UNSUPPORTED_MEDIA_TYPE")]
        CCM_SIP_415_UNSUPPORTED_MEDIA_TYPE = 268435535,

        [Description("CCM_SIP_416_UNSUPPORTED_URI_SCHEME")]
        CCM_SIP_416_UNSUPPORTED_URI_SCHEME = 285212799,

        [Description("CCM_SIP_420_BAD_EXTENSION")]
        CCM_SIP_420_BAD_EXTENSION = 83886207,

        [Description("CCM_SIP_421_EXTENSION_REQUIRED")]
        CCM_SIP_421_EXTENSION_REQUIRED = 369098879,

        [Description("CCM_SIP_423_INTERVAL_TOO_BRIEF")]
        CCM_SIP_423_INTERVAL_TOO_BRIEF = 402653311,

        [Description("CCM_SIP_424_BAD_LOCATION_INFO")]
        CCM_SIP_424_BAD_LOCATION_INFO = 419430421,

        [Description("CCM_SIP_429_PROVIDE_REFER_IDENTITY")]
        CCM_SIP_429_PROVIDE_REFER_IDENTITY = 503316501,

        [Description("CCM_SIP_480_TEMPORARILY_UNAVAILABLE")]
        CCM_SIP_480_TEMPORARILY_UNAVAILABLE = 1073741842,

        [Description("CCM_SIP_481_CALL_LEG_DOES_NOT_EXIST")]
        CCM_SIP_481_CALL_LEG_DOES_NOT_EXIST = 1090519081,

        [Description("CCM_SIP_482_LOOP_DETECTED")]
        CCM_SIP_482_LOOP_DETECTED = 1107296281,

        [Description("CCM_SIP_483_TOO_MANY_HOOPS")]
        CCM_SIP_483_TOO_MANY_HOOPS = 1124073497,

        [Description("CCM_SIP_484_ADDRESS_INCOMPLETE")]
        CCM_SIP_484_ADDRESS_INCOMPLETE = 1140850716,

        [Description("CCM_SIP_485_AMBIGUOUS")]
        CCM_SIP_485_AMBIGUOUS = 1157627905,

        [Description("CCM_SIP_486_BUSY_HERE")]
        CCM_SIP_486_BUSY_HERE = 1174405137,

        [Description("CCM_SIP_487_REQUEST_TERMINATED")]
        CCM_SIP_487_REQUEST_TERMINATED = 1191182367,

        [Description("CCM_SIP_488_NOT_ACCEPTABLE_HERE")]
        CCM_SIP_488_NOT_ACCEPTABLE_HERE = 1207959583,

        [Description("CCM_SIP_491_REQUEST_PENDING")]
        CCM_SIP_491_REQUEST_PENDING = 1258291217,

        [Description("CCM_SIP_493_UNDECIPHERABLE")]
        CCM_SIP_493_UNDECIPHERABLE = 1291845649,

        [Description("CCM_SIP_500_SERVER_INTERNAL_ERROR")]
        CCM_SIP_500_SERVER_INTERNAL_ERROR = 1409286185,

        [Description("CCM_SIP_502_BAD_GATEWAY")]
        CCM_SIP_502_BAD_GATEWAY = 1442840614,

        [Description("CCM_SIP_503_SERVICE_UNAVAILABLE")]
        CCM_SIP_503_SERVICE_UNAVAILABLE = 1459617833,

        [Description("CCM_SIP_503_SERVICE_UNAVAILABLE_SER_OPTION_NOAV")]
        CCM_SIP_503_SERVICE_UNAVAILABLE_SER_OPTION_NOAV = 2801795135,

        [Description("CCM_SIP_504_SERVER_TIME_OUT")]
        CCM_SIP_504_SERVER_TIME_OUT = 1476395110,

        [Description("CCM_SIP_505_SIP_VERSION_NOT_SUPPORTED")]
        CCM_SIP_505_SIP_VERSION_NOT_SUPPORTED = 1493172351,

        [Description("CCM_SIP_513_MESSAGE_TOO_LARGE")]
        CCM_SIP_513_MESSAGE_TOO_LARGE = 1509949567,

        [Description("CCM_SIP_600_BUSY_EVERYWHERE")]
        CCM_SIP_600_BUSY_EVERYWHERE = 2701131793,

        [Description("CCM_SIP_603_DECLINE")]
        CCM_SIP_603_DECLINE = 2717909013,

        [Description("CCM_SIP_604_DOES_NOT_EXIST_ANYWHERE")]
        CCM_SIP_604_DOES_NOT_EXIST_ANYWHERE = 2734686209,

        [Description("CCM_SIP_606_NOT_ACCEPTABLE")]
        CCM_SIP_606_NOT_ACCEPTABLE = 2751463455,

        [Description("CTI_REQUEST_INVALID_PRIMARY_CALL_STATE")]
        CTI_REQUEST_INVALID_PRIMARY_CALL_STATE = 655360,

        [Description("PREEMPTION_NON_IP")]
        PREEMPTION_NON_IP = 851976,

        [Description("PREEMPTION_NETWORK")]
        PREEMPTION_NETWORK = 917512,

        [Description("CCM_SIP_417_ERR_RESOURCE_PRIORITY_ROUTINE")]
        CCM_SIP_417_ERR_RESOURCE_PRIORITY_ROUTINE = 301989951,

        [Description("CCM_SIP_433_ANONYMITY_DISALLOWED")]
        CCM_SIP_433_ANONYMITY_DISALLOWED = 570425365
    }
}