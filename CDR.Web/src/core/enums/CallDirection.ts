import i18n from "../plugins/i18n";

enum CallDirection {
  INCOMING = 1,
  OUTGOING = 2,
  INTERNAL = 3,
}

class CallDirectionHelper {
  static getDescription(type: CallDirection): string {
    switch (type) {
      case CallDirection.INTERNAL:
        return i18n.global.t("internal");
      case CallDirection.OUTGOING:
        return i18n.global.t("outgoing");
      case CallDirection.INCOMING:
        return i18n.global.t("incoming");
      default:
        return "--";
    }
  }

  static getColor(type: CallDirection): string {
    switch (type) {
      case CallDirection.INTERNAL:
        return "primary";
      case CallDirection.OUTGOING:
        return "success";
      case CallDirection.INCOMING:
        return "info";
      default:
        return "warning";
    }
  }
}

export { CallDirectionHelper, CallDirection };
