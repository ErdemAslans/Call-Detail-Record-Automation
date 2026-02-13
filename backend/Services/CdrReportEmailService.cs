using Cdr.Api.Context;
using Cdr.Api.Helpers;
using Cdr.Api.Interfaces;
using Cdr.Api.Models;
using Cdr.Api.Models.Entities;
using Cdr.Api.Models.Response;
using Interfaces.Notification;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Net.Mail;
using System.Text;

namespace Cdr.Api.Services;

/// <summary>
/// Service interface for sending CDR report emails.
/// </summary>
public interface ICdrReportEmailService
{
    /// <summary>
    /// Send a generated report via email to all admin recipients.
    /// </summary>
    Task<EmailDeliveryResult> SendReportEmailAsync(CdrReportResult report, string? triggerType = "Scheduled");

    /// <summary>
    /// Send a generated report to specific recipients.
    /// </summary>
    Task<EmailDeliveryResult> SendReportEmailAsync(CdrReportResult report, List<string> recipients, string? triggerType = "OnDemand");

    /// <summary>
    /// Get the list of admin email recipients (excluding switchboard staff).
    /// </summary>
    Task<List<string>> GetAdminRecipientsAsync();
}

/// <summary>
/// Service for composing and sending CDR report emails with retry logic.
/// </summary>
public class CdrReportEmailService : ICdrReportEmailService
{
    private readonly INotification<EmailMessage> _emailNotification;
    private readonly IEmailDeliveryAuditRepository _emailDeliveryAuditRepository;
    private readonly IReportExecutionLogRepository _reportExecutionLogRepository;
    private readonly UserManager<User> _userManager;
    private readonly CdrReportingSettings _reportingSettings;
    private readonly EmailSettings _emailSettings;
    private readonly ILogger<CdrReportEmailService> _logger;
    private readonly IWebHostEnvironment _environment;

    // Retry configuration from spec
    private const int MaxRetryAttempts = 3;
    private const int RetryDelayMinutes = 5;
    private const int EmailDelayMs = 2000; // 2 seconds between emails

    public CdrReportEmailService(
        INotification<EmailMessage> emailNotification,
        IEmailDeliveryAuditRepository emailDeliveryAuditRepository,
        IReportExecutionLogRepository reportExecutionLogRepository,
        UserManager<User> userManager,
        IOptions<CdrReportingSettings> reportingSettings,
        IOptions<EmailSettings> emailSettings,
        ILogger<CdrReportEmailService> logger,
        IWebHostEnvironment environment)
    {
        _emailNotification = emailNotification;
        _emailDeliveryAuditRepository = emailDeliveryAuditRepository;
        _reportExecutionLogRepository = reportExecutionLogRepository;
        _userManager = userManager;
        _reportingSettings = reportingSettings.Value;
        _emailSettings = emailSettings.Value;
        _logger = logger;
        _environment = environment;
    }

    /// <inheritdoc />
    public async Task<EmailDeliveryResult> SendReportEmailAsync(CdrReportResult report, string? triggerType = "Scheduled")
    {
        var recipients = await GetAdminRecipientsAsync();
        return await SendReportEmailAsync(report, recipients, triggerType);
    }

    /// <inheritdoc />
    public async Task<EmailDeliveryResult> SendReportEmailAsync(
        CdrReportResult report, 
        List<string> recipients, 
        string? triggerType = "OnDemand")
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = new EmailDeliveryResult
        {
            ReportExecutionId = report.ExecutionId,
            TotalRecipients = recipients.Count
        };

        if (recipients.Count == 0)
        {
            _logger.LogWarning("No recipients found for {ReportType} report", report.ReportType);
            return result;
        }

        // Save report file to disk
        var filePath = await SaveReportFileAsync(report);

        // Compose email content
        var subject = CdrReportHelper.GenerateEmailSubject(
            report.ReportType, 
            report.PeriodStartDate, 
            report.PeriodEndDate);
        var body = ComposeEmailBody(report);

        _logger.LogInformation(
            "Sending {ReportType} report to {RecipientCount} recipients",
            report.ReportType, recipients.Count);

        // Send to each recipient with delay between sends
        foreach (var recipient in recipients)
        {
            var deliveryStatus = await SendToRecipientWithRetryAsync(
                report.ExecutionId,
                recipient,
                subject,
                body,
                filePath);

            result.RecipientStatuses.Add(deliveryStatus);

            if (deliveryStatus.Status == "Sent")
            {
                result.SuccessfulDeliveries++;
            }
            else
            {
                result.FailedDeliveries++;
            }

            // Add delay between emails (except for last one)
            if (recipient != recipients.Last())
            {
                await Task.Delay(EmailDelayMs);
            }
        }

        stopwatch.Stop();
        result.DeliveryDurationMs = stopwatch.ElapsedMilliseconds;

        // Update report execution log with email delivery stats
        var executionLog = await _reportExecutionLogRepository.GetByIdAsync(report.ExecutionId);
        if (executionLog != null)
        {
            executionLog.RecipientsCount = result.TotalRecipients;
            executionLog.SuccessfulDeliveries = result.SuccessfulDeliveries;
            executionLog.FailedDeliveries = result.FailedDeliveries;
            executionLog.EmailDeliveryDurationMs = result.DeliveryDurationMs;
            await _reportExecutionLogRepository.UpdateAsync(executionLog);
        }

        _logger.LogInformation(
            "{ReportType} email delivery completed. Success: {Success}, Failed: {Failed}, Duration: {Duration}ms",
            report.ReportType, result.SuccessfulDeliveries, result.FailedDeliveries, result.DeliveryDurationMs);

        return result;
    }

    /// <inheritdoc />
    public async Task<List<string>> GetAdminRecipientsAsync()
    {
        var recipients = new List<string>();

        try
        {
            // Get Admin users from Identity
            var adminUsers = await _userManager.GetUsersInRoleAsync("Admin");
            
            foreach (var user in adminUsers)
            {
                if (!string.IsNullOrEmpty(user.Email) && 
                    CdrReportHelper.IsValidEmail(user.Email) &&
                    !IsExcludedRecipient(user.Email))
                {
                    recipients.Add(user.Email);
                }
            }

            // Add default recipients from configuration (if not already included)
            foreach (var defaultRecipient in _reportingSettings.DefaultRecipients)
            {
                if (!recipients.Contains(defaultRecipient, StringComparer.OrdinalIgnoreCase) &&
                    !IsExcludedRecipient(defaultRecipient))
                {
                    recipients.Add(defaultRecipient);
                }
            }

            _logger.LogDebug("Found {Count} admin recipients for report emails", recipients.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve admin recipients. Using default list.");
            
            // Fallback to configured default recipients
            recipients = _reportingSettings.DefaultRecipients
                .Where(r => !IsExcludedRecipient(r))
                .ToList();
        }

        return recipients.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
    }

    /// <summary>
    /// Check if an email address should be excluded (e.g., switchboard staff).
    /// </summary>
    private bool IsExcludedRecipient(string email)
    {
        return _reportingSettings.ExcludedRecipients
            .Any(e => e.Equals(email, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Send email to a single recipient with retry logic.
    /// </summary>
    private async Task<EmailDeliveryStatusResponse> SendToRecipientWithRetryAsync(
        Guid reportExecutionId,
        string recipient,
        string subject,
        string body,
        string attachmentPath)
    {
        var status = new EmailDeliveryStatusResponse
        {
            RecipientEmail = recipient,
            Status = "Pending"
        };

        // Create audit record
        var audit = new EmailDeliveryAudit
        {
            Id = Guid.NewGuid(),
            ReportExecutionId = reportExecutionId,
            RecipientEmail = recipient,
            DeliveryStatus = "Pending",
            EmailSubject = subject,
            AttachmentFileName = Path.GetFileName(attachmentPath),
            AttachmentSizeBytes = File.Exists(attachmentPath) ? new FileInfo(attachmentPath).Length : 0,
            CreatedAt = DateTime.UtcNow
        };
        await _emailDeliveryAuditRepository.AddAsync(audit);

        // Attempt delivery with retry
        for (int attempt = 1; attempt <= MaxRetryAttempts; attempt++)
        {
            try
            {
                audit.AttemptCount = attempt;
                audit.LastAttemptAt = DateTime.UtcNow;
                
                if (attempt == 1)
                {
                    audit.FirstAttemptAt = DateTime.UtcNow;
                }

                var emailMessage = new EmailMessage
                {
                    Subject = subject,
                    Body = body,
                    To = new List<string> { recipient },
                    Attachments = File.Exists(attachmentPath) 
                        ? new List<string> { attachmentPath } 
                        : new List<string>()
                };

                await _emailNotification.SendAsync(emailMessage);

                // Success
                audit.DeliveryStatus = "Sent";
                audit.DeliveredAt = DateTime.UtcNow;
                await _emailDeliveryAuditRepository.UpdateAsync(audit);

                status.Status = "Sent";
                status.AttemptCount = attempt;
                status.DeliveredAt = DateTime.UtcNow;

                _logger.LogDebug("Email sent successfully to {Recipient} on attempt {Attempt}", recipient, attempt);
                return status;
            }
            catch (SmtpException ex)
            {
                _logger.LogWarning(ex, 
                    "SMTP error sending to {Recipient}, attempt {Attempt}/{MaxAttempts}",
                    recipient, attempt, MaxRetryAttempts);

                audit.ErrorMessage = ex.Message;
                audit.SmtpErrorCode = ex.StatusCode.ToString();

                status.ErrorMessage = ex.Message;
                status.SmtpErrorCode = ex.StatusCode.ToString();
                status.AttemptCount = attempt;

                // Wait before retry (except on last attempt)
                if (attempt < MaxRetryAttempts)
                {
                    await Task.Delay(TimeSpan.FromMinutes(RetryDelayMinutes));
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, 
                    "Error sending to {Recipient}, attempt {Attempt}/{MaxAttempts}",
                    recipient, attempt, MaxRetryAttempts);

                audit.ErrorMessage = ex.Message;
                status.ErrorMessage = ex.Message;
                status.AttemptCount = attempt;

                // Wait before retry (except on last attempt)
                if (attempt < MaxRetryAttempts)
                {
                    await Task.Delay(TimeSpan.FromMinutes(RetryDelayMinutes));
                }
            }
        }

        // All attempts failed
        audit.DeliveryStatus = "Failed";
        await _emailDeliveryAuditRepository.UpdateAsync(audit);

        status.Status = "Failed";
        status.CanRetry = false;

        _logger.LogError("Failed to send email to {Recipient} after {MaxAttempts} attempts", 
            recipient, MaxRetryAttempts);

        return status;
    }

    /// <summary>
    /// Compose HTML email body with report summary.
    /// </summary>
    private string ComposeEmailBody(CdrReportResult report)
    {
        var sb = new StringBuilder();
        var trCulture = new CultureInfo("tr-TR");
        var turkeyStart = CdrReportHelper.ToTurkeyTime(report.PeriodStartDate);
        var turkeyEnd = CdrReportHelper.ToTurkeyTime(report.PeriodEndDate);
        var turkeyGeneratedAt = CdrReportHelper.ToTurkeyTime(report.GeneratedAt);

        sb.AppendLine("<!DOCTYPE html>");
        sb.AppendLine("<html lang=\"tr\">");
        sb.AppendLine("<head><meta charset=\"UTF-8\"></head>");
        sb.AppendLine("<body style=\"font-family: Arial, sans-serif; line-height: 1.6; color: #333;\">");

        sb.AppendLine("<div style=\"max-width: 600px; margin: 0 auto; padding: 20px;\">");

        // Header
        sb.AppendLine("<h2 style=\"color: #1a5f7a; border-bottom: 2px solid #1a5f7a; padding-bottom: 10px;\">");
        sb.AppendLine($"📊 Doğuş Oto {report.ReportType} CDR Raporu");
        sb.AppendLine("</h2>");

        // Period info
        sb.AppendLine("<p>");
        sb.AppendLine($"<strong>Rapor Dönemi:</strong> {turkeyStart.ToString("dd MMMM yyyy", trCulture)} - {turkeyEnd.ToString("dd MMMM yyyy", trCulture)}<br>");
        sb.AppendLine($"<strong>Oluşturulma Zamanı:</strong> {turkeyGeneratedAt.ToString("dd MMMM yyyy HH:mm:ss", trCulture)}");
        sb.AppendLine("</p>");

        // Metrics summary
        if (report.MetricsSummary != null)
        {
            sb.AppendLine("<h3 style=\"color: #1a5f7a;\">📈 Özet İstatistikler</h3>");
            sb.AppendLine("<table style=\"width: 100%; border-collapse: collapse; margin-bottom: 20px;\">");
            
            AddMetricRow(sb, "Toplam Gelen Arama", report.MetricsSummary.TotalIncomingCalls.ToString("N0"));
            AddMetricRow(sb, "Cevaplanan Arama", report.MetricsSummary.TotalAnsweredCalls.ToString("N0"));
            AddMetricRow(sb, "Cevapsız Arama", report.MetricsSummary.TotalMissedCalls.ToString("N0"));
            AddMetricRow(sb, "Molada Gelen Çağrı", report.MetricsSummary.TotalOnBreakCalls.ToString("N0"));
            AddMetricRow(sb, "Yönlendirilen Çağrı", report.MetricsSummary.TotalRedirectedCalls.ToString("N0"));
            AddMetricRow(sb, "Cevaplama Oranı", $"{report.MetricsSummary.AnswerRate:F1}%");
            AddMetricRow(sb, "Toplam Giden Arama", report.MetricsSummary.TotalOutgoingCalls.ToString("N0"));
            
            sb.AppendLine("</table>");

            // Break summary section (actual breaks only)
            if (report.MetricsSummary.BreakSummaries.Count > 0)
            {
                sb.AppendLine("<h3 style=\"color: #1a5f7a;\">☕ Mola Özeti</h3>");
                sb.AppendLine($"<p><strong>Toplam Mola Sayısı:</strong> {report.MetricsSummary.TotalBreakCount} &nbsp; | &nbsp; ");
                sb.AppendLine($"<strong>Toplam Mola Süresi:</strong> {FormatMinutes(report.MetricsSummary.TotalBreakDurationMinutes)}</p>");

                AppendBreakTable(sb, report.MetricsSummary.BreakSummaries, "#1a5f7a", "Mola Sayısı", "Mola Saatleri", trCulture);
            }

            // End-of-shift summary section
            if (report.MetricsSummary.ShiftEndSummaries.Count > 0)
            {
                sb.AppendLine("<h3 style=\"color: #d97706;\">🏢 Mesai Bitimi</h3>");
                sb.AppendLine($"<p><strong>Toplam Mesai Bitimi Kaydı:</strong> {report.MetricsSummary.TotalShiftEndCount}</p>");

                AppendBreakTable(sb, report.MetricsSummary.ShiftEndSummaries, "#d97706", "Kayıt Sayısı", "Mesai Bitiş Saatleri", trCulture);
            }
        }

        // File info
        sb.AppendLine("<h3 style=\"color: #1a5f7a;\">📎 Ek Dosya</h3>");
        sb.AppendLine("<p>");
        sb.AppendLine($"<strong>Dosya Adı:</strong> {report.FileName}<br>");
        sb.AppendLine($"<strong>Dosya Boyutu:</strong> {FormatFileSize(report.FileSizeBytes)}<br>");
        sb.AppendLine($"<strong>İşlenen Kayıt:</strong> {report.RecordsProcessed:N0} adet");
        sb.AppendLine("</p>");

        // Footer
        sb.AppendLine("<hr style=\"border: none; border-top: 1px solid #ddd; margin: 20px 0;\">");
        sb.AppendLine("<p style=\"font-size: 12px; color: #666;\">");
        sb.AppendLine("Bu e-posta Doğuş Oto CDR Raporlama Sistemi tarafından otomatik olarak gönderilmiştir.<br>");
        sb.AppendLine("Sorularınız için sistem yöneticinize başvurunuz.");
        sb.AppendLine("</p>");

        sb.AppendLine("</div>");
        sb.AppendLine("</body>");
        sb.AppendLine("</html>");

        return sb.ToString();
    }

    private void AddMetricRow(StringBuilder sb, string label, string value)
    {
        sb.AppendLine("<tr>");
        sb.AppendLine($"<td style=\"padding: 8px; border-bottom: 1px solid #ddd;\">{label}</td>");
        sb.AppendLine($"<td style=\"padding: 8px; border-bottom: 1px solid #ddd; text-align: right; font-weight: bold;\">{value}</td>");
        sb.AppendLine("</tr>");
    }

    private string FormatMinutes(double totalMinutes)
    {
        var hours = (int)(totalMinutes / 60);
        var minutes = (int)(totalMinutes % 60);
        if (hours > 0)
            return $"{hours} sa {minutes} dk";
        return $"{minutes} dk";
    }

    private void AppendBreakTable(StringBuilder sb, List<OperatorBreakSummary> summaries, string headerColor, string countLabel, string timesLabel, System.Globalization.CultureInfo culture)
    {
        sb.AppendLine("<table style=\"width: 100%; border-collapse: collapse; margin-bottom: 20px;\">");
        sb.AppendLine($"<tr style=\"background-color: {headerColor}; color: white;\">");
        sb.AppendLine("<th style=\"padding: 8px; text-align: left;\">Operatör</th>");
        sb.AppendLine($"<th style=\"padding: 8px; text-align: center;\">{countLabel}</th>");
        sb.AppendLine("<th style=\"padding: 8px; text-align: center;\">Toplam Süre</th>");
        sb.AppendLine($"<th style=\"padding: 8px; text-align: left;\">{timesLabel}</th>");
        sb.AppendLine("</tr>");

        foreach (var opBreak in summaries)
        {
            var breakTimes = string.Join("<br>", opBreak.Breaks.Select(b =>
            {
                var start = b.StartTime.ToString("HH:mm", culture);
                var end = b.EndTime?.ToString("HH:mm", culture) ?? "Devam";
                return $"{start} - {end} ({b.DurationMinutes:F0} dk)";
            }));

            sb.AppendLine("<tr>");
            sb.AppendLine($"<td style=\"padding: 8px; border-bottom: 1px solid #ddd;\">{opBreak.OperatorName}<br><small style=\"color: #666;\">{opBreak.PhoneNumber}</small></td>");
            sb.AppendLine($"<td style=\"padding: 8px; border-bottom: 1px solid #ddd; text-align: center; font-weight: bold;\">{opBreak.BreakCount}</td>");
            sb.AppendLine($"<td style=\"padding: 8px; border-bottom: 1px solid #ddd; text-align: center;\">{FormatMinutes(opBreak.TotalDurationMinutes)}</td>");
            sb.AppendLine($"<td style=\"padding: 8px; border-bottom: 1px solid #ddd; font-size: 12px;\">{breakTimes}</td>");
            sb.AppendLine("</tr>");
        }

        sb.AppendLine("</table>");
    }

    private string FormatFileSize(long bytes)
    {
        if (bytes < 1024) return $"{bytes} B";
        if (bytes < 1024 * 1024) return $"{bytes / 1024.0:F1} KB";
        return $"{bytes / (1024.0 * 1024.0):F1} MB";
    }

    /// <summary>
    /// Save report Excel file to disk for attachment.
    /// </summary>
    private async Task<string> SaveReportFileAsync(CdrReportResult report)
    {
        var reportsDir = Path.Combine(
            _environment.ContentRootPath,
            _reportingSettings.ReportStoragePath);

        Directory.CreateDirectory(reportsDir);

        // Add executionId to filename to avoid file locking conflicts
        var uniqueFileName = Path.GetFileNameWithoutExtension(report.FileName)
            + $"_{report.ExecutionId:N}"
            + Path.GetExtension(report.FileName);
        var filePath = Path.Combine(reportsDir, uniqueFileName);
        await File.WriteAllBytesAsync(filePath, report.ExcelData);

        _logger.LogDebug("Saved report file to {FilePath}", filePath);
        return filePath;
    }
}
