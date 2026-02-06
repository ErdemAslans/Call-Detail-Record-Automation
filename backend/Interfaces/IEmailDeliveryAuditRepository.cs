using Cdr.Api.Models.Entities;

namespace Cdr.Api.Interfaces;

/// <summary>
/// Repository interface for EmailDeliveryAudit entity operations.
/// </summary>
public interface IEmailDeliveryAuditRepository
{
    /// <summary>
    /// Get a single audit record by ID.
    /// </summary>
    Task<EmailDeliveryAudit?> GetByIdAsync(Guid id);

    /// <summary>
    /// Get all delivery records for a specific report execution.
    /// </summary>
    Task<List<EmailDeliveryAudit>> GetByReportExecutionIdAsync(Guid reportExecutionId);

    /// <summary>
    /// Get delivery records by status (for retry processing).
    /// </summary>
    Task<List<EmailDeliveryAudit>> GetByStatusAsync(string status, int maxResults = 100);

    /// <summary>
    /// Get delivery records that need retry (Failed status, attempt count < max).
    /// </summary>
    Task<List<EmailDeliveryAudit>> GetPendingRetriesAsync(int maxAttempts = 3);

    /// <summary>
    /// Add a new delivery audit record.
    /// </summary>
    Task<EmailDeliveryAudit> AddAsync(EmailDeliveryAudit audit);

    /// <summary>
    /// Add multiple delivery audit records.
    /// </summary>
    Task<List<EmailDeliveryAudit>> AddRangeAsync(List<EmailDeliveryAudit> audits);

    /// <summary>
    /// Update an existing audit record.
    /// </summary>
    Task<EmailDeliveryAudit> UpdateAsync(EmailDeliveryAudit audit);

    /// <summary>
    /// Update delivery status for a recipient.
    /// </summary>
    Task UpdateStatusAsync(Guid auditId, string status, string? errorMessage = null, string? smtpErrorCode = null);

    /// <summary>
    /// Get delivery statistics for a time period.
    /// </summary>
    Task<EmailDeliveryStatistics> GetStatisticsAsync(DateTime startDate, DateTime endDate);
}
