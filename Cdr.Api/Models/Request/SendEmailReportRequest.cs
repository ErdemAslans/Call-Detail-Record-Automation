namespace Cdr.Api.Models.Request;

/// <summary>
/// Request to send a CDR report via email
/// </summary>
public class SendEmailReportRequest
{
    /// <summary>
    /// Report execution ID
    /// </summary>
    public Guid ReportExecutionId { get; set; }

    /// <summary>
    /// Target email addresses for report delivery
    /// </summary>
    public List<string> EmailRecipients { get; set; } = new();

    /// <summary>
    /// Whether to include attachments
    /// </summary>
    public bool IncludeAttachments { get; set; } = true;
}
