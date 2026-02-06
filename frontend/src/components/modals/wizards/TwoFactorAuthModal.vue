<template>
  <!--begin::Modal - Two-factor authentication-->
  <div
    class="modal fade"
    id="kt_modal_two_factor_authentication"
    tabindex="-1"
    aria-hidden="true"
  >
    <!--begin::Modal header-->
    <div class="modal-dialog modal-dialog-centered mw-650px">
      <!--begin::Modal content-->
      <div class="modal-content">
        <!--begin::Modal header-->
        <div class="modal-header flex-stack">
          <!--begin::Title-->
          <h2>{{ t("chooseAuthMethod") }}</h2>
          <!--end::Title-->

          <!--begin::Close-->
          <div
            class="btn btn-sm btn-icon btn-active-color-primary"
            data-bs-dismiss="modal"
          >
            <KTIcon icon-name="cross" icon-class="fs-1" />
          </div>
          <!--end::Close-->
        </div>
        <!--begin::Modal header-->

        <!--begin::Modal body-->
        <div class="modal-body scroll-y pt-10 pb-15 px-lg-17">
          <!--begin::Options-->
          <div :class="[state !== '' && 'd-none']">
            <!--begin::Notice-->
            <p class="text-gray-500 fs-5 fw-semibold mb-10">
              {{ t("twoFactorDescription") }}
            </p>
            <!--end::Notice-->

            <!--begin::Wrapper-->
            <div class="pb-10">
              <!--begin::Option-->
              <input
                type="radio"
                class="btn-check"
                name="auth_option"
                value="apps"
                checked
                id="kt_modal_two_factor_authentication_option_1"
                v-model="value"
              />
              <label
                class="btn btn-outline btn-outline-dashed btn-outline-default p-7 d-flex align-items-center mb-5"
                for="kt_modal_two_factor_authentication_option_1"
              >
                <KTIcon icon-name="setting-2" icon-class="fs-4x me-4" />

                <span class="d-block fw-semibold text-start">
                  <span class="text-gray-900 fw-bold d-block fs-3"
                    >{{ t("authenticatorAppsTitle") }}</span
                  >
                  <span class="text-gray-500 fw-semibold fs-6">
                    {{ t("authenticatorAppsDesc") }}
                  </span>
                </span>
              </label>
              <!--end::Option-->

              <!--begin::Option-->
              <input
                type="radio"
                class="btn-check"
                name="auth_option"
                value="sms"
                id="kt_modal_two_factor_authentication_option_2"
                v-model="value"
              />
              <label
                class="btn btn-outline btn-outline-dashed btn-outline-default p-7 d-flex align-items-center"
                for="kt_modal_two_factor_authentication_option_2"
              >
                <KTIcon icon-name="message-text-2" icon-class="fs-4x me-4" />

                <span class="d-block fw-semibold text-start">
                  <span class="text-gray-900 fw-bold d-block fs-3">{{ t("smsTitle") }}</span>
                  <span class="text-gray-500 fw-semibold fs-6"
                    >{{ t("smsDesc") }}</span
                  >
                </span>
              </label>
              <!--end::Option-->
            </div>
            <!--end::Options-->

            <!--begin::Action-->
            <button @click="state = value" class="btn btn-primary w-100">
              {{ t("continueButton") }}
            </button>
            <!--end::Action-->
          </div>
          <!--end::Options-->

          <!--begin::Apps-->
          <div :class="[state !== 'apps' && 'd-none']" data-kt-element="apps">
            <!--begin::Heading-->
            <h3 class="text-gray-900 fw-bold mb-7">{{ t("authenticatorAppsHeading") }}</h3>
            <!--end::Heading-->

            <!--begin::Description-->
            <div class="text-gray-500 fw-semibold fs-6 mb-10">
              {{ t("authenticatorAppsDesc2") }}

              <!--begin::QR code image-->
              <div class="pt-5 text-center">
                <img
                  :src="getAssetPath('media/misc/qr.png')"
                  alt=""
                  class="mw-150px"
                />
              </div>
              <!--end::QR code image-->
            </div>
            <!--end::Description-->

            <div
              class="notice d-flex bg-light-warning rounded border-warning border border-dashed mb-10 p-6"
            >
              <KTIcon
                icon-name="formation-5"
                icon-class="fs-2tx text-warning me-4"
              />
              <!--begin::Wrapper-->
              <div class="d-flex flex-stack flex-grow-1">
                <!--begin::Content-->
                <div class="fw-semibold">
                  <div class="fs-6 text-gray-600">
                    {{ t("qrCodeManualEntry") }}
                    <div class="fw-bold text-gray-900 pt-2">
                      KBSS3QDAAFUMCBY63YCKI5WSSVACUMPN
                    </div>
                  </div>
                </div>
                <!--end::Content-->
              </div>
              <!--end::Wrapper-->
            </div>

            <!--begin::Form-->
            <VForm
              class="form"
              @submit="submitAuthCodeForm()"
              :validation-schema="schema2"
            >
              <!--begin::Input group-->
              <div class="mb-10 fv-row">
                <Field
                  type="text"
                  class="form-control form-control-lg form-control-solid"
                  :placeholder="t('enterAuthCode')"
                  name="code"
                />
                <div class="fv-plugins-message-container">
                  <div class="fv-help-block">
                    <ErrorMessage name="code" />
                  </div>
                </div>
              </div>
              <!--end::Input group-->

              <!--begin::Actions-->
              <div class="d-flex flex-center">
                <button
                  type="reset"
                  @click="state = ''"
                  class="btn btn-light me-3"
                >
                  {{ t("cancel") }}
                </button>

                <button
                  ref="submitAuthCodeButtonRef"
                  type="submit"
                  data-kt-element="apps-submit"
                  class="btn btn-primary"
                >
                  <span class="indicator-label"> {{ t("save") }} </span>
                  <span class="indicator-progress">
                    {{ t("pleaseWait") }}
                    <span
                      class="spinner-border spinner-border-sm align-middle ms-2"
                    ></span>
                  </span>
                </button>
              </div>
              <!--end::Actions-->
            </VForm>
            <!--end::Form-->
          </div>
          <!--end::Options-->

          <!--begin::SMS-->
          <div :class="[state !== 'sms' && 'd-none']" data-kt-element="sms">
            <!--begin::Heading-->
            <h3 class="text-gray-900 fw-bold fs-3 mb-5">
              {{ t("smsCodeHeading") }}
            </h3>
            <!--end::Heading-->

            <!--begin::Notice-->
            <div class="text-gray-500 fw-semibold mb-10">
              {{ t("smsCodeDesc") }}
            </div>
            <!--end::Notice-->

            <!--begin::Form-->
            <VForm
              class="form"
              @submit="submitMobileForm()"
              :validation-schema="schema1"
            >
              <!--begin::Input group-->
              <div class="mb-10 fv-row">
                <Field
                  type="text"
                  class="form-control form-control-lg form-control-solid"
                  placeholder="Mobile number with country code..."
                  name="mobile"
                />
                <div class="fv-plugins-message-container">
                  <div class="fv-help-block">
                    <ErrorMessage name="mobile" />
                  </div>
                </div>
              </div>
              <!--end::Input group-->

              <!--begin::Actions-->
              <div class="d-flex flex-center">
                <button @click="state = ''" class="btn btn-light me-3">
                  {{ t("cancel") }}
                </button>

                <button
                  ref="submitMobileButtonRef"
                  type="submit"
                  data-kt-element="sms-submit"
                  class="btn btn-primary"
                >
                  <span class="indicator-label"> {{ t("save") }} </span>
                  <span class="indicator-progress">
                    {{ t("pleaseWait") }}
                    <span
                      class="spinner-border spinner-border-sm align-middle ms-2"
                    ></span>
                  </span>
                </button>
              </div>
              <!--end::Actions-->
            </VForm>
            <!--end::Form-->
          </div>
          <!--end::SMS-->
        </div>
        <!--begin::Modal body-->
      </div>
      <!--end::Modal content-->
    </div>
    <!--end::Modal header-->
  </div>
  <!--end::Modal - Two-factor authentication-->
</template>

<script lang="ts">
import { getAssetPath } from "@/core/helpers/assets";
import { defineComponent, ref } from "vue";
import * as Yup from "yup";
import { ErrorMessage, Field, Form as VForm } from "vee-validate";
import Swal from "sweetalert2/dist/sweetalert2.js";
import { useI18n } from "vue-i18n";

export default defineComponent({
  name: "two-factor-auth-modal",
  components: {
    ErrorMessage,
    Field,
    VForm,
  },
  setup() {
    const { t } = useI18n();
    const value = ref("apps");

    const state = ref("");

    const submitAuthCodeButtonRef = ref<null | HTMLButtonElement>(null);
    const submitMobileButtonRef = ref<null | HTMLButtonElement>(null);

    const schema1 = Yup.object().shape({
      mobile: Yup.string().required().label("Mobile"),
    });

    const schema2 = Yup.object().shape({
      code: Yup.string().required().label("Code"),
    });

    const submitAuthCodeForm = () => {
      if (submitAuthCodeButtonRef.value) {
        // Activate indicator
        submitAuthCodeButtonRef.value.setAttribute("data-kt-indicator", "on");

        setTimeout(() => {
          submitAuthCodeButtonRef.value?.removeAttribute("data-kt-indicator");

          Swal.fire({
            text: "Form has been successfully submitted!",
            icon: "success",
            buttonsStyling: false,
            confirmButtonText: "Ok, got it!",
            heightAuto: false,
            customClass: {
              confirmButton: "btn btn-primary",
            },
          }).then(() => {
            state.value = "";
          });
        }, 2000);
      }
    };

    const submitMobileForm = () => {
      if (!submitMobileButtonRef.value) {
        return;
      }

      //Disable button
      submitMobileButtonRef.value.disabled = true;
      // Activate indicator
      submitMobileButtonRef.value.setAttribute("data-kt-indicator", "on");

      setTimeout(() => {
        if (submitMobileButtonRef.value) {
          submitMobileButtonRef.value.disabled = false;

          submitMobileButtonRef.value?.removeAttribute("data-kt-indicator");
        }

        Swal.fire({
          text: "Form has been successfully submitted!",
          icon: "success",
          buttonsStyling: false,
          confirmButtonText: "Ok, got it!",
          heightAuto: false,
          customClass: {
            confirmButton: "btn btn-primary",
          },
        }).then(() => {
          state.value = "";
        });
      }, 2000);
    };

    return {
      value,
      state,
      schema1,
      schema2,
      submitAuthCodeForm,
      submitMobileForm,
      submitAuthCodeButtonRef,
      submitMobileButtonRef,
      getAssetPath,
      t,
    };
  },
});
</script>
