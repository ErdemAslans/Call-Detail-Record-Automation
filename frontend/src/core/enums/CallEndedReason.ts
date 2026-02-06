import i18n from "../plugins/i18n";

enum CallEndedReason {
  UNKNOWN = 1,
  NORMAL_TERMINATION = 2,
  BUSY = 3,
  TECHNICAL_ISSUE = 4,
}

class CallEndedReasonHelper {
  static getDescription(reason: CallEndedReason): string {
    switch (reason) {
      case CallEndedReason.UNKNOWN:
        return i18n.global.t("unknown");
      case CallEndedReason.NORMAL_TERMINATION:
        return i18n.global.t("normalTermination");
      case CallEndedReason.BUSY:
        return i18n.global.t("busy");
      case CallEndedReason.TECHNICAL_ISSUE:
        return i18n.global.t("technicalIssue");
      default:
        return i18n.global.t("--");
    }
  }

  static getColor(reason: CallEndedReason): string {
    switch (reason) {
      case CallEndedReason.UNKNOWN:
        return "";
      case CallEndedReason.NORMAL_TERMINATION:
        return "success";
      case CallEndedReason.BUSY:
        return "warning";
      case CallEndedReason.TECHNICAL_ISSUE:
        return "info";
      default:
        return "info";
    }
  }
}

export { CallEndedReasonHelper, CallEndedReason };
