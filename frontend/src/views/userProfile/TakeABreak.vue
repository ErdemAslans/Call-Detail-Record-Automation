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
      <!--begin::Ongoing Break Alert-->
      <div
        v-if="ongoingBreak"
        class="notice d-flex bg-light-danger rounded border-danger border border-dashed p-4 mb-4"
      >
        <i class="fa fa-exclamation-triangle fs-2tx text-danger me-4"></i>
        <div class="d-flex flex-stack flex-grow-1 flex-wrap flex-md-nowrap">
          <div class="mb-3 mb-md-0 fw-semibold">
            <h4 class="text-gray-900 fw-bold">
              {{ translate("breaks_ongoingBreak") || "Açık Mola" }}
            </h4>
            <div class="fs-7 text-gray-700">
              {{ ongoingBreak.reason }} -
              {{ DateHelper.toLocaleDateStringWithCulture(new Date(ongoingBreak.startTime)) }}
              {{ DateHelper.toLocaleTimeStringWithCulture(new Date(ongoingBreak.startTime)) }}
            </div>
          </div>
          <button
            class="btn btn-sm btn-danger fw-bold"
            @click="forceEndBreak"
            :disabled="forceEndLoading"
          >
            <i class="fa fa-stop-circle me-1"></i>
            {{ forceEndLoading ? "..." : translate("breaks_forceEnd") || "Molayı Kapat" }}
          </button>
        </div>
      </div>
      <!--end::Ongoing Break Alert-->

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
                :class="`fa fa-genderless ${item.type === 'shiftEnd' ? 'text-warning' : item.type === 'breakStart' ? 'text-danger' : 'text-success'} fs-1`"
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
import { ElMessage } from "element-plus";

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
    const ongoingBreak = ref<BreakTime | null>(null);
    const forceEndLoading = ref(false);

    const checkOngoingBreak = async () => {
      const ongoing = await userStatisticsStore.fetchAdminOngoingBreak(
        props.number,
      );
      ongoingBreak.value = ongoing || null;
    };

    const fetchBreakTimes = async () => {
      const response = await userStatisticsStore.fetchBreakTimes({
        startDate: props.startDate,
        endDate: props.endDate,
        number: props.number,
      });
      breakTimes.value = response;
      await checkOngoingBreak();
    };

    const forceEndBreak = async () => {
      if (!ongoingBreak.value) return;
      forceEndLoading.value = true;
      try {
        await userStatisticsStore.adminForceEndBreak(
          ongoingBreak.value.userId,
        );
        ElMessage.success(
          translate("breaks_forceEndSuccess") ||
            "Mola başarıyla kapatıldı.",
        );
        await fetchBreakTimes();
      } catch {
        ElMessage.error(
          translate("breaks_forceEndError") ||
            "Mola kapatılırken hata oluştu.",
        );
      } finally {
        forceEndLoading.value = false;
      }
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
            type: item.breakType === "EndOfShift" ? "shiftEnd" : "breakStart",
            reason: item.reason,
          });
        }

        if (item.endTime && item.breakType !== "EndOfShift") {
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
      ongoingBreak,
      forceEndBreak,
      forceEndLoading,
    };
  },
});
</script>
