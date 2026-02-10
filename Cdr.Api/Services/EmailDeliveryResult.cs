using Cdr.Api.Models.Response;

namespace Cdr.Api.Services;

/// <summary>
/// Result of email delivery operation.
/// </summary>
public class EmailDeliveryResult
{
    public Guid ReportExecutionId { get; set; }
    public int TotalRecipients { get; set; }
    public int SuccessfulDeliveries { get; set; }
    public int FailedDeliveries { get; set; }
    public List<EmailDeliveryStatusResponse> RecipientStatuses { get; set; } = new();
    public long DeliveryDurationMs { get; set; }
    public bool IsSuccess => FailedDeliveries == 0;
    public bool IsPartialSuccess => SuccessfulDeliveries > 0 && FailedDeliveries > 0;
}
