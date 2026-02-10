using Cdr.Api.Context;
using Cdr.Api.Interfaces;
using Cdr.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Cdr.Api.Repositories;

/// <summary>
/// Repository implementation for ReportExecutionLog entity.
/// </summary>
public class ReportExecutionLogRepository : IReportExecutionLogRepository
{
    private readonly CdrContext _context;

    public ReportExecutionLogRepository(CdrContext context)
    {
        _context = context;
    }

    public async Task<ReportExecutionLog?> GetByIdAsync(Guid id)
    {
        return await _context.ReportExecutionLogs
            .Include(r => r.EmailDeliveries)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<ReportExecutionLog?> GetByHangfireJobIdAsync(string hangfireJobId)
    {
        return await _context.ReportExecutionLogs
            .Include(r => r.EmailDeliveries)
            .FirstOrDefaultAsync(r => r.HangfireJobId == hangfireJobId);
    }

    public async Task<List<ReportExecutionLog>> GetRecentAsync(int count = 20)
    {
        return await _context.ReportExecutionLogs
            .Include(r => r.EmailDeliveries)
            .OrderByDescending(r => r.StartedAt)
            .Take(count)
            .ToListAsync();
    }

    public async Task<List<ReportExecutionLog>> GetByReportTypeAsync(string reportType, int count = 50)
    {
        return await _context.ReportExecutionLogs
            .Where(r => r.ReportType == reportType)
            .OrderByDescending(r => r.StartedAt)
            .Take(count)
            .ToListAsync();
    }

    public async Task<List<ReportExecutionLog>> GetByStatusAsync(string status, int count = 50)
    {
        return await _context.ReportExecutionLogs
            .Where(r => r.ExecutionStatus == status)
            .OrderByDescending(r => r.StartedAt)
            .Take(count)
            .ToListAsync();
    }

    public async Task<List<ReportExecutionLog>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.ReportExecutionLogs
            .Where(r => r.StartedAt >= startDate && r.StartedAt <= endDate)
            .OrderByDescending(r => r.StartedAt)
            .ToListAsync();
    }

    public async Task<List<ReportExecutionLog>> GetFailedExecutionsAsync(int hours = 24)
    {
        var cutoff = DateTime.UtcNow.AddHours(-hours);
        return await _context.ReportExecutionLogs
            .Where(r => r.ExecutionStatus == "Failed" && r.StartedAt >= cutoff)
            .OrderByDescending(r => r.StartedAt)
            .ToListAsync();
    }

    public async Task<ReportExecutionLog> CreateAsync(ReportExecutionLog log)
    {
        log.StartedAt = DateTime.UtcNow;
        _context.ReportExecutionLogs.Add(log);
        await _context.SaveChangesAsync();
        return log;
    }

    public async Task<ReportExecutionLog> UpdateAsync(ReportExecutionLog log)
    {
        _context.ReportExecutionLogs.Update(log);
        await _context.SaveChangesAsync();
        return log;
    }

    public async Task UpdateStatusAsync(
        Guid id, 
        string status, 
        long? generationDurationMs = null,
        int? recordsProcessed = null,
        long? fileSizeBytes = null,
        string? errorMessage = null)
    {
        var log = await _context.ReportExecutionLogs.FindAsync(id);
        if (log == null) return;

        log.ExecutionStatus = status;
        
        if (status == "Completed" || status == "Failed")
        {
            log.CompletedAt = DateTime.UtcNow;
        }

        if (generationDurationMs.HasValue)
        {
            log.GenerationDurationMs = generationDurationMs.Value;
        }

        if (recordsProcessed.HasValue)
        {
            log.RecordsProcessed = recordsProcessed.Value;
        }

        if (fileSizeBytes.HasValue)
        {
            log.FileSizeBytes = fileSizeBytes.Value;
        }

        if (!string.IsNullOrEmpty(errorMessage))
        {
            log.ErrorMessage = errorMessage;
        }

        await _context.SaveChangesAsync();
    }

    public async Task<ReportExecutionStatistics> GetStatisticsAsync(DateTime startDate, DateTime endDate)
    {
        var logs = await _context.ReportExecutionLogs
            .Where(r => r.StartedAt >= startDate && r.StartedAt <= endDate)
            .ToListAsync();

        var completedLogs = logs.Where(l => l.ExecutionStatus == "Completed").ToList();

        var stats = new ReportExecutionStatistics
        {
            TotalExecutions = logs.Count,
            SuccessfulExecutions = completedLogs.Count,
            FailedExecutions = logs.Count(l => l.ExecutionStatus == "Failed"),
            AverageGenerationTimeMs = completedLogs.Any() 
                ? (long)completedLogs.Average(l => l.GenerationDurationMs ?? 0) 
                : 0,
            MaxGenerationTimeMs = completedLogs.Any() 
                ? completedLogs.Max(l => l.GenerationDurationMs ?? 0) 
                : 0,
            TotalRecordsProcessed = logs.Sum(l => l.RecordsProcessed ?? 0),
            ExecutionsByReportType = logs
                .GroupBy(l => l.ReportType)
                .ToDictionary(g => g.Key, g => g.Count()),
            ExecutionsByStatus = logs
                .GroupBy(l => l.ExecutionStatus)
                .ToDictionary(g => g.Key, g => g.Count())
        };

        return stats;
    }

    public async Task<ReportExecutionLog?> GetLastSuccessfulAsync(string reportType)
    {
        return await _context.ReportExecutionLogs
            .Where(r => r.ReportType == reportType && r.ExecutionStatus == "Completed")
            .OrderByDescending(r => r.CompletedAt)
            .FirstOrDefaultAsync();
    }
}
