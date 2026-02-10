<template>
  <div class="row g-5 g-xl-8 mb-5 mb-xl-8">
    <!--begin::Today Label-->
    <div class="col-12">
      <div class="d-flex align-items-center">
        <h3 class="fw-bold text-gray-800 mb-0 me-3">{{ translate("todaySummary") }}</h3>
        <span class="badge badge-light-primary fs-7 fw-semibold">{{ todayLabel }}</span>
      </div>
    </div>
    <!--end::Today Label-->

    <!--begin::Card Gelen-->
    <div class="col">
      <div class="card card-flush h-100">
        <div class="card-body d-flex flex-column align-items-center justify-content-center py-5">
          <KTIcon icon-name="call" icon-class="fs-2x text-primary mb-2" />
          <div class="fs-2hx fw-bold text-gray-800">{{ dailyReport.totalCalls ?? 0 }}</div>
          <div class="fs-7 fw-semibold text-gray-500">{{ translate("totalCalls") }}</div>
        </div>
      </div>
    </div>
    <!--end::Card Gelen-->

    <!--begin::Card Cevaplanan-->
    <div class="col">
      <div class="card card-flush h-100">
        <div class="card-body d-flex flex-column align-items-center justify-content-center py-5">
          <KTIcon icon-name="call" icon-class="fs-2x text-success mb-2" />
          <div class="fs-2hx fw-bold text-success">{{ dailyReport.answeredCalls ?? 0 }}</div>
          <div class="fs-7 fw-semibold text-gray-500">{{ translate("answered") }}</div>
        </div>
      </div>
    </div>
    <!--end::Card Cevaplanan-->

    <!--begin::Card Cevapsiz-->
    <div class="col">
      <div class="card card-flush h-100">
        <div class="card-body d-flex flex-column align-items-center justify-content-center py-5">
          <KTIcon icon-name="disconnect" icon-class="fs-2x text-danger mb-2" />
          <div class="fs-2hx fw-bold text-danger">{{ dailyReport.missedCalls ?? 0 }}</div>
          <div class="fs-7 fw-semibold text-gray-500">{{ translate("missed") }}</div>
        </div>
      </div>
    </div>
    <!--end::Card Cevapsiz-->

    <!--begin::Card Molada Gelen-->
    <div class="col">
      <div class="card card-flush h-100">
        <div class="card-body d-flex flex-column align-items-center justify-content-center py-5">
          <KTIcon icon-name="coffee" icon-class="fs-2x text-warning mb-2" />
          <div class="fs-2hx fw-bold text-warning">{{ dailyReport.onBreakCalls ?? 0 }}</div>
          <div class="fs-7 fw-semibold text-gray-500">{{ translate("onBreakCalls") }}</div>
        </div>
      </div>
    </div>
    <!--end::Card Molada Gelen-->

    <!--begin::Card Oran-->
    <div class="col">
      <div class="card card-flush h-100">
        <div class="card-body d-flex flex-column align-items-center justify-content-center py-5">
          <KTIcon icon-name="percentage" icon-class="fs-2x text-info mb-2" />
          <div class="fs-2hx fw-bold text-info">%{{ dailyReport.answeredCallRate ?? 0 }}</div>
          <div class="fs-7 fw-semibold text-gray-500">{{ translate("answerRate") }}</div>
        </div>
      </div>
    </div>
    <!--end::Card Oran-->

    <!--begin::Card Süre-->
    <div class="col">
      <div class="card card-flush h-100">
        <div class="card-body d-flex flex-column align-items-center justify-content-center py-5">
          <KTIcon icon-name="timer" icon-class="fs-2x text-warning mb-2" />
          <div class="fs-2hx fw-bold text-warning">{{ formattedDuration }}</div>
          <div class="fs-7 fw-semibold text-gray-500">{{ translate("totalDuration") }}</div>
        </div>
      </div>
    </div>
    <!--end::Card Süre-->
  </div>
</template>

<script lang="ts">
import { computed, defineComponent, onMounted } from "vue";
import { useDashboardStore } from "@/stores/dashboard";
import { useI18n } from "vue-i18n";
import DateHelper from "@/core/helpers/DateHelper";

export default defineComponent({
  name: "daily-summary-cards",
  setup() {
    const dashboardStore = useDashboardStore();
    const { t, te } = useI18n();
    const translate = (text: string) => (te(text) ? t(text) : text);

    const dailyReport = computed(() => dashboardStore.dailyCallReport);

    const todayLabel = computed(() => {
      const now = new Date();
      return now.toLocaleDateString("tr-TR", {
        weekday: "long",
        year: "numeric",
        month: "long",
        day: "numeric",
        timeZone: "Europe/Istanbul",
      });
    });

    const formattedDuration = computed(() => {
      return DateHelper.formatDuration(dailyReport.value.totalDuration ?? 0);
    });

    onMounted(() => {
      dashboardStore.fetchDailyCallReport();
    });

    return {
      dailyReport,
      todayLabel,
      formattedDuration,
      translate,
    };
  },
});
</script>
