<template>
  <!--begin::Charts Widget 1-->
  <div class="card" :class="widgetClasses">
    <!--begin::Header-->
    <div class="card-header border-0 pt-5">
      <!--begin::Title-->
      <h3 class="card-title align-items-start flex-column">
        <span class="card-label fw-bold fs-3 mb-1">{{
          translate("answeredCallRate")
        }}</span>

        <span class="text-muted fw-semibold fs-7">{{
          translate("answeredCallRate")
        }}</span>
      </h3>
      <!--end::Title-->

      <!--begin::Toolbar-->
      <div class="card-toolbar" data-kt-buttons="true">
        <a
          class="btn btn-sm btn-color-muted btn-active btn-active-primary px-4 me-1"
          :class="{ active: reportPeriod === ReportPeriod.YEARLY }"
          id="kt_charts_widget_2_year_btn"
          @click="reportPeriod = ReportPeriod.YEARLY"
          >{{ translate("year") }}</a
        >

        <a
          class="btn btn-sm btn-color-muted btn-active btn-active-primary px-4 me-1"
          :class="{ active: reportPeriod === ReportPeriod.MONTHLY }"
          id="kt_charts_widget_2_month_btn"
          @click="reportPeriod = ReportPeriod.MONTHLY"
          >{{ translate("month") }}</a
        >

        <a
          class="btn btn-sm btn-color-muted btn-active btn-active-primary px-4"
          :class="{ active: reportPeriod === ReportPeriod.WEEKLY }"
          id="kt_charts_widget_2_week_btn"
          @click="reportPeriod = ReportPeriod.WEEKLY"
          >{{ translate("week") }}</a
        >
      </div>
      <!--end::Toolbar-->
    </div>
    <!--end::Header-->

    <!--begin::Body-->
    <div class="card-body">
      <!--begin::Chart-->
      <apexchart
        ref="chartRef"
        type="bar"
        :options="chart"
        :series="series"
        :height="height"
      ></apexchart>
      <!--end::Chart-->
    </div>
    <!--end::Body-->
  </div>
  <!--end::Charts Widget 1-->
</template>

<script lang="ts">
import { getAssetPath } from "@/core/helpers/assets";
import { computed, defineComponent, onBeforeMount, ref, watch } from "vue";
import { useThemeStore } from "@/stores/theme";
import type { ApexOptions } from "apexcharts";
import { getCSSVariableValue } from "@/assets/ts/_utils";
import type VueApexCharts from "vue3-apexcharts";
import { useI18n } from "vue-i18n";
import { useDashboardStore } from "@/stores/dashboard";
import ReportPeriod from "@/core/enums/ReportPeriod";

interface Series {
  name: string;
  data: number[];
  labels: string[];
}

export default defineComponent({
  name: "answeredCallRatechart",
  props: {
    widgetClasses: String,
    height: Number,
  },
  components: {},
  setup() {
    const chartRef = ref<typeof VueApexCharts | null>(null);
    const series = ref<Series[]>([]);
    const chart = ref<ApexOptions>({});
    const store = useThemeStore();
    const reportPeriod = ref<ReportPeriod>(ReportPeriod.WEEKLY);
    const dashboardStore = useDashboardStore();
    const { t, te } = useI18n();
    const translate = (text: string) => {
      if (te(text)) {
        return t(text);
      } else {
        return text;
      }
    };

    const themeMode = computed(() => {
      return store.mode;
    });

    const fetchAnserweredCalls = async (period: number) => {
      // Fetch weekly data
      const data = await dashboardStore.fetchWeeklyAnsweredCalls(period);
      console.log("data", data);

      const error = Object.values(dashboardStore.errors);

      if (error.length === 0) {
        series.value = data.series;
        chart.value = {
          ...chartOptions,
          xaxis: {
            categories: data.labels,
          },
        };
        refreshChart();
      }
    };

    onBeforeMount(() => {
      fetchAnserweredCalls(reportPeriod.value);
      Object.assign(chart.value, chartOptions());
    });

    const refreshChart = () => {
      if (!chartRef.value) {
        return;
      }

      chartRef.value.updateOptions(chartOptions());
    };

    watch(themeMode, () => {
      refreshChart();
    });

    watch(reportPeriod, () => {
      fetchAnserweredCalls(reportPeriod.value);
    });

    return {
      chart,
      series,
      chartRef,
      getAssetPath,
      translate,
      reportPeriod,
      ReportPeriod,
    };
  },
});

const chartOptions = (): ApexOptions => {
  const labelColor = getCSSVariableValue("--bs-gray-500");
  const borderColor = getCSSVariableValue("--bs-gray-200");
  const baseColor = getCSSVariableValue("--bs-primary");
  const secondaryColor = getCSSVariableValue("--bs-gray-300");

  return {
    chart: {
      fontFamily: "inherit",
      type: "bar",
      toolbar: {
        show: true,
      },
    },
    plotOptions: {
      bar: {
        horizontal: false,
        borderRadius: 5,
      },
    },
    legend: {
      show: true,
    },
    dataLabels: {
      enabled: false,
    },
    stroke: {
      show: true,
      width: 2,
      colors: ["transparent"],
    },
    xaxis: {
      // categories: ["Feb", "Mar", "Apr", "May", "Jun", "Jul"],
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
      labels: {
        style: {
          colors: labelColor,
          fontSize: "12px",
        },
      },
    },
    fill: {
      opacity: 1,
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
          return "%" + val;
        },
      },
    },
    colors: [baseColor, secondaryColor],
    grid: {
      borderColor: borderColor,
      strokeDashArray: 4,
      yaxis: {
        lines: {
          show: true,
        },
      },
    },
  };
};
</script>
