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
                class="btn btn-sm btn-success align-self-center"
                @click="endCurrentBreak"
              >
                <KTIcon icon-name="timer" icon-class="fs-3 text-white me-2" />
                {{ $t("breaks_end") }}
              </button>
              <button
                v-else
                class="btn btn-sm btn-danger align-self-center"
                @click="startNewBreak"
              >
                <KTIcon icon-name="watch" icon-class="fs-3 text-white me-2" />
                {{ $t("breaks_newBreak") }}
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
          <div class="mb-3">
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
            :disabled="breakReason.length < 10"
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

export default defineComponent({
  name: "kt-break-timeline",
  components: {
    BreakItem,
  },
  setup() {
    const breaks = ref<FormatedBreakTimesItems[]>([]);
    const breaksTimeStore = useBreaksStore();
    const ongoingBreaks = ref(false);
    const breakReason = ref("");
    const breakReasonError = ref("");
    const breakModal = ref<Modal | null>(null);
    const formatDate = (date: Date): string => date.toISOString().split("T")[0];
    type DateRange = "today" | "week" | "month" | "year";

    const getDateRange = (range: DateRange): { start: string; end: string } => {
      const today = new Date();

      const ranges: Record<DateRange, { start: Date; end: Date }> = {
        today: {
          start: new Date(today.setHours(0, 0, 0, 0)),
          end: new Date(today.setHours(23, 59, 59, 999)),
        },
        week: {
          start: new Date(
            today.getFullYear(),
            today.getMonth(),
            today.getDate() - today.getDay(),
          ),
          end: new Date(
            today.getFullYear(),
            today.getMonth(),
            today.getDate() - today.getDay() + 6,
          ),
        },
        month: {
          start: new Date(today.getFullYear(), today.getMonth(), 1),
          end: new Date(today.getFullYear(), today.getMonth() + 1, 0),
        },
        year: {
          start: new Date(today.getFullYear(), 0, 1),
          end: new Date(today.getFullYear(), 11, 31),
        },
      };

      return {
        start: formatDate(ranges[range].start),
        end: formatDate(ranges[range].end),
      };
    };

    //TODO: Uncomment this code when the API is ready
    // const { start, end } = getDateRange("today");
    // const startDate = ref(start);
    // const endDate = ref(end);

    const { start, end } = getDateRange("week");
    const startDate = ref(start);
    const endDate = ref(end);

    const fetchBreaksAndUpdateStatus = async () => {
      breaks.value = await breaksTimeStore.fetchBreaks({
        startDate: startDate.value,
        endDate: endDate.value,
      });
      ongoingBreaks.value = breaks.value.some(
        (breakItem) => !breakItem.isEnd && breakItem.type === "breakStart",
      );
    };

    const startNewBreak = async () => {
      breakReason.value = "";
      breakReasonError.value = "";

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

      try {
        await breaksTimeStore.startNewBreak(breakReason.value);
        breakModal.value?.hide();
        await fetchBreaksAndUpdateStatus();
      } catch (error) {
        console.error("Error starting break:", error);
      }
    };

    const endCurrentBreak = async () => {
      const breakId = breaks.value.find(
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
      setDateRange,
      fetchBreaksAndUpdateStatus,
      breakReason,
      breakReasonError,
      submitBreakReason,
    };
  },
});
</script>
