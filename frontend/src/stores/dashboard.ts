import { ref } from "vue";
import { defineStore } from "pinia";
import ApiService from "@/core/services/ApiService";
import { apiUrlConstants } from "@/stores/consts/ApiUrlConstants";
import ResponseMessageService from "@/core/helpers/ResponseMessageService";

export interface DailyCallReport {
  totalCalls: number;
  answeredCalls: number;
  missedCalls: number;
  totalDuration: number;
  answeredCallRate: number;
  averageCallDuration: number;
  closedAtIVR: number;
  missedCallbackCalls: number;
}

export const useDashboardStore = defineStore("dashboard", () => {
  const errors = ref({});
  const dailyCallReport = ref<DailyCallReport>({} as DailyCallReport);

  function setError(error: any) {
    errors.value = { ...error };
  }

  function fetchWeeklyAnsweredCalls(period: number) {
    const url = `${apiUrlConstants.DASHBOARD_ANSWERED_CALLS}/${period}`;

    return ApiService.get(url)
      .then(({ data }) => {
        ResponseMessageService.showMessageByType(
          "dashboard_weeklyAnsweredCallsFetched",
          "success",
        );
        return data;
      })
      .catch(({ response }) => {
        setError(response.data.errors);
      });
  }

  function fetchDailyCallReport() {
    const url = `${apiUrlConstants.DASHBOARD_DAILY_CALL_REPORT}`;

    return ApiService.get(url)
      .then(({ data }) => {
        ResponseMessageService.showMessageByType(
          "dashboard_dailyCallReportFetched",
          "success",
        );
        dailyCallReport.value = data;
      })
      .catch(({ response }) => {
        setError(response.data.errors);
      });
  }

  function fetchCallRecords(params: {
    startDate?: string;
    endDate?: string;
    callDirection?: number;
    user?: string;
    department?: string;
    pageIndex?: number;
    pageSize?: number;
  }) {
    const queryString = Object.entries(params)
      .filter(([, value]) => value !== undefined && value !== null)
      .map(
        ([key, value]) =>
          `${encodeURIComponent(key)}=${encodeURIComponent(value)}`,
      )
      .join("&");

    const url = `${apiUrlConstants.DASHBOARD_CALL_RECORDS}?${queryString}`;
    return ApiService.get(url)
      .then(({ data }) => {
        ResponseMessageService.showMessageByType(
          "dashboard_callRecordsFetched",
          "success",
        );
        return data;
      })
      .catch(({ response }) => {
        setError(response.data.errors);
      });
  }

  function fetchDepartmentStatistics(params: {
    startDate: string;
    endDate: string;
  }) {
    const queryString = `startDate=${encodeURIComponent(params.startDate)}&endDate=${encodeURIComponent(params.endDate)}`;
    const url = `${apiUrlConstants.DEPARTMENT_STATISTICS}?${queryString}`;

    return ApiService.get(url)
      .then(({ data }) => {
        return data;
      })
      .catch(({ response }) => {
        setError(response?.data?.errors);
        return [];
      });
  }

  function downloadDepartmentExcel(params: {
    startDate: string;
    endDate: string;
  }) {
    const queryString = `startDate=${encodeURIComponent(params.startDate)}&endDate=${encodeURIComponent(params.endDate)}`;
    const url = `${apiUrlConstants.DEPARTMENT_STATISTICS_EXCEL}?${queryString}`;

    return ApiService.get(url, { responseType: "blob" })
      .then(({ data }) => {
        const blob = new Blob([data], {
          type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        });
        const link = document.createElement("a");
        link.href = URL.createObjectURL(blob);
        link.download = `departman-istatistikleri-${params.startDate}-${params.endDate}.xlsx`;
        link.click();
        URL.revokeObjectURL(link.href);
      })
      .catch(({ response }) => {
        setError(response?.data?.errors);
      });
  }

  return {
    errors,
    dailyCallReport,
    fetchWeeklyAnsweredCalls,
    fetchDailyCallReport,
    fetchCallRecords,
    fetchDepartmentStatistics,
    downloadDepartmentExcel,
  };
});
