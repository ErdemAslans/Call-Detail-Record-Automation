<template>
  <!--begin::Menu 1-->
  <div
    class="menu menu-sub menu-sub-dropdown w-400px w-md-480px"
    data-kt-menu="true"
  >
    <!--begin::Header-->
    <div class="px-7 py-5">
      <div class="fs-5 text-gray-900 fw-bold">{{ $t("reportOptions") }}</div>
    </div>
    <!--end::Header-->

    <!--begin::Menu separator-->
    <div class="separator border-gray-200"></div>
    <!--end::Menu separator-->

    <!--begin::Form-->
    <div class="px-7 py-5">
      <!--begin::Input group-->
      <div class="mb-10">
        <!--begin::Label-->
        <label class="form-label fw-semibold">{{ $t("startDate") }}:</label>
        <!--end::Label-->

        <!--begin::Input-->
        <div>
          <el-date-picker
            type="date"
            locale="tr"
            :placeholder="$t('startDate')"
            :teleported="false"
            popper-class="override-styles"
            name="startDate"
            v-model="filter.startDate"
          />
        </div>
        <!--end::Input-->
      </div>
      <!--end::Input group-->

      <!--begin::Input group EndDate-->
      <div class="mb-10">
        <!--begin::Label-->
        <label class="form-label fw-semibold">{{ $t("endDate") }}:</label>
        <!--end::Label-->

        <!--begin::Input-->
        <div>
          <el-date-picker
            type="date"
            locale="tr"
            :placeholder="$t('endDate')"
            :teleported="false"
            popper-class="override-styles"
            name="endDate"
            v-model="filter.endDate"
          />
        </div>
        <!--end::Input-->
      </div>
      <!--end::Input group End Date-->

      <!--begin::Actions-->
      <div class="d-flex justify-content-end">
        <button
          type="reset"
          class="btn btn-sm btn-light btn-active-light-primary me-2"
          data-kt-menu-dismiss="true"
        >
          {{ $t("reset") }}
        </button>

        <button
          type="submit"
          class="btn btn-sm btn-primary me-2"
          @click="applyFilter"
        >
          {{ $t("applyFilter") }}
        </button>

        <!--begin::Button-->
        <button
          :data-kt-indicator="exportLoading ? 'on' : null"
          type="submit"
          class="btn btn-sm btn-light btn-light-success me-2"
          @click="exportReport"
        >
          <span v-if="!exportLoading" class="indicator-label">
            <!-- <KTIcon icon-name="file-down" icon-class="fs-3 ms-2 me-0" /> -->
            {{ $t("export") }}
          </span>
          <span v-if="exportLoading" class="indicator-progress">
            {{ $t("pleaseWait") }}
            <span
              class="spinner-border spinner-border-sm align-middle ms-2"
            ></span>
          </span>
        </button>
        <!--end::Button-->
      </div>
      <!--end::Actions-->
    </div>
    <!--end::Form-->
  </div>
  <!--end::Menu 1-->
</template>

<script lang="ts">
import DateHelper from "@/core/helpers/DateHelper";
import { defineComponent, ref } from "vue";
import { useUserStatisticsStore } from "@/stores/userStatistics";

interface Filter {
  startDate: string;
  endDate: string;
  number: string;
}

export default defineComponent({
  name: "profile-report",
  components: {},
  props: {
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
  setup(props, { emit }) {
    const exportLoading = ref(false);
    const filter = ref<Filter>({
      startDate: props.startDate,
      endDate: props.endDate,
      number: props.number,
    });

    const userStatisticsStore = useUserStatisticsStore();

    const applyFilter = () => {
      const formattedStartDate = filter.value.startDate
        ? DateHelper.formatDate(filter.value.startDate)
        : "";
      const formattedEndDate = filter.value.endDate
        ? DateHelper.formatDate(filter.value.endDate)
        : "";
      emit("update-dates", formattedStartDate, formattedEndDate);
    };

    const exportReport = async () => {
      exportLoading.value = true;
      const formattedStartDate = filter.value.startDate
        ? DateHelper.formatDate(filter.value.startDate)
        : "";
      const formattedEndDate = filter.value.endDate
        ? DateHelper.formatDate(filter.value.endDate)
        : "";

      await userStatisticsStore.exportUserSpecificReport({
        startDate: formattedStartDate,
        endDate: formattedEndDate,
        number: filter.value.number,
      });
      exportLoading.value = false;
    };

    return {
      filter,
      exportLoading,
      applyFilter,
      exportReport,
    };
  },
});
</script>
