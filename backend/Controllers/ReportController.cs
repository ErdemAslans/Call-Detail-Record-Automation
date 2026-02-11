using Cdr.Api.Models;
using Cdr.Api.Models.Request;
using Cdr.Api.Models.Response;
using Cdr.Api.Models.Response.UserStatistics;
using Cdr.Api.Services;
using Cdr.Api.Services.Interfaces;
using Common.Enums;
using Hangfire;
using Interfaces.Notification;
using Microsoft.AspNetCore.Mvc;
using Cdr.Api.Extensions;
using Cdr.Api.Helpers;
using Cdr.Api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Cdr.Api.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize(Roles = "Admin")]
public class ReportController : ControllerBase
{
    private readonly ICdrRecordsService _cdrRecordsService;
    private readonly IOperatorService _operatorService;
    private readonly INotification<EmailMessage> _emailNotification;
    private readonly ICdrReportService _cdrReportService;
    private readonly ICdrReportEmailService _cdrReportEmailService;
    private readonly IReportExecutionLogRepository _reportExecutionLogRepository;
    private readonly ILogger<ReportController> _logger;

    public ReportController(
        ICdrRecordsService cdrRecordsService, 
        IOperatorService operatorService, 
        INotification<EmailMessage> emailNotification,
        ICdrReportService cdrReportService,
        ICdrReportEmailService cdrReportEmailService,
        IReportExecutionLogRepository reportExecutionLogRepository,
        ILogger<ReportController> logger)
    {
        _cdrRecordsService = cdrRecordsService;
        _operatorService = operatorService;
        _emailNotification = emailNotification;
        _cdrReportService = cdrReportService;
        _cdrReportEmailService = cdrReportEmailService;
        _reportExecutionLogRepository = reportExecutionLogRepository;
        _logger = logger;
    }

    [HttpPost("send-test-email")]
    public async Task<IActionResult> SendTestEmail([FromQuery] string? recipient = null)
    {
        // Eğer recipient belirtilmemişse, kullanıcının kendi emailini kullan
        var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
        var targetEmail = recipient ?? userEmail;

        if (string.IsNullOrEmpty(targetEmail))
        {
            return BadRequest("Email address is required");
        }

        var emailMessage = new EmailMessage
        {
            To = new List<string> { targetEmail },
            Body = "This is a test email from CDR Reporting System.",
            Subject = "CDR Test Email"
        };

        try
        {
            await _emailNotification.SendAsync(emailMessage);
            return Ok(new { message = "Test email sent successfully", recipient = targetEmail });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send test email to {Recipient}", targetEmail);
            return StatusCode(500, new { error = "Failed to send email", details = ex.Message });
        }
    }

    [HttpGet("by-date-range")]
    public async Task<IActionResult> GetByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        // if (startDate is default or null || endDate is default or null)
        // {
        //     return BadRequest("Start date and end date must be provided.");
        // }

        if (startDate > endDate)
        {
            return BadRequest("Start date must be earlier than end date.");
        }

        var records = await _cdrRecordsService.GetByDateRangeAsync(startDate, endDate);
        return Ok(records);
    }

    [HttpGet("answered-calls/{period}")]
    public async Task<IActionResult> GetAnsweredCalls(int period)
    {
        if (!Enum.IsDefined(typeof(ReportPeriod), period))
        {
            return BadRequest("Invalid period value.");
        }

        var records = await _cdrRecordsService.GetAnsweredCallsAsync((ReportPeriod)period);
        return Ok(records);
    }

    [HttpGet("daily-call-report")]
    public async Task<IActionResult> GetDailyCallReport()
    {
        var records = await _cdrRecordsService.GetDailyCallReportAsync();
        return Ok(records);
    }

    [HttpGet("call-records")]
    public async Task<IActionResult> GetCallReportList([FromQuery] CdrFilter filter)
    {
        var records = await _cdrRecordsService.GetCallReportListAsync(filter);
        return Ok(records);
    }

    [HttpGet("number-statistics")]
    public async Task<IActionResult> GetNumberStatisticsByNumber([FromQuery] string number, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        if (string.IsNullOrEmpty(number))
        {
            return BadRequest("Number must be provided.");
        }

        if (startDate > endDate)
        {
            return BadRequest("Start date must be earlier than end date.");
        }

        var statistics = await _cdrRecordsService.GetNumberStatisticsByNumberAsync(number, startDate, endDate);
        return Ok(statistics);
    }

    [HttpGet("user-last-calls")]
    public async Task<IActionResult> GetUserLastCalls([FromQuery] UserStatisticsFilter filter)
    {
        if (string.IsNullOrEmpty(filter.Number))
        {
            return BadRequest("Number must be provided.");
        }

        if (filter.StartDate > filter.EndDate)
        {
            return BadRequest("Start date must be earlier than end date.");
        }

        var records = await _cdrRecordsService.GetUserLastCallsAsync(filter);
        return Ok(records);
    }

    [HttpGet("department-statistics")]
    public async Task<IActionResult> GetDepartmentStatistics([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        if (startDate > endDate)
        {
            return BadRequest("Start date must be earlier than end date.");
        }

        var statistics = await _cdrRecordsService.GetDepartmentCallStatisticsAsync(startDate, endDate);
        return Ok(statistics);
    }

    [HttpGet("user-info/{number}")]
    public async Task<IActionResult> GetUserInfo(string number)
    {
        if (string.IsNullOrEmpty(number))
        {
            return BadRequest("Number must be provided.");
        }

        var userInfo = await _operatorService.GetUserInfoAsync(number);
        return Ok(userInfo);
    }

    [HttpGet("department-statistics/excel")]
    public async Task<IActionResult> GetDepartmentStatisticsExcel([FromQuery] DateRangeFilter filter)
    {
        var statistics = await _cdrRecordsService.GetDepartmentCallStatisticsAsync(filter.StartDate, filter.EndDate);
        var excelBytes = statistics.ToExcelFile();

        var filePath = Path.Combine("Files", $"departman-istatistikleri-{filter.StartDate:yyyy-MM-dd}-{filter.EndDate:yyyy-MM-dd}.xlsx");
        await System.IO.File.WriteAllBytesAsync(filePath, excelBytes);

        return File(
            excelBytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"departman-istatistikleri-{filter.StartDate:yyyy-MM-dd}-{filter.EndDate:yyyy-MM-dd}.xlsx"
        );
    }

    [HttpGet("user-specific-report")]
    public async Task<IActionResult> GetUserCalls([FromQuery] StatisticsFilter filter)
    {
        var result = await _cdrRecordsService.GetUserCalls(filter);
        return Ok(result);
    }

    [HttpGet("user-specific-report/export")]
    public async Task<IActionResult> UserSpecificReportExport([FromQuery] StatisticsFilter filter)
    {
        var result = await _cdrRecordsService.GetUserCalls(filter);
        return Ok(result.ToExcelFile());
    }

    [HttpGet("work-hours-statistics")]
    public async Task<IActionResult> GetWorkHoursStatistics([FromQuery] StatisticsFilter filter)
    {
        var result = await _cdrRecordsService.GetWorkHoursStatistics(filter);
        return Ok(result);
    }

    [HttpGet("non-work-hours-statistics")]
    public async Task<IActionResult> GetNonWorkHoursStatistics([FromQuery] StatisticsFilter filter)
    {
        var result = await _cdrRecordsService.GetNonWorkHoursStatistics(filter);
        return Ok(result);
    }

    [HttpGet("break-times")]
    public async Task<IActionResult> GetBreakTimes([FromQuery] StatisticsFilter filter)
    {
        if (filter == null)
        {
            return BadRequest("Filter must be provided.");
        }

        if (string.IsNullOrEmpty(filter.Number))
        {
            return BadRequest("Number must be provided.");
        }

        if (filter.StartDate > filter.EndDate)
        {
            return BadRequest("Start date must be earlier than end date.");
        }

        var result = await _operatorService.GetUserBreakTimesAsync(filter.Number, filter.StartDate, filter.EndDate);
        return Ok(result);
    }

    #region CDR Email Report Endpoints (FR-010, US3)

    /// <summary>
    /// Generate and optionally send a CDR email report (Admin only).
    /// </summary>
    /// <param name="request">Report generation parameters</param>
    /// <returns>Report generation result with execution ID</returns>
    /// <response code="200">Report generated successfully</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="403">User not authorized (requires Admin role)</response>
    /// <response code="408">Request timeout (generation exceeded 30 minutes)</response>
    /// <response code="500">Internal server error (SMTP failure, etc.)</response>
    [HttpPost("generate-email-report")]
    [ProducesResponseType(typeof(CdrEmailReportResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status408RequestTimeout)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GenerateEmailReport([FromBody] CdrReportRequest request)
    {
        // Input validation
        var validationResult = ValidateReportRequest(request);
        if (validationResult != null)
        {
            return validationResult;
        }

        try
        {
            // Log the request
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userIp = HttpContext.Connection.RemoteIpAddress?.ToString();
            _logger.LogInformation(
                "User {UserId} requested {ReportType} report generation from IP {IpAddress}",
                userId, request.ReportType, userIp);

            // Generate report
            CdrReportResult reportResult;
            if (request.StartDate.HasValue && request.EndDate.HasValue)
            {
                reportResult = await _cdrReportService.GenerateReportAsync(
                    request.StartDate.Value,
                    request.EndDate.Value,
                    request.ReportType);
            }
            else
            {
                reportResult = request.ReportType == ReportPeriod.Monthly
                    ? await _cdrReportService.GenerateMonthlyReportAsync()
                    : await _cdrReportService.GenerateWeeklyReportAsync();
            }

            if (!reportResult.IsSuccess)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiErrorResponse
                {
                    Code = "ReportGenerationFailed",
                    Message = "Failed to generate report",
                    Details = reportResult.ErrorMessage,
                    Timestamp = DateTime.UtcNow
                });
            }

            // Send emails if requested
            EmailDeliveryResult? deliveryResult = null;
            if (request.SendEmail)
            {
                var recipients = request.EmailRecipients?.Count > 0
                    ? request.EmailRecipients
                    : await _cdrReportEmailService.GetAdminRecipientsAsync();

                deliveryResult = await _cdrReportEmailService.SendReportEmailAsync(
                    reportResult, 
                    recipients, 
                    "OnDemand");
            }

            // Build response
            var response = new CdrEmailReportResponse
            {
                ReportId = reportResult.ExecutionId,
                ReportType = reportResult.ReportType,
                GeneratedAt = reportResult.GeneratedAt,
                PeriodStartDate = reportResult.PeriodStartDate,
                PeriodEndDate = reportResult.PeriodEndDate,
                FileName = reportResult.FileName,
                FileSizeBytes = reportResult.FileSizeBytes,
                RecordsProcessed = reportResult.RecordsProcessed,
                GenerationDurationMs = reportResult.GenerationDurationMs,
                MetricsSummary = reportResult.MetricsSummary,
                EmailsSent = request.SendEmail,
                RecipientsCount = deliveryResult?.TotalRecipients ?? 0,
                SuccessfulDeliveries = deliveryResult?.SuccessfulDeliveries ?? 0,
                FailedDeliveries = deliveryResult?.FailedDeliveries ?? 0,
                DeliveryStatus = deliveryResult?.RecipientStatuses ?? new List<EmailDeliveryStatusResponse>()
            };

            return Ok(response);
        }
        catch (TimeoutException ex)
        {
            _logger.LogError(ex, "Report generation timed out");
            return StatusCode(StatusCodes.Status408RequestTimeout, new ApiErrorResponse
            {
                Code = "RequestTimeout",
                Message = "Report generation exceeded maximum allowed time",
                Timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during report generation");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiErrorResponse
            {
                Code = "InternalServerError",
                Message = "An unexpected error occurred",
                Details = ex.Message,
                Timestamp = DateTime.UtcNow
            });
        }
    }

    /// <summary>
    /// Send a previously generated report via email.
    /// </summary>
    /// <param name="request">Email sending parameters with report ID and recipients</param>
    /// <returns>Email delivery status for each recipient</returns>
    [HttpPost("send-email-report")]
    [ProducesResponseType(typeof(EmailDeliveryAggregateResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SendEmailReport([FromBody] SendEmailReportRequest request)
    {
        // Validate email addresses
        var invalidEmails = request.EmailRecipients
            .Where(e => !CdrReportHelper.IsValidEmail(e))
            .ToList();

        if (invalidEmails.Any())
        {
            return BadRequest(new ApiErrorResponse
            {
                Code = "InvalidEmailAddresses",
                Message = "One or more email addresses are invalid",
                Details = string.Join(", ", invalidEmails),
                Timestamp = DateTime.UtcNow
            });
        }

        try
        {
            // Get the report execution
            var executionLog = await _reportExecutionLogRepository.GetByIdAsync(request.ReportExecutionId);
            if (executionLog == null)
            {
                return NotFound(new ApiErrorResponse
                {
                    Code = "ReportNotFound",
                    Message = $"Report execution with ID {request.ReportExecutionId} not found",
                    Timestamp = DateTime.UtcNow
                });
            }

            // Enqueue email sending job via Hangfire for async processing
            var jobId = BackgroundJob.Enqueue<ICdrReportJobService>(x =>
                x.SendOnDemandReportAsync(
                    executionLog.PeriodStartDate,
                    executionLog.PeriodEndDate,
                    executionLog.ReportType,
                    request.EmailRecipients,
                    executionLog.Id));

            var response = new EmailDeliveryAggregateResponse
            {
                ReportExecutionId = request.ReportExecutionId,
                TotalRecipients = request.EmailRecipients.Count,
                OverallStatus = "Queued"
            };

            _logger.LogInformation(
                "Email sending job {JobId} queued for report {ReportId} to {Count} recipients",
                jobId, request.ReportExecutionId, request.EmailRecipients.Count);

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error queuing email sending job");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiErrorResponse
            {
                Code = "InternalServerError",
                Message = "Failed to queue email sending job",
                Details = ex.Message,
                Timestamp = DateTime.UtcNow
            });
        }
    }

    /// <summary>
    /// Get report execution history.
    /// </summary>
    /// <param name="count">Number of recent executions to retrieve (default: 20)</param>
    /// <returns>List of recent report executions</returns>
    [HttpGet("execution-history")]
    [ProducesResponseType(typeof(List<ReportExecutionSummary>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetExecutionHistory([FromQuery] int count = 20)
    {
        try
        {
            var executions = await _reportExecutionLogRepository.GetRecentAsync(count);
            
            var summaries = executions.Select(e => new ReportExecutionSummary
            {
                ExecutionId = e.Id,
                ReportType = e.ReportType,
                TriggerType = e.TriggerType,
                PeriodStartDate = e.PeriodStartDate,
                PeriodEndDate = e.PeriodEndDate,
                ExecutionStatus = e.ExecutionStatus,
                StartedAt = e.StartedAt,
                CompletedAt = e.CompletedAt,
                RecordsProcessed = e.RecordsProcessed ?? 0,
                RecipientsCount = e.RecipientsCount ?? 0,
                SuccessfulDeliveries = e.SuccessfulDeliveries ?? 0,
                FailedDeliveries = e.FailedDeliveries ?? 0
            }).ToList();

            return Ok(summaries);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving execution history");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiErrorResponse
            {
                Code = "InternalServerError",
                Message = "Failed to retrieve execution history",
                Timestamp = DateTime.UtcNow
            });
        }
    }

    /// <summary>
    /// Download a generated report file.
    /// </summary>
    /// <param name="executionId">Report execution ID</param>
    /// <returns>Excel file download</returns>
    [HttpGet("download/{executionId}")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DownloadReport(Guid executionId)
    {
        try
        {
            var executionLog = await _reportExecutionLogRepository.GetByIdAsync(executionId);
            if (executionLog == null || string.IsNullOrEmpty(executionLog.GeneratedFileName))
            {
                return NotFound(new ApiErrorResponse
                {
                    Code = "ReportNotFound",
                    Message = "Report not found or file not available",
                    Timestamp = DateTime.UtcNow
                });
            }

            // Regenerate the report for download
            var reportResult = await _cdrReportService.GenerateReportAsync(
                executionLog.PeriodStartDate,
                executionLog.PeriodEndDate,
                executionLog.ReportType.Equals("Monthly", StringComparison.OrdinalIgnoreCase) 
                    ? ReportPeriod.Monthly 
                    : ReportPeriod.Weekly);

            if (!reportResult.IsSuccess)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiErrorResponse
                {
                    Code = "ReportGenerationFailed",
                    Message = "Failed to regenerate report for download",
                    Timestamp = DateTime.UtcNow
                });
            }

            return File(
                reportResult.ExcelData,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                reportResult.FileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading report {ExecutionId}", executionId);
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiErrorResponse
            {
                Code = "InternalServerError",
                Message = "Failed to download report",
                Timestamp = DateTime.UtcNow
            });
        }
    }

    /// <summary>
    /// Validate report generation request.
    /// </summary>
    private IActionResult? ValidateReportRequest(CdrReportRequest request)
    {
        if (request == null)
        {
            return BadRequest(new ApiErrorResponse
            {
                Code = "ValidationError",
                Message = "Request body is required",
                Timestamp = DateTime.UtcNow
            });
        }

        // Validate date range if provided
        if (request.StartDate.HasValue && request.EndDate.HasValue)
        {
            if (request.StartDate > request.EndDate)
            {
                return BadRequest(new ApiErrorResponse
                {
                    Code = "ValidationError",
                    Message = "Start date must be before end date",
                    Timestamp = DateTime.UtcNow
                });
            }

            // Max 366 days
            if ((request.EndDate.Value - request.StartDate.Value).TotalDays > 366)
            {
                return BadRequest(new ApiErrorResponse
                {
                    Code = "ValidationError",
                    Message = "Date range cannot exceed 366 days",
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        // Validate email addresses if provided
        if (request.EmailRecipients?.Count > 0)
        {
            var invalidEmails = request.EmailRecipients
                .Where(e => !CdrReportHelper.IsValidEmail(e))
                .ToList();

            if (invalidEmails.Any())
            {
                return BadRequest(new ApiErrorResponse
                {
                    Code = "ValidationError",
                    Message = "One or more email addresses are invalid",
                    Details = string.Join(", ", invalidEmails),
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        return null;
    }

    #endregion
}