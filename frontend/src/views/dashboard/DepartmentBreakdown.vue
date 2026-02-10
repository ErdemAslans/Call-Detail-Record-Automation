<template>
  <div :class="widgetClasses" class="card">
    <!--begin::Header-->
    <div class="card-header border-0 pt-5">
      <h3 class="card-title align-items-start flex-column">
        <span class="card-label fw-bold fs-3 mb-1">{{ translate("departmentBreakdown") }}</span>
        <span class="text-muted fw-semibold fs-7">{{ periodBadge }}</span>
      </h3>

      <div class="card-toolbar d-flex align-items-center gap-2" data-kt-buttons="true">
        <a
          class="btn btn-sm btn-color-muted btn-active btn-active-primary px-4 me-1"
          :class="{ active: selectedPeriod === 'today' }"
          @click="changePeriod('today')"
        >{{ translate("daily") }}</a>
        <a
          class="btn btn-sm btn-color-muted btn-active btn-active-primary px-4 me-1"
          :class="{ active: selectedPeriod === 'week' }"
          @click="changePeriod('week')"
        >{{ translate("weekly") }}</a>
        <a
          class="btn btn-sm btn-color-muted btn-active btn-active-primary px-4 me-1"
          :class="{ active: selectedPeriod === 'month' }"
          @click="changePeriod('month')"
        >{{ translate("monthly") }}</a>

        <button
          class="btn btn-sm btn-light-success"
          @click="exportExcel"
          :disabled="isExporting"
        >
          <KTIcon icon-name="file-down" icon-class="fs-4 me-1" />
          {{ translate("exportExcel") }}
        </button>
      </div>
    </div>
    <!--end::Header-->

    <!--begin::Body-->
    <div class="card-body py-3">
      <!--begin::Loading-->
      <div v-if="isLoading" class="d-flex align-items-center justify-content-center py-10">
        <div class="spinner-border text-primary" role="status">
          <span class="visually-hidden">Loading...</span>
        </div>
      </div>
      <!--end::Loading-->

      <!--begin::Empty-->
      <div v-else-if="departments.length === 0" class="text-center py-10">
        <KTIcon icon-name="abstract-26" icon-class="fs-3x text-gray-400 mb-3" />
        <p class="text-muted fs-6">{{ translate("noDepartmentData") }}</p>
      </div>
      <!--end::Empty-->

      <!--begin::Table-->
      <div v-else class="table-responsive">
        <table class="table table-row-dashed table-row-gray-200 align-middle gs-0 gy-4">
          <thead>
            <tr class="border-0">
              <th class="p-0 min-w-200px">
                <span class="text-muted fw-semibold fs-7">{{ translate("department") }}</span>
              </th>
              <th class="p-0 min-w-80px text-center">
                <span class="text-muted fw-semibold fs-7">{{ translate("departmentTotalCalls") }}</span>
              </th>
              <th class="p-0 min-w-80px text-center">
                <span class="text-muted fw-semibold fs-7">{{ translate("departmentAnsweredCalls") }}</span>
              </th>
              <th class="p-0 min-w-80px text-center">
                <span class="text-muted fw-semibold fs-7">{{ translate("departmentMissedCalls") }}</span>
              </th>
              <th class="p-0 min-w-80px text-center">
                <span class="text-muted fw-semibold fs-7">{{ translate("departmentOnBreakCalls") }}</span>
              </th>
              <th class="p-0 min-w-100px text-center">
                <span class="text-muted fw-semibold fs-7">{{ translate("departmentAnswerRate") }}</span>
              </th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="dept in departments" :key="dept.departmentName">
              <td>
                <span class="text-gray-800 fw-bold fs-6">{{ dept.departmentName }}</span>
              </td>
              <td class="text-center">
                <span class="badge badge-light-primary fs-7 fw-bold">{{ dept.totalCalls }}</span>
              </td>
              <td class="text-center">
                <span class="badge badge-light-success fs-7 fw-bold">{{ dept.answeredCalls }}</span>
              </td>
              <td class="text-center">
                <span class="badge badge-light-danger fs-7 fw-bold">{{ dept.missedCalls }}</span>
              </td>
              <td class="text-center">
                <span class="badge badge-light-warning fs-7 fw-bold">{{ dept.onBreakCalls ?? 0 }}</span>
              </td>
              <td class="text-center">
                <div class="d-flex align-items-center justify-content-center">
                  <span class="fw-bold fs-6 me-2" :class="getRateColor(dept.answeredCallRate)">
                    %{{ dept.answeredCallRate }}
                  </span>
                  <div class="progress h-6px w-50px">
                    <div
                      class="progress-bar"
                      :class="getRateBarColor(dept.answeredCallRate)"
                      :style="{ width: dept.answeredCallRate + '%' }"
                    ></div>
                  </div>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
      <!--end::Table-->
    </div>
    <!--end::Body-->
  </div>
</template>

<script lang="ts">
import { computed, defineComponent, onMounted, ref, watch } from "vue";
import { useDashboardStore } from "@/stores/dashboard";
import { useI18n } from "vue-i18n";
import DateHelper from "@/core/helpers/DateHelper";

interface DepartmentStat {
  departmentName: string;
  totalCalls: number;
  answeredCalls: number;
  missedCalls: number;
  onBreakCalls: number;
  answeredCallRate: number;
}

export default defineComponent({
  name: "department-breakdown",
  props: {
    widgetClasses: String,
  },
  setup() {
    const dashboardStore = useDashboardStore();
    const { t, te } = useI18n();
    const translate = (text: string) => (te(text) ? t(text) : text);

    const selectedPeriod = ref<"today" | "week" | "month">("today");
    const departments = ref<DepartmentStat[]>([]);
    const isLoading = ref(false);
    const isExporting = ref(false);

    const dateRange = computed(() => DateHelper.getDateRange(selectedPeriod.value));

    const periodBadge = computed(() => {
      const start = new Date(dateRange.value.start);
      const end = new Date(dateRange.value.end);
      const opts: Intl.DateTimeFormatOptions = {
        day: "numeric",
        month: "long",
        year: "numeric",
        timeZone: "Europe/Istanbul",
      };
      if (selectedPeriod.value === "today") {
        return start.toLocaleDateString("tr-TR", opts);
      }
      return `${start.toLocaleDateString("tr-TR", { day: "numeric", month: "long", timeZone: "Europe/Istanbul" })} - ${end.toLocaleDateString("tr-TR", opts)}`;
    });

    const fetchData = async () => {
      isLoading.value = true;
      try {
        const data = await dashboardStore.fetchDepartmentStatistics({
          startDate: dateRange.value.start,
          endDate: dateRange.value.end,
        });
        // Backend returns { incoming: [...], outgoing: [...], internal: [...] }
        // Use incoming calls for department breakdown (most relevant)
        departments.value = data?.incoming ?? [];
      } finally {
        isLoading.value = false;
      }
    };

    const changePeriod = (period: "today" | "week" | "month") => {
      selectedPeriod.value = period;
    };

    const exportExcel = async () => {
      isExporting.value = true;
      try {
        await dashboardStore.downloadDepartmentExcel({
          startDate: dateRange.value.start,
          endDate: dateRange.value.end,
        });
      } finally {
        isExporting.value = false;
      }
    };

    const getRateColor = (rate: number) => {
      if (rate >= 80) return "text-success";
      if (rate >= 60) return "text-warning";
      return "text-danger";
    };

    const getRateBarColor = (rate: number) => {
      if (rate >= 80) return "bg-success";
      if (rate >= 60) return "bg-warning";
      return "bg-danger";
    };

    onMounted(fetchData);
    watch(selectedPeriod, fetchData);

    return {
      departments,
      selectedPeriod,
      isLoading,
      isExporting,
      periodBadge,
      translate,
      changePeriod,
      exportExcel,
      getRateColor,
      getRateBarColor,
    };
  },
});
</script>
