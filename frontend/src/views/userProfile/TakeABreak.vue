<template>
  <!--begin::List Widget 5-->
  <div class="card" :class="widgetClasses">
    <!--begin::Header-->
    <div class="card-header align-items-center border-0 mt-4">
      <h3 class="card-title align-items-start flex-column">
        <span class="fw-bold mb-2 text-gray-900">{{
          translate("breaks")
        }}</span>
      </h3>
    </div>
    <!--end::Header-->

    <!--begin::Body-->
    <div class="card-body card-scroll h-200px pt-5">
      <!--begin::Timeline-->
      <div class="timeline-label">
        <div
          v-for="(item, index) in transformedBreakTimesWithDates"
          :key="index"
          class="timeline-item"
        >
          <template v-if="item.type === 'date'">
            <!--begin::Date Separator-->
            <div class="timeline-date text-muted fs-7 fw-bold">
              {{
                DateHelper.toLocaleDateStringWithCulture(
                  new Date(item.breakTime),
                )
              }}
            </div>
            <!--end::Date Separator-->
          </template>
          <template v-else>
            <!--begin::Label-->
            <div
              class="timeline-label fw-bold text-gray-800 fs-6"
              data-bs-toggle="tooltip"
              :title="item.reason || ''"
            >
              {{
                DateHelper.toLocaleTimeStringWithCulture(
                  new Date(item.breakTime),
                )
              }}
            </div>
            <!--end::Label-->

            <!--begin::Badge-->
            <div class="timeline-badge">
              <i
                :class="`fa fa-genderless ${item.type === 'breakStart' ? 'text-danger' : 'text-success'} fs-1`"
              ></i>
            </div>
            <!--end::Badge-->

            <!--begin::Desc-->
            <div class="timeline-content fw-bold text-gray-800 ps-3">
              {{ translate(item.type) }}
            </div>
            <!--end::Desc-->
          </template>
        </div>
      </div>
      <!--end::Timeline-->
    </div>
    <!--end: Card Body-->
  </div>
  <!--end: List Widget 5-->
</template>

<script lang="ts">
import { getAssetPath } from "@/core/helpers/assets";
import { defineComponent, onMounted, ref, watch, computed } from "vue";
import { useUserStatisticsStore } from "@/stores/userStatistics";
import DateHelper from "@/core/helpers/DateHelper";
import { useI18n } from "vue-i18n";

interface TransformedBreakTime {
  breakTime: string;
  type: string;
  reason?: string;
}

export default defineComponent({
  name: "kt-widget-takeBreak-timeline",
  props: {
    widgetClasses: String,
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
  setup(props) {
    const { t, te } = useI18n();
    const translate = (text: string) => {
      if (te(text)) {
        return t(text);
      } else {
        return text;
      }
    };
    const userStatisticsStore = useUserStatisticsStore();
    const breakTimes = ref<BreakTime[]>([]);

    const fetchBreakTimes = async () => {
      const response = await userStatisticsStore.fetchBreakTimes({
        startDate: props.startDate,
        endDate: props.endDate,
        number: props.number,
      });
      breakTimes.value = response;
    };

    const transformedBreakTimesWithDates = computed(() => {
      const result: TransformedBreakTime[] = [];
      let lastDate = "";

      breakTimes.value.forEach((item) => {
        if (item.startTime) {
          const breakStartDate = new Date(item.startTime).toLocaleDateString();
          if (breakStartDate !== lastDate) {
            result.push({ breakTime: item.startTime, type: "date" });
            lastDate = breakStartDate;
          }
          result.push({
            breakTime: item.startTime,
            type: "breakStart",
            reason: item.reason,
          });
        }

        if (item.endTime) {
          const breakEndDate = new Date(item.endTime).toLocaleDateString();
          if (breakEndDate !== lastDate) {
            result.push({ breakTime: item.endTime, type: "date" });
            lastDate = breakEndDate;
          }
          result.push({
            breakTime: item.endTime,
            type: "breakEnd",
            reason: item.reason,
          });
        }
      });

      return result;
    });

    onMounted(() => {
      fetchBreakTimes();
    });

    watch(
      () => [props.startDate, props.endDate, props.number],
      () => {
        fetchBreakTimes();
      },
    );

    return {
      getAssetPath,
      translate,
      DateHelper,
      transformedBreakTimesWithDates,
    };
  },
});
</script>
