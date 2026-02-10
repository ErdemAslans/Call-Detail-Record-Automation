<template>
  <!--begin::Charts Widget 1-->
  <div class="card" :class="widgetClasses">
    <!--begin::Header-->
    <div class="card-header border-0 pt-5">
      <!--begin::Title-->
      <h3 class="card-title align-items-start flex-column">
        <span class="card-label fw-bold fs-3 mb-1">{{
          translate("incomingCallRate")
        }}</span>

        <span class="text-muted fw-semibold fs-7"
          >More than {{ seriesSum }} call</span
        >
      </h3>
      <!--end::Title-->
    </div>
    <!--end::Header-->

    <!--begin::Body-->
    <div class="card-body">
      <!--begin::Chart-->
      <apexchart
        ref="chartRef"
        type="pie"
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

export default defineComponent({
  name: "widget-1",
  props: {
    widgetClasses: String,
    height: Number,
  },
  components: {},
  setup() {
    const chartRef = ref<typeof VueApexCharts | null>(null);
    const chart = ref<ApexOptions>({});
    const store = useThemeStore();
    const { t, te } = useI18n();
    const translate = (text: string) => {
      if (te(text)) {
        return t(text);
      } else {
        return text;
      }
    };

    const series = [10, 3];

    const seriesSum = series.reduce((acc, item) => {
      return acc + item;
    }, 0);

    const themeMode = computed(() => {
      return store.mode;
    });

    onBeforeMount(() => {
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

    return {
      chart,
      series,
      chartRef,
      getAssetPath,
      translate,
      seriesSum,
    };
  },
});

const chartOptions = (): ApexOptions => {
  const baseColor = getCSSVariableValue("--bs-success");
  const secondaryColor = getCSSVariableValue("--bs-info");

  return {
    chart: {
      fontFamily: "inherit",
      type: "pie",
      toolbar: {
        show: true,
      },
    },
    plotOptions: {
      pie: {
        expandOnClick: false,
        donut: {
          size: "70%",
        },
      },
    },
    legend: {
      show: true,
      position: "bottom",
    },
    dataLabels: {
      enabled: false,
    },
    stroke: {
      show: true,
      width: 2,
      colors: ["transparent"],
    },
    labels: ["Satış Danışmanı", "Satış Yetkilisi"],
    tooltip: {
      style: {
        fontSize: "12px",
      },
      y: {
        formatter: function (val) {
          return val?.toString();
        },
      },
    },
    colors: [baseColor, secondaryColor],
  };
};
</script>
