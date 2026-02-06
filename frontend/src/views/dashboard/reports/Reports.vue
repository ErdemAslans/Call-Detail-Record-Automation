<template>
  <div class="row g-5 g-xl-10 mb-5 mb-xl-10">
    <div class="col-xl-6">
      <ReportTable widget-classes="card-xl-stretch mb-xl-8"></ReportTable>
    </div>
    <div class="col-xl-3">
      <ReportPieChart
        widget-classes="card-xl-stretch mb-xl-8"
        :height="400"
      ></ReportPieChart>
    </div>
    <div class="col-xl-3">
      <ReportBarChart
        widget-classes="card-xl-stretch mb-xl-8"
        :height="400"
      ></ReportBarChart>
    </div>
  </div>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import { useI18n } from "vue-i18n";
import { useThemeStore } from "@/stores/theme";
import Dropdown1 from "@/components/dropdown/Dropdown1.vue";
import ReportTable from "./ReportTable.vue";
import ReportPieChart from "./ReportPieChart.vue";
import ReportBarChart from "./ReportBarChart.vue";

export default defineComponent({
  name: "Reports",
  components: {
    Dropdown1,
    ReportTable,
    ReportPieChart,
    ReportBarChart,
  },
  setup() {
    const store = useThemeStore();
    const { t, te } = useI18n();
    const translate = (text: string) => {
      if (te(text)) {
        return t(text);
      } else {
        return text;
      }
    };

    return {
      translate,
    };
  },
});
</script>
