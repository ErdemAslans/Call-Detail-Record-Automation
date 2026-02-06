import { configure } from "vee-validate";
import { setYupLocale } from "@/core/plugins/yup-locale";

export function initVeeValidate() {

  const locale = localStorage.getItem("locale") || "tr";
  // Updating default vee-validate configuration
  configure({
    validateOnBlur: true,
    validateOnChange: true,
    validateOnInput: true,
    validateOnModelUpdate: true,
  });

    // Set Yup locale
    setYupLocale(locale);
}
