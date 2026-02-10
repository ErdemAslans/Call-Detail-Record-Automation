<template>
  <div class="card mb-5 mb-xl-8" id="kt_profile_details_view">
    <!--begin::Card body-->
    <div class="card-body p-9">
      <!--begin::Row-->
      <div class="row g-5 g-xl-8">
        <div class="col-xl-2">
          <StatisticsWidget5
            widget-classes="card-xl-stretch mb-xl-8"
            icon-name="timer"
            icon-color="success"
            :title="minDuration"
            :description="translate('minimumCallDuration')"
          ></StatisticsWidget5>
        </div>

        <div class="col-xl-2">
          <StatisticsWidget5
            widget-classes="card-xl-stretch mb-xl-8"
            icon-name="time"
            icon-color="danger"
            :title="maxDuration"
            :description="translate('maximumCallDuration')"
          ></StatisticsWidget5>
        </div>

        <div class="col-xl-2">
          <StatisticsWidget5
            widget-classes="card-xl-stretch mb-5 mb-xl-8"
            icon-name="watch"
            icon-color="warning"
            :title="averageDuration"
            :description="translate('averageCallDuration')"
          ></StatisticsWidget5>
        </div>
      </div>
      <!--end::Row-->

      <!--begin::Row-->
      <div class="row g-5 g-xl-8">
        <div class="col">
          <WorkhoursWidget
            widget-classes="card-xxl-stretch mb-5 mb-xl-10"
            widget-color="primary"
            :heading="$t('workHours')"
            :startDate="startDate"
            :endDate="endDate"
            :number="number"
            :is-work-hours="true"
          ></WorkhoursWidget>
        </div>
        <div class="col">
          <WorkhoursWidget
            widget-classes="card-xxl-stretch mb-5 mb-xl-10"
            widget-color="warning"
            :heading="$t('nonWorkHours')"
            :startDate="startDate"
            :endDate="endDate"
            :number="number"
            :is-work-hours="false"
          ></WorkhoursWidget>
        </div>
        <div class="col-xl-auto">
          <TakeABreak
            widget-classes="card-xxl-stretch mb-5 mb-xl-10"
            :startDate="startDate"
            :endDate="endDate"
            :number="number"
          ></TakeABreak>
        </div>
      </div>
      <!--end::Row-->
    </div>
    <!--begin::Row-->
    <div class="row g-5 g-xl-8">
      <div class="col">
        <LastCalls
          widget-classes="card-xxl-stretch mb-5 mb-xl-10"
          :start-date="startDate"
          :end-date="endDate"
          :number="number"
        ></LastCalls>
      </div>
    </div>

    <!--end::Card Body-->
  </div>
</template>

<script lang="ts">
import { getAssetPath } from "@/core/helpers/assets";
import { defineComponent } from "vue";
import StatisticsWidget5 from "@/components/widgets/statsistics/Widget5.vue";
import LastCalls from "./LastCalls.vue";
import WorkhoursWidget from "./WorkhoursWidget.vue";
import TakeABreak from "./TakeABreak.vue";
import { useI18n } from "vue-i18n";
import { end } from "@popperjs/core";

export default defineComponent({
  name: "user-statistics",
  components: {
    StatisticsWidget5,
    LastCalls,
    WorkhoursWidget,
    TakeABreak,
  },
  props: {
    minDuration: String,
    maxDuration: String,
    averageDuration: String,
    startDate: {
      type: String,
      required: true,
    },
    endDate: {
      type: String,
      required: true,
    },
    number: {
      type: String,
      required: true,
    },
  },

  setup() {
    const { t, te } = useI18n();
    const translate = (text: string) => {
      if (te(text)) {
        return t(text);
      } else {
        return text;
      }
    };

    return {
      getAssetPath,
      translate,
    };
  },
});
</script>
