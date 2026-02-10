import Swal from "sweetalert2";
import { ElMessage } from "element-plus";
import router from "@/router";
import i18n from "../plugins/i18n";
import HttpStatusCodeEnum from "../enums/HttpStatusCodeEnum";

class ResponseMessageService {
  static success(message: string) {
    Swal.fire({
      icon: "success",
      title: i18n.global.t(message),
      showConfirmButton: false,
      timer: 1500,
    });
  }

  static error(message: string) {
    Swal.fire({
      icon: "error",
      title: i18n.global.t(message),
      showConfirmButton: false,
      timer: 1500,
    });
  }

  static warning(message: string) {
    Swal.fire({
      icon: "warning",
      title: message,
      showConfirmButton: false,
      timer: 1500,
    });
  }

  static info(message: string) {
    Swal.fire({
      icon: "info",
      title: i18n.global.t(message),
      showConfirmButton: false,
      timer: 1500,
    });
  }

  public static showMessageByType(message, type): Promise<string> {
    return new Promise(() => {
      ElMessage({
        showClose: true,
        message: i18n.global.t(message),
        type: type,
      });
      Promise.resolve(message);
    });
  }

  static async handleHttpError(status: number, message: string) {
    switch (status) {
      case HttpStatusCodeEnum.UNAUTHORIZED:
        await Swal.fire({
          icon: "warning",
          title: i18n.global.t("auth.sessionExpired").toString(),
          showConfirmButton: false,
          timer: 1500,
        });
        router.push({ name: "sign-in" });
        break;
      case HttpStatusCodeEnum.FORBIDDEN:
        ElMessage.error(i18n.global.t("auth.forbidden").toString());
        break;
      case HttpStatusCodeEnum.NOT_FOUND:
        ElMessage.error(i18n.global.t("auth.notFound").toString());
        break;
      case HttpStatusCodeEnum.INTERNAL_SERVER_ERROR:
        ElMessage.error(i18n.global.t("auth.internalServerError").toString());
        break;
      default:
        ElMessage.error(message);
        break;
    }
  }
}

export default ResponseMessageService;
