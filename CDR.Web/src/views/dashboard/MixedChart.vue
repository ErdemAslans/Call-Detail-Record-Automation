<template>
  <!--begin::Mixed Widget 11-->
  <div :class="widgetClasses" class="card">
    <!--begin::Header-->
    <div :class="`bg-${widgetColor}`" class="card-header border-0 py-5">
      <h3 class="card-title fw-bold text-white">
        {{ translate("salesDailyReport") }}
      </h3>
    </div>
    <!--end::Header-->

    <!--begin::Body-->
    <div class="card-body p-0 position-relative">
      <!-- Loading Overlay (T027) -->
      <div v-if="isLoading" class="position-absolute top-0 start-0 w-100 h-100 bg-white bg-opacity-75 d-flex align-items-center justify-content-center" style="z-index: 10;">
        <div class="spinner-border text-primary" role="status">
          <span class="visually-hidden">Loading...</span>
        </div>
      </div>

      <!-- Empty State (T029) -->
      <div v-else-if="!isLoading && dailyCallsReport && Object.keys(dailyCallsReport).length === 0" class="d-flex align-items-center justify-content-center" style="height: 300px;">
        <EmptyState 
          title="Veri Bulunamadı" 
          description="İstatistik verisi mevcut değil"
        />
      </div>

      <!--begin::Chart-->
      <apexchart
        v-else
        ref="chartRef"
        :class="`bg-${widgetColor}`"
        class="mixed-widget-12-chart card-rounded-bottom"
        :options="chart"
        :series="series"
        height="200"
        type="bar"
      ></apexchart>
      <!--end::Chart-->

      <!--begin::Stats-->
      <div class="card-rounded bg-body position-relative card-px py-15">
        <!--begin::Row-->
        <div class="row g-0 mb-7">
          <!--begin::Col-->
          <div class="col mx-5">
            <div class="fs-6 text-gray-500">
              {{ translate("incomingCallcount") }}
            </div>
            <div class="fs-2 fw-bold text-gray-800">
              {{ dailyCallsReport.totalCalls }}
            </div>
          </div>
          <!--end::Col-->

          <!--begin::Col-->
          <div class="col mx-5">
            <div class="fs-6 text-gray-500">
              {{ translate("answeredCallCount") }}
            </div>
            <div class="fs-2 fw-bold text-gray-800">
              {{ dailyCallsReport.answeredCalls }}
            </div>
          </div>
          <!--end::Col-->
        </div>
        <!--end::Row-->

        <!--begin::Row-->
        <div class="row g-0 mb-7">
          <!--begin::Col-->
          <div class="col mx-5">
            <div class="fs-6 text-gray-500">
              {{ translate("missedCallCount") }}
            </div>
            <div class="fs-2 fw-bold text-gray-800">
              {{ dailyCallsReport.missedCalls }}
            </div>
          </div>
          <!--end::Col-->

          <!--begin::Col-->
          <div class="col mx-5">
            <div class="fs-6 text-gray-500">{{ translate("closedAtIVR") }}</div>
            <div class="fs-2 fw-bold text-gray-800">
              {{ dailyCallsReport.closedAtIVR }}
            </div>
          </div>
          <!--end::Col-->
        </div>
        <!--end::Row-->

        <!--begin::Row-->
        <div class="row g-0">
          <!--begin::Col-->
          <div class="col mx-5">
            <div class="fs-6 text-gray-500">
              {{ translate("answeredCallRate") }}
            </div>
            <div class="fs-2 fw-bold text-gray-800">
              {{ dailyCallsReport.answeredCallRate }}%
            </div>
          </div>
          <!--end::Col-->

          <!--begin::Col-->
          <div class="col mx-5">
            <div class="fs-6 text-gray-500">
              {{ translate("missedcallbackCallCountIn48Hours") }}
            </div>
            <div class="fs-2 fw-bold text-gray-800">
              {{ dailyCallsReport.missedCallbackCalls }}
            </div>
          </div>
          <!--end::Col-->
        </div>
        <!--end::Row-->
      </div>
      <!--end::Stats-->
    </div>
    <!--end::Body-->
  </div>
  <!--end::Mixed Widget 11-->
</template>

<script lang="ts">
import { getAssetPath } from "@/core/helpers/assets";
import { computed, defineComponent, onBeforeMount, ref, watch } from "vue";
import { getCSSVariableValue } from "@/assets/ts/_utils";
import type VueApexCharts from "vue3-apexcharts";
import type { ApexOptions } from "apexcharts";
import { useThemeStore } from "@/stores/theme";
import { useI18n } from "vue-i18n";
import { useDashboardStore } from "@/stores/dashboard";
import EmptyState from "@/components/common/EmptyState.vue";

export default defineComponent({
  name: "widget-12",
  props: {
    widgetClasses: String,
    widgetColor: String,
    chartHeight: String,
  },
  components: {
    EmptyState,
  },
  setup(props) {
    const chartRef = ref<typeof VueApexCharts | null>(null);
    const chart = ref<ApexOptions>({});
    const store = useThemeStore();
    const dashboardStore = useDashboardStore();
    const dailyCallsReport = computed(() => dashboardStore.dailyCallReport);
    const { t, te } = useI18n();
    const translate = (text: string) => {
      if (te(text)) {
        return t(text);
      } else {
        return text;
      }
    };

    const series = ref([
      {
        name: translate("incomingCallCount"),
        data: [] as number[],
      },
      {
        name: translate("answeredCallCount"),
        data: [] as number[],
      },
      {
        name: translate("missedCallCount"),
        data: [] as number[],
      },
      {
        name: translate("closedAtIVR"),
        data: [] as number[],
      },
      {
        name: translate("missedcallbackCallCountIn48Hours"),
        data: [] as number[],
      },
    ]);

    const isLoading = ref(false);

    const fetchLocationStatistics = async () => {
      isLoading.value = true;
      try {
        // Placeholder for location statistics - this is chart initialization
        // The chart data will be populated when API endpoint is available
        Object.assign(chart.value, chartOptions(props.chartHeight));
      } catch (error) {
        console.error("Error fetching location statistics:", error);
      } finally {
        isLoading.value = false;
      }
    };

    const themeMode = computed(() => {
      return store.mode;
    });

    const fetchDailyCallReport = async () => {
      await dashboardStore.fetchDailyCallReport();
    };

    onBeforeMount(() => {
      fetchDailyCallReport();
      fetchLocationStatistics();
      Object.assign(chart.value, chartOptions(props.chartHeight));
    });

    const refreshChart = () => {
      if (!chartRef.value) {
        return;
      }

      chartRef.value.updateOptions(chartOptions(props.chartHeight));
    };

    watch(themeMode, () => {
      refreshChart();
    });

    return {
      chart,
      series,
      chartRef,
      dailyCallsReport,
      refreshChart,
      getAssetPath,
      translate,
      isLoading,
    };
  },
});

const chartOptions = (chartHeight: string = "auto", categories: string[] = ["Ankara", "Esenyurt", "Kartal", "Maslak"]): ApexOptions => {
  const labelColor = getCSSVariableValue("--bs-gray-500");
  const borderColor = getCSSVariableValue("--bs-gray-200");

  const baseColor = getCSSVariableValue("--bs-danger");
  const baseLightColor = getCSSVariableValue("--bs-danger-light");
  const secondaryColor = getCSSVariableValue("--bs-info");
  const secondaryInfoColor = getCSSVariableValue("--bs-info--light");

  return {
    chart: {
      fontFamily: "inherit",
      type: "bar",
      height: chartHeight,
      toolbar: {
        show: false,
      },
      sparkline: {
        enabled: true,
      },
    },
    plotOptions: {
      bar: {
        horizontal: false,
        columnWidth: "60%",
      },
    },
    legend: {
      show: true,
      position: "top",
    },
    dataLabels: {
      enabled: false,
    },
    stroke: {
      show: true,
      width: 1,
      colors: ["transparent"],
    },
    xaxis: {
      categories: categories,
      axisBorder: {
        show: false,
      },
      axisTicks: {
        show: false,
      },
      labels: {
        style: {
          colors: labelColor,
          fontSize: "12px",
        },
      },
    },
    yaxis: {
      min: 0,
      max: 400,
      labels: {
        style: {
          colors: labelColor,
          fontSize: "12px",
        },
      },
    },
    fill: {
      type: ["solid", "solid"],
      opacity: [0.25, 1],
    },
    states: {
      normal: {
        filter: {
          type: "none",
          value: 0,
        },
      },
      hover: {
        filter: {
          type: "none",
          value: 0,
        },
      },
      active: {
        allowMultipleDataPointsSelection: false,
        filter: {
          type: "none",
          value: 0,
        },
      },
    },
    tooltip: {
      style: {
        fontSize: "12px",
      },
      y: {
        formatter: function (val) {
          return val.toString();
        },
      },
      marker: {
        show: false,
      },
    },
    colors: [baseColor, secondaryColor, baseLightColor, secondaryInfoColor],
    grid: {
      borderColor: borderColor,
      strokeDashArray: 4,
      yaxis: {
        lines: {
          show: true,
        },
      },
      padding: {
        left: 20,
        right: 20,
      },
    },
  };
};
</script>
