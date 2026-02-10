<template>
  <!--begin::Row 1: Daily Summary Cards-->
  <DailySummaryCards />
  <!--end::Row 1-->

  <!--begin::Row 2: Charts-->
  <div class="row g-5 g-xl-8 mb-5 mb-xl-8">
    <div class="col-xl-6">
      <AnsweredCallRateChart
        widget-classes="card-xl-stretch mb-xl-8"
        :height="400"
      ></AnsweredCallRateChart>
    </div>
    <div class="col-xl-6">
      <DepartmentBreakdown
        widget-classes="card-xl-stretch mb-xl-8"
      ></DepartmentBreakdown>
    </div>
  </div>
  <!--end::Row 2-->

  <!--begin::Row 3: Central Statistics-->
  <Profile title="centralStatistics" :operator-number="`80369090`"></Profile>
  <!--end::Row 3-->

  <!--begin::Row 4: Call Records-->
  <div class="row g-5 g-xl-10 mb-5 mb-xl-10">
    <div class="col-xl-12">
      <CdrList></CdrList>
    </div>
  </div>
  <!--end::Row 4-->
</template>

<script lang="ts">
import { getAssetPath } from "@/core/helpers/assets";
import { defineComponent } from "vue";
import AnsweredCallRateChart from "./dashboard/AnsweredCallRateChart.vue";
import DailySummaryCards from "./dashboard/DailySummaryCards.vue";
import DepartmentBreakdown from "./dashboard/DepartmentBreakdown.vue";
import CdrList from "./dashboard/CdrList.vue";
import Profile from "./userProfile/Profile.vue";

export default defineComponent({
  name: "main-dashboard",
  components: {
    AnsweredCallRateChart,
    DailySummaryCards,
    DepartmentBreakdown,
    CdrList,
    Profile,
  },
  setup() {
    return {
      getAssetPath,
    };
  },
});
</script>
