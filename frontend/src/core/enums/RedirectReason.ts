import i18n from "../plugins/i18n";

enum RedirectReason {
  UNKNOWN = 0,
  CALL_FORWARD_BUSY = 1,
  CALL_FORWARD_NO_ANSWER = 2,
  CALL_TRANSFER = 4,
  CALL_PICKUP = 5,
  CALL_PARK = 7,
  CALL_PARK_PICKUP = 8,
  CPE_OUT_OF_ORDER = 9,
  CALL_FORWARD = 10,
  CALL_PARK_REVERSION = 11,
  CALL_FORWARD_ALL = 15,
  CALL_DEFLECTION = 18,
  BLIND_TRANSFER = 34,
  CALL_IMMEDIATE_DIVERT = 50,
  CALL_FORWARD_ALTERNATE_PARTY = 66,
  CALL_FORWARD_ON_FAILURE = 82,
  CONFERENCE = 98,
  BARGE = 114,
  AAR = 129,
  REFER = 130,
  REPLACES = 146,
  REDIRECTION_3XX = 162,
  SIP_FORWARD_BUSY_GREETING = 177,
  CALL_FORWARD_UNREGISTERED = 178,
  FOLLOW_ME_SIP_FORWARD_ALL_GREETING = 207,
  OUT_OF_SERVICE_SIP_FORWARD_BUSY_GREETING = 209,
  TIME_OF_DAY_SIP_FORWARD_ALL_GREETING = 239,
  DO_NOT_DISTURB_SIP_FORWARD_NO_ANSWER_GREETING = 242,
  UNAVAILABLE_SIP_FORWARD_BUSY_GREETING = 257,
  AWAY_SIP_FORWARD_NO_ANSWER_GREETING = 274,
  MOBILITY_HAND_IN = 303,
  MOBILITY_HAND_OUT = 319,
  MOBILITY_FOLLOW_ME = 335,
  MOBILITY_REDIAL = 351,
  RECORDING = 354,
  MONITORING = 370,
  MOBILITY_IVR = 399,
  MOBILITY_DVOR = 401,
  MOBILITY_EFA = 402,
  MOBILITY_SESSION_HANDOFF = 403,
  MOBILITY_CELL_PICKUP = 415,
  CLICK_TO_CONFERENCE = 418,
  FORWARD_NO_RETRIEVE = 434,
  FORWARD_NO_RETRIEVE_SEND_BACK_TO_PARKER = 450,
  CALL_CONTROL_DISCOVERY = 464,
  IME = 480,
  IME_CONNECTION_TIMED_OUT = 496,
  IME_NOT_ENROLLED = 512,
  IME_SOCKET_ERROR = 528,
  IME_DOMAIN_BLACKLISTED = 544,
  IME_PREFIX_BLACKLISTED = 560,
  IME_EXPIRED_TICKET = 576,
  IME_REMOTE_NO_MATCHING_ROUTE = 592,
  IME_REMOTE_UNREGISTERED = 608,
  IME_REMOTE_IME_DISABLED = 624,
  IME_REMOTE_INVALID_IME_TRUNK_URI = 640,
  IME_REMOTE_URI_NOT_E164 = 656,
  IME_REMOTE_CALLED_NUMBER_NOT_AVAILABLE = 672,
  IME_INVALID_TICKET = 688,
  IME_UNKNOWN = 704,
  IME_PSTN_FALLBACK = 720,
  PRESENCE_ENABLED_ROUTING = 738,
  AGENT_GREETING = 752,
  NURD = 783,
  NATIVE_CALL_QUEUING_QUEUE_CALL = 786,
  NATIVE_CALL_QUEUING_DEQUEUE_CALL = 802,
  NATIVE_CALL_QUEUING_REDIRECT_NO_AGENT = 818,
  NATIVE_CALL_QUEUING_REDIRECT_QUEUE_FULL = 834,
  NATIVE_CALL_QUEUING_REDIRECT_MAX_WAIT = 850,
}

class RedirectReasonHelper {
  static getDescription(reason: RedirectReason): string {
    switch (reason) {
      case RedirectReason.UNKNOWN:
        return i18n.global.t("unknown");
      case RedirectReason.CALL_FORWARD_BUSY:
        return i18n.global.t("callForwardBusy");
      case RedirectReason.CALL_FORWARD_NO_ANSWER:
        return i18n.global.t("callForwardNoAnswer");
      case RedirectReason.CALL_TRANSFER:
        return i18n.global.t("callTransfer");
      case RedirectReason.CALL_PICKUP:
        return i18n.global.t("callPickup");
      case RedirectReason.CALL_PARK:
        return i18n.global.t("callPark");
      case RedirectReason.CALL_PARK_PICKUP:
        return i18n.global.t("callParkPickup");
      case RedirectReason.CPE_OUT_OF_ORDER:
        return i18n.global.t("CPEOutOfOrder");
      case RedirectReason.CALL_FORWARD:
        return i18n.global.t("callForward");
      case RedirectReason.CALL_PARK_REVERSION:
        return i18n.global.t("callParkReversion");
      case RedirectReason.CALL_FORWARD_ALL:
        return i18n.global.t("callForwardAll");
      case RedirectReason.CALL_DEFLECTION:
        return i18n.global.t("callDeflection");
      case RedirectReason.BLIND_TRANSFER:
        return i18n.global.t("blindTransfer");
      case RedirectReason.CALL_IMMEDIATE_DIVERT:
        return i18n.global.t("callImmediateDivert");
      case RedirectReason.CALL_FORWARD_ALTERNATE_PARTY:
        return i18n.global.t("callForwardAlternateParty");
      case RedirectReason.CALL_FORWARD_ON_FAILURE:
        return i18n.global.t("callForwardOnFailure");
      case RedirectReason.CONFERENCE:
        return i18n.global.t("conference");
      case RedirectReason.BARGE:
        return i18n.global.t("barge");
      case RedirectReason.AAR:
        return i18n.global.t("aar");
      case RedirectReason.REFER:
        return i18n.global.t("refer");
      case RedirectReason.REPLACES:
        return i18n.global.t("replaces");
      case RedirectReason.REDIRECTION_3XX:
        return i18n.global.t("redirection3xx");
      case RedirectReason.SIP_FORWARD_BUSY_GREETING:
        return i18n.global.t("SIPForwardBusyGreeting");
      case RedirectReason.CALL_FORWARD_UNREGISTERED:
        return i18n.global.t("callForwardUnregistered");
      case RedirectReason.FOLLOW_ME_SIP_FORWARD_ALL_GREETING:
        return i18n.global.t("followMeSIPForwardAllGreeting");
      case RedirectReason.OUT_OF_SERVICE_SIP_FORWARD_BUSY_GREETING:
        return i18n.global.t("outOfServiceSIPForwardBusyGreeting");
      case RedirectReason.TIME_OF_DAY_SIP_FORWARD_ALL_GREETING:
        return i18n.global.t("timeOfDaySIPForwardAllGreeting");
      case RedirectReason.DO_NOT_DISTURB_SIP_FORWARD_NO_ANSWER_GREETING:
        return i18n.global.t("doNotDisturbSIPForwardNoAnswerGreeting");
      case RedirectReason.UNAVAILABLE_SIP_FORWARD_BUSY_GREETING:
        return i18n.global.t("unavailableSIPForwardBusyGreeting");
      case RedirectReason.AWAY_SIP_FORWARD_NO_ANSWER_GREETING:
        return i18n.global.t("awaySIPForwardNoAnswerGreeting");
      case RedirectReason.MOBILITY_HAND_IN:
        return i18n.global.t("mobilityHandIn");
      case RedirectReason.MOBILITY_HAND_OUT:
        return i18n.global.t("mobilityHandOut");
      case RedirectReason.MOBILITY_FOLLOW_ME:
        return i18n.global.t("mobilityFollowMe");
      case RedirectReason.MOBILITY_REDIAL:
        return i18n.global.t("mobilityRedial");
      case RedirectReason.RECORDING:
        return i18n.global.t("recording");
      case RedirectReason.MONITORING:
        return i18n.global.t("monitoring");
      case RedirectReason.MOBILITY_IVR:
        return i18n.global.t("mobilityIVR");
      case RedirectReason.MOBILITY_DVOR:
        return i18n.global.t("mobilityDVOR");
      case RedirectReason.MOBILITY_EFA:
        return i18n.global.t("mobilityEFA");
      case RedirectReason.MOBILITY_SESSION_HANDOFF:
        return i18n.global.t("mobilitySessionHandoff");
      case RedirectReason.MOBILITY_CELL_PICKUP:
        return i18n.global.t("mobilityCellPickup");
      case RedirectReason.CLICK_TO_CONFERENCE:
        return i18n.global.t("clickToConference");
      case RedirectReason.FORWARD_NO_RETRIEVE:
        return i18n.global.t("forwardNoRetrieve");
      case RedirectReason.FORWARD_NO_RETRIEVE_SEND_BACK_TO_PARKER:
        return i18n.global.t("forwardNoRetrieveSendBackToParker");
      case RedirectReason.CALL_CONTROL_DISCOVERY:
        return i18n.global.t("callControlDiscovery");
      case RedirectReason.IME:
        return i18n.global.t("IME");
      case RedirectReason.IME_CONNECTION_TIMED_OUT:
        return i18n.global.t("IMEConnectionTimedOut");
      case RedirectReason.IME_NOT_ENROLLED:
        return i18n.global.t("IMENotEnrolled");
      case RedirectReason.IME_SOCKET_ERROR:
        return i18n.global.t("IMESocketError");
      case RedirectReason.IME_DOMAIN_BLACKLISTED:
        return i18n.global.t("IMEDomainBlacklisted");
      case RedirectReason.IME_PREFIX_BLACKLISTED:
        return i18n.global.t("IMEPrefixBlacklisted");
      case RedirectReason.IME_EXPIRED_TICKET:
        return i18n.global.t("IMEExpiredTicket");
      case RedirectReason.IME_REMOTE_NO_MATCHING_ROUTE:
        return i18n.global.t("IMERemoteNoMatchingRoute");
      case RedirectReason.IME_REMOTE_UNREGISTERED:
        return i18n.global.t("IMERemoteUnregistered");
      case RedirectReason.IME_REMOTE_IME_DISABLED:
        return i18n.global.t("IMERemoteIMEDisabled");
      case RedirectReason.IME_REMOTE_INVALID_IME_TRUNK_URI:
        return i18n.global.t("IMERemoteInvalidIMETrunkURI");
      case RedirectReason.IME_REMOTE_URI_NOT_E164:
        return i18n.global.t("IMERemoteURINotE164");
      case RedirectReason.IME_REMOTE_CALLED_NUMBER_NOT_AVAILABLE:
        return i18n.global.t("IMERemoteCalledNumberNotAvailable");
      case RedirectReason.IME_INVALID_TICKET:
        return i18n.global.t("IMEInvalidTicket");
      case RedirectReason.IME_UNKNOWN:
        return i18n.global.t("IMEUnknown");
      case RedirectReason.IME_PSTN_FALLBACK:
        return i18n.global.t("IMEPSTNFallback");
      case RedirectReason.PRESENCE_ENABLED_ROUTING:
        return i18n.global.t("presenceEnabledRouting");
      case RedirectReason.AGENT_GREETING:
        return i18n.global.t("agentGreeting");
      case RedirectReason.NURD:
        return i18n.global.t("NuRD");
      case RedirectReason.NATIVE_CALL_QUEUING_QUEUE_CALL:
        return i18n.global.t("nativeCallQueuingQueueCall");
      case RedirectReason.NATIVE_CALL_QUEUING_DEQUEUE_CALL:
        return i18n.global.t("nativeCallQueuingDequeueCall");
      case RedirectReason.NATIVE_CALL_QUEUING_REDIRECT_NO_AGENT:
        return i18n.global.t("nativeCallQueuingRedirectNoAgent");
      case RedirectReason.NATIVE_CALL_QUEUING_REDIRECT_QUEUE_FULL:
        return i18n.global.t("nativeCallQueuingRedirectQueueFull");
      case RedirectReason.NATIVE_CALL_QUEUING_REDIRECT_MAX_WAIT:
        return i18n.global.t("nativeCallQueuingRedirectMaxWait");
      default:
        return i18n.global.t("--");
    }
  }

  static getColor(reason: RedirectReason): string {
    switch (reason) {
      case RedirectReason.UNKNOWN:
        return "";
      case RedirectReason.CALL_FORWARD_BUSY:
        return "warning";
      case RedirectReason.CALL_FORWARD_NO_ANSWER:
        return "warning";
      case RedirectReason.CALL_TRANSFER:
        return "info";
      case RedirectReason.CALL_PICKUP:
        return "info";
      case RedirectReason.CALL_PARK:
        return "info";
      case RedirectReason.CALL_PARK_PICKUP:
        return "info";
      case RedirectReason.CPE_OUT_OF_ORDER:
        return "danger";
      case RedirectReason.CALL_FORWARD:
        return "info";
      case RedirectReason.CALL_PARK_REVERSION:
        return "info";
      case RedirectReason.CALL_FORWARD_ALL:
        return "info";
      case RedirectReason.CALL_DEFLECTION:
        return "info";
      case RedirectReason.BLIND_TRANSFER:
        return "info";
      case RedirectReason.CALL_IMMEDIATE_DIVERT:
        return "info";
      case RedirectReason.CALL_FORWARD_ALTERNATE_PARTY:
        return "info";
      case RedirectReason.CALL_FORWARD_ON_FAILURE:
        return "danger";
      case RedirectReason.CONFERENCE:
        return "info";
      case RedirectReason.BARGE:
        return "info";
      case RedirectReason.AAR:
        return "info";
      case RedirectReason.REFER:
        return "info";
      case RedirectReason.REPLACES:
        return "info";
      case RedirectReason.REDIRECTION_3XX:
        return "info";
      case RedirectReason.SIP_FORWARD_BUSY_GREETING:
        return "info";
      case RedirectReason.CALL_FORWARD_UNREGISTERED:
        return "info";
      case RedirectReason.FOLLOW_ME_SIP_FORWARD_ALL_GREETING:
        return "info";
      case RedirectReason.OUT_OF_SERVICE_SIP_FORWARD_BUSY_GREETING:
        return "danger";
      case RedirectReason.TIME_OF_DAY_SIP_FORWARD_ALL_GREETING:
        return "info";
      case RedirectReason.DO_NOT_DISTURB_SIP_FORWARD_NO_ANSWER_GREETING:
        return "info";
      case RedirectReason.UNAVAILABLE_SIP_FORWARD_BUSY_GREETING:
        return "danger";
      case RedirectReason.AWAY_SIP_FORWARD_NO_ANSWER_GREETING:
        return "info";
      case RedirectReason.MOBILITY_HAND_IN:
        return "info";
      case RedirectReason.MOBILITY_HAND_OUT:
        return "info";
      case RedirectReason.MOBILITY_FOLLOW_ME:
        return "info";
      case RedirectReason.MOBILITY_REDIAL:
        return "info";
      case RedirectReason.RECORDING:
        return "info";
      case RedirectReason.MONITORING:
        return "info";
      case RedirectReason.MOBILITY_IVR:
        return "info";
      case RedirectReason.MOBILITY_DVOR:
        return "info";
      case RedirectReason.MOBILITY_EFA:
        return "info";
      case RedirectReason.MOBILITY_SESSION_HANDOFF:
        return "info";
      case RedirectReason.MOBILITY_CELL_PICKUP:
        return "info";
      case RedirectReason.CLICK_TO_CONFERENCE:
        return "info";
      case RedirectReason.FORWARD_NO_RETRIEVE:
        return "info";
      case RedirectReason.FORWARD_NO_RETRIEVE_SEND_BACK_TO_PARKER:
        return "info";
      case RedirectReason.CALL_CONTROL_DISCOVERY:
        return "info";
      case RedirectReason.IME:
        return "info";
      case RedirectReason.IME_CONNECTION_TIMED_OUT:
        return "danger";
      case RedirectReason.IME_NOT_ENROLLED:
        return "danger";
      case RedirectReason.IME_SOCKET_ERROR:
        return "danger";
      case RedirectReason.IME_DOMAIN_BLACKLISTED:
        return "danger";
      case RedirectReason.IME_PREFIX_BLACKLISTED:
        return "danger";
      case RedirectReason.IME_EXPIRED_TICKET:
        return "danger";
      case RedirectReason.IME_REMOTE_NO_MATCHING_ROUTE:
        return "danger";
      case RedirectReason.IME_REMOTE_UNREGISTERED:
        return "danger";
      case RedirectReason.IME_REMOTE_IME_DISABLED:
        return "danger";
      case RedirectReason.IME_REMOTE_INVALID_IME_TRUNK_URI:
        return "danger";
      case RedirectReason.IME_REMOTE_URI_NOT_E164:
        return "danger";
      case RedirectReason.IME_REMOTE_CALLED_NUMBER_NOT_AVAILABLE:
        return "danger";
      case RedirectReason.IME_INVALID_TICKET:
        return "danger";
      case RedirectReason.IME_UNKNOWN:
        return "danger";
      case RedirectReason.IME_PSTN_FALLBACK:
        return "info";
      case RedirectReason.PRESENCE_ENABLED_ROUTING:
        return "info";
      case RedirectReason.AGENT_GREETING:
        return "info";
      case RedirectReason.NURD:
        return "info";
      case RedirectReason.NATIVE_CALL_QUEUING_QUEUE_CALL:
        return "info";
      case RedirectReason.NATIVE_CALL_QUEUING_DEQUEUE_CALL:
        return "info";
      case RedirectReason.NATIVE_CALL_QUEUING_REDIRECT_NO_AGENT:
        return "info";
      case RedirectReason.NATIVE_CALL_QUEUING_REDIRECT_QUEUE_FULL:
        return "info";
      case RedirectReason.NATIVE_CALL_QUEUING_REDIRECT_MAX_WAIT:
        return "info";
      default:
        return "info";
    }
  }
}

export { RedirectReasonHelper, RedirectReason };
