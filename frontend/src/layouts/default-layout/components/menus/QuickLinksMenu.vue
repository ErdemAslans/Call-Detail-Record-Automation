<template>
  <!--begin::Menu-->
  <div
    class="menu menu-sub menu-sub-dropdown menu-column w-250px w-lg-325px"
    data-kt-menu="true"
  >
    <!--begin::Heading-->
    <div
      class="d-flex flex-column flex-center bgi-no-repeat rounded-top px-9 py-10"
      :style="{
        backgroundImage: `url(${getAssetPath(
          '/media/misc/menu-header-bg.jpg'
        )})`,
      }"
    >
      <h3 class="text-white fw-semibold mb-3">{{ $t("quickActions") }}</h3>
    </div>
    <!--end::Heading-->

    <!--begin:Nav-->
    <div class="row g-0">
      <!--begin:Item-->
      <div class="col-6">
        <router-link
          to="/dashboard"
          class="d-flex flex-column flex-center h-100 p-6 bg-hover-light border-end border-bottom"
        >
          <KTIcon icon-name="element-11" icon-class="fs-3x text-primary mb-2" />
          <span class="fs-5 fw-semibold text-gray-800 mb-0">{{ $t("dashboard") }}</span>
          <span class="fs-7 text-gray-500">CDR</span>
        </router-link>
      </div>
      <!--end:Item-->

      <!--begin:Item-->
      <div class="col-6">
        <router-link
          to="/email-reports"
          class="d-flex flex-column flex-center h-100 p-6 bg-hover-light border-bottom"
        >
          <KTIcon icon-name="sms" icon-class="fs-3x text-success mb-2" />
          <span class="fs-5 fw-semibold text-gray-800 mb-0">{{ $t("generateReport") }}</span>
          <span class="fs-7 text-gray-500">Excel / E-posta</span>
        </router-link>
      </div>
      <!--end:Item-->

      <!--begin:Item-->
      <div class="col-6">
        <router-link
          to="/breaks"
          class="d-flex flex-column flex-center h-100 p-6 bg-hover-light border-end"
        >
          <KTIcon icon-name="coffee" icon-class="fs-3x text-warning mb-2" />
          <span class="fs-5 fw-semibold text-gray-800 mb-0">{{ $t("breakStatus") }}</span>
          <span class="fs-7 text-gray-500">{{ $t("breaks") }}</span>
        </router-link>
      </div>
      <!--end:Item-->

      <!--begin:Item-->
      <div class="col-6">
        <router-link
          to="/dashboard"
          class="d-flex flex-column flex-center h-100 p-6 bg-hover-light"
        >
          <KTIcon icon-name="chart-simple" icon-class="fs-3x text-info mb-2" />
          <span class="fs-5 fw-semibold text-gray-800 mb-0">{{ $t("departmentBreakdown") }}</span>
          <span class="fs-7 text-gray-500">{{ $t("statistics") }}</span>
        </router-link>
      </div>
      <!--end:Item-->
    </div>
    <!--end:Nav-->
  </div>
  <!--end::Menu-->
</template>

<script lang="ts">
import { getAssetPath } from "@/core/helpers/assets";
import { defineComponent } from "vue";

export default defineComponent({
  name: "kt-quick-links-menu",
  components: {},
  setup() {
    return {
      getAssetPath,
    };
  },
});
</script>
