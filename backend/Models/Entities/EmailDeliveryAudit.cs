using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cdr.Api.Models.Entities;

/// <summary>
/// Tracks email delivery status for CDR reports - used for audit logging
/// and retry mechanism tracking per FR-012 requirements.
/// </summary>
public class EmailDeliveryAudit
{
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// Reference to the report execution that generated this email
    /// </summary>
    public Guid ReportExecutionId { get; set; }

    /// <summary>
    /// Email recipient address
    /// </summary>
    [Required]
    [MaxLength(255)]
    public string RecipientEmail { get; set; } = string.Empty;

    /// <summary>
    /// Delivery status: Pending, Sent, Failed, Bounced
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string DeliveryStatus { get; set; } = "Pending";

    /// <summary>
    /// Number of delivery attempts made (max 3 per FR-012)
    /// </summary>
    public int AttemptCount { get; set; } = 0;

    /// <summary>
    /// Timestamp of first delivery attempt
    /// </summary>
    public DateTime? FirstAttemptAt { get; set; }

    /// <summary>
    /// Timestamp of last delivery attempt
    /// </summary>
    public DateTime? LastAttemptAt { get; set; }

    /// <summary>
    /// Timestamp when email was successfully delivered
    /// </summary>
    public DateTime? DeliveredAt { get; set; }

    /// <summary>
    /// Error message if delivery failed
    /// </summary>
    [MaxLength(2000)]
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// SMTP error code if available
    /// </summary>
    [MaxLength(50)]
    public string? SmtpErrorCode { get; set; }

    /// <summary>
    /// Email subject line for reference
    /// </summary>
    [MaxLength(500)]
    public string? EmailSubject { get; set; }

    /// <summary>
    /// Attachment filename if present
    /// </summary>
    [MaxLength(255)]
    public string? AttachmentFileName { get; set; }

    /// <summary>
    /// Attachment size in bytes
    /// </summary>
    public long? AttachmentSizeBytes { get; set; }

    /// <summary>
    /// Record creation timestamp (UTC)
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Record last update timestamp (UTC)
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    [ForeignKey(nameof(ReportExecutionId))]
    public virtual ReportExecutionLog? ReportExecution { get; set; }
}
