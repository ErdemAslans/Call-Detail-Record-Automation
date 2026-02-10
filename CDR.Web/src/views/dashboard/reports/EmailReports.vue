<template>
  <div>
    <!--begin::Row-->
    <div class="row g-5 g-xl-10 mb-5 mb-xl-10">
      <div class="col-12">
        <CdrReportGenerator />
      </div>
    </div>
    <!--end::Row-->

    <!--begin::Row-->
    <div class="row g-5 g-xl-10">
      <div class="col-12">
        <ReportExecutionHistory />
      </div>
    </div>
    <!--end::Row-->
  </div>
</template>

<script setup lang="ts">
import CdrReportGenerator from "@/components/reports/CdrReportGenerator.vue";
import ReportExecutionHistory from "@/components/reports/ReportExecutionHistory.vue";
</script>
