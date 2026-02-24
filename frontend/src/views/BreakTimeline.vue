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
            <!-- Central: Buttons -->
            <li v-if="!isAdmin" class="nav-item" role="presentation">
              <!-- Ongoing regular break: show Molayı Bitir -->
              <button
                v-if="ongoingBreaks && ongoingBreakType !== 'EndOfShift'"
                class="btn btn-sm btn-success align-self-center me-2"
                @click="endCurrentBreak"
              >
                <KTIcon icon-name="timer" icon-class="fs-3 text-white me-2" />
                {{ $t("breaks_end") }}
              </button>
              <!-- Ongoing EndOfShift: show Mesai'ye Başla -->
              <button
                v-if="ongoingBreaks && ongoingBreakType === 'EndOfShift'"
                class="btn btn-sm btn-info align-self-center me-2"
                @click="handleStartShift"
              >
                <KTIcon icon-name="entrance-left" icon-class="fs-3 text-white me-2" />
                {{ $t("startShift") }}
              </button>
              <!-- No ongoing break: show Yeni Mola -->
              <button
                v-if="!ongoingBreaks"
                class="btn btn-sm btn-danger align-self-center me-2"
                @click="startNewBreak"
              >
                <KTIcon icon-name="watch" icon-class="fs-3 text-white me-2" />
                {{ $t("breaks_newBreak") }}
              </button>
              <!-- Mesai Bitir: always visible (except when shift already ended) -->
              <button
                v-if="ongoingBreakType !== 'EndOfShift'"
                class="btn btn-sm btn-warning align-self-center"
                @click="handleEndShift"
              >
                <KTIcon icon-name="exit-right" icon-class="fs-3 text-white me-2" />
                {{ $t("endShift") }}
              </button>
            </li>
            <!-- Admin: Buttons based on ongoing break type -->
            <li v-if="isAdmin && ongoingBreaks && selectedOperator" class="nav-item" role="presentation">
              <!-- Ongoing EndOfShift: show Mesai'ye Başla -->
              <button
                v-if="ongoingBreakType === 'EndOfShift'"
                class="btn btn-sm btn-info align-self-center me-2"
                @click="adminForceEnd"
              >
                <KTIcon icon-name="entrance-left" icon-class="fs-3 text-white me-2" />
                {{ $t("startShift") }}
              </button>
              <!-- Ongoing regular break: show Molayı Zorla Bitir -->
              <button
                v-else
                class="btn btn-sm btn-danger align-self-center me-2"
                @click="adminForceEnd"
              >
                <KTIcon icon-name="cross-circle" icon-class="fs-3 text-white me-2" />
                {{ $t("forceEndBreak") }}
              </button>
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

      <!-- Admin: Operator Selector -->
      <div v-if="isAdmin" class="card-body border-bottom py-4">
        <div class="row align-items-center">
          <div class="col-md-6">
            <label class="form-label fw-bold">{{ $t("selectOperator") }}</label>
            <select
              class="form-select form-select-solid"
              v-model="selectedOperator"
              @change="onOperatorChange"
            >
              <option value="">{{ $t("selectOperatorPlaceholder") }}</option>
              <option
                v-for="op in operators"
                :key="op.number"
                :value="op.number"
              >
                {{ op.name }} ({{ op.number }}) - {{ op.department }}
              </option>
            </select>
          </div>
          <div v-if="selectedOperator && ongoingBreaks" class="col-md-6 mt-3 mt-md-0">
            <div
              :class="ongoingBreakType === 'EndOfShift'
                ? 'alert alert-info d-flex align-items-center mb-0 py-3'
                : 'alert alert-warning d-flex align-items-center mb-0 py-3'"
            >
              <KTIcon icon-name="information-3" icon-class="fs-2 me-3" />
              <span>{{ ongoingBreakType === 'EndOfShift' ? $t("operatorShiftEnded") : $t("operatorHasOngoingBreak") }}</span>
            </div>
          </div>
        </div>
      </div>

      <!--begin::Body-->
      <div class="card-body" id="kt_activities_body">
        <!-- Admin: no operator selected message -->
        <div v-if="isAdmin && !selectedOperator" class="text-center text-muted py-10">
          <KTIcon icon-name="people" icon-class="fs-2tx text-gray-300 mb-5" />
          <p class="fs-5">{{ $t("selectOperatorToViewBreaks") }}</p>
        </div>
        <!--begin::Content-->
        <div
          v-else
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
              :isAdmin="isAdmin"
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
      </div>
      <!--end::Footer-->
    </div>
  </div>
  <!--end::Activities drawer-->

  <!-- Break Reason Modal (Central only) -->
  <div
    v-if="!isAdmin"
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
import { defineComponent, onMounted, ref, computed } from "vue";
import BreakItem from "@/components/activity-timeline-items/BreakItem.vue";
import { useBreaksStore } from "@/stores/breaksTime";
import { useUserStatisticsStore } from "@/stores/userStatistics";
import { useOperatorStore } from "@/stores/operator";
import { useAuthStore } from "@/stores/auth";
import { Modal } from "bootstrap";
import { ElMessage, ElMessageBox } from "element-plus";
import i18n from "@/core/plugins/i18n";

export default defineComponent({
  name: "kt-break-timeline",
  components: {
    BreakItem,
  },
  setup() {
    const authStore = useAuthStore();
    const breaksTimeStore = useBreaksStore();
    const userStatisticsStore = useUserStatisticsStore();
    const operatorStore = useOperatorStore();

    const isAdmin = computed(() => authStore.hasRole("Admin"));
    const operators = computed(() => operatorStore.operators);
    const selectedOperator = ref("");

    const breaks = ref<FormatedBreakTimesItems[]>([]);
    const ongoingBreaks = ref(false);
    const ongoingBreakId = ref<string | null>(null);
    const ongoingBreakUserId = ref<string | null>(null);
    const ongoingBreakType = ref<string | null>(null);
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

    const resetOngoingState = () => {
      ongoingBreaks.value = false;
      ongoingBreakId.value = null;
      ongoingBreakUserId.value = null;
      ongoingBreakType.value = null;
    };

    // --- Ongoing break check (tracks break type) ---
    const checkOngoingBreak = async () => {
      if (isAdmin.value) {
        if (!selectedOperator.value) return;
        const ongoing = await userStatisticsStore.fetchAdminOngoingBreak(selectedOperator.value);
        if (ongoing) {
          ongoingBreaks.value = true;
          ongoingBreakId.value = ongoing.id;
          ongoingBreakUserId.value = ongoing.userId || null;
          ongoingBreakType.value = ongoing.breakType || null;
        } else {
          resetOngoingState();
        }
      } else {
        const ongoing = await breaksTimeStore.fetchOngoingBreak();
        if (ongoing) {
          ongoingBreaks.value = true;
          ongoingBreakId.value = ongoing.id;
          ongoingBreakType.value = ongoing.breakType || null;
        } else {
          resetOngoingState();
        }
      }
    };

    // --- Fetch breaks: role-based API ---
    const fetchBreaksAndUpdateStatus = async () => {
      if (isAdmin.value) {
        if (!selectedOperator.value) return;
        const data = await userStatisticsStore.fetchBreakTimes({
          startDate: startDate.value,
          endDate: endDate.value,
          number: selectedOperator.value,
        });
        breaks.value = formatBreakTimes(data || []);
      } else {
        breaks.value = await breaksTimeStore.fetchBreaks({
          startDate: startDate.value,
          endDate: endDate.value,
        });
      }
      await checkOngoingBreak();
    };

    // --- Format break times (reused from breaksTime store for admin data) ---
    const formatBreakTimes = (
      breakTimes: BreakListItem[],
    ): FormatedBreakTimesItems[] => {
      const breaksByDate = new Map<string, FormatedBreakTimesItems[]>();

      breakTimes.forEach((item) => {
        if (item.startTime) {
          const breakStartDate = new Date(item.startTime).toLocaleDateString();

          if (!breaksByDate.has(breakStartDate)) {
            breaksByDate.set(breakStartDate, []);
          }

          breaksByDate.get(breakStartDate)!.push({
            id: item.id,
            breakTime: item.startTime,
            type: item.breakType === "EndOfShift" ? "shiftEnd" : "breakStart",
            reason: item.reason,
            isEnd: item.endTime ? true : false,
            breakType: item.breakType,
          });
        }

        if (item.endTime) {
          const breakEndDate = new Date(item.endTime).toLocaleDateString();

          if (!breaksByDate.has(breakEndDate)) {
            breaksByDate.set(breakEndDate, []);
          }

          breaksByDate.get(breakEndDate)!.push({
            id: item.id,
            breakTime: item.endTime,
            type: item.breakType === "EndOfShift" ? "shiftStart" : "breakEnd",
            isEnd: true,
          });
        }
      });

      const result: FormatedBreakTimesItems[] = [];
      const sortedDates = Array.from(breaksByDate.keys()).sort(
        (a, b) => new Date(b).getTime() - new Date(a).getTime(),
      );

      for (const date of sortedDates) {
        result.push({
          id: date,
          breakTime: new Date(date).toISOString(),
          type: "date",
          isEnd: false,
        });

        const dateItems = breaksByDate
          .get(date)!
          .sort(
            (a, b) =>
              new Date(b.breakTime).getTime() - new Date(a.breakTime).getTime(),
          );

        result.push(...dateItems);
      }

      return result;
    };

    // --- Admin: force end break / start shift ---
    const adminForceEnd = async () => {
      if (!ongoingBreakUserId.value && !ongoingBreakId.value) return;
      const isShiftEnd = ongoingBreakType.value === "EndOfShift";
      try {
        await userStatisticsStore.adminForceEndBreak(
          ongoingBreakUserId.value || ongoingBreakId.value!,
        );
        ElMessage.success(
          isShiftEnd
            ? i18n.global.t("shiftStartedSuccess")
            : i18n.global.t("breakForceEnded"),
        );
        await fetchBreaksAndUpdateStatus();
      } catch {
        ElMessage.error(i18n.global.t("breakForceEndError"));
      }
    };

    // --- Admin: operator change ---
    const onOperatorChange = () => {
      breaks.value = [];
      resetOngoingState();
      if (selectedOperator.value) {
        fetchBreaksAndUpdateStatus();
      }
    };

    // --- Central: Start shift (cancel EndOfShift) ---
    const handleStartShift = async () => {
      if (!ongoingBreakId.value) return;
      try {
        await breaksTimeStore.endBreak(ongoingBreakId.value);
        ElMessage.success(i18n.global.t("shiftStartedSuccess"));
        await fetchBreaksAndUpdateStatus();
      } catch {
        ElMessage.error(i18n.global.t("shiftStartError"));
      }
    };

    // --- Central: End shift (with confirmation if break active) ---
    const handleEndShift = async () => {
      // If there's an ongoing regular break, show confirmation
      if (ongoingBreaks.value && ongoingBreakType.value !== "EndOfShift") {
        try {
          await ElMessageBox.confirm(
            i18n.global.t("endShiftWithBreakConfirmation"),
            i18n.global.t("endShift"),
            {
              confirmButtonText: i18n.global.t("continue"),
              cancelButtonText: i18n.global.t("cancel"),
              type: "warning",
            },
          );
          // User confirmed: end break first, then end shift
          await breaksTimeStore.endBreak(ongoingBreakId.value!);
          await breaksTimeStore.endShift();
          await fetchBreaksAndUpdateStatus();
        } catch (action) {
          // User cancelled — do nothing
          if (action === "cancel") return;
        }
        return;
      }

      // No ongoing break — just end shift
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

    // --- Central: modal & break actions ---
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
          (breakItem) => !breakItem.isEnd && breakItem.type === "breakStart",
        )?.id;
      if (!breakId) return;

      await breaksTimeStore.endBreak(breakId);
      await fetchBreaksAndUpdateStatus();
    };

    const setDateRange = (range: DateRange) => {
      const { start, end } = getDateRange(range);
      startDate.value = start;
      endDate.value = end;
      if (!isAdmin.value || selectedOperator.value) {
        fetchBreaksAndUpdateStatus();
      }
    };

    onMounted(async () => {
      if (isAdmin.value) {
        await operatorStore.fetchOperators();
      } else {
        await fetchBreaksAndUpdateStatus();
      }
    });

    return {
      getAssetPath,
      breaks,
      ongoingBreaks,
      ongoingBreakType,
      isAdmin,
      operators,
      selectedOperator,
      onOperatorChange,
      adminForceEnd,
      handleStartShift,
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
