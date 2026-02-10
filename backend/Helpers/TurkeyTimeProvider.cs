namespace Cdr.Api.Helpers;

/// <summary>
/// Turkey timezone provider for consistent date/time operations across the application.
/// Uses Turkey Standard Time (UTC+3) for all local time calculations.
/// </summary>
public static class TurkeyTimeProvider
{
    private static readonly TimeZoneInfo TurkeyTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time");

    /// <summary>
    /// Gets the current date and time in Turkey timezone.
    /// </summary>
    public static DateTime Now => TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TurkeyTimeZone);

    /// <summary>
    /// Gets the current date (midnight) in Turkey timezone.
    /// </summary>
    public static DateTime Today => Now.Date;

    /// <summary>
    /// Gets the Turkey timezone info.
    /// </summary>
    public static TimeZoneInfo TimeZone => TurkeyTimeZone;

    /// <summary>
    /// Converts a UTC DateTime to Turkey local time.
    /// </summary>
    public static DateTime ConvertFromUtc(DateTime utcDateTime)
    {
        return TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc), TurkeyTimeZone);
    }

    /// <summary>
    /// Converts a Turkey local DateTime to UTC.
    /// </summary>
    public static DateTime ConvertToUtc(DateTime turkeyDateTime)
    {
        return TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(turkeyDateTime, DateTimeKind.Unspecified), TurkeyTimeZone);
    }

    /// <summary>
    /// Gets the start of day (midnight) in Turkey timezone, returned as UTC.
    /// Handles any DateTimeKind: if UTC, first converts to Turkey local before truncating.
    /// </summary>
    public static DateTime GetStartOfDayUtc(DateTime date)
    {
        // If the input is UTC (e.g., from ISO 8601 parsing), convert to Turkey local first
        var turkeyDate = date.Kind == DateTimeKind.Utc ? ConvertFromUtc(date) : date;
        var startOfDay = turkeyDate.Date;
        return ConvertToUtc(startOfDay);
    }

    /// <summary>
    /// Gets the end of day (next day midnight) in Turkey timezone, returned as UTC.
    /// Handles any DateTimeKind: if UTC, first converts to Turkey local before truncating.
    /// Use Lt (less than) with this value for exclusive end boundary.
    /// </summary>
    public static DateTime GetEndOfDayUtc(DateTime date)
    {
        var turkeyDate = date.Kind == DateTimeKind.Utc ? ConvertFromUtc(date) : date;
        var endOfDay = turkeyDate.Date.AddDays(1);
        return ConvertToUtc(endOfDay);
    }

    /// <summary>
    /// Converts date range from Turkey local time to UTC for MongoDB queries.
    /// Frontend sends dates as Turkey local time (e.g., 2026-01-01 to 2026-01-31).
    /// This method converts them to UTC boundaries for accurate MongoDB filtering.
    /// </summary>
    /// <param name="startDate">Start date in Turkey local time</param>
    /// <param name="endDate">End date in Turkey local time</param>
    /// <returns>Tuple of (StartDateUtc, EndDateUtc) for MongoDB filter</returns>
    public static (DateTime StartDateUtc, DateTime EndDateUtc) ConvertDateRangeToUtc(DateTime startDate, DateTime endDate)
    {
        // Start: Turkey midnight -> UTC
        var startUtc = GetStartOfDayUtc(startDate);
        // End: Turkey next day midnight -> UTC (use Lt for exclusive boundary)
        var endUtc = GetEndOfDayUtc(endDate);
        return (startUtc, endUtc);
    }
}
