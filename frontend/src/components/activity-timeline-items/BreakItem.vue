<template>
  <template v-if="breakItem.type === 'date'">
    <!--begin::Date Separator-->
    <h5 class="fw-bold text-gray-800 fs-5 pb-5">
      {{
        DateHelper.toLocaleDateStringWithCulture(new Date(breakItem.breakTime))
      }}
    </h5>
    <!--end::Date Separator-->
  </template>
  <!--begin::Timeline item-->
  <div v-else class="timeline-item">
    <!--begin::Timeline line-->
    <div class="timeline-line w-40px"></div>
    <!--end::Timeline line-->

    <!--begin::Timeline icon-->
    <div class="timeline-icon symbol symbol-circle symbol-40px">
      <div class="symbol-label bg-light">
        <KTIcon
          :icon-name="breakItem.type === 'breakStart' ? 'timer' : 'watch'"
          :icon-class="
            breakItem.type === 'breakStart'
              ? 'fs-2 text-danger'
              : 'fs-2 text-success'
          "
        />
      </div>
    </div>
    <!--end::Timeline icon-->

    <!--begin::Timeline content-->
    <div class="timeline-content mb-10 mt-n1">
      <!--begin::Timeline heading-->
      <div class="pe-3 mb-5">
        <!--begin::Title-->
        <div
          :class="
            breakItem.type === 'breakStart'
              ? 'fs-5 fw-semibold mb-2 text-danger'
              : 'fs-5 fw-semibold mb-2 text-success'
          "
        >
          {{ $t(breakItem.type) }}
        </div>
        <!--end::Title-->

        <!--begin::Description-->
        <div class="d-flex align-items-center mt-1 fs-6">
          <!--begin::Info-->
          <div
            :class="
              breakItem.type === 'breakStart'
                ? 'text-danger me-2 fs-7'
                : 'text-success me-2 fs-7'
            "
          >
            {{
              DateHelper.toLocaleTimeStringWithCulture(
                new Date(breakItem.breakTime),
              )
            }}
          </div>
          <!--end::Info-->
        </div>
        <!--end::Description-->
      </div>
      <!--end::Timeline heading-->

      <!--begin::Timeline details-->
      <div v-if="breakItem.type !== 'breakEnd'" class="overflow-auto pb-5">
        <div
          class="notice d-flex bg-light-primary rounded border-primary border border-dashed min-w-lg-600px flex-shrink-0 p-6"
        >
          <!--begin::Icon-->
          <KTIcon icon-name="notepad" icon-class="fs-2tx text-primary me-4" />
          <!--end::Icon-->

          <!--begin::Wrapper-->
          <div class="d-flex flex-stack flex-grow-1 flex-wrap flex-md-nowrap">
            <!--begin::Content-->
            <div class="mb-3 mb-md-0 fw-semibold">
              <h4 class="text-gray-800 fw-bold">
                {{ breakItem.reason }}
              </h4>
            </div>
            <!--end::Content-->

            <!--begin::Action-->
            <button
              v-if="!breakItem.isEnd"
              class="btn btn-success px-6 align-self-center text-nowrap"
              @click="handleEndBreak"
            >
              <KTIcon icon-name="timer" icon-class="fs-3 text-white me-2" />
              {{ $t("breaks_end") }}
            </button>
            <!--end::Action-->
          </div>
          <!--end::Wrapper-->
        </div>
      </div>
      <!--end::Timeline details-->
    </div>
    <!--end::Timeline content-->
  </div>
  <!--end::Timeline item-->
</template>

<script lang="ts">
import { computed, defineComponent } from "vue";
import { getAssetPath } from "@/core/helpers/assets";
import DateHelper from "@/core/helpers/DateHelper";
import { useBreaksStore } from "@/stores/breaksTime";

export default defineComponent({
  name: "BreakItem",
  props: {
    breakItem: {
      type: Object as () => FormatedBreakTimesItems,
      required: true,
    },
  },
  emits: ["breakEnded"],
  setup(props, { emit }) {
    const breaksStore = useBreaksStore();

    const handleEndBreak = async () => {
      await breaksStore.endBreak(props.breakItem.id);
      emit("breakEnded");
    };

    const formattedTime = computed(() => {
      return new Date(props.breakItem.breakTime).toLocaleTimeString();
    });

    return {
      getAssetPath,
      formattedTime,
      DateHelper,
      handleEndBreak,
    };
  },
});
</script>
