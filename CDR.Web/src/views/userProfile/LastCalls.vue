<template>
  <!--begin::Tables Widget 13-->
  <div :class="widgetClasses" class="card">
    <!--begin::Header-->
    <div class="card-header border-0 pt-5">
      <h3 class="card-title align-items-start flex-column">
        <span class="card-label fw-bold fs-3 mb-1">{{
          translate("recentCalls")
        }}</span>
      </h3>
      <div class="card-toolbar">
        <!--begin::Menu-->
        <!--end::Menu-->
      </div>
    </div>
    <!--end::Header-->

    <!--begin::Body-->
    <div class="card-body py-3">
      <!--begin::Table container-->
      <KTDatatable
        @page-change="onPageChange"
        @on-items-per-page-change="onItemsPerPageChange"
        :data="lastCalls"
        :total="lastCalls.totalCount"
        :current-page="lastCalls.pageIndex"
        :header="headerConfig"
        :checkbox-enabled="false"
        :items-per-page="lastCalls.pageSize"
      >
        <!-- CallingPartyNumber -->
        <template v-slot:callingPartyNumber="{ row: call }">
          <router-link
            v-if="call.callingPartyNumber !== number"
            :to="{
              name: 'user-profile',
              params: { number: call.callingPartyNumber || 'default' },
            }"
          >
            {{ call.callingPartyUserName }}
            <span class="fw-semibold text-muted d-block fs-7">{{
              call.callingPartyNumber
            }}</span>
            <span class="fw-semibold text-info d-block fs-7">
              {{ call.callingPartyDepartmentName }}</span
            >
          </router-link>
          <span v-else>
            {{ call.callingPartyUserName }}
            <span class="fw-semibold text-muted d-block fs-7">{{
              call.callingPartyNumber
            }}</span>
            <span class="fw-semibold text-info d-block fs-7">
              {{ call.callingPartyDepartmentName }}</span
            >
          </span>
        </template>

        <!-- OriginalCalledPartyNumber -->
        <template v-slot:originalCalledPartyNumber="{ row: call }">
          <router-link
            v-if="call.originalCalledPartyNumber !== number"
            :to="{
              name: 'user-profile',
              params: { number: call.originalCalledPartyNumber || 'default' },
            }"
          >
            {{ call.originalCalledPartyUserName }}
            <span class="fw-semibold text-muted d-block fs-7">{{
              call.originalCalledPartyNumber
            }}</span>
            <span class="fw-semibold text-info d-block fs-7">
              {{ call.originalCalledPartyDepartmentName }}</span
            >
          </router-link>
          <span v-else>
            {{ call.originalCalledPartyUserName }}
            <span class="fw-semibold text-muted d-block fs-7">{{
              call.originalCalledPartyNumber
            }}</span>
            <span class="fw-semibold text-info d-block fs-7">
              {{ call.originalCalledPartyDepartmentName }}</span
            >
          </span>
        </template>

        <!-- FinalCalledPartyNumber -->
        <template v-slot:finalCalledPartyNumber="{ row: call }">
          <router-link
            v-if="call.finalCalledPartyNumber !== number"
            :to="{
              name: 'user-profile',
              params: { number: call.finalCalledPartyNumber || 'default' },
            }"
          >
            {{ call.finalCalledPartyUserName }}
            <span class="fw-semibold text-muted d-block fs-7">{{
              call.finalCalledPartyNumber
            }}</span>
            <span class="fw-semibold text-info d-block fs-7">
              {{ call.finalCalledPartyDepartmentName }}</span
            >
          </router-link>
          <span v-else>
            {{ call.finalCalledPartyUserName }}
            <span class="fw-semibold text-muted d-block fs-7">{{
              call.finalCalledPartyNumber
            }}</span>
            <span class="fw-semibold text-info d-block fs-7">
              {{ call.finalCalledPartyDepartmentName }}</span
            >
          </span>
        </template>

        <!-- CallDirection -->
        <template v-slot:callDirection="{ row: call }">
          <span
            :class="`badge-light-${CallDirectionHelper.getColor(call.callDirection)}`"
            class="badge fw-semibold me-1"
            >{{ CallDirectionHelper.getDescription(call.callDirection) }}</span
          >
        </template>

        <!-- CallType-->
        <template v-slot:callType="{ row: call }">
          <span
            :class="`badge-light-${CallTypeHelper.getColor(call.callType)}`"
            class="badge fw-semibold me-1"
            >{{ CallTypeHelper.getDescription(call.callType) }}</span
          >
        </template>

        <!-- DateTimeOrigination -->
        <template v-slot:dateTimeOrigination="{ row: call }">
          <div>
            <span v-if="call.dateTimeOrigination">
              {{
                DateHelper.toLocaleDateTimeStringWithCulture(
                  new Date(call.dateTimeOrigination),
                )
              }}
            </span>
          </div>
        </template>

        <!-- Duration-->
        <template v-slot:duration="{ row: call }">
          <span
            class="text-end fs-7 fw-bold"
            :class="`text-${callDurationColorDecider(call.duration)}`"
          >
            {{ DateHelper.formatDuration(call.duration) }}
          </span>
        </template>

        <!--DateTimeDisconnect-->
        <template v-slot:dateTimeDisconnect="{ row: call }">
          <div>
            <span v-if="call.dateTimeDisconnect">
              {{
                DateHelper.toLocaleDateTimeStringWithCulture(
                  new Date(call.dateTimeDisconnect),
                )
              }}
            </span>
          </div>
        </template>
      </KTDatatable>
      <!--end::Table container-->
    </div>
    <!--begin::Body-->
  </div>
  <!--end::Tables Widget 13-->
</template>

<script lang="ts">
import { getAssetPath } from "@/core/helpers/assets";
import { defineComponent, onMounted, ref, inject, watch } from "vue";
import { useI18n } from "vue-i18n";
import KTDatatable from "@/components/kt-datatable/KTDataTable.vue";
import { useUserStatisticsStore } from "@/stores/userStatistics";
import { CallTypeHelper } from "@/core/enums/CallType";
import DurationHelper from "@/core/helpers/DurationHelper";
import DateHelper from "@/core/helpers/DateHelper";
import ResponseMessageService from "@/core/helpers/ResponseMessageService";
import { useRouter } from "vue-router";
import { CallDirectionHelper } from "@/core/enums/CallDirection";
import { number } from "yup";

export default defineComponent({
  name: "kt-profile-lastCalls",
  components: {
    KTDatatable,
  },
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
    const router = useRouter();
    const userStatisticsStore = useUserStatisticsStore();
    const lastCalls = ref<PagedResult<ILastCall>>({} as PagedResult<ILastCall>);
    const loading = ref<boolean>(false);
    const callDurationColorDecider = DurationHelper.callDurationColorDecider;
    const translate = (text: string) => {
      if (te(text)) {
        return t(text);
      } else {
        return text;
      }
    };

    // const number = inject<string | undefined>("number");

    const fetchUserLastCalls = async (pageIndex = 1, pageSize = 10) => {
      if (!number) {
        ResponseMessageService.showMessageByType(
          "Number is undefined. Cannot fetch user last calls.",
          "warning",
        );
        return;
      }

      loading.value = true;
      try {
        const response = await userStatisticsStore.fetchUserLastCalls({
          startDate: props.startDate,
          endDate: props.endDate,
          pageIndex: pageIndex,
          pageSize: pageSize,
          number: props.number,
        });
        lastCalls.value = response;
      } catch (error) {
        console.error(error);
      } finally {
        loading.value = false;
      }
    };

    const onPageChange = (page: number) => {
      fetchUserLastCalls(page, lastCalls.value.pageSize);
    };

    const onItemsPerPageChange = (pageSize: number) => {
      fetchUserLastCalls(1, pageSize);
    };

    const headerConfig = [
      {
        columnName: translate("callingPartyNumber"),
        columnLabel: "callingPartyNumber",
        sortEnabled: false,
      },
      {
        columnName: translate("originalCalledPartyNumber"),
        columnLabel: "originalCalledPartyNumber",
        sortEnabled: false,
      },
      {
        columnName: translate("finalCalledPartyNumber"),
        columnLabel: "finalCalledPartyNumber",
        sortEnabled: false,
      },
      {
        columnName: translate("callDirection"),
        columnLabel: "callDirection",
        sortEnabled: false,
      },
      {
        columnName: translate("callType"),
        columnLabel: "callType",
        sortEnabled: false,
      },
      {
        columnName: translate("dateTimeOrigination"),
        columnLabel: "dateTimeOrigination",
        sortEnabled: false,
      },
      {
        columnName: translate("duration"),
        columnLabel: "duration",
        sortEnabled: false,
      },
      {
        columnName: translate("dateTimeDisconnect"),
        columnLabel: "dateTimeDisconnect",
        sortEnabled: false,
      },
    ];

    onMounted(() => {
      fetchUserLastCalls();
    });

    watch(
      () => [props.startDate, props.endDate, props.number],
      () => {
        fetchUserLastCalls();
      },
    );

    const openInNewTab = (number: string) => {
      const routeData = router.resolve({
        name: "user-profile",
        params: { number },
      });
      window.open(routeData.href, "_blank");
    };

    return {
      getAssetPath,
      translate,
      lastCalls,
      onPageChange,
      onItemsPerPageChange,
      headerConfig,
      CallTypeHelper,
      callDurationColorDecider,
      DateHelper,
      openInNewTab,
      CallDirectionHelper,
    };
  },
});
</script>
