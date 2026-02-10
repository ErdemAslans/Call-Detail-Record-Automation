<template>
  <div class="card card-xl-stretch mb-xl-8">
    <!--begin::Card header-->
    <div class="card-header border-0 pt-6">
      <div class="card-title">
        <KTIcon icon-name="time" icon-class="fs-1 me-3 text-info" />
        <span class="fs-3 fw-bold">{{ translate("reportExecutionHistory") }}</span>
      </div>
      <div class="card-toolbar">
        <button
          type="button"
          class="btn btn-sm btn-light-primary"
          :disabled="isLoading"
          @click="refreshHistory"
        >
          <KTIcon icon-name="arrows-circle" icon-class="fs-4" />
          {{ translate("refresh") }}
        </button>
      </div>
    </div>
    <!--end::Card header-->

    <!--begin::Card body-->
    <div class="card-body pt-0">
      <!-- Loading state -->
      <div v-if="isLoading" class="text-center py-10">
        <span class="spinner-border spinner-border-lg text-primary"></span>
        <p class="text-muted mt-3">{{ translate("loadingHistory") }}</p>
      </div>

      <!-- Empty state -->
      <div v-else-if="executionHistory.length === 0" class="text-center py-10">
        <KTIcon icon-name="document" icon-class="fs-3x text-muted mb-5" />
        <h4 class="fs-5 fw-semibold text-muted">{{ translate("noReportHistory") }}</h4>
        <p class="text-muted fs-7">{{ translate("noReportHistoryDesc") }}</p>
      </div>

      <!-- History table -->
      <div v-else class="table-responsive">
        <table class="table table-row-dashed table-row-gray-300 align-middle gs-0 gy-4">
          <thead>
            <tr class="fw-bold text-muted bg-light">
              <th class="ps-4 min-w-100px rounded-start">{{ translate("reportType") }}</th>
              <th class="min-w-150px">{{ translate("period") }}</th>
              <th class="min-w-100px">{{ translate("status") }}</th>
              <th class="min-w-100px">{{ translate("trigger") }}</th>
              <th class="min-w-100px">{{ translate("records") }}</th>
              <th class="min-w-100px">{{ translate("emailStatus") }}</th>
              <th class="min-w-150px">{{ translate("executedAt") }}</th>
              <th class="min-w-100px text-end rounded-end">{{ translate("actions") }}</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="execution in executionHistory" :key="execution.id">
              <!-- Report Type -->
              <td class="ps-4">
                <span class="badge" :class="getReportTypeBadge(execution.reportType)">
                  {{ translate(`reportType_${execution.reportType}`) }}
                </span>
              </td>

              <!-- Period -->
              <td>
                <span class="text-dark fw-semibold d-block fs-7">
                  {{ formatDate(execution.periodStartDate) }}
                </span>
                <span class="text-muted fw-semibold d-block fs-8">
                  {{ formatDate(execution.periodEndDate) }}
                </span>
              </td>

              <!-- Execution Status -->
              <td>
                <span class="badge" :class="getStatusBadge(execution.executionStatus)">
                  {{ translate(`execStatus_${execution.executionStatus}`) }}
                </span>
              </td>

              <!-- Trigger Type -->
              <td>
                <span class="text-muted fs-7">
                  {{ translate(`trigger_${execution.triggerType}`) }}
                </span>
              </td>

              <!-- Records Processed -->
              <td>
                <span v-if="execution.recordsProcessed" class="text-dark fw-semibold">
                  {{ execution.recordsProcessed.toLocaleString("tr-TR") }}
                </span>
                <span v-else class="text-muted">-</span>
              </td>

              <!-- Email Status -->
              <td>
                <div v-if="execution.totalEmailRecipients > 0">
                  <span class="text-success fw-semibold">
                    {{ execution.successfulEmailDeliveries }}
                  </span>
                  <span class="text-muted">/</span>
                  <span class="text-dark">{{ execution.totalEmailRecipients }}</span>
                  <span v-if="execution.failedEmailDeliveries > 0" class="text-danger ms-1">
                    ({{ execution.failedEmailDeliveries }} {{ translate("failed") }})
                  </span>
                </div>
                <span v-else class="text-muted">-</span>
              </td>

              <!-- Executed At -->
              <td>
                <span class="text-dark fw-semibold d-block fs-7">
                  {{ formatDateTime(execution.startedAt) }}
                </span>
                <span v-if="execution.completedAt" class="text-muted fw-semibold d-block fs-8">
                  {{ formatDuration(execution.startedAt, execution.completedAt) }}
                </span>
              </td>

              <!-- Actions -->
              <td class="text-end">
                <button
                  v-if="execution.executionStatus === 'Completed'"
                  type="button"
                  class="btn btn-icon btn-light-primary btn-sm me-1"
                  :title="translate('downloadReport')"
                  @click="handleDownload(execution.id)"
                >
                  <KTIcon icon-name="file-down" icon-class="fs-4" />
                </button>
                <button
                  v-if="execution.errorMessage"
                  type="button"
                  class="btn btn-icon btn-light-danger btn-sm"
                  :title="translate('viewError')"
                  @click="showError(execution.errorMessage)"
                >
                  <KTIcon icon-name="information-5" icon-class="fs-4" />
                </button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
    <!--end::Card body-->
  </div>

  <!-- Error Modal -->
  <el-dialog v-model="errorModalVisible" :title="translate('errorDetails')" width="500px">
    <div class="alert alert-danger mb-0">
      <pre class="mb-0 text-wrap">{{ currentErrorMessage }}</pre>
    </div>
    <template #footer>
      <button type="button" class="btn btn-light" @click="errorModalVisible = false">
        {{ translate("close") }}
      </button>
    </template>
  </el-dialog>
</template>

<script setup lang="ts">
import { ref, onMounted } from "vue";
import { useI18n } from "vue-i18n";
import { storeToRefs } from "pinia";
import { useEmailReportStore } from "@/stores/emailReport";
import KTIcon from "@/core/helpers/kt-icon/KTIcon.vue";

// i18n
const { t, te } = useI18n();
const translate = (text: string) => (te(text) ? t(text) : text);

// Store
const emailReportStore = useEmailReportStore();
const { isLoading, executionHistory } = storeToRefs(emailReportStore);

// Local state
const errorModalVisible = ref(false);
const currentErrorMessage = ref("");

// Format helpers
const formatDate = (dateStr: string): string => {
  return new Date(dateStr).toLocaleDateString("tr-TR");
};

const formatDateTime = (dateStr: string): string => {
  return new Date(dateStr).toLocaleString("tr-TR");
};

const formatDuration = (start: string, end: string): string => {
  const startTime = new Date(start).getTime();
  const endTime = new Date(end).getTime();
  const durationMs = endTime - startTime;
  
  if (durationMs < 1000) return `${durationMs}ms`;
  if (durationMs < 60000) return `${(durationMs / 1000).toFixed(1)}s`;
  return `${Math.floor(durationMs / 60000)}m ${Math.round((durationMs % 60000) / 1000)}s`;
};

const getReportTypeBadge = (reportType: string): string => {
  switch (reportType.toLowerCase()) {
    case "weekly":
      return "badge-light-primary";
    case "monthly":
      return "badge-light-info";
    default:
      return "badge-light-secondary";
  }
};

const getStatusBadge = (status: string): string => {
  switch (status) {
    case "Completed":
      return "badge-light-success";
    case "Failed":
      return "badge-light-danger";
    case "Running":
      return "badge-light-warning";
    default:
      return "badge-light-secondary";
  }
};

// Handlers
const refreshHistory = async () => {
  await emailReportStore.fetchExecutionHistory(20);
};

const handleDownload = async (executionId: string) => {
  await emailReportStore.downloadReport(executionId);
};

const showError = (errorMessage: string) => {
  currentErrorMessage.value = errorMessage;
  errorModalVisible.value = true;
};

// Lifecycle
onMounted(() => {
  refreshHistory();
});
</script>
