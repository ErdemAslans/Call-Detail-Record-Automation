import ResponseMessageService from "@/core/helpers/ResponseMessageService";
import ApiService from "@/core/services/ApiService";
import { defineStore } from "pinia";
import { apiUrlConstants } from "./consts/ApiUrlConstants";

export const useBreaksStore = defineStore("breaks", () => {
  function fetchBreaks(params: {
    startDate: string;
    endDate: string;
  }): Promise<FormatedBreakTimesItems[]> {
    const queryString = Object.entries(params)
      .filter(([, value]) => value !== undefined && value !== null)
      .map(
        ([key, value]) =>
          `${encodeURIComponent(key)}=${encodeURIComponent(value)}`,
      )
      .join("&");

    const url = `${apiUrlConstants.BREAKS}?${queryString}`;
    return ApiService.get(url)
      .then(({ data }) => {
        ResponseMessageService.showMessageByType(
          "breaks_fetchBreaks",
          "success",
        );
        return formatBreakTimes(data);
      })
      .catch((error: any) => {
        return error.response?.data?.errors || { general: "Bir hata oluştu" };
      });
  }

  function startNewBreak(reason: string): Promise<any> {
    const url = `${apiUrlConstants.START_NEW_BREAK}`;
    return ApiService.post(url, { reason })
      .then(() => {
        ResponseMessageService.showMessageByType(
          "breaks_startNewBreak",
          "success",
        );
      })
      .catch((error: any) => {
        return error.response?.data?.errors || { general: "Bir hata oluştu" };
      });
  }

  function endBreak(breakId: string | null): Promise<any> {
    const url = `${apiUrlConstants.END_BREAK}/${breakId}`;
    return ApiService.post(url, {})
      .then(() => {
        ResponseMessageService.showMessageByType("breaks_endBreak", "success");
      })
      .catch((error: any) => {
        return error.response?.data?.errors || { general: "Bir hata oluştu" };
      });
  }

  function formatBreakTimes(
    breakTimes: BreakListItem[],
  ): FormatedBreakTimesItems[] {
    // Group breaks by date first
    const breaksByDate = new Map<string, FormatedBreakTimesItems[]>();

    // Process each break item
    breakTimes.forEach((item) => {
      if (item.startTime) {
        const breakStartDate = new Date(item.startTime).toLocaleDateString();

        // Initialize array for this date if not exists
        if (!breaksByDate.has(breakStartDate)) {
          breaksByDate.set(breakStartDate, []);
        }

        // Add break start item
        breaksByDate.get(breakStartDate)!.push({
          id: item.id,
          breakTime: item.startTime,
          type: "breakStart",
          reason: item.reason,
          isEnd: item.endTime ? true : false,
        });
      }

      if (item.endTime) {
        const breakEndDate = new Date(item.endTime).toLocaleDateString();

        // Initialize array for this date if not exists
        if (!breaksByDate.has(breakEndDate)) {
          breaksByDate.set(breakEndDate, []);
        }

        // Add break end item
        breaksByDate.get(breakEndDate)!.push({
          id: item.id,
          breakTime: item.endTime,
          type: "breakEnd",
          isEnd: true,
        });
      }
    });

    const result: FormatedBreakTimesItems[] = [];

    // Sort dates in descending order (newest first)
    const sortedDates = Array.from(breaksByDate.keys()).sort(
      (a, b) => new Date(b).getTime() - new Date(a).getTime(),
    );

    // For each date, add a date header followed by the sorted break items
    for (const date of sortedDates) {
      // Add date header
      result.push({
        id: date, // Using date as id for header
        breakTime: new Date(date).toISOString(),
        type: "date",
        isEnd: false,
      });

      // Sort items for this date in descending order and add them
      const dateItems = breaksByDate
        .get(date)!
        .sort(
          (a, b) =>
            new Date(b.breakTime).getTime() - new Date(a.breakTime).getTime(),
        );

      result.push(...dateItems);
    }

    return result;
  }

  return {
    fetchBreaks,
    startNewBreak,
    endBreak,
  };
});
