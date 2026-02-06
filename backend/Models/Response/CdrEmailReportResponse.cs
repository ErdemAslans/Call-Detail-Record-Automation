namespace Cdr.Api.Models.Response;

/// <summary>
/// Response model returned after generating a CDR email report.
/// </summary>
public class CdrEmailReportResponse
{
    /// <summary>
    /// Unique identifier for this report execution
    /// </summary>
    public Guid ReportId { get; set; }

    /// <summary>
    /// Type of report: Weekly, Monthly
    /// </summary>
    public string ReportType { get; set; } = string.Empty;

    /// <summary>
    /// When the report was generated (UTC)
    /// </summary>
    public DateTime GeneratedAt { get; set; }

    /// <summary>
    /// Reporting period start date (Turkey time)
    /// </summary>
    public DateTime PeriodStartDate { get; set; }

    /// <summary>
    /// Reporting period end date (Turkey time)
    /// </summary>
    public DateTime PeriodEndDate { get; set; }

    /// <summary>
    /// Generated report file name
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// Report file size in bytes
    /// </summary>
    public long FileSizeBytes { get; set; }

    /// <summary>
    /// Number of CDR records included in the report
    /// </summary>
    public int RecordsProcessed { get; set; }

    /// <summary>
    /// Time taken to generate the report in milliseconds
    /// </summary>
    public long GenerationDurationMs { get; set; }

    /// <summary>
    /// Whether emails were sent
    /// </summary>
    public bool EmailsSent { get; set; }

    /// <summary>
    /// Number of email recipients
    /// </summary>
    public int RecipientsCount { get; set; }

    /// <summary>
    /// Number of successful email deliveries
    /// </summary>
    public int SuccessfulDeliveries { get; set; }

    /// <summary>
    /// Number of failed email deliveries
    /// </summary>
    public int FailedDeliveries { get; set; }

    /// <summary>
    /// Summary of key metrics in the report
    /// </summary>
    public CdrReportMetricsSummary? MetricsSummary { get; set; }

    /// <summary>
    /// Per-recipient delivery status
    /// </summary>
    public List<EmailDeliveryStatusResponse> DeliveryStatus { get; set; } = new();
}
