import ResponseMessageService from "@/core/helpers/ResponseMessageService";
import ApiService from "@/core/services/ApiService";
import { defineStore } from "pinia";
import { ref } from "vue";
import { apiUrlConstants } from "./consts/ApiUrlConstants";

export const useUserStatisticsStore = defineStore("userStatistics", () => {
  const errors = ref({});

  function setError(error: any) {
    errors.value = { ...error };
  }

  function fetchUserStatistics(params: {
    startDate: string;
    endDate: string;
    number: string;
  }) {
    const queryString = Object.entries(params)
      .filter(([, value]) => value !== undefined && value !== null)
      .map(
        ([key, value]) =>
          `${encodeURIComponent(key)}=${encodeURIComponent(value)}`,
      )
      .join("&");

    const url = `${apiUrlConstants.USER_STATISTICS}?${queryString}`;
    return ApiService.get(url)
      .then(({ data }) => {
        ResponseMessageService.showMessageByType(
          "userStatistics_userStatisticsFetched",
          "success",
        );
        return data;
      })
      .catch((error: any) => {
        setError(error.response?.data?.errors || { general: "Bir hata oluştu" });
      });
  }

  function fetchUserLastCalls(params: {
    number: string;
    startDate?: string;
    endDate?: string;
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

    const url = `${apiUrlConstants.USER_LAST_CALLS}?${queryString}`;
    return ApiService.get(url)
      .then(({ data }) => {
        ResponseMessageService.showMessageByType(
          "userStatistics_userLastCallsFetched",
          "success",
        );
        return data;
      })
      .catch((error: any) => {
        setError(error.response?.data?.errors || { general: "Bir hata oluştu" });
      });
  }

  function fetchUserInfo(number: string) {
    const url = `${apiUrlConstants.USER_INFO}/${number}`;
    return ApiService.get(url)
      .then(({ data }) => {
        ResponseMessageService.showMessageByType(
          "userStatistics_userInfoFetched",
          "success",
        );
        return data;
      })
      .catch((error: any) => {
        setError(error.response?.data?.errors || { general: "Bir hata oluştu" });
      });
  }

  function fetchUserWorkHourStatistics(params: {
    startDate: string;
    endDate: string;
    number: string;
  }) {
    const queryString = Object.entries(params)
      .filter(([, value]) => value !== undefined && value !== null)
      .map(
        ([key, value]) =>
          `${encodeURIComponent(key)}=${encodeURIComponent(value)}`,
      )
      .join("&");

    const url = `${apiUrlConstants.WORKING_HOURS}?${queryString}`;
    return ApiService.get(url)
      .then(({ data }) => {
        ResponseMessageService.showMessageByType(
          "userStatistics_userWorkHourStatisticsFetched",
          "success",
        );
        return data;
      })
      .catch((error: any) => {
        setError(error.response?.data?.errors || { general: "Bir hata oluştu" });
      });
  }

  function fetchUserNonWorkHourStatistics(params: {
    startDate: string;
    endDate: string;
    number: string;
  }) {
    const queryString = Object.entries(params)
      .filter(([, value]) => value !== undefined && value !== null)
      .map(
        ([key, value]) =>
          `${encodeURIComponent(key)}=${encodeURIComponent(value)}`,
      )
      .join("&");

    const url = `${apiUrlConstants.NON_WORKING_HOURS}?${queryString}`;
    return ApiService.get(url)
      .then(({ data }) => {
        ResponseMessageService.showMessageByType(
          "userStatistics_userNonWorkHourStatisticsFetched",
          "success",
        );
        return data;
      })
      .catch((error: any) => {
        setError(error.response?.data?.errors || { general: "Bir hata oluştu" });
      });
  }

  function fetchBreakTimes(params: {
    startDate: string;
    endDate: string;
    number: string;
  }) {
    const queryString = Object.entries(params)
      .filter(([, value]) => value !== undefined && value !== null)
      .map(
        ([key, value]) =>
          `${encodeURIComponent(key)}=${encodeURIComponent(value)}`,
      )
      .join("&");

    const url = `${apiUrlConstants.BREAK_TIMES}?${queryString}`;
    return ApiService.get(url)
      .then(({ data }) => {
        ResponseMessageService.showMessageByType(
          "userStatistics_breakTimesFetched",
          "success",
        );
        return data;
      })
      .catch((error: any) => {
        setError(error.response?.data?.errors || { general: "Bir hata oluştu" });
      });
  }

  function exportUserSpecificReport(params: {
    startDate: string;
    endDate: string;
    number: string;
  }) {
    const queryString = Object.entries(params)
      .filter(([, value]) => value !== undefined && value !== null)
      .map(
        ([key, value]) =>
          `${encodeURIComponent(key)}=${encodeURIComponent(value)}`,
      )
      .join("&");

    const url = `${apiUrlConstants.USER_SPECIFIC_REPORT_EXPORT}?${queryString}`;
    return ApiService.get(url)
      .then((response) => {
        // Base64 string'i byte array'e çevir
        const byteCharacters = atob(response.data);
        const byteNumbers = new Array(byteCharacters.length);
        for (let i = 0; i < byteCharacters.length; i++) {
          byteNumbers[i] = byteCharacters.charCodeAt(i);
        }
        const byteArray = new Uint8Array(byteNumbers);

        // Blob nesnesi oluştur
        const blob = new Blob([byteArray], {
          type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        });

        // Geçici bir URL oluştur
        const url = window.URL.createObjectURL(blob);

        // Link oluşturarak indirme işlemini tetikle
        const link = document.createElement("a");
        link.href = url;
        link.setAttribute(
          "download",
          `user-specific-report${params.number}-${params.startDate}-${params.endDate}.xlsx`,
        );
        document.body.appendChild(link);
        link.click();

        // Belleği temizle
        window.URL.revokeObjectURL(url);
        document.body.removeChild(link);

        ResponseMessageService.showMessageByType(
          "userStatistics_userSpecificReportExported",
          "success",
        );
      })
      .catch((error: any) => {
        setError(error.response?.data?.errors || { general: "Bir hata oluştu" });
      });
  }

  return {
    errors,
    setError,
    fetchUserStatistics,
    fetchUserLastCalls,
    fetchUserInfo,
    fetchUserWorkHourStatistics,
    fetchUserNonWorkHourStatistics,
    fetchBreakTimes,
    exportUserSpecificReport,
  };
});
