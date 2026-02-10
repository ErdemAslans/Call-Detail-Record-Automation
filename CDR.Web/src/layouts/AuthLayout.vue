<template>
  <!--begin::Authentication Layout -->
  <div class="d-flex flex-column flex-lg-row flex-column-fluid">
    <!--begin::Body-->
    <div
      class="d-flex flex-column flex-lg-row-fluid w-lg-50 p-10 order-2 order-lg-1"
    >
      <!--begin::Form-->
      <div class="d-flex flex-center flex-column flex-lg-row-fluid">
        <!--begin::Wrapper-->
        <div class="w-lg-500px p-10">
          <router-view></router-view>
        </div>
        <!--end::Wrapper-->
      </div>
      <!--end::Form-->

      <!--begin::Footer-->
      <div class="d-flex flex-center flex-wrap px-5">
        <!--begin::Links-->
        <div class="d-flex fw-semibold text-primary fs-base">
          <a href="#" class="px-5" target="_blank">{{ translate("terms") }}</a>

          <a href="#" class="px-5" target="_blank">{{ translate("plans") }}</a>

          <a href="#" class="px-5" target="_blank">{{ translate("contactUs") }}</a>
        </div>
        <!--end::Links-->
      </div>
      <!--end::Footer-->
    </div>
    <!--end::Body-->

    <!--begin::Aside-->
    <div
      class="d-flex flex-lg-row-fluid w-lg-50 bgi-size-cover bgi-position-center order-1 order-lg-2"
      :style="`background-image: url('${getAssetPath(
        '/media/misc/auth-bg.png',
      )}')`"
    >
      <!--begin::Content-->
      <div
        class="d-flex flex-column flex-center py-7 py-lg-15 px-5 px-md-15 w-100"
      >
        <!--begin::Logo-->
        <router-link to="/" class="mb-0 mb-lg-12">
          <img
            alt="Logo"
            :src="getAssetPath('media/logos/Dogus Otomotiv-01.png')"
            class="h-60px h-lg-75px"
          />
        </router-link>
        <!--end::Logo-->

        <!--begin::Image-->
        <img
          class="d-none d-lg-block mx-auto w-275px w-md-50 w-xl-500px mb-10 mb-lg-20"
          :src="getAssetPath('media/svg/illustrations/sigma/volume-1.svg')"
          alt=""
        />
        <!--end::Image-->

        <!--begin::Title-->
        <h1 class="d-none d-lg-block fs-2qx fw-bolder text-center mb-7">
          {{ translate("fastEfficientProductive") }}
        </h1>
        <!--end::Title-->

        <!--begin::Text-->
        <div class="d-none d-lg-block fs-base text-center">
          {{ translate("bloggerIntroduction") }}
        </div>
        <!--end::Text-->
      </div>
      <!--end::Content-->
    </div>
    <!--end::Aside-->
  </div>
  <!--end::Authentication Layout -->
</template>

<script lang="ts">
import { getAssetPath } from "@/core/helpers/assets";
import { defineComponent, onMounted } from "vue";
import LayoutService from "@/core/services/LayoutService";
import { useBodyStore } from "@/stores/body";
import { useI18n } from "vue-i18n";

export default defineComponent({
  name: "auth-layout",
  components: {},
  setup() {
    const store = useBodyStore();
    const { t, te } = useI18n();
    
    const translate = (text: string) => {
      if (te(text)) {
        return t(text);
      } else {
        return text;
      }
    };

    onMounted(() => {
      LayoutService.emptyElementClassesAndAttributes(document.body);

      store.addBodyClassname("app-blank");
      store.addBodyClassname("bg-body");
    });

    return {
      getAssetPath,
      translate,
    };
  },
});
</script>
