import { ref, computed } from "vue";
import { defineStore } from "pinia";
import ApiService from "@/core/services/ApiService";
import { apiUrlConstants } from "@/stores/consts/ApiUrlConstants";
import ResponseMessageService from "@/core/helpers/ResponseMessageService";

/**
 * Email report types enum
 */
export type ReportType = "weekly" | "monthly";

/**
 * Metrics summary returned from report generation
 */
export interface BreakDetail {
  startTime: string;
  endTime: string | null;
  durationMinutes: number;
  reason: string | null;
}

export interface OperatorBreakSummary {
  operatorName: string;
  phoneNumber: string;
  breakCount: number;
  totalDurationMinutes: number;
  breaks: BreakDetail[];
}

export interface CdrReportMetricsSummary {
  totalIncomingCalls: number;
  totalAnsweredCalls: number;
  totalMissedCalls: number;
  totalOnBreakCalls: number;
  totalOutgoingCalls: number;
  answerRate: number;
  workHoursCalls: number;
  afterHoursCalls: number;
  breakSummaries: OperatorBreakSummary[];
  totalBreakCount: number;
  totalBreakDurationMinutes: number;
}

/**
 * Report generation response
 */
export interface CdrEmailReportResponse {
  reportId: string;
  reportType: string;
  periodStartDate: string;
  periodEndDate: string;
  generatedAt: string;
  fileName: string;
  fileSizeBytes: number;
  recordsProcessed: number;
  generationDurationMs: number;
  metricsSummary: CdrReportMetricsSummary;
  emailsSent: boolean;
  recipientsCount: number;
  successfulDeliveries: number;
  failedDeliveries: number;
  deliveryStatus: EmailDeliveryStatus[];
}

/**
 * Email delivery status per recipient
 */
export interface EmailDeliveryStatus {
  recipientEmail: string;
  deliveryStatus: "Pending" | "Sent" | "Failed" | "Retrying";
  attemptCount: number;
  lastAttemptAt: string | null;
  errorMessage: string | null;
}

/**
 * Send email report response
 */
export interface SendEmailReportResponse {
  reportExecutionId: string;
  totalRecipients: number;
  successfulDeliveries: number;
  failedDeliveries: number;
  pendingDeliveries: number;
  overallStatus: string;
  recipientStatuses: EmailDeliveryStatus[];
}

/**
 * Report execution history item
 */
export interface ReportExecutionHistory {
  executionId: string;
  reportType: string;
  triggerType: string;
  periodStartDate: string;
  periodEndDate: string;
  executionStatus: string;
  startedAt: string;
  completedAt: string | null;
  recordsProcessed: number;
  recipientsCount: number;
  successfulDeliveries: number;
  failedDeliveries: number;
}

/**
 * Report request parameters
 */
export interface GenerateReportRequest {
  reportType: ReportType;
  startDate?: string;
  endDate?: string;
  sendEmail?: boolean;
  emailRecipients?: string[];
}

/**
 * Send email request parameters
 */
export interface SendEmailRequest {
  reportExecutionId: string;
  emailRecipients: string[];
}

/**
 * Email Report Store
 * Manages state for CDR email report generation and delivery
 */
export const useEmailReportStore = defineStore("emailReport", () => {
  // State
  const errors = ref<Record<string, string>>({});
  const isLoading = ref(false);
  const isGenerating = ref(false);
  const isSending = ref(false);
  const currentReport = ref<CdrEmailReportResponse | null>(null);
  const deliveryStatus = ref<SendEmailReportResponse | null>(null);
  const executionHistory = ref<ReportExecutionHistory[]>([]);

  // Getters
  const hasCurrentReport = computed(() => currentReport.value !== null);
  const hasDeliveryStatus = computed(() => deliveryStatus.value !== null);
  
  const formattedPeriod = computed(() => {
    if (!currentReport.value) return "";
    const start = new Date(currentReport.value.periodStartDate);
    const end = new Date(currentReport.value.periodEndDate);
    return `${start.toLocaleDateString("tr-TR")} - ${end.toLocaleDateString("tr-TR")}`;
  });

  const metricsDisplayData = computed(() => {
    if (!currentReport.value?.metricsSummary) return [];
    const m = currentReport.value.metricsSummary;
    return [
      { label: "Toplam Gelen Arama", value: m.totalIncomingCalls, color: "primary" },
      { label: "Cevaplanan", value: m.totalAnsweredCalls, color: "success" },
      { label: "Kaçırılan", value: m.totalMissedCalls, color: "danger" },
      { label: "Molada Gelen", value: m.totalOnBreakCalls, color: "dark" },
      { label: "Giden Arama", value: m.totalOutgoingCalls, color: "info" },
      { label: "Cevaplama Oranı", value: `${m.answerRate}%`, color: "warning" },
    ];
  });

  // Actions
  function setError(error: Record<string, string>) {
    errors.value = { ...error };
  }

  function clearError() {
    errors.value = {};
  }

  function clearReport() {
    currentReport.value = null;
    deliveryStatus.value = null;
    stopPolling();
    clearError();
  }

  /**
   * Generate a CDR email report
   */
  async function generateReport(request: GenerateReportRequest): Promise<CdrEmailReportResponse | null> {
    isGenerating.value = true;
    clearError();

    try {
      const { data } = await ApiService.post(apiUrlConstants.GENERATE_EMAIL_REPORT, request);
      currentReport.value = data;

      // If emails were sent synchronously, populate delivery status from response
      if (data.emailsSent && data.recipientsCount > 0) {
        deliveryStatus.value = {
          reportExecutionId: data.reportId,
          totalRecipients: data.recipientsCount,
          successfulDeliveries: data.successfulDeliveries,
          failedDeliveries: data.failedDeliveries,
          pendingDeliveries: 0,
          overallStatus: data.failedDeliveries === 0 ? "Completed" : "PartialFailure",
          recipientStatuses: data.deliveryStatus ?? [],
        };

        if (data.failedDeliveries === 0) {
          ResponseMessageService.showMessageByType("emailReport_sent", "success");
        } else {
          ResponseMessageService.showMessageByType("emailReport_partialSent", "warning");
        }
      } else {
        ResponseMessageService.showMessageByType("emailReport_generated", "success");
      }

      return data;
    } catch (error: any) {
      const errorMessage = error.response?.data?.error?.message || "Rapor oluşturulurken hata oluştu";
      setError({ generation: errorMessage });
      return null;
    } finally {
      isGenerating.value = false;
    }
  }

  /**
   * Generate weekly report for previous week
   */
  async function generateWeeklyReport(): Promise<CdrEmailReportResponse | null> {
    return generateReport({ reportType: "weekly" });
  }

  /**
   * Generate monthly report for previous month
   */
  async function generateMonthlyReport(): Promise<CdrEmailReportResponse | null> {
    return generateReport({ reportType: "monthly" });
  }

  /**
   * Send the current report via email
   */
  async function sendReport(recipients: string[]): Promise<SendEmailReportResponse | null> {
    if (!currentReport.value?.reportId) {
      setError({ send: "Önce bir rapor oluşturmalısınız" });
      return null;
    }

    isSending.value = true;
    clearError();

    try {
      const request: SendEmailRequest = {
        reportExecutionId: currentReport.value.reportId,
        emailRecipients: recipients,
      };

      const { data } = await ApiService.post(apiUrlConstants.SEND_EMAIL_REPORT, request);
      deliveryStatus.value = data;

      ResponseMessageService.showMessageByType("emailReport_sent", "success");

      // Start polling for delivery results (Hangfire processes async)
      pollDeliveryStatus(currentReport.value.reportId);

      return data;
    } catch (error: any) {
      const errorMessage = error.response?.data?.error?.message || "E-posta gönderilirken hata oluştu";
      setError({ send: errorMessage });
      return null;
    } finally {
      isSending.value = false;
    }
  }

  /**
   * Poll execution history to get updated email delivery status
   */
  let pollTimer: ReturnType<typeof setTimeout> | null = null;
  let pollAttempts = 0;
  const MAX_POLL_ATTEMPTS = 12; // 12 * 10s = 2 minutes max polling
  const POLL_INTERVAL_MS = 10000; // 10 seconds

  function pollDeliveryStatus(reportId: string) {
    pollAttempts = 0;
    if (pollTimer) clearTimeout(pollTimer);
    schedulePoll(reportId);
  }

  function schedulePoll(reportId: string) {
    pollTimer = setTimeout(async () => {
      pollAttempts++;
      try {
        await fetchExecutionHistory();
        // Check if the original execution now has email delivery data
        const execution = executionHistory.value.find(
          (e) => e.executionId === reportId
        );
        if (execution && (execution.successfulDeliveries > 0 || execution.failedDeliveries > 0)) {
          // Update delivery status display with real results
          if (deliveryStatus.value) {
            deliveryStatus.value.successfulDeliveries = execution.successfulDeliveries;
            deliveryStatus.value.failedDeliveries = execution.failedDeliveries;
            deliveryStatus.value.totalRecipients = execution.recipientsCount;
            deliveryStatus.value.overallStatus = execution.failedDeliveries === 0 ? "Completed" : "PartialFailure";
          }
          return; // Stop polling
        }
      } catch {
        // Ignore poll errors
      }

      if (pollAttempts < MAX_POLL_ATTEMPTS) {
        schedulePoll(reportId);
      }
    }, POLL_INTERVAL_MS);
  }

  function stopPolling() {
    if (pollTimer) {
      clearTimeout(pollTimer);
      pollTimer = null;
    }
    pollAttempts = 0;
  }

  /**
   * Fetch report execution history
   */
  async function fetchExecutionHistory(count = 20): Promise<ReportExecutionHistory[]> {
    isLoading.value = true;

    try {
      const url = `${apiUrlConstants.REPORT_EXECUTION_HISTORY}?count=${count}`;
      const { data } = await ApiService.get(url);
      executionHistory.value = data;
      return data;
    } catch (error: any) {
      const errorMessage = error.response?.data?.error?.message || "Geçmiş yüklenemedi";
      setError({ history: errorMessage });
      return [];
    } finally {
      isLoading.value = false;
    }
  }

  /**
   * Download a report by execution ID
   */
  async function downloadReport(executionId: string): Promise<void> {
    try {
      const url = `${apiUrlConstants.DOWNLOAD_REPORT}/${executionId}`;
      const response = await ApiService.query(url, { responseType: "blob" });
      
      // Create blob and trigger download
      const blob = new Blob([response.data], {
        type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
      });
      
      const downloadUrl = window.URL.createObjectURL(blob);
      const link = document.createElement("a");
      link.href = downloadUrl;
      
      // Extract filename from response headers or use default
      const contentDisposition = response.headers?.["content-disposition"];
      let fileName = `cdr-report-${executionId}.xlsx`;
      if (contentDisposition) {
        const match = contentDisposition.match(/filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/);
        if (match && match[1]) {
          fileName = match[1].replace(/['"]/g, "");
        }
      }
      
      link.setAttribute("download", fileName);
      document.body.appendChild(link);
      link.click();
      document.body.removeChild(link);
      window.URL.revokeObjectURL(downloadUrl);
      
      ResponseMessageService.showMessageByType("emailReport_downloaded", "success");
    } catch (error: any) {
      const errorMessage = error.response?.data?.error?.message || "Rapor indirilemedi";
      setError({ download: errorMessage });
    }
  }

  return {
    // State
    errors,
    isLoading,
    isGenerating,
    isSending,
    currentReport,
    deliveryStatus,
    executionHistory,
    // Getters
    hasCurrentReport,
    hasDeliveryStatus,
    formattedPeriod,
    metricsDisplayData,
    // Actions
    setError,
    clearError,
    clearReport,
    generateReport,
    generateWeeklyReport,
    generateMonthlyReport,
    sendReport,
    fetchExecutionHistory,
    downloadReport,
  };
});
