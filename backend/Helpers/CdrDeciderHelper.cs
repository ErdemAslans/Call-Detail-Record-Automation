using Common.Enums;

namespace CallCenter.Helpers;

public static class CdrDeciderHelper
{
    public static CallEndedReason DecideCallEndedReason(OnBehalfOfCode? callTerminationOnBehalfOf, CauseValue? causeCode) =>
        callTerminationOnBehalfOf switch
        {
            null => CallEndedReason.Unknown,
            OnBehalfOfCode.Unknown => CallEndedReason.Unknown,
            OnBehalfOfCode.CctiLine when causeCode == CauseValue.NormalCallClearing => CallEndedReason.NormalTermination,
            _ when causeCode == CauseValue.UserBusy => CallEndedReason.Busy,
            _ => CallEndedReason.TechnicalIssue
        };

    public static CallType DecideCallType(int? duration, DateTime? connectTime) =>
        (duration, connectTime) switch
        {
            (not null, not null) => CallType.AnsweredCall,
            _ => CallType.MissedCall
        };
}