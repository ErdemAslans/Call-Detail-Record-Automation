import i18n from "../plugins/i18n";

enum CallType {
  ANSWERED_CALL = 1,
  MISSED_CALL = 2,
}

class CallTypeHelper {
  static getDescription(type: CallType): string {
    switch (type) {
      case CallType.ANSWERED_CALL:
        return i18n.global.t("answeredCall");
      case CallType.MISSED_CALL:
        return i18n.global.t("missedCall");
      default:
        return "--";
    }
  }
  static getColor(type: CallType): string {
    switch (type) {
      case CallType.ANSWERED_CALL:
        return "success";
      case CallType.MISSED_CALL:
        return "danger";
      default:
        return "warning";
    }
  }
}

export { CallTypeHelper, CallType };
