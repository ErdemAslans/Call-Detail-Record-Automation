<template>
  <div class="card card-xl-stretch mb-xl-8">
    <!--begin::Card header-->
    <div class="card-header border-0 pt-6">
      <div class="card-title">
        <KTIcon icon-name="envelope" icon-class="fs-1 me-3 text-primary" />
        <span class="fs-3 fw-bold">{{ translate("emailReportGenerator") }}</span>
      </div>
      <div class="card-toolbar">
        <button
          v-if="hasCurrentReport"
          type="button"
          class="btn btn-sm btn-light-danger"
          @click="clearReport"
        >
          <KTIcon icon-name="cross" icon-class="fs-4" />
          {{ translate("clearReport") }}
        </button>
      </div>
    </div>
    <!--end::Card header-->

    <!--begin::Card body-->
    <div class="card-body pt-0">
      <!-- Step 1: Report Type Selection -->
      <div v-if="!hasCurrentReport" class="mb-10">
        <h4 class="fs-5 fw-semibold mb-5">{{ translate("selectReportType") }}</h4>
        
        <div class="row g-5">
          <!-- Weekly Report Card -->
          <div class="col-md-6">
            <div
              class="card border border-2 cursor-pointer h-100"
              :class="{ 'border-primary bg-light-primary': selectedReportType === 'weekly' }"
              @click="selectedReportType = 'weekly'"
            >
              <div class="card-body text-center py-10">
                <KTIcon icon-name="calendar" icon-class="fs-3x text-primary mb-5" />
                <h3 class="fs-4 fw-bold mb-3">{{ translate("weeklyReport") }}</h3>
                <p class="text-muted fs-6">{{ translate("weeklyReportDesc") }}</p>
                <el-radio v-model="selectedReportType" value="weekly" class="mt-3" />
              </div>
            </div>
          </div>

          <!-- Monthly Report Card -->
          <div class="col-md-6">
            <div
              class="card border border-2 cursor-pointer h-100"
              :class="{ 'border-primary bg-light-primary': selectedReportType === 'monthly' }"
              @click="selectedReportType = 'monthly'"
            >
              <div class="card-body text-center py-10">
                <KTIcon icon-name="calendar-2" icon-class="fs-3x text-info mb-5" />
                <h3 class="fs-4 fw-bold mb-3">{{ translate("monthlyReport") }}</h3>
                <p class="text-muted fs-6">{{ translate("monthlyReportDesc") }}</p>
                <el-radio v-model="selectedReportType" value="monthly" class="mt-3" />
              </div>
            </div>
          </div>
        </div>

        <!-- Custom Date Range (Optional) -->
        <div class="mt-8">
          <el-collapse>
            <el-collapse-item :title="translate('customDateRange')" name="custom">
              <div class="row mt-4">
                <div class="col-md-6">
                  <label class="fs-6 fw-semibold mb-2">{{ translate("startDate") }}</label>
                  <el-date-picker
                    v-model="customStartDate"
                    type="date"
                    :placeholder="translate('selectStartDate')"
                    class="w-100"
                    :disabled-date="disabledStartDate"
                    format="DD/MM/YYYY"
                    value-format="YYYY-MM-DD"
                  />
                </div>
                <div class="col-md-6">
                  <label class="fs-6 fw-semibold mb-2">{{ translate("endDate") }}</label>
                  <el-date-picker
                    v-model="customEndDate"
                    type="date"
                    :placeholder="translate('selectEndDate')"
                    class="w-100"
                    :disabled-date="disabledEndDate"
                    format="DD/MM/YYYY"
                    value-format="YYYY-MM-DD"
                  />
                </div>
              </div>
            </el-collapse-item>
          </el-collapse>
        </div>

        <!-- Generate Button -->
        <div class="text-center mt-10">
          <button
            type="button"
            class="btn btn-primary btn-lg"
            :disabled="isGenerating || !selectedReportType"
            @click="handleGenerateReport"
          >
            <span v-if="!isGenerating">
              <KTIcon icon-name="document" icon-class="fs-3 me-2" />
              {{ translate("generateReport") }}
            </span>
            <span v-else class="indicator-progress d-block">
              {{ translate("generatingReport") }}
              <span class="spinner-border spinner-border-sm align-middle ms-2"></span>
            </span>
          </button>
        </div>

        <!-- Error Display -->
        <div v-if="errors.generation" class="alert alert-danger mt-5">
          <div class="d-flex align-items-center">
            <KTIcon icon-name="information-5" icon-class="fs-2 text-danger me-3" />
            <div>{{ errors.generation }}</div>
          </div>
        </div>
      </div>

      <!-- Step 2: Report Preview -->
      <div v-if="hasCurrentReport && currentReport" class="mb-10">
        <h4 class="fs-5 fw-semibold mb-5">{{ translate("reportPreview") }}</h4>

        <!-- Report Info Card -->
        <div class="card bg-light-success border-0 mb-5">
          <div class="card-body">
            <div class="d-flex align-items-center">
              <KTIcon icon-name="check-circle" icon-class="fs-2x text-success me-4" />
              <div>
                <h5 class="fs-5 fw-bold mb-1">{{ translate("reportGenerated") }}</h5>
                <p class="text-muted mb-0">
                  {{ formattedPeriod }} | {{ formatFileSize(currentReport.fileSizeBytes) }} |
                  {{ currentReport.recordsProcessed }} {{ translate("records") }}
                </p>
              </div>
            </div>
          </div>
        </div>

        <!-- Metrics Summary -->
        <div class="row g-5 mb-8">
          <div
            v-for="metric in metricsDisplayData"
            :key="metric.label"
            class="col-6 col-md-4 col-lg"
          >
            <div class="card h-100" :class="`bg-light-${metric.color}`">
              <div class="card-body text-center py-5">
                <span class="fs-2x fw-bold" :class="`text-${metric.color}`">
                  {{ metric.value }}
                </span>
                <p class="text-muted fs-7 mb-0 mt-2">{{ metric.label }}</p>
              </div>
            </div>
          </div>
        </div>

        <!-- Download Button -->
        <div class="d-flex justify-content-center gap-3 mb-8">
          <button
            type="button"
            class="btn btn-light-primary"
            @click="handleDownloadReport"
          >
            <KTIcon icon-name="file-down" icon-class="fs-3 me-2" />
            {{ translate("downloadExcel") }}
          </button>
        </div>

        <!-- Email Recipients Section -->
        <div class="separator my-8"></div>

        <h4 class="fs-5 fw-semibold mb-5">{{ translate("emailRecipients") }}</h4>

        <!-- Recipient Tags Input -->
        <div class="mb-5">
          <label class="fs-6 fw-semibold mb-2">{{ translate("enterRecipients") }}</label>
          <el-select
            v-model="selectedRecipients"
            multiple
            filterable
            allow-create
            default-first-option
            :placeholder="translate('selectOrEnterEmails')"
            class="w-100"
          >
            <el-option
              v-for="admin in adminRecipients"
              :key="admin"
              :label="admin"
              :value="admin"
            />
          </el-select>
          <p class="text-muted fs-7 mt-2">{{ translate("recipientsHelp") }}</p>
        </div>

        <!-- Send Email Button -->
        <div class="text-center mt-8">
          <button
            type="button"
            class="btn btn-success btn-lg"
            :disabled="isSending || selectedRecipients.length === 0"
            @click="handleSendEmail"
          >
            <span v-if="!isSending">
              <KTIcon icon-name="send" icon-class="fs-3 me-2" />
              {{ translate("sendViaEmail") }} ({{ selectedRecipients.length }})
            </span>
            <span v-else class="indicator-progress d-block">
              {{ translate("sendingEmail") }}
              <span class="spinner-border spinner-border-sm align-middle ms-2"></span>
            </span>
          </button>
        </div>

        <!-- Send Error Display -->
        <div v-if="errors.send" class="alert alert-danger mt-5">
          <div class="d-flex align-items-center">
            <KTIcon icon-name="information-5" icon-class="fs-2 text-danger me-3" />
            <div>{{ errors.send }}</div>
          </div>
        </div>
      </div>

      <!-- Step 3: Delivery Status -->
      <div v-if="hasDeliveryStatus && deliveryStatus" class="mb-10">
        <div class="separator my-8"></div>

        <h4 class="fs-5 fw-semibold mb-5">{{ translate("deliveryStatus") }}</h4>

        <!-- Summary Stats -->
        <div class="row g-5 mb-5">
          <div class="col-md-4">
            <div class="card bg-light-info">
              <div class="card-body text-center py-4">
                <span class="fs-2x fw-bold text-info">{{ deliveryStatus.totalRecipients }}</span>
                <p class="text-muted fs-7 mb-0">{{ translate("totalRecipients") }}</p>
              </div>
            </div>
          </div>
          <div class="col-md-4">
            <div class="card bg-light-success">
              <div class="card-body text-center py-4">
                <span class="fs-2x fw-bold text-success">{{ deliveryStatus.successfulDeliveries }}</span>
                <p class="text-muted fs-7 mb-0">{{ translate("successfulDeliveries") }}</p>
              </div>
            </div>
          </div>
          <div class="col-md-4">
            <div class="card bg-light-danger">
              <div class="card-body text-center py-4">
                <span class="fs-2x fw-bold text-danger">{{ deliveryStatus.failedDeliveries }}</span>
                <p class="text-muted fs-7 mb-0">{{ translate("failedDeliveries") }}</p>
              </div>
            </div>
          </div>
        </div>

        <!-- Delivery Details Table -->
        <div class="table-responsive">
          <table class="table table-row-bordered table-row-gray-100 align-middle gs-0 gy-3">
            <thead>
              <tr class="fw-bold text-muted">
                <th>{{ translate("recipient") }}</th>
                <th>{{ translate("status") }}</th>
                <th>{{ translate("attempts") }}</th>
                <th>{{ translate("lastAttempt") }}</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="status in deliveryStatus.deliveryStatuses" :key="status.recipientEmail">
                <td>{{ status.recipientEmail }}</td>
                <td>
                  <span
                    class="badge"
                    :class="getStatusBadgeClass(status.deliveryStatus)"
                  >
                    {{ translate(`status_${status.deliveryStatus}`) }}
                  </span>
                </td>
                <td>{{ status.attemptCount }}</td>
                <td>
                  <span v-if="status.lastAttemptAt">
                    {{ formatDateTime(status.lastAttemptAt) }}
                  </span>
                  <span v-else class="text-muted">-</span>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>
    <!--end::Card body-->
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from "vue";
import { useI18n } from "vue-i18n";
import { storeToRefs } from "pinia";
import { useEmailReportStore, type ReportType } from "@/stores/emailReport";
import KTIcon from "@/core/helpers/kt-icon/KTIcon.vue";

// i18n
const { t, te } = useI18n();
const translate = (text: string) => (te(text) ? t(text) : text);

// Store
const emailReportStore = useEmailReportStore();
const {
  isGenerating,
  isSending,
  currentReport,
  deliveryStatus,
  errors,
  hasCurrentReport,
  hasDeliveryStatus,
  formattedPeriod,
  metricsDisplayData,
} = storeToRefs(emailReportStore);

// Local state
const selectedReportType = ref<ReportType>("weekly");
const customStartDate = ref<string | null>(null);
const customEndDate = ref<string | null>(null);
const selectedRecipients = ref<string[]>([]);

// Default admin recipients (can be loaded from API)
const adminRecipients = ref<string[]>([
  "VeGuler@dogusotomotiv.com.tr",
  "MTurun@dogusotomotiv.com.tr",
  "MKanberoglu@dogusotomotiv.com.tr",
]);

// Initialize selected recipients with admin list
selectedRecipients.value = [...adminRecipients.value];

// Date picker disabled date functions
const disabledStartDate = (date: Date) => {
  const today = new Date();
  today.setHours(0, 0, 0, 0);
  return date > today;
};

const disabledEndDate = (date: Date) => {
  const today = new Date();
  today.setHours(0, 0, 0, 0);
  if (customStartDate.value) {
    const start = new Date(customStartDate.value);
    return date > today || date < start;
  }
  return date > today;
};

// Format helpers
const formatFileSize = (bytes: number): string => {
  if (bytes < 1024) return `${bytes} B`;
  if (bytes < 1024 * 1024) return `${(bytes / 1024).toFixed(1)} KB`;
  return `${(bytes / (1024 * 1024)).toFixed(1)} MB`;
};

const formatDateTime = (dateStr: string): string => {
  return new Date(dateStr).toLocaleString("tr-TR");
};

const getStatusBadgeClass = (status: string): string => {
  switch (status) {
    case "Sent":
      return "badge-light-success";
    case "Failed":
      return "badge-light-danger";
    case "Retrying":
      return "badge-light-warning";
    default:
      return "badge-light-info";
  }
};

// Handlers
const handleGenerateReport = async () => {
  const request = {
    reportType: selectedReportType.value,
    startDate: customStartDate.value || undefined,
    endDate: customEndDate.value || undefined,
  };
  await emailReportStore.generateReport(request);
};

const handleDownloadReport = async () => {
  if (currentReport.value?.executionId) {
    await emailReportStore.downloadReport(currentReport.value.executionId);
  }
};

const handleSendEmail = async () => {
  if (selectedRecipients.value.length > 0) {
    await emailReportStore.sendReport(selectedRecipients.value);
  }
};

const clearReport = () => {
  emailReportStore.clearReport();
  customStartDate.value = null;
  customEndDate.value = null;
  selectedRecipients.value = [...adminRecipients.value];
};
</script>

<style scoped>
.cursor-pointer {
  cursor: pointer;
}
.cursor-pointer:hover {
  transform: translateY(-2px);
  transition: transform 0.2s ease-in-out;
}
</style>
