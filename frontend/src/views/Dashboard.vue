<template>
  <!--begin::Date Picker Bar-->
  <div class="card mb-5">
    <div class="card-body py-4">
      <div class="d-flex align-items-center justify-content-between flex-wrap gap-3">
        <!--begin::Date Navigation-->
        <div class="d-flex align-items-center gap-2">
          <button class="btn btn-sm btn-icon btn-light-primary" @click="prevDay" :title="translate('previousDay')">
            <KTIcon icon-name="arrow-left" icon-class="fs-4" />
          </button>

          <el-date-picker
            v-model="selectedDate"
            type="date"
            :placeholder="translate('selectDate')"
            format="DD.MM.YYYY"
            value-format="YYYY-MM-DD"
            :clearable="false"
            size="default"
            style="width: 160px"
          />

          <button class="btn btn-sm btn-icon btn-light-primary" @click="nextDay" :disabled="isToday" :title="translate('nextDay')">
            <KTIcon icon-name="arrow-right" icon-class="fs-4" />
          </button>

          <button
            class="btn btn-sm btn-light-success ms-2"
            :class="{ 'btn-success text-white': isToday }"
            @click="goToToday"
          >
            {{ translate("today") }}
          </button>
        </div>
        <!--end::Date Navigation-->

        <!--begin::Date Label-->
        <div class="d-flex align-items-center">
          <span class="badge badge-light-primary fs-7 fw-semibold px-4 py-2">
            {{ selectedDateLabel }}
          </span>
        </div>
        <!--end::Date Label-->
      </div>
    </div>
  </div>
  <!--end::Date Picker Bar-->

  <!--begin::Row 1: Daily Summary Cards-->
  <DailySummaryCards :selected-date="selectedDate" />
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
        :selected-date="selectedDate"
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
import { computed, defineComponent, ref } from "vue";
import { useI18n } from "vue-i18n";
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
    const { t, te } = useI18n();
    const translate = (text: string) => (te(text) ? t(text) : text);

    const getTodayStr = () => {
      return new Date().toLocaleDateString("en-CA", {
        timeZone: "Europe/Istanbul",
      });
    };

    const selectedDate = ref(getTodayStr());

    const isToday = computed(() => selectedDate.value === getTodayStr());

    const selectedDateLabel = computed(() => {
      const d = new Date(selectedDate.value + "T12:00:00");
      return d.toLocaleDateString("tr-TR", {
        weekday: "long",
        year: "numeric",
        month: "long",
        day: "numeric",
      });
    });

    const prevDay = () => {
      const d = new Date(selectedDate.value + "T12:00:00");
      d.setDate(d.getDate() - 1);
      selectedDate.value = d.toISOString().split("T")[0];
    };

    const nextDay = () => {
      if (isToday.value) return;
      const d = new Date(selectedDate.value + "T12:00:00");
      d.setDate(d.getDate() + 1);
      selectedDate.value = d.toISOString().split("T")[0];
    };

    const goToToday = () => {
      selectedDate.value = getTodayStr();
    };

    return {
      getAssetPath,
      translate,
      selectedDate,
      isToday,
      selectedDateLabel,
      prevDay,
      nextDay,
      goToToday,
    };
  },
});
</script>
