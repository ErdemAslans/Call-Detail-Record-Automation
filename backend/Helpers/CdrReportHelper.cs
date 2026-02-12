using System.Globalization;

namespace Cdr.Api.Helpers;

/// <summary>
/// Helper methods for CDR report date calculations and business logic.
/// Uses Turkey Standard Time (UTC+3) for all date operations.
/// </summary>
public static class CdrReportHelper
{
    /// <summary>
    /// Turkey Standard Time zone info.
    /// </summary>
    public static readonly TimeZoneInfo TurkeyTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time");

    /// <summary>
    /// Work hours start time (07:45 local time).
    /// </summary>
    public static readonly TimeSpan WorkHoursStart = new TimeSpan(7, 45, 0);

    /// <summary>
    /// Work hours end time (16:45 local time).
    /// </summary>
    public static readonly TimeSpan WorkHoursEnd = new TimeSpan(16, 45, 0);

    /// <summary>
    /// Get the current time in Turkey timezone.
    /// </summary>
    public static DateTime GetTurkeyNow()
    {
        return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TurkeyTimeZone);
    }

    /// <summary>
    /// Convert UTC time to Turkey local time.
    /// </summary>
    public static DateTime ToTurkeyTime(DateTime utcTime)
    {
        return TimeZoneInfo.ConvertTimeFromUtc(utcTime, TurkeyTimeZone);
    }

    /// <summary>
    /// Convert Turkey local time to UTC.
    /// </summary>
    public static DateTime ToUtc(DateTime turkeyTime)
    {
        return TimeZoneInfo.ConvertTimeToUtc(turkeyTime, TurkeyTimeZone);
    }

    /// <summary>
    /// Get the start and end dates for a daily report period.
    /// Daily period: Previous day 00:00:00 to 23:59:59.999 (Turkey time).
    /// </summary>
    /// <param name="referenceDate">Reference date (defaults to current Turkey time)</param>
    /// <returns>Tuple of (StartDate, EndDate) in UTC</returns>
    public static (DateTime StartDate, DateTime EndDate) GetDailyReportPeriod(DateTime? referenceDate = null)
    {
        var turkeyNow = referenceDate ?? GetTurkeyNow();
        var previousDay = turkeyNow.AddDays(-1).Date;
        var nextDay = previousDay.AddDays(1).AddMilliseconds(-1);
        
        return (ToUtc(previousDay), ToUtc(nextDay));
    }

    /// <summary>
    /// Get the start and end dates for a weekly report period.
    /// Weekly period: Previous Monday 00:00:00 to Sunday 23:59:59 (Turkey time).
    /// </summary>
    /// <param name="referenceDate">Reference date (defaults to current Turkey time)</param>
    /// <returns>Tuple of (StartDate, EndDate) in UTC</returns>
    public static (DateTime StartDate, DateTime EndDate) GetWeeklyReportPeriod(DateTime? referenceDate = null)
    {
        var turkeyNow = referenceDate ?? GetTurkeyNow();

        // Find the previous complete week's Monday
        // daysFromMonday: Mon=0, Tue=1, Wed=2, Thu=3, Fri=4, Sat=5, Sun=6
        var daysFromMonday = ((int)turkeyNow.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
        // Go back to this week's Monday (-daysFromMonday), then one more week (-7)
        var previousMonday = turkeyNow.Date.AddDays(-daysFromMonday - 7);
        var previousSunday = previousMonday.AddDays(6);
        
        // Start: Monday 00:00:00, End: Sunday 23:59:59.999
        var startDate = previousMonday;
        var endDate = previousSunday.AddDays(1).AddMilliseconds(-1);
        
        return (ToUtc(startDate), ToUtc(endDate));
    }

    /// <summary>
    /// Get the start and end dates for a monthly report period.
    /// Monthly period: First day of previous month 00:00:00 to last day 23:59:59 (Turkey time).
    /// </summary>
    /// <param name="referenceDate">Reference date (defaults to current Turkey time)</param>
    /// <returns>Tuple of (StartDate, EndDate) in UTC</returns>
    public static (DateTime StartDate, DateTime EndDate) GetMonthlyReportPeriod(DateTime? referenceDate = null)
    {
        var turkeyNow = referenceDate ?? GetTurkeyNow();
        
        // Get previous month
        var previousMonth = turkeyNow.AddMonths(-1);
        var firstDay = new DateTime(previousMonth.Year, previousMonth.Month, 1);
        var lastDay = firstDay.AddMonths(1).AddMilliseconds(-1);
        
        return (ToUtc(firstDay), ToUtc(lastDay));
    }

    /// <summary>
    /// Check if a given time falls within work hours (07:45-16:45, weekdays only).
    /// </summary>
    /// <param name="utcTime">Time in UTC</param>
    /// <param name="holidayDates">List of holiday dates to exclude from work hours</param>
    /// <returns>True if within work hours</returns>
    public static bool IsWithinWorkHours(DateTime utcTime, IEnumerable<DateOnly>? holidayDates = null)
    {
        var turkeyTime = ToTurkeyTime(utcTime);
        
        // Check if weekend
        if (turkeyTime.DayOfWeek == DayOfWeek.Saturday || turkeyTime.DayOfWeek == DayOfWeek.Sunday)
        {
            return false;
        }
        
        // Check if holiday
        var dateOnly = DateOnly.FromDateTime(turkeyTime);
        if (holidayDates?.Contains(dateOnly) == true)
        {
            return false;
        }
        
        // Check time range
        var timeOfDay = turkeyTime.TimeOfDay;
        return timeOfDay >= WorkHoursStart && timeOfDay <= WorkHoursEnd;
    }

    /// <summary>
    /// Check if a given time is after hours (outside work hours, weekends, or holidays).
    /// </summary>
    public static bool IsAfterHours(DateTime utcTime, IEnumerable<DateOnly>? holidayDates = null)
    {
        return !IsWithinWorkHours(utcTime, holidayDates);
    }

    /// <summary>
    /// Generate a report file name based on type and period.
    /// Format: CDR_{Type}_{StartDate}-{EndDate}.xlsx
    /// </summary>
    public static string GenerateReportFileName(string reportType, DateTime startDate, DateTime endDate)
    {
        var turkeyStart = ToTurkeyTime(startDate);
        var turkeyEnd = ToTurkeyTime(endDate);
        
        var startStr = turkeyStart.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
        var endStr = turkeyEnd.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
        
        return $"CDR_{reportType}_{startStr}-{endStr}.xlsx";
    }

    /// <summary>
    /// Generate email subject line for a report.
    /// </summary>
    public static string GenerateEmailSubject(string reportType, DateTime startDate, DateTime endDate)
    {
        var turkeyStart = ToTurkeyTime(startDate);
        var turkeyEnd = ToTurkeyTime(endDate);
        var culture = new CultureInfo("tr-TR");
        
        var startStr = turkeyStart.ToString("dd MMMM yyyy", culture);
        var endStr = turkeyEnd.ToString("dd MMMM yyyy", culture);
        
        return $"[Doğuş Oto] {reportType} CDR Raporu - {startStr} - {endStr}";
    }

    /// <summary>
    /// Validate email addresses.
    /// </summary>
    public static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Calculate the number of work days in a period (excluding weekends and holidays).
    /// </summary>
    public static int GetWorkDaysInPeriod(DateTime startUtc, DateTime endUtc, IEnumerable<DateOnly>? holidayDates = null)
    {
        var turkeyStart = ToTurkeyTime(startUtc).Date;
        var turkeyEnd = ToTurkeyTime(endUtc).Date;
        var holidaySet = holidayDates?.ToHashSet() ?? new HashSet<DateOnly>();
        
        int workDays = 0;
        for (var date = turkeyStart; date <= turkeyEnd; date = date.AddDays(1))
        {
            if (date.DayOfWeek != DayOfWeek.Saturday && 
                date.DayOfWeek != DayOfWeek.Sunday &&
                !holidaySet.Contains(DateOnly.FromDateTime(date)))
            {
                workDays++;
            }
        }
        
        return workDays;
    }
}
