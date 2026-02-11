<template>
  <!--begin::WorkHours Widget-->
  <div :class="widgetClasses" class="card">
    <!--begin::Body-->
    <div class="card-body p-0">
      <!--begin::Header-->
      <div
        :class="`bg-${widgetColor}`"
        class="px-9 pt-7 card-rounded h-275px w-100"
      >
        <!--begin::Heading-->
        <div class="d-flex flex-stack">
          <h3 class="m-0 text-white fw-bold fs-3">{{ heading }}</h3>
        </div>
        <!--end::Heading-->

        <!--begin::Balance-->
        <div class="d-flex text-center flex-column text-white pt-8">
          <span class="fw-semibold fs-7">{{ $t("totalCallDuration") }}</span>
          <span class="fw-bold fs-2x pt-1">{{
            DateHelper.formatDuration(workHourStatistics.totalDuration)
          }}</span>
        </div>
        <!--end::Balance-->
      </div>
      <!--end::Header-->

      <div
        class="bg-body shadow-sm card-rounded mx-9 mb-9 px-6 py-9 position-relative z-index-1"
        style="margin-top: -100px"
      >
        <!--begin::totalCounts-->
        <div class="d-flex align-items-center mb-7">
          <!--begin::Symbol-->
          <div class="symbol symbol-45px w-40px me-5">
            <span class="symbol-label bg-lighten">
              <KTIcon icon-name="call" icon-class="fs-1 text-primary" />
            </span>
          </div>
          <!--end::Symbol-->

          <!--begin::Description-->
          <div class="d-flex align-items-center flex-wrap w-100">
            <!--begin::Title-->
            <div class="mb-1 pe-3 flex-grow-1 text-primary">
              <a href="#" class="fs-5 text-primary fw-bold">
                {{ $t("totalCallCount") }}</a
              >
            </div>
            <!--end::Title-->

            <!--begin::Label-->
            <div class="d-flex align-items-center">
              <div class="fw-bold fs-5 text-primary pe-1">
                {{ workHourStatistics.totalCalls }}
              </div>

              <!-- <KTIcon
                v-if="item.arrow === 'up'"
                icon-name="arrow-up"
                icon-class="fs-5 text-success ms-1"
              />

              <KTIcon
                v-else-if="item.arrow === 'down'"
                icon-name="arrow-down"
                icon-class="fs-5 text-danger ms-1"
              /> -->
            </div>
            <!--end::Label-->
          </div>
          <!--end::Description-->
        </div>
        <!--end::totalCounts-->

        <!--begin::IncomingCalls-->
        <div class="d-flex align-items-center mb-7">
          <!--begin::Symbol-->
          <div class="symbol symbol-45px w-40px me-5">
            <span class="symbol-label bg-lighten">
              <KTIcon icon-name="call" icon-class="fs-1 text-warning" />
            </span>
          </div>
          <!--end::Symbol-->

          <!--begin::Description-->
          <div class="d-flex align-items-center flex-wrap w-100">
            <!--begin::Title-->
            <div class="mb-1 pe-3 flex-grow-1">
              <a href="#" class="fs-5 text-warning fw-bold">
                {{ $t("incomingCalls") }}</a
              >
            </div>
            <!--end::Title-->

            <!--begin::Label-->
            <div class="d-flex align-items-center">
              <div class="fw-bold fs-5 text-warning pe-1">
                {{ workHourStatistics.incomingCalls }}
              </div>
            </div>
            <!--end::Label-->
          </div>
          <!--end::Description-->
        </div>
        <!--end::IncomingCalls-->

        <!--begin:AnsweredCalls-->
        <div class="d-flex align-items-center mb-7">
          <!--begin::Symbol-->
          <div class="symbol symbol-45px w-40px me-5">
            <span class="symbol-label bg-lighten">
              <KTIcon icon-name="call" icon-class="fs-1 text-success" />
            </span>
          </div>
          <!--end::Symbol-->

          <!--begin::Description-->
          <div class="d-flex align-items-center flex-wrap w-100">
            <!--begin::Title-->
            <div class="mb-1 pe-3 flex-grow-1">
              <a href="#" class="fs-5 text-success fw-bold">
                {{ $t("answeredCallCount") }}</a
              >
            </div>
            <!--end::Title-->

            <!--begin::Label-->
            <div class="d-flex align-items-center">
              <div class="fw-bold fs-5 text-success pe-1">
                {{ workHourStatistics.answeredCalls }}
              </div>
            </div>
            <!--end::Label-->
          </div>
          <!--end::Description-->
        </div>
        <!--end:AnsweredCalls-->

        <!--begin::MissedCalls-->
        <div class="d-flex align-items-center mb-7">
          <!--begin::Symbol-->
          <div class="symbol symbol-45px w-40px me-5">
            <span class="symbol-label bg-lighten">
              <KTIcon icon-name="disconnect" icon-class="fs-1 text-danger" />
            </span>
          </div>
          <!--end::Symbol-->

          <!--begin::Description-->
          <div class="d-flex align-items-center flex-wrap w-100">
            <!--begin::Title-->
            <div class="mb-1 pe-3 flex-grow-1">
              <a href="#" class="fs-5 text-danger fw-bold">
                {{ $t("missedCallCount") }}</a
              >
            </div>
            <!--end::Title-->

            <!--begin::Label-->
            <div class="d-flex align-items-center">
              <div class="fw-bold fs-5 text-danger pe-1">
                {{ workHourStatistics.missedCalls }}
              </div>
            </div>
            <!--end::Label-->
          </div>
          <!--end::Description-->
        </div>
        <!--end::MissedCalls-->

        <!--begin::OnBreakCalls-->
        <div class="d-flex align-items-center mb-7">
          <!--begin::Symbol-->
          <div class="symbol symbol-45px w-40px me-5">
            <span class="symbol-label bg-lighten">
              <KTIcon icon-name="coffee" icon-class="fs-1 text-warning" />
            </span>
          </div>
          <!--end::Symbol-->

          <!--begin::Description-->
          <div class="d-flex align-items-center flex-wrap w-100">
            <!--begin::Title-->
            <div class="mb-1 pe-3 flex-grow-1">
              <a href="#" class="fs-5 text-warning fw-bold">
                {{ $t("onBreakCalls") }}</a
              >
            </div>
            <!--end::Title-->

            <!--begin::Label-->
            <div class="d-flex align-items-center">
              <div class="fw-bold fs-5 text-warning pe-1">
                {{ workHourStatistics.onBreakCalls ?? 0 }}
              </div>
            </div>
            <!--end::Label-->
          </div>
          <!--end::Description-->
        </div>
        <!--end::OnBreakCalls-->

        <!--begin::OutgoingCalls-->
        <div class="d-flex align-items-center mb-7">
          <!--begin::Symbol-->
          <div class="symbol symbol-45px w-40px me-5">
            <span class="symbol-label bg-lighten">
              <KTIcon icon-name="call" icon-class="fs-1 text-info" />
            </span>
          </div>
          <!--end::Symbol-->

          <!--begin::Description-->
          <div class="d-flex align-items-center flex-wrap w-100">
            <!--begin::Title-->
            <div class="mb-1 pe-3 flex-grow-1">
              <a href="#" class="fs-5 text-info fw-bold">
                {{ $t("outboundCalls") }}</a
              >
            </div>
            <!--end::Title-->

            <!--begin::Label-->
            <div class="d-flex align-items-center">
              <div class="fw-bold fs-5 text-info pe-1">
                {{ workHourStatistics.outgoingCalls ?? 0 }}
              </div>
            </div>
            <!--end::Label-->
          </div>
          <!--end::Description-->
        </div>
        <!--end::OutgoingCalls-->

        <!--begin::RedirectedCalls-->
        <div class="d-flex align-items-center mb-7">
          <!--begin::Symbol-->
          <div class="symbol symbol-45px w-40px me-5">
            <span class="symbol-label bg-lighten">
              <KTIcon icon-name="disconnect" icon-class="fs-1 text-muted" />
            </span>
          </div>
          <!--end::Symbol-->

          <!--begin::Description-->
          <div class="d-flex align-items-center flex-wrap w-100">
            <!--begin::Title-->
            <div class="mb-1 pe-3 flex-grow-1">
              <a href="#" class="fs-5 text-muted fw-bold">
                {{ $t("redirectedCallCount") }}</a
              >
            </div>
            <!--end::Title-->

            <!--begin::Label-->
            <div class="d-flex align-items-center">
              <div class="fw-bold fs-5 text-muted pe-1">
                {{ workHourStatistics.redirectedCalls }}
              </div>
            </div>
            <!--end::Label-->
          </div>
          <!--end::Description-->
        </div>
        <!--end::MissedCalls-->

        <!--begin::AnsweredCallRate-->
        <div class="d-flex align-items-center mb-7">
          <!--begin::Symbol-->
          <div class="symbol symbol-45px w-40px me-5">
            <span class="symbol-label bg-lighten">
              <KTIcon icon-name="percentage" icon-class="fs-1 text-info" />
            </span>
          </div>
          <!--end::Symbol-->

          <!--begin::Description-->
          <div class="d-flex align-items-center flex-wrap w-100">
            <!--begin::Title-->
            <div class="mb-1 pe-3 flex-grow-1">
              <a href="#" class="fs-5 text-info fw-bold">
                {{ $t("answeredCallRate") }}</a
              >
            </div>
            <!--end::Title-->

            <!--begin::Label-->
            <div class="d-flex align-items-center">
              <div class="fw-bold fs-5 text-info pe-1">
                % {{ workHourStatistics.answeredCallRate }}
              </div>
            </div>
            <!--end::Label-->
          </div>
          <!--end::Description-->
        </div>
        <!--end::AnsweredCallRate-->
      </div>
    </div>
    <!--end::Body-->
  </div>
  <!--end::WorkHours Widget-->
</template>

<script lang="ts">
import { defineComponent, onMounted, ref, watch } from "vue";
import { useUserStatisticsStore } from "@/stores/userStatistics";
import DateHelper from "@/core/helpers/DateHelper";
// import Dropdown3 from "@/components/dropdown/Dropdown3.vue";

export default defineComponent({
  name: "workhours-widget",
  components: {
    // Dropdown3,
  },
  props: {
    widgetClasses: String,
    widgetColor: String,
    chartHeight: Number,
    heading: {
      type: String,
      required: true,
    },
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
    isWorkHours: {
      type: Boolean,
      required: true,
    },
  },
  setup(props) {
    const userStatisticsStore = useUserStatisticsStore();
    const workHourStatistics = ref<WorkHoursStatistics>(
      {} as WorkHoursStatistics,
    );

    const fetchUserWorkHourStatistics = async () => {
      var params = {
        startDate: props.startDate,
        endDate: props.endDate,
        number: props.number,
      };

      const response =
        await userStatisticsStore.fetchUserWorkHourStatistics(params);
      workHourStatistics.value = response;
    };

    const fetchUserNonWorkHourStatistics = async () => {
      var params = {
        startDate: props.startDate,
        endDate: props.endDate,
        number: props.number,
      };

      const response =
        await userStatisticsStore.fetchUserNonWorkHourStatistics(params);
      workHourStatistics.value = response;
    };

    onMounted(() => {
      if (props.isWorkHours) {
        fetchUserWorkHourStatistics();
      } else {
        fetchUserNonWorkHourStatistics();
      }
    });

    watch(
      () => [props.startDate, props.endDate, props.number, props.isWorkHours],
      () => {
        if (props.isWorkHours) {
          fetchUserWorkHourStatistics();
        } else {
          fetchUserNonWorkHourStatistics();
        }
      },
    );

    return {
      workHourStatistics,
      DateHelper,
    };
  },
});
</script>
