using System.ComponentModel.DataAnnotations;

namespace Cdr.Api.Models.Entities;

/// <summary>
/// Tracks scheduled and on-demand report job executions.
/// Used for monitoring, retry detection, and audit trail per NFR-007.
/// </summary>
public class ReportExecutionLog
{
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// Hangfire job ID for tracking
    /// </summary>
    [MaxLength(100)]
    public string? HangfireJobId { get; set; }

    /// <summary>
    /// Type of report: Weekly, Monthly
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string ReportType { get; set; } = string.Empty;

    /// <summary>
    /// How the report was triggered: Scheduled, OnDemand, ManualRerun
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string TriggerType { get; set; } = "Scheduled";

    /// <summary>
    /// Start date of the reporting period (Turkey time)
    /// </summary>
    public DateTime PeriodStartDate { get; set; }

    /// <summary>
    /// End date of the reporting period (Turkey time)
    /// </summary>
    public DateTime PeriodEndDate { get; set; }

    /// <summary>
    /// Execution status: Pending, Running, Completed, Failed, Timeout
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string ExecutionStatus { get; set; } = "Pending";

    /// <summary>
    /// When the job started executing (UTC)
    /// </summary>
    public DateTime? StartedAt { get; set; }

    /// <summary>
    /// When the job completed (UTC)
    /// </summary>
    public DateTime? CompletedAt { get; set; }

    /// <summary>
    /// Duration of report generation in milliseconds
    /// </summary>
    public long? GenerationDurationMs { get; set; }

    /// <summary>
    /// Duration of email delivery in milliseconds
    /// </summary>
    public long? EmailDeliveryDurationMs { get; set; }

    /// <summary>
    /// Number of records processed in the report
    /// </summary>
    public int? RecordsProcessed { get; set; }

    /// <summary>
    /// Number of email recipients
    /// </summary>
    public int? RecipientsCount { get; set; }

    /// <summary>
    /// Number of successful email deliveries
    /// </summary>
    public int? SuccessfulDeliveries { get; set; }

    /// <summary>
    /// Number of failed email deliveries
    /// </summary>
    public int? FailedDeliveries { get; set; }

    /// <summary>
    /// Generated Excel file name
    /// </summary>
    [MaxLength(255)]
    public string? GeneratedFileName { get; set; }

    /// <summary>
    /// Generated file size in bytes
    /// </summary>
    public long? FileSizeBytes { get; set; }

    /// <summary>
    /// Error message if execution failed
    /// </summary>
    [MaxLength(4000)]
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Full exception stack trace for debugging
    /// </summary>
    public string? ExceptionStackTrace { get; set; }

    /// <summary>
    /// User ID if triggered on-demand (null for scheduled)
    /// </summary>
    public Guid? TriggeredByUserId { get; set; }

    /// <summary>
    /// IP address if triggered via API
    /// </summary>
    [MaxLength(50)]
    public string? TriggeredFromIp { get; set; }

    /// <summary>
    /// Record creation timestamp (UTC)
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Record last update timestamp (UTC)
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public virtual ICollection<EmailDeliveryAudit> EmailDeliveries { get; set; } = new List<EmailDeliveryAudit>();
}
