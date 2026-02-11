<template>
  <!--begin::Activities drawer-->
  <div id="kt_activities">
    <div class="card">
      <!--begin::Header-->
      <div class="card-header card-header-stretch" id="kt_activities_header">
        <h3 class="card-title fw-bold text-gray-900">{{ $t("breaks") }}</h3>
        <!--begin::Toolbar-->
        <div class="card-toolbar m-0">
          <!--begin::Tab nav-->
          <ul
            class="nav nav-tabs nav-line-tabs nav-stretch fs-6 border-0 fw-bold"
            role="tablist"
          >
            <li class="nav-item" role="presentation">
              <button
                v-if="ongoingBreaks"
                class="btn btn-sm btn-success align-self-center me-2"
                @click="endCurrentBreak"
              >
                <KTIcon icon-name="timer" icon-class="fs-3 text-white me-2" />
                {{ $t("breaks_end") }}
              </button>
              <template v-else>
                <button
                  class="btn btn-sm btn-danger align-self-center me-2"
                  @click="startNewBreak"
                >
                  <KTIcon icon-name="watch" icon-class="fs-3 text-white me-2" />
                  {{ $t("breaks_newBreak") }}
                </button>
                <button
                  class="btn btn-sm btn-warning align-self-center"
                  @click="handleEndShift"
                >
                  <KTIcon icon-name="exit-right" icon-class="fs-3 text-white me-2" />
                  {{ $t("endShift") }}
                </button>
              </template>
            </li>
            <li class="nav-item" role="presentation">
              <a
                id="kt_activity_today_tab"
                class="nav-link justify-content-center text-active-gray-800 active"
                data-bs-toggle="tab"
                role="tab"
                href="#kt_activity_today"
                @click="setDateRange('today')"
              >
                {{ $t("today") }}
              </a>
            </li>
            <li class="nav-item" role="presentation">
              <a
                id="kt_activity_week_tab"
                class="nav-link justify-content-center text-active-gray-800"
                data-bs-toggle="tab"
                role="tab"
                href="#kt_activity_week"
                @click="setDateRange('week')"
              >
                {{ $t("week") }}
              </a>
            </li>
            <li class="nav-item" role="presentation">
              <a
                id="kt_activity_month_tab"
                class="nav-link justify-content-center text-active-gray-800"
                data-bs-toggle="tab"
                role="tab"
                href="#kt_activity_month"
                @click="setDateRange('month')"
              >
                {{ $t("month") }}
              </a>
            </li>
            <li class="nav-item" role="presentation">
              <a
                id="kt_activity_year_tab"
                class="nav-link justify-content-center text-active-gray-800"
                data-bs-toggle="tab"
                role="tab"
                href="#kt_activity_year"
                @click="setDateRange('year')"
              >
                {{ $t("year") }}
              </a>
            </li>
          </ul>
          <!--end::Tab nav-->
        </div>
        <!--end::Toolbar-->
      </div>
      <!--end::Header-->

      <!--begin::Body-->
      <div class="card-body" id="kt_activities_body">
        <!--begin::Content-->
        <div
          id="kt_activities_scroll"
          class="scroll-y me-n5 pe-5"
          data-kt-scroll="true"
          data-kt-scroll-height="auto"
          data-kt-scroll-wrappers="#kt_activities_body"
          data-kt-scroll-dependencies="#kt_activities_header, #kt_activities_footer"
          data-kt-scroll-offset="5px"
        >
          <!--begin::Timeline items-->
          <div class="timeline">
            <BreakItem
              v-for="breakItem in breaks"
              :key="breakItem.id"
              :breakItem="breakItem"
              @breakEnded="fetchBreaksAndUpdateStatus"
            />
          </div>
          <!--end::Timeline items-->
        </div>
        <!--end::Content-->
      </div>
      <!--end::Body-->

      <!--begin::Footer-->
      <div class="card-footer py-5 text-center" id="kt_activities_footer">
        <!-- <a href="#" class="btn btn-bg-body text-primary">
          View All Activities
          <KTIcon icon-name="arrow-right" icon-class="fs-3 text-primary" />
        </a> -->
      </div>
      <!--end::Footer-->
    </div>
  </div>
  <!--end::Activities drawer-->

  <!-- Break Reason Modal -->
  <div
    class="modal fade"
    id="breakReasonModal"
    tabindex="-1"
    aria-hidden="true"
  >
    <div class="modal-dialog modal-dialog-centered">
      <div class="modal-content">
        <div class="modal-header">
          <h5 class="modal-title">{{ $t("breaks_newBreak") }}</h5>
          <button
            type="button"
            class="btn-close"
            data-bs-dismiss="modal"
            aria-label="Close"
          ></button>
        </div>
        <div class="modal-body">
          <div class="mb-5">
            <label for="breakReason" class="form-label">{{
              $t("breaks_reason")
            }}</label>
            <textarea
              class="form-control"
              id="breakReason"
              v-model="breakReason"
              rows="4"
              :class="{ 'is-invalid': breakReasonError }"
            ></textarea>
            <div class="invalid-feedback" v-if="breakReasonError">
              {{ breakReasonError }}
            </div>
            <small class="text-muted">{{ $t("breaks_reasonMinLength") }}</small>
          </div>
          <div class="mb-3">
            <label for="plannedEndTime" class="form-label required">{{
              $t("plannedEndTime")
            }}</label>
            <input
              type="time"
              class="form-control"
              id="plannedEndTime"
              v-model="plannedEndTimeLocal"
              :class="{ 'is-invalid': plannedEndTimeError }"
            />
            <div class="invalid-feedback" v-if="plannedEndTimeError">
              {{ plannedEndTimeError }}
            </div>
            <small class="text-muted">{{ $t("breakMaxDuration") }}</small>
          </div>
        </div>
        <div class="modal-footer">
          <button
            type="button"
            class="btn btn-secondary"
            data-bs-dismiss="modal"
          >
            {{ $t("cancel") }}
          </button>
          <button
            type="button"
            class="btn btn-danger"
            @click="submitBreakReason"
            :disabled="breakReason.length < 10 || !plannedEndTimeLocal"
          >
            {{ $t("save") }}
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script lang="ts">
import { getAssetPath } from "@/core/helpers/assets";
import { defineComponent, onMounted, ref } from "vue";
import BreakItem from "@/components/activity-timeline-items/BreakItem.vue";
import { useBreaksStore } from "@/stores/breaksTime";
import { Modal } from "bootstrap";
import { ElMessage } from "element-plus";
import i18n from "@/core/plugins/i18n";

export default defineComponent({
  name: "kt-break-timeline",
  components: {
    BreakItem,
  },
  setup() {
    const breaks = ref<FormatedBreakTimesItems[]>([]);
    const breaksTimeStore = useBreaksStore();
    const ongoingBreaks = ref(false);
    const ongoingBreakId = ref<string | null>(null);
    const breakReason = ref("");
    const breakReasonError = ref("");
    const plannedEndTimeLocal = ref("");
    const plannedEndTimeError = ref("");
    const breakModal = ref<Modal | null>(null);
    const formatDate = (date: Date): string =>
      date.toLocaleDateString("en-CA", { timeZone: "Europe/Istanbul" });
    type DateRange = "today" | "week" | "month" | "year";

    const getDateRange = (range: DateRange): { start: string; end: string } => {
      const now = new Date();
      const y = now.getFullYear();
      const m = now.getMonth();
      const d = now.getDate();

      const ranges: Record<DateRange, { start: Date; end: Date }> = {
        today: {
          start: new Date(y, m, d),
          end: new Date(y, m, d),
        },
        week: {
          // Pazartesi başlangıçlı hafta (getDay: 0=Pazar, 1=Pazartesi...)
          start: new Date(y, m, d - ((now.getDay() + 6) % 7)),
          end: new Date(y, m, d - ((now.getDay() + 6) % 7) + 6),
        },
        month: {
          start: new Date(y, m, 1),
          end: new Date(y, m + 1, 0),
        },
        year: {
          start: new Date(y, 0, 1),
          end: new Date(y, 11, 31),
        },
      };

      return {
        start: formatDate(ranges[range].start),
        end: formatDate(ranges[range].end),
      };
    };

    const { start, end } = getDateRange("today");
    const startDate = ref(start);
    const endDate = ref(end);

    const checkOngoingBreak = async () => {
      const ongoing = await breaksTimeStore.fetchOngoingBreak();
      if (ongoing) {
        ongoingBreaks.value = true;
        ongoingBreakId.value = ongoing.id;
      } else {
        ongoingBreaks.value = false;
        ongoingBreakId.value = null;
      }
    };

    const fetchBreaksAndUpdateStatus = async () => {
      breaks.value = await breaksTimeStore.fetchBreaks({
        startDate: startDate.value,
        endDate: endDate.value,
      });
      await checkOngoingBreak();
    };

    const getDefaultPlannedEndTime = (): string => {
      const now = new Date();
      now.setMinutes(now.getMinutes() + 30);
      return `${String(now.getHours()).padStart(2, "0")}:${String(now.getMinutes()).padStart(2, "0")}`;
    };

    const startNewBreak = async () => {
      breakReason.value = "";
      breakReasonError.value = "";
      plannedEndTimeLocal.value = getDefaultPlannedEndTime();
      plannedEndTimeError.value = "";

      // Initialize and show the modal
      if (!breakModal.value) {
        const modalElement = document.getElementById("breakReasonModal");
        if (modalElement) {
          breakModal.value = new Modal(modalElement);
        }
      }
      breakModal.value?.show();
    };

    const submitBreakReason = async () => {
      if (breakReason.value.length < 10) {
        breakReasonError.value = "Break reason must be at least 10 characters";
        return;
      }

      if (!plannedEndTimeLocal.value) {
        plannedEndTimeError.value = i18n.global.t("breakEndTimeRequired");
        return;
      }

      // Convert local time input to UTC ISO string
      const [hours, minutes] = plannedEndTimeLocal.value.split(":").map(Number);
      const plannedEnd = new Date();
      plannedEnd.setHours(hours, minutes, 0, 0);

      if (plannedEnd <= new Date()) {
        plannedEndTimeError.value = i18n.global.t("breakEndTimeFuture");
        return;
      }

      const diffHours = (plannedEnd.getTime() - Date.now()) / (1000 * 60 * 60);
      if (diffHours > 4) {
        plannedEndTimeError.value = i18n.global.t("breakMaxDuration");
        return;
      }

      plannedEndTimeError.value = "";

      try {
        await breaksTimeStore.startNewBreak(
          breakReason.value,
          plannedEnd.toISOString(),
        );
        breakModal.value?.hide();
        await fetchBreaksAndUpdateStatus();
      } catch (error: any) {
        breakModal.value?.hide();
        if (error?.status === 400) {
          ElMessage.error(
            i18n.global.t("breaks_ongoingBreakError") ||
              "Zaten açık bir molanız var. Lütfen önce mevcut molayı kapatın.",
          );
          await checkOngoingBreak();
        }
      }
    };

    const endCurrentBreak = async () => {
      const breakId =
        ongoingBreakId.value ||
        breaks.value.find(
          (breakItem) => !breakItem.isEnd && (breakItem.type === "breakStart" || breakItem.type === "shiftEnd"),
        )?.id;
      if (!breakId) return;

      await breaksTimeStore.endBreak(breakId);
      await fetchBreaksAndUpdateStatus();
    };

    const handleEndShift = async () => {
      try {
        await breaksTimeStore.endShift();
        await fetchBreaksAndUpdateStatus();
      } catch (error: any) {
        if (error?.status === 400) {
          ElMessage.error(
            i18n.global.t("breaks_ongoingBreakError") ||
              "Zaten açık bir molanız var. Lütfen önce mevcut molayı kapatın.",
          );
          await checkOngoingBreak();
        }
      }
    };

    const setDateRange = (range: DateRange) => {
      const { start, end } = getDateRange(range);
      startDate.value = start;
      endDate.value = end;
      fetchBreaksAndUpdateStatus();
    };

    onMounted(async () => {
      await fetchBreaksAndUpdateStatus();
    });

    return {
      getAssetPath,
      breaks,
      ongoingBreaks,
      startNewBreak,
      endCurrentBreak,
      handleEndShift,
      setDateRange,
      fetchBreaksAndUpdateStatus,
      breakReason,
      breakReasonError,
      plannedEndTimeLocal,
      plannedEndTimeError,
      submitBreakReason,
    };
  },
});
</script>
