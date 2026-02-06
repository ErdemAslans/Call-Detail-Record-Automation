<template>
  <!--begin::Row-->
  <div class="row g-5 g-xl-8">
    <div class="col-xl-6">
      <AnsweredCallRateChart
        widget-classes="card-xl-stretch mb-xl-8"
        :height="400"
      ></AnsweredCallRateChart>
    </div>
    <!-- <div class="col-xl-4">
      <AnsweredSumCallRateChart
        widget-classes="card-xl-stretch mb-xl-8"
        :height="400"
      ></AnsweredSumCallRateChart>
    </div> -->

    <div class="col-xl-6">
      <MixedChart
        widget-classes="card-xl-stretch mb-xl-8"
        widget-color="warning"
        chart-height="10"
      ></MixedChart>
    </div>
  </div>
  <!--end::Row-->

  <!--begin::Row-->
  <div class="row g-5 g-xl-10 mb-5 mb-xl-10">
    <div class="col-xl-12">
      <CdrList></CdrList>
    </div>
  </div>
  <!--end::Row-->

  <!--begin::Row-->
  <Profile title="centralStatistics" :operator-number="`80369090`"></Profile>
  <!--end::Row-->
</template>

<script lang="ts">
import { getAssetPath } from "@/core/helpers/assets";
import { defineComponent } from "vue";
import AnsweredCallRateChart from "./dashboard/AnsweredCallRateChart.vue";
import CdrList from "./dashboard/CdrList.vue";
import MixedChart from "./dashboard/MixedChart.vue";
// import Reports from "./dashboard/reports/Reports.vue";
import Profile from "./userProfile/Profile.vue";

export default defineComponent({
  name: "main-dashboard",
  components: {
    AnsweredCallRateChart,
    CdrList,
    MixedChart,
    // Reports,
    Profile,
  },
  setup() {
    return {
      getAssetPath,
    };
  },
});
</script>
