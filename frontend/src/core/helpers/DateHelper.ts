class DateHelper {
  public static toLocaleDateStringWithCulture = (date: Date): string => {
    const lang = localStorage.getItem("lang");
    if (lang) {
      return date.toLocaleDateString(lang);
    }
    return date.toLocaleDateString("tr-TR");
  };

  public static toLocaleDateTimeStringWithCulture = (date: Date): string => {
    const lang = localStorage.getItem("lang");
    if (lang) {
      return date.toLocaleString(lang);
    }
    return date.toLocaleString("tr-TR");
  };

  public static formatDate = (date: string) => {
    // UTC string'i Turkey local time'a çevir
    const d = new Date(date);
    return d.toLocaleDateString('en-CA', { timeZone: 'Europe/Istanbul' }); // YYYY-MM-DD format
  };

  public static formatDuration = (seconds: number): string => {
    const h = Math.floor(seconds / 3600);
    const m = Math.floor((seconds % 3600) / 60);
    const s = Math.round(seconds % 60);
    const hDisplay = h > 0 ? h + "h " : "";
    const mDisplay = m > 0 ? m + "m " : "";
    const sDisplay = s > 0 ? s + "s" : "";
    return hDisplay + mDisplay + sDisplay;
  };

  public static toLocaleTimeStringWithCulture = (date: Date): string => {
    const lang = localStorage.getItem("lang");
    if (lang) {
      return date.toLocaleTimeString(lang);
    }
    return date.toLocaleTimeString("tr-TR");
  };

  // New functions per design/03-data-integrity.md §2.3-2.5
  public static getDateRange = (
    range: 'today' | 'week' | 'month' | 'year',
  ): { start: string; end: string } => {
    const now = new Date();
    const today = new Date(now.getFullYear(), now.getMonth(), now.getDate());

    const ranges: Record<string, { start: Date; end: Date }> = {
      today: {
        start: new Date(today),
        end: new Date(today.getFullYear(), today.getMonth(), today.getDate(), 23, 59, 59, 999),
      },
      week: {
        start: new Date(today.getFullYear(), today.getMonth(), today.getDate() - today.getDay()),
        end: new Date(
          today.getFullYear(),
          today.getMonth(),
          today.getDate() - today.getDay() + 6,
          23,
          59,
          59,
          999,
        ),
      },
      month: {
        start: new Date(today.getFullYear(), today.getMonth(), 1),
        end: new Date(today.getFullYear(), today.getMonth() + 1, 0, 23, 59, 59, 999),
      },
      year: {
        start: new Date(today.getFullYear(), 0, 1),
        end: new Date(today.getFullYear(), 11, 31, 23, 59, 59, 999),
      },
    };

    const selectedRange = ranges[range];
    return {
      start: selectedRange.start.toISOString(),
      end: selectedRange.end.toISOString(),
    };
  };

  public static toUTC = (localDate: Date): string => {
    return localDate.toISOString();
  };

  public static fromUTC = (utcString: string): Date => {
    return new Date(utcString);
  };

  public static formatForDisplay = (utcString: string, locale: string = 'tr-TR'): string => {
    const date = new Date(utcString);
    return date.toLocaleString(locale, {
      year: 'numeric',
      month: '2-digit',
      day: '2-digit',
      hour: '2-digit',
      minute: '2-digit',
    });
  };

  public static isValidISO8601 = (dateString: string): boolean => {
    try {
      const date = new Date(dateString);
      return date.toString() !== 'Invalid Date';
    } catch {
      return false;
    }
  };
}

export default DateHelper;
