namespace Cdr.Api.Models;

/// <summary>
/// Configuration settings for CDR email reporting.
/// </summary>
public class CdrReportingSettings
{
    /// <summary>
    /// Default email recipients for scheduled reports.
    /// </summary>
    public List<string> DefaultRecipients { get; set; } = new();

    /// <summary>
    /// Email addresses to exclude from receiving reports (e.g., switchboard staff).
    /// </summary>
    public List<string> ExcludedRecipients { get; set; } = new();

    /// <summary>
    /// CRON expression for weekly report schedule (default: Monday 02:00 AM).
    /// </summary>
    public string WeeklyCron { get; set; } = "0 2 * * 1";

    /// <summary>
    /// CRON expression for monthly report schedule (default: 1st day 02:00 AM).
    /// </summary>
    public string MonthlyCron { get; set; } = "0 2 1 * *";

    /// <summary>
    /// Directory path for storing generated report files.
    /// </summary>
    public string ReportStoragePath { get; set; } = "Files/Reports";
}
