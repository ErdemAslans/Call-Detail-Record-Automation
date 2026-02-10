using System.ComponentModel.DataAnnotations;
using Common.Enums;

namespace Cdr.Api.Models.Request;

/// <summary>
/// Request model for generating CDR email reports.
/// Used by both scheduled jobs and on-demand API requests.
/// </summary>
public class CdrReportRequest
{
    /// <summary>
    /// Type of report to generate: Weekly or Monthly
    /// </summary>
    [Required]
    public ReportPeriod ReportType { get; set; }

    /// <summary>
    /// Optional custom start date (ISO8601 format, Turkey timezone).
    /// If not provided, uses previous complete period boundaries.
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// Optional custom end date (ISO8601 format, Turkey timezone).
    /// If not provided, uses previous complete period boundaries.
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Optional list of specific email recipients.
    /// If not provided, uses all Admin users (excluding switchboard staff).
    /// </summary>
    public List<string>? EmailRecipients { get; set; }

    /// <summary>
    /// Whether to send the report via email after generation.
    /// Default is true for scheduled jobs, can be false for preview/download only.
    /// </summary>
    public bool SendEmail { get; set; } = true;

    /// <summary>
    /// Optional notes to include in the email body.
    /// </summary>
    [MaxLength(1000)]
    public string? CustomNotes { get; set; }
}
