namespace Cdr.Api.Models.Response;

/// <summary>
/// Summary of a report execution for history listing.
/// </summary>
public class ReportExecutionSummary
{
    public Guid ExecutionId { get; set; }
    public string ReportType { get; set; } = string.Empty;
    public string TriggerType { get; set; } = string.Empty;
    public DateTime PeriodStartDate { get; set; }
    public DateTime PeriodEndDate { get; set; }
    public string ExecutionStatus { get; set; } = string.Empty;
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int RecordsProcessed { get; set; }
    public int RecipientsCount { get; set; }
    public int SuccessfulDeliveries { get; set; }
    public int FailedDeliveries { get; set; }
}
