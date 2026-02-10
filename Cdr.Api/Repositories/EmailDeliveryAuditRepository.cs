using Cdr.Api.Context;
using Cdr.Api.Interfaces;
using Cdr.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Cdr.Api.Repositories;

/// <summary>
/// Repository implementation for EmailDeliveryAudit entity.
/// </summary>
public class EmailDeliveryAuditRepository : IEmailDeliveryAuditRepository
{
    private readonly CdrContext _context;

    public EmailDeliveryAuditRepository(CdrContext context)
    {
        _context = context;
    }

    public async Task<EmailDeliveryAudit?> GetByIdAsync(Guid id)
    {
        return await _context.EmailDeliveryAudits
            .Include(e => e.ReportExecution)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<List<EmailDeliveryAudit>> GetByReportExecutionIdAsync(Guid reportExecutionId)
    {
        return await _context.EmailDeliveryAudits
            .Where(e => e.ReportExecutionId == reportExecutionId)
            .OrderBy(e => e.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<EmailDeliveryAudit>> GetByStatusAsync(string status, int maxResults = 100)
    {
        return await _context.EmailDeliveryAudits
            .Where(e => e.DeliveryStatus == status)
            .OrderByDescending(e => e.CreatedAt)
            .Take(maxResults)
            .ToListAsync();
    }

    public async Task<List<EmailDeliveryAudit>> GetPendingRetriesAsync(int maxAttempts = 3)
    {
        return await _context.EmailDeliveryAudits
            .Where(e => e.DeliveryStatus == "Failed" && e.AttemptCount < maxAttempts)
            .OrderBy(e => e.LastAttemptAt)
            .ToListAsync();
    }

    public async Task<EmailDeliveryAudit> AddAsync(EmailDeliveryAudit audit)
    {
        audit.CreatedAt = DateTime.UtcNow;
        _context.EmailDeliveryAudits.Add(audit);
        await _context.SaveChangesAsync();
        return audit;
    }

    public async Task<List<EmailDeliveryAudit>> AddRangeAsync(List<EmailDeliveryAudit> audits)
    {
        var now = DateTime.UtcNow;
        foreach (var audit in audits)
        {
            audit.CreatedAt = now;
        }
        
        _context.EmailDeliveryAudits.AddRange(audits);
        await _context.SaveChangesAsync();
        return audits;
    }

    public async Task<EmailDeliveryAudit> UpdateAsync(EmailDeliveryAudit audit)
    {
        _context.EmailDeliveryAudits.Update(audit);
        await _context.SaveChangesAsync();
        return audit;
    }

    public async Task UpdateStatusAsync(Guid auditId, string status, string? errorMessage = null, string? smtpErrorCode = null)
    {
        var audit = await _context.EmailDeliveryAudits.FindAsync(auditId);
        if (audit == null) return;

        audit.DeliveryStatus = status;
        audit.AttemptCount++;
        audit.LastAttemptAt = DateTime.UtcNow;
        
        if (status == "Sent")
        {
            audit.DeliveredAt = DateTime.UtcNow;
        }
        
        if (!string.IsNullOrEmpty(errorMessage))
        {
            audit.ErrorMessage = errorMessage;
        }
        
        if (!string.IsNullOrEmpty(smtpErrorCode))
        {
            audit.SmtpErrorCode = smtpErrorCode;
        }

        await _context.SaveChangesAsync();
    }

    public async Task<EmailDeliveryStatistics> GetStatisticsAsync(DateTime startDate, DateTime endDate)
    {
        var audits = await _context.EmailDeliveryAudits
            .Where(e => e.CreatedAt >= startDate && e.CreatedAt <= endDate)
            .ToListAsync();

        var stats = new EmailDeliveryStatistics
        {
            TotalSent = audits.Count,
            TotalSuccessful = audits.Count(a => a.DeliveryStatus == "Sent"),
            TotalFailed = audits.Count(a => a.DeliveryStatus == "Failed"),
            TotalPending = audits.Count(a => a.DeliveryStatus == "Pending"),
            FailureReasons = audits
                .Where(a => a.DeliveryStatus == "Failed" && !string.IsNullOrEmpty(a.ErrorMessage))
                .GroupBy(a => a.ErrorMessage!)
                .ToDictionary(g => g.Key, g => g.Count())
        };

        return stats;
    }
}
