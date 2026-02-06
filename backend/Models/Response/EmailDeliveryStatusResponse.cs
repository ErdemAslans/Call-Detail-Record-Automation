namespace Cdr.Api.Models.Response;

/// <summary>
/// Email delivery status for a single recipient.
/// </summary>
public class EmailDeliveryStatusResponse
{
    /// <summary>
    /// Recipient email address
    /// </summary>
    public string RecipientEmail { get; set; } = string.Empty;

    /// <summary>
    /// Recipient display name (if available)
    /// </summary>
    public string? RecipientName { get; set; }

    /// <summary>
    /// Delivery status: Pending, Sent, Failed, Bounced
    /// </summary>
    public string Status { get; set; } = "Pending";

    /// <summary>
    /// Number of delivery attempts made
    /// </summary>
    public int AttemptCount { get; set; }

    /// <summary>
    /// When the email was successfully delivered (null if not yet delivered)
    /// </summary>
    public DateTime? DeliveredAt { get; set; }

    /// <summary>
    /// Error message if delivery failed
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// SMTP error code if available
    /// </summary>
    public string? SmtpErrorCode { get; set; }

    /// <summary>
    /// Whether this is a retry-able failure
    /// </summary>
    public bool CanRetry { get; set; }
}
