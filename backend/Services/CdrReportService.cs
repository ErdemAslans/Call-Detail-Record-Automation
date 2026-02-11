using Cdr.Api.Helpers;
using Cdr.Api.Interfaces;
using Cdr.Api.Models;
using Cdr.Api.Models.Entities;
using Cdr.Api.Models.Response;
using Cdr.Api.Models.Response.Dashboard;
using Cdr.Api.Models.Response.UserStatistics;
using Cdr.Api.Extensions;
using Cdr.Api.Services.Interfaces;
using Common.Enums;
using System.Diagnostics;

namespace Cdr.Api.Services;

/// <summary>
/// Service interface for generating CDR email reports.
/// </summary>
public interface ICdrReportService
{
    /// <summary>
    /// Generate a daily CDR report for the previous complete day.
    /// </summary>
    Task<CdrReportResult> GenerateDailyReportAsync(DateTime? referenceDate = null);

    /// <summary>
    /// Generate a weekly CDR report for the previous complete week.
    /// </summary>
    Task<CdrReportResult> GenerateWeeklyReportAsync(DateTime? referenceDate = null);

    /// <summary>
    /// Generate a monthly CDR report for the previous complete month.
    /// </summary>
    Task<CdrReportResult> GenerateMonthlyReportAsync(DateTime? referenceDate = null);

    /// <summary>
    /// Generate a custom period CDR report.
    /// </summary>
    Task<CdrReportResult> GenerateReportAsync(DateTime startDate, DateTime endDate, ReportPeriod reportType);

    /// <summary>
    /// Generate a daily operator report for the previous complete day.
    /// </summary>
    Task<CdrReportResult> GenerateDailyOperatorReportAsync(string operatorNumber);

    /// <summary>
    /// Generate a weekly operator report for the previous complete week.
    /// </summary>
    Task<CdrReportResult> GenerateWeeklyOperatorReportAsync(string operatorNumber);

    /// <summary>
    /// Generate a monthly operator report for the previous complete month.
    /// </summary>
    Task<CdrReportResult> GenerateMonthlyOperatorReportAsync(string operatorNumber);
}

/// <summary>
/// Service for generating CDR email reports with Excel attachments.
/// </summary>
public class CdrReportService : ICdrReportService
{
    private readonly ICdrRecordsRepository _cdrRecordsRepository;
    private readonly IReportExecutionLogRepository _reportExecutionLogRepository;
    private readonly IHolidayCalendarRepository _holidayCalendarRepository;
    private readonly ICdrRecordsService _cdrRecordsService;
    private readonly IBreakRepository _breakRepository;
    private readonly IOperatorRepository _operatorRepository;
    private readonly ILogger<CdrReportService> _logger;

    public CdrReportService(
        ICdrRecordsRepository cdrRecordsRepository,
        IReportExecutionLogRepository reportExecutionLogRepository,
        IHolidayCalendarRepository holidayCalendarRepository,
        ICdrRecordsService cdrRecordsService,
        IBreakRepository breakRepository,
        IOperatorRepository operatorRepository,
        ILogger<CdrReportService> logger)
    {
        _cdrRecordsRepository = cdrRecordsRepository;
        _reportExecutionLogRepository = reportExecutionLogRepository;
        _holidayCalendarRepository = holidayCalendarRepository;
        _cdrRecordsService = cdrRecordsService;
        _breakRepository = breakRepository;
        _operatorRepository = operatorRepository;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<CdrReportResult> GenerateDailyReportAsync(DateTime? referenceDate = null)
    {
        var (startDate, endDate) = CdrReportHelper.GetDailyReportPeriod(referenceDate);
        return await GenerateReportAsync(startDate, endDate, ReportPeriod.Daily);
    }

    /// <inheritdoc />
    public async Task<CdrReportResult> GenerateWeeklyReportAsync(DateTime? referenceDate = null)
    {
        var (startDate, endDate) = CdrReportHelper.GetWeeklyReportPeriod(referenceDate);
        return await GenerateReportAsync(startDate, endDate, ReportPeriod.Weekly);
    }

    /// <inheritdoc />
    public async Task<CdrReportResult> GenerateMonthlyReportAsync(DateTime? referenceDate = null)
    {
        var (startDate, endDate) = CdrReportHelper.GetMonthlyReportPeriod(referenceDate);
        return await GenerateReportAsync(startDate, endDate, ReportPeriod.Monthly);
    }

    /// <inheritdoc />
    public async Task<CdrReportResult> GenerateReportAsync(DateTime startDate, DateTime endDate, ReportPeriod reportType)
    {
        var stopwatch = Stopwatch.StartNew();
        var executionId = Guid.NewGuid();
        var result = new CdrReportResult
        {
            ExecutionId = executionId,
            ReportType = reportType.ToString(),
            PeriodStartDate = startDate,
            PeriodEndDate = endDate,
            GeneratedAt = DateTime.UtcNow
        };

        // Create execution log entry
        var executionLog = new ReportExecutionLog
        {
            Id = executionId,
            ReportType = reportType.ToString(),
            TriggerType = "Scheduled",
            PeriodStartDate = startDate,
            PeriodEndDate = endDate,
            ExecutionStatus = "Running",
            StartedAt = DateTime.UtcNow
        };

        try
        {
            await _reportExecutionLogRepository.CreateAsync(executionLog);
            
            _logger.LogInformation(
                "Starting {ReportType} report generation for period {StartDate} - {EndDate}",
                reportType, startDate, endDate);

            // Get holidays for work hours classification
            var holidayDates = await GetHolidaysInPeriodAsync(startDate, endDate);

            // Get CDR statistics using existing repository method
            // This applies ApplyGlobalFilter() internally to filter "8036..." numbers
            var statistics = await _cdrRecordsRepository.GetDepartmentCallStatisticsAsync(startDate, endDate);

            // Calculate metrics summary
            result.MetricsSummary = CalculateMetricsSummary(statistics);
            result.RecordsProcessed = result.MetricsSummary.TotalIncomingCalls +
                                       result.MetricsSummary.TotalOutgoingCalls;

            // Calculate break summaries
            await PopulateBreakSummariesAsync(result.MetricsSummary, startDate, endDate);

            // Generate Excel file using existing extension method
            result.ExcelData = statistics.ToExcelFile(result.MetricsSummary.BreakSummaries);
            result.FileSizeBytes = result.ExcelData.Length;
            result.FileName = CdrReportHelper.GenerateReportFileName(
                reportType.ToString(), 
                startDate, 
                endDate);

            stopwatch.Stop();
            result.GenerationDurationMs = stopwatch.ElapsedMilliseconds;

            // Update execution log with success
            await _reportExecutionLogRepository.UpdateStatusAsync(
                executionId,
                "Completed",
                result.GenerationDurationMs,
                result.RecordsProcessed,
                result.FileSizeBytes,
                generatedFileName: result.FileName);

            _logger.LogInformation(
                "Successfully generated {ReportType} report. Records: {Records}, Size: {Size} bytes, Duration: {Duration}ms",
                reportType, result.RecordsProcessed, result.FileSizeBytes, result.GenerationDurationMs);

            return result;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            result.GenerationDurationMs = stopwatch.ElapsedMilliseconds;
            result.ErrorMessage = ex.Message;

            // Update execution log with failure
            await _reportExecutionLogRepository.UpdateStatusAsync(
                executionId,
                "Failed",
                stopwatch.ElapsedMilliseconds,
                errorMessage: ex.Message);

            _logger.LogError(ex,
                "Failed to generate {ReportType} report for period {StartDate} - {EndDate}",
                reportType, startDate, endDate);

            return result;
        }
    }

    /// <summary>
    /// Get holiday dates within a period for work hours classification.
    /// </summary>
    private async Task<List<DateOnly>> GetHolidaysInPeriodAsync(DateTime startDate, DateTime endDate)
    {
        try
        {
            var startDateOnly = DateOnly.FromDateTime(CdrReportHelper.ToTurkeyTime(startDate));
            var endDateOnly = DateOnly.FromDateTime(CdrReportHelper.ToTurkeyTime(endDate));
            return await _holidayCalendarRepository.GetHolidayDatesInRangeAsync(startDateOnly, endDateOnly);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to retrieve holidays. After-hours classification may be inaccurate.");
            return new List<DateOnly>();
        }
    }

    /// <summary>
    /// Calculate summary metrics from department statistics.
    /// </summary>
    private CdrReportMetricsSummary CalculateMetricsSummary(DepartmentCallStatisticsByCallDirection statistics)
    {
        var summary = new CdrReportMetricsSummary();

        // Incoming calls metrics
        if (statistics.Incoming != null)
        {
            foreach (var dept in statistics.Incoming)
            {
                summary.TotalIncomingCalls += dept.TotalCalls;
                summary.TotalAnsweredCalls += dept.AnsweredCalls;
                summary.TotalMissedCalls += dept.MissedCalls;
                summary.TotalOnBreakCalls += dept.OnBreakCalls;
            }
        }

        // Outgoing calls metrics
        if (statistics.Outgoing != null)
        {
            foreach (var dept in statistics.Outgoing)
            {
                summary.TotalOutgoingCalls += dept.TotalCalls;
            }
        }

        // Calculate answer rate
        if (summary.TotalIncomingCalls > 0)
        {
            summary.AnswerRate = Math.Round(
                (double)summary.TotalAnsweredCalls / summary.TotalIncomingCalls * 100, 
                2);
        }

        // Note: WorkHoursCalls and AfterHoursCalls would need additional CDR data
        // that includes timestamps - this is a placeholder for future enhancement
        summary.WorkHoursCalls = 0; // TODO: Calculate from CDR timestamps
        summary.AfterHoursCalls = 0; // TODO: Calculate from CDR timestamps

        return summary;
    }

    /// <summary>
    /// Populate break summaries per operator for the given period.
    /// </summary>
    private async Task PopulateBreakSummariesAsync(CdrReportMetricsSummary summary, DateTime startDate, DateTime endDate)
    {
        try
        {
            var (startUtc, endUtc) = TurkeyTimeProvider.ConvertDateRangeToUtc(startDate, endDate);
            var breaks = await _breakRepository.GetAllBreaksByDateRangeAsync(startUtc, endUtc);

            if (breaks.Count == 0) return;

            // Map userId -> (Name, PhoneNumber) using Operator collection
            var userIds = breaks.Select(b => b.UserId).Distinct().ToList();
            var allOperators = await _operatorRepository.GetAllAsync();
            var operatorMap = allOperators.ToDictionary(o => o.Id, o => o);

            // Group breaks by userId
            var grouped = breaks.GroupBy(b => b.UserId);
            foreach (var group in grouped)
            {
                var userId = group.Key;
                operatorMap.TryGetValue(userId, out var op);

                var operatorSummary = new OperatorBreakSummary
                {
                    OperatorName = op?.Name ?? "Bilinmeyen",
                    PhoneNumber = op?.PhoneNumber ?? "",
                    BreakCount = group.Count()
                };

                double totalMinutes = 0;
                foreach (var b in group.OrderBy(x => x.StartTime))
                {
                    var effectiveEnd = b.EndTime ?? b.PlannedEndTime;
                    var duration = (effectiveEnd - b.StartTime).TotalMinutes;
                    if (duration < 0) duration = 0;

                    totalMinutes += duration;
                    operatorSummary.Breaks.Add(new BreakDetail
                    {
                        StartTime = TurkeyTimeProvider.ConvertFromUtc(b.StartTime),
                        EndTime = b.EndTime.HasValue ? TurkeyTimeProvider.ConvertFromUtc(b.EndTime.Value) : null,
                        DurationMinutes = Math.Round(duration, 1),
                        Reason = b.Reason
                    });
                }

                operatorSummary.TotalDurationMinutes = Math.Round(totalMinutes, 1);
                summary.BreakSummaries.Add(operatorSummary);
            }

            // Sort by break count descending
            summary.BreakSummaries = summary.BreakSummaries.OrderByDescending(x => x.BreakCount).ToList();
            summary.TotalBreakCount = summary.BreakSummaries.Sum(x => x.BreakCount);
            summary.TotalBreakDurationMinutes = Math.Round(summary.BreakSummaries.Sum(x => x.TotalDurationMinutes), 1);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to populate break summaries for report. Continuing without break data.");
        }
    }

    /// <inheritdoc />
    public async Task<CdrReportResult> GenerateDailyOperatorReportAsync(string operatorNumber)
    {
        var (startDate, endDate) = CdrReportHelper.GetDailyReportPeriod();
        return await GenerateOperatorReportAsync(operatorNumber, startDate, endDate, "Daily");
    }

    /// <inheritdoc />
    public async Task<CdrReportResult> GenerateWeeklyOperatorReportAsync(string operatorNumber)
    {
        var (startDate, endDate) = CdrReportHelper.GetWeeklyReportPeriod();
        return await GenerateOperatorReportAsync(operatorNumber, startDate, endDate, "Weekly");
    }

    /// <inheritdoc />
    public async Task<CdrReportResult> GenerateMonthlyOperatorReportAsync(string operatorNumber)
    {
        var (startDate, endDate) = CdrReportHelper.GetMonthlyReportPeriod();
        return await GenerateOperatorReportAsync(operatorNumber, startDate, endDate, "Monthly");
    }

    /// <summary>
    /// Generate operator report for specified period.
    /// </summary>
    private async Task<CdrReportResult> GenerateOperatorReportAsync(
        string operatorNumber, 
        DateTime startDate, 
        DateTime endDate, 
        string reportType)
    {
        var stopwatch = Stopwatch.StartNew();
        var executionId = Guid.NewGuid();
        var result = new CdrReportResult
        {
            ExecutionId = executionId,
            ReportType = reportType,
            PeriodStartDate = startDate,
            PeriodEndDate = endDate,
            GeneratedAt = DateTime.UtcNow
        };

        // Create execution log entry
        var executionLog = new ReportExecutionLog
        {
            Id = executionId,
            ReportType = reportType,
            TriggerType = "Scheduled",
            PeriodStartDate = startDate,
            PeriodEndDate = endDate,
            ExecutionStatus = "Running",
            StartedAt = DateTime.UtcNow
        };

        try
        {
            await _reportExecutionLogRepository.CreateAsync(executionLog);

            _logger.LogInformation(
                "Starting {ReportType} operator report generation for {OperatorNumber}, period {StartDate} - {EndDate}",
                reportType, operatorNumber, startDate, endDate);

            // Generate operator report using GetUserCalls (generates 6-sheet Excel)
            var filter = new StatisticsFilter
            {
                Number = operatorNumber,
                StartDate = startDate,
                EndDate = endDate
            };

            var userReport = await _cdrRecordsService.GetUserCalls(filter);

            // Create report result from user calls
            result.ExcelData = userReport.ToExcelFile();
            result.FileSizeBytes = result.ExcelData.Length;
            result.RecordsProcessed = userReport.CallDetails.Count;
            result.FileName = CdrReportHelper.GenerateReportFileName(
                $"Operator-{operatorNumber}-{reportType}",
                startDate,
                endDate);

            stopwatch.Stop();
            result.GenerationDurationMs = stopwatch.ElapsedMilliseconds;

            // Update execution log with success
            await _reportExecutionLogRepository.UpdateStatusAsync(
                executionId,
                "Completed",
                result.GenerationDurationMs,
                result.RecordsProcessed,
                result.FileSizeBytes,
                generatedFileName: result.FileName);

            _logger.LogInformation(
                "Successfully generated {ReportType} operator report for {OperatorNumber}. Records: {Records}, Size: {Size} bytes, Duration: {Duration}ms",
                reportType, operatorNumber, result.RecordsProcessed, result.FileSizeBytes, result.GenerationDurationMs);

            return result;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            result.GenerationDurationMs = stopwatch.ElapsedMilliseconds;
            result.ErrorMessage = ex.Message;

            // Update execution log with failure
            await _reportExecutionLogRepository.UpdateStatusAsync(
                executionId,
                "Failed",
                stopwatch.ElapsedMilliseconds,
                errorMessage: ex.Message);

            _logger.LogError(ex,
                "Failed to generate {ReportType} operator report for {OperatorNumber}, period {StartDate} - {EndDate}",
                reportType, operatorNumber, startDate, endDate);

            return result;
        }
    }
}
