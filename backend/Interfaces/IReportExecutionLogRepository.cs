using Cdr.Api.Models.Entities;

namespace Cdr.Api.Interfaces;

/// <summary>
/// Repository interface for ReportExecutionLog entity operations.
/// </summary>
public interface IReportExecutionLogRepository
{
    /// <summary>
    /// Get a single execution log by ID.
    /// </summary>
    Task<ReportExecutionLog?> GetByIdAsync(Guid id);

    /// <summary>
    /// Get execution log by Hangfire job ID.
    /// </summary>
    Task<ReportExecutionLog?> GetByHangfireJobIdAsync(string hangfireJobId);

    /// <summary>
    /// Get recent execution logs (for dashboard).
    /// </summary>
    Task<List<ReportExecutionLog>> GetRecentAsync(int count = 20);

    /// <summary>
    /// Get execution logs by report type.
    /// </summary>
    Task<List<ReportExecutionLog>> GetByReportTypeAsync(string reportType, int count = 50);

    /// <summary>
    /// Get execution logs by status (for monitoring).
    /// </summary>
    Task<List<ReportExecutionLog>> GetByStatusAsync(string status, int count = 50);

    /// <summary>
    /// Get execution logs within a date range.
    /// </summary>
    Task<List<ReportExecutionLog>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);

    /// <summary>
    /// Get failed executions for alerting/retry.
    /// </summary>
    Task<List<ReportExecutionLog>> GetFailedExecutionsAsync(int hours = 24);

    /// <summary>
    /// Create a new execution log entry.
    /// </summary>
    Task<ReportExecutionLog> CreateAsync(ReportExecutionLog log);

    /// <summary>
    /// Update an existing execution log.
    /// </summary>
    Task<ReportExecutionLog> UpdateAsync(ReportExecutionLog log);

    /// <summary>
    /// Update execution status and optionally set completion details.
    /// </summary>
    Task UpdateStatusAsync(
        Guid id, 
        string status, 
        long? generationDurationMs = null,
        int? recordsProcessed = null,
        long? fileSizeBytes = null,
        string? errorMessage = null);

    /// <summary>
    /// Get execution statistics for monitoring dashboard.
    /// </summary>
    Task<ReportExecutionStatistics> GetStatisticsAsync(DateTime startDate, DateTime endDate);

    /// <summary>
    /// Get the last successful execution for a report type.
    /// </summary>
    Task<ReportExecutionLog?> GetLastSuccessfulAsync(string reportType);
}
