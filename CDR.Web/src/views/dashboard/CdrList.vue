<template>
  <!--begin::Card-->
  <div class="card card-xl-stretch mb-xl-8">
    <!--begin::Card header-->
    <div class="card-header border-0 pt-6">
      <!--begin::Card title-->
      <div class="card-title">
        <!--begin::Search-->
        <div class="d-flex align-items-center position-relative my-1">
          <!-- <KTIcon
            icon-name="magnifier"
            icon-class="fs-1 position-absolute ms-6"
          /> -->
        </div>
        <!--end::Search-->
        <!-- Çağrı Kayıtları -->
        {{ translate("callRecords") }}
      </div>
      <!--begin::Card title-->

      <!--begin::Card toolbar-->
      <div class="card-toolbar">
        <!--begin::Toolbar-->
        <!--end::Toolbar-->
      </div>
      <!--end::Card toolbar-->
    </div>
    <!--end::Card header-->

    <!--begin::Card body-->
    <div class="card-body pt-0">
      <!--begin::Accordion-->
      <div class="accordion" id="kt_accordion_1">
        <div class="accordion-item">
          <h2 class="accordion-header" id="kt_accordion_1_header_1">
            <button
              class="accordion-button fs-4 fw-semibold collapsed"
              type="button"
              data-bs-toggle="collapse"
              data-bs-target="#kt_accordion_1_body_1"
              aria-expanded="true"
              aria-controls="kt_accordion_1_body_1"
            >
              {{ translate("filters") }}
            </button>
          </h2>
          <div
            id="kt_accordion_1_body_1"
            class="accordion-collapse collapse"
            aria-labelledby="kt_accordion_1_header_1"
            data-bs-parent="#kt_accordion_1"
          >
            <div class="accordion-body">
              <!-- @submit="submitAuthCodeForm()"
            :validation-schema="schema2" -->
              <VForm class="form" @submit="submit">
                <div class="row mb-5">
                  <ul class="nav">
                    <li class="nav-item">
                      <a
                        class="nav-link btn btn-sm btn-color-muted btn-active btn-active-light-primary active fw-bold px-4 me-1"
                        data-bs-toggle="tab"
                        href="#kt_table_widget_4_tab_1"
                        >{{ translate("month") }}</a
                      >
                    </li>

                    <li class="nav-item">
                      <a
                        class="nav-link btn btn-sm btn-color-muted btn-active btn-active-light-primary fw-bold px-4 me-1"
                        data-bs-toggle="tab"
                        href="#kt_table_widget_4_tab_2"
                        >{{ translate("week") }}</a
                      >
                    </li>

                    <li class="nav-item">
                      <a
                        class="nav-link btn btn-sm btn-color-muted btn-active btn-active-light-primary fw-bold px-4"
                        data-bs-toggle="tab"
                        href="#kt_table_widget_4_tab_3"
                        >{{ translate("day") }}</a
                      >
                    </li>
                  </ul>
                </div>
                <div class="row mb-5">
                  <!--begin: StartDate -->
                  <div class="col-md-3 mb-10 fv-row">
                    <label class="fs-6 fw-bold form-label mb-2">
                      {{ translate("startDate") }}
                    </label>
                    <Field
                      name="startDate"
                      type="text"
                      class="form-control form-control-solid"
                      v-model="filters.startDate"
                    >
                      <el-date-picker
                        type="date"
                        locale="tr"
                        :placeholder="translate('startDate')"
                        :teleported="false"
                        popper-class="override-styles"
                        name="startDate"
                        v-model="filters.startDate"
                      />
                    </Field>

                    <ErrorMessage class="invalid-feedback" name="startDate" />
                  </div>
                  <!--end:StartDate -->

                  <!--begin: EndDate -->
                  <div class="col-md-3 mb-10 fv-row">
                    <label class="fs-6 fw-bold form-label mb-2">
                      {{ translate("endDate") }}
                    </label>
                    <Field
                      name="endDate"
                      type="text"
                      class="form-control form-control-solid"
                      v-model="filters.endDate"
                    >
                      <el-date-picker
                        type="date"
                        locale="tr"
                        :placeholder="translate('endDate')"
                        :teleported="false"
                        popper-class="override-styles"
                        v-model="filters.endDate"
                        name="endDate"
                      />
                    </Field>

                    <ErrorMessage class="invalid-feedback" name="endDate" />
                  </div>
                  <!--end:EndDate -->

                  <!--begin: Department -->
                  <!-- <div class="col-md-2 mb-10 fv-row">
                    <label class="fs-6 fw-bold form-label mb-2">
                      {{ translate("department") }}
                    </label>
                    <Field
                      name="department"
                      class="form-select form-select-sm"
                      v-model="filters.department"
                    >
                      <el-select
                        v-model="filters.department"
                        placeholder="Select Department"
                        clearable
                        filterable
                      >
                        <el-option
                          v-for="department in departments"
                          :key="department.id"
                          :label="department.name"
                          :value="department.id"
                        >
                        </el-option>
                      </el-select>
                    </Field>
                    <ErrorMessage class="invalid-feedback" name="department" />
                  </div> -->
                  <!--end: Department -->

                  <!--begin: CallDirection -->
                  <div class="col-md-2 mb-10 fv-row">
                    <label class="fs-6 fw-bold form-label mb-2">
                      {{ translate("callDirection") }}
                    </label>
                    <Field
                      name="callDirection"
                      class="form-select form-select-sm"
                      v-model="filters.callDirection"
                    >
                      <el-select
                        v-model="filters.callDirection"
                        :placeholder="translate('selectCallDirection')"
                        clearable
                        filterable
                      >
                        <el-option
                          :label="translate('incoming')"
                          value="1"
                          :key="1"
                        ></el-option>
                        <el-option
                          :label="translate('outgoing')"
                          value="2"
                          :key="2"
                        ></el-option>
                        <el-option
                          :label="translate('internal')"
                          value="3"
                          :key="3"
                        ></el-option>
                      </el-select>
                    </Field>
                    <ErrorMessage
                      class="invalid-feedback"
                      name="callDirection"
                    />
                  </div>
                  <!--end: CallDirection -->

                  <!--start: User-->
                  <div class="col-md-4 mb-10 fv-row">
                    <label class="fs-6 fw-bold form-label mb-2">
                      {{ translate("user") }}
                    </label>
                    <Field
                      name="user"
                      v-model="filters.user"
                      class="form-select form-select-sm"
                    >
                      <el-select
                        v-model="filters.user"
                        :placeholder="translate('selectUser')"
                        clearable
                        filterable
                      >
                        <el-option
                          v-for="operator in operators"
                          :key="operator.number"
                          :label="`${operator.name}-${operator.number}`"
                          :value="operator.number"
                        >
                          <span
                            >{{ operator.name }} -
                            <span class="text-info">{{
                              operator.number
                            }}</span></span
                          >
                        </el-option>
                      </el-select>
                    </Field>
                    <ErrorMessage class="invalid-feedback" name="user" />
                  </div>
                  <!--end: User-->
                </div>

                <div class="row mb-5">
                  <div class="col-md-12 text-end">
                    <!--begin::Button-->
                    <button
                      ref="submitButtonRef"
                      type="submit"
                      id="kt_cdrList_submit"
                      class="btn btn-primary"
                      :disabled="loading"
                    >
                      <span class="indicator-label">{{
                        translate("search")
                      }}</span>
                      <span class="indicator-progress">
                        {{ translate("searching") }}
                        <span
                          class="spinner-border spinner-border-sm align-middle ms-2"
                        ></span>
                      </span>
                    </button>
                    <!--end::Button-->
                  </div>
                </div>
              </VForm>
            </div>
          </div>
        </div>
      </div>
      <!--end::Accordion-->

      <KTDatatable
        @on-sort="sort"
        @on-items-select="onItemSelect"
        @page-change="onPageChange"
        @on-items-per-page-change="onItemsPerPageChange"
        :data="callRecords"
        :total="callRecords.totalCount"
        :current-page="callRecords.pageIndex"
        :header="headerConfig"
        :checkbox-enabled="false"
        :items-per-page="callRecords.pageSize"
      >
        <template v-slot:callingPartyNumber="{ row: cdr }">
          <router-link
            :to="{
              name: 'user-profile',
              params: { number: cdr.callingPartyNumber || 'default' },
            }"
          >
            {{ cdr.userName }}
            <span class="fw-semibold text-muted d-block fs-7">{{
              cdr.callingPartyNumber
            }}</span>
            <span class="fw-semibold text-info d-block fs-7">
              {{ cdr.departmentName }}</span
            >
          </router-link>
        </template>
        <!-- <template v-slot:departmentName="{ row: cdr }">
          {{ cdr.departmentName }}
        </template> -->
        <template v-slot:originalCalledPartyNumber="{ row: cdr }">
          <router-link
            :to="{
              name: 'user-profile',
              params: { number: cdr.originalCalledPartyNumber || 'default' },
            }"
          >
            {{ cdr.originalCalledPartyUserName }}
            <span class="fw-semibold text-muted d-block fs-7">{{
              cdr.originalCalledPartyNumber
            }}</span>
            <span class="fw-semibold text-info d-block fs-7">
              {{ cdr.originalCalledPartyDepartmentName }}</span
            >
          </router-link>
        </template>
        <template v-slot:finalCalledPartyNumber="{ row: cdr }">
          <router-link
            :to="{
              name: 'user-profile',
              params: { number: cdr.finalCalledPartyNumber || 'default' },
            }"
          >
            {{ cdr.finalCalledPartyUserName }}
            <span class="text-muted fw-semibold text-muted d-block fs-7">{{
              cdr.finalCalledPartyNumber
            }}</span>
            <span class="fw-semibold text-info d-block fs-7">
              {{ cdr.finalCalledPartyDepartmentName }}</span
            >
          </router-link>
        </template>
        <template v-slot:dateTimeOrigination="{ row: cdr }">
          <div>
            <span v-if="cdr.dateTimeOrigination">
              {{
                DateHelper.toLocaleDateTimeStringWithCulture(
                  new Date(cdr.dateTimeOrigination),
                )
              }}
            </span>
          </div>
        </template>

        <template v-slot:callAwaitDuration="{ row: cdr }">
          <span
            class="text-end fs-7 fw-bold"
            :class="`text-${callAwaitDurationColorDecider(cdr.callAwaitDuration)}`"
          >
            {{ DateHelper.formatDuration(cdr.callAwaitDuration) }}
          </span>
        </template>

        <template v-slot:duration="{ row: cdr }">
          <span
            class="text-end fs-7 fw-bold"
            :class="`text-${callDurationColorDecider(cdr.duration)}`"
          >
            {{ DateHelper.formatDuration(cdr.duration) }}
          </span>
        </template>

        <template v-slot:hasRedirected="{ row: cdr }">
          <span
            :class="`badge-light-${cdr.hasRedirected ? 'warning' : 'success'}`"
            class="badge fw-semibold me-1"
            >{{ cdr.hasRedirected ? translate("yes") : translate("no") }}</span
          >
        </template>

        <template v-slot:callDirection="{ row: cdr }">
          <span
            :class="`badge-light-${CallDirectionHelper.getColor(cdr.callDirection)}`"
            class="badge fw-semibold me-1"
            >{{ CallDirectionHelper.getDescription(cdr.callDirection) }}</span
          >
        </template>

        <!-- <template v-slot:redirectReason="{ row: cdr }">
          <span
            v-if="cdr.hasRedirected"
            :class="`badge-light-${RedirectReasonHelper.getColor(cdr.redirectReason)}`"
            class="badge fw-semibold me-1"
            >{{ RedirectReasonHelper.getDescription(cdr.redirectReason) }}</span
          >
          <span v-else> --- </span>
        </template> -->

        <template v-slot:callType="{ row: cdr }">
          <span
            :class="`badge-light-${CallTypeHelper.getColor(cdr.callType)}`"
            class="badge fw-semibold me-1"
            >{{ CallTypeHelper.getDescription(cdr.callType) }}</span
          >
        </template>
      </KTDatatable>
    </div>
    <!--end::Card body-->
  </div>
  <!--end::Card-->
</template>

<script lang="ts">
import { getAssetPath } from "@/core/helpers/assets";
import { computed, defineComponent, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";
import { Field, ErrorMessage, Form as VForm } from "vee-validate";
import { useDashboardStore } from "@/stores/dashboard";
import DateHelper from "@/core/helpers/DateHelper";
import { CallEndedReasonHelper } from "@/core/enums/CallEndedReason";
import { CallTypeHelper } from "@/core/enums/CallType";
import { RedirectReasonHelper } from "@/core/enums/RedirectReason";
import DurationHelper from "@/core/helpers/DurationHelper";
import { CallDirectionHelper } from "@/core/enums/CallDirection";
import { useOperatorStore } from "@/stores/operator";

import type { Sort } from "@/components/kt-datatable/table-partials/models";
import arraySort from "array-sort";
import KTDatatable from "@/components/kt-datatable/KTDataTable.vue";

export default defineComponent({
  name: "kt-subscription-list",
  components: {
    VForm,
    Field,
    ErrorMessage,
    KTDatatable,
  },
  setup() {
    const { t, te } = useI18n();
    const dashboardStore = useDashboardStore();
    const operatorStore = useOperatorStore();
    const submitButtonRef = ref<HTMLButtonElement | null>(null);

    const operators = computed(() => operatorStore.operators);
    const departments = computed(() => operatorStore.departments);
    const callRecords = ref<PagedResult<ICallDetail>>(
      {} as PagedResult<ICallDetail>,
    );
    const loading = ref<boolean>(false);
    const translate = (text: string) => {
      if (te(text)) {
        return t(text);
      } else {
        return text;
      }
    };

    const selectedIds = ref<Array<number>>([]);

    const { start, end } = DateHelper.getDateRange("week");
    const filters = ref({
      callDirection: undefined,
      user: undefined,
      department: undefined,
      startDate: start,
      endDate: end,
      // ...other filters...
    });

    const fetchCallRecords = async (pageIndex = 1, pageSize = 10) => {
      loading.value = true;

      const formattedStartDate = filters.value.startDate
        ? DateHelper.formatDate(filters.value.startDate)
        : undefined;
      const formattedEndDate = filters.value.endDate
        ? DateHelper.formatDate(filters.value.endDate)
        : undefined;

      const data = await dashboardStore.fetchCallRecords({
        startDate: formattedStartDate,
        endDate: formattedEndDate,
        callDirection: filters.value.callDirection,
        user: filters.value.user,
        department: filters.value.department,
        pageIndex: pageIndex,
        pageSize: pageSize,
      });
      callRecords.value = data;
      loading.value = false;
    };

    const fetchOperators = async () => {
      await operatorStore.fetchOperators();
    };

    const fetchDepartments = async () => {
      await operatorStore.fetchDepartments();
    };

    const submit = async () => {
      if (submitButtonRef.value) {
        // eslint-disable-next-line
        submitButtonRef.value!.disabled = true;
        // Activate indicator
        submitButtonRef.value.setAttribute("data-kt-indicator", "on");
      }
      await fetchCallRecords(1, callRecords.value.pageSize).finally(() => {
        //Deactivate indicator
        submitButtonRef.value?.removeAttribute("data-kt-indicator");
        // eslint-disable-next-line
        submitButtonRef.value!.disabled = false;
      });
    };

    onMounted(() => {
      fetchCallRecords();
      fetchOperators();
      fetchDepartments();
    });

    const callDurationColorDecider = DurationHelper.callDurationColorDecider;
    const callAwaitDurationColorDecider =
      DurationHelper.callAwaitDurationColorDecider;

    const headerConfig = ref([
      {
        columnName: translate("callingPartyNumber"),
        columnLabel: "callingPartyNumber",
        sortEnabled: true,
      },
      // {
      //   columnName: translate("departmentName"),
      //   columnLabel: "departmentName",
      //   sortEnabled: true,
      // },
      {
        columnName: translate("originalCalledPartyNumber"),
        columnLabel: "originalCalledPartyNumber",
        sortEnabled: true,
      },
      {
        columnName: translate("finalCalledPartyNumber"),
        columnLabel: "finalCalledPartyNumber",
        sortEnabled: true,
      },
      {
        columnName: translate("dateTimeOrigination"),
        columnLabel: "dateTimeOrigination",
        sortEnabled: true,
      },
      {
        columnName: translate("callAwaitDuration"),
        columnLabel: "callAwaitDuration",
        sortEnabled: true,
      },
      {
        columnName: translate("duration"),
        columnLabel: "duration",
        sortEnabled: true,
      },
      {
        columnName: translate("hasRedirected"),
        columnLabel: "hasRedirected",
        sortEnabled: true,
      },
      {
        columnName: translate("callDirection"),
        columnLabel: "callDirection",
        sortEnabled: true,
      },
      // {
      //   columnName: translate("redirectReason"),
      //   columnLabel: "redirectReason",
      //   sortEnabled: true,
      // },
      {
        columnName: translate("callType"),
        columnLabel: "callType",
        sortEnabled: true,
      },
    ]);

    const sort = (sort: Sort) => {
      const reverse: boolean = sort.order === "asc";
      if (sort.label) {
        arraySort(callRecords.value.items, sort.label, { reverse });
      }
    };

    const onItemSelect = (selectedItems: Array<number>) => {
      selectedIds.value = selectedItems;
    };

    const onPageChange = (page: number) => {
      fetchCallRecords(page, callRecords.value.pageSize);
    };

    const onItemsPerPageChange = (pageSize: number) => {
      fetchCallRecords(1, pageSize);
    };

    return {
      translate,
      callRecords,
      getAssetPath,
      onPageChange,
      fetchCallRecords,
      DateHelper,
      loading,
      CallEndedReasonHelper,
      CallTypeHelper,
      RedirectReasonHelper,
      callDurationColorDecider,
      callAwaitDurationColorDecider,
      sort,
      onItemSelect,
      headerConfig,
      onItemsPerPageChange,
      CallDirectionHelper,
      filters,
      submit,
      submitButtonRef,
      operators,
      departments,
    };
  },
});
</script>
