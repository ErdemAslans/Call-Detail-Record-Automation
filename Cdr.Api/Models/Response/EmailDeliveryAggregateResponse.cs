namespace Cdr.Api.Models.Response;

/// <summary>
/// Aggregate email delivery response
/// </summary>
public class EmailDeliveryAggregateResponse
{
    public Guid ReportExecutionId { get; set; }
    public int TotalRecipients { get; set; }
    public int SuccessfulDeliveries { get; set; }
    public int FailedDeliveries { get; set; }
    public int PendingDeliveries { get; set; }
    public string OverallStatus { get; set; } = "Pending";
    public List<EmailDeliveryStatusResponse> RecipientStatuses { get; set; } = new();
}
