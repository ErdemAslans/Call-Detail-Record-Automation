using Cdr.Api.Helpers;
using Cdr.Api.Interfaces;
using Cdr.Api.Models;
using Cdr.Api.Models.Entities;
using Cdr.Api.Models.Response.UserStatistics;
using Cdr.Api.Extensions;
using Cdr.Api.Services;
using Hangfire;
using Microsoft.Extensions.Options;
using Cdr.Api.Services.Interfaces;

namespace Cdr.Api.Services
{
    public static class HangfireJobs
    {
        // Turkey Standard Time for all scheduled jobs
        private static readonly TimeZoneInfo TurkeyTimeZone = CdrReportHelper.TurkeyTimeZone;

        public static void ExecuteJobs()
        {
            // Daily CDR Email Report - Every day at 06:00 AM Turkey Time
            RecurringJob.AddOrUpdate<ICdrReportJobService>(
                "SendDailyCdrReport",
                "reports",
                x => x.SendDailyReportAsync(),
                "0 6 * * *", // Every day at 06:00
                new RecurringJobOptions
                {
                    TimeZone = TurkeyTimeZone,
                    MisfireHandling = MisfireHandlingMode.Relaxed
                });

            // Weekly CDR Email Report - Monday 02:00 AM Turkey Time
            RecurringJob.AddOrUpdate<ICdrReportJobService>(
                "SendWeeklyCdrReport",
                "reports",
                x => x.SendWeeklyReportAsync(),
                "0 2 * * 1", // Every Monday at 02:00
                new RecurringJobOptions
                {
                    TimeZone = TurkeyTimeZone,
                    MisfireHandling = MisfireHandlingMode.Relaxed
                });

            // Monthly CDR Email Report - 1st of each month 02:00 AM Turkey Time
            RecurringJob.AddOrUpdate<ICdrReportJobService>(
                "SendMonthlyCdrReport",
                "reports",
                x => x.SendMonthlyReportAsync(),
                "0 2 1 * *", // 1st of each month at 02:00
                new RecurringJobOptions
                {
                    TimeZone = TurkeyTimeZone,
                    MisfireHandling = MisfireHandlingMode.Relaxed
                });

            // Daily Operator Report for 80369090 - Every day at 06:00 AM Turkey Time
            RecurringJob.AddOrUpdate<ICdrReportJobService>(
                "SendDailyOperatorReport",
                "reports",
                x => x.SendDailyOperatorReportAsync("80369090"),
                "0 6 * * *", // Every day at 06:00
                new RecurringJobOptions
                {
                    TimeZone = TurkeyTimeZone,
                    MisfireHandling = MisfireHandlingMode.Relaxed
                });

            // Weekly Operator Report for 80369090 - Monday 02:00 AM Turkey Time
            RecurringJob.AddOrUpdate<ICdrReportJobService>(
                "SendWeeklyOperatorReport",
                "reports",
                x => x.SendWeeklyOperatorReportAsync("80369090"),
                "0 2 * * 1", // Every Monday at 02:00
                new RecurringJobOptions
                {
                    TimeZone = TurkeyTimeZone,
                    MisfireHandling = MisfireHandlingMode.Relaxed
                });

            // Monthly Operator Report for 80369090 - 1st of each month 02:00 AM Turkey Time
            RecurringJob.AddOrUpdate<ICdrReportJobService>(
                "SendMonthlyOperatorReport",
                "reports",
                x => x.SendMonthlyOperatorReportAsync("80369090"),
                "0 2 1 * *", // 1st of each month at 02:00
                new RecurringJobOptions
                {
                    TimeZone = TurkeyTimeZone,
                    MisfireHandling = MisfireHandlingMode.Relaxed
                });
        }
    }

    /// <summary>
    /// Service interface for CDR report Hangfire jobs.
    /// </summary>
    public interface ICdrReportJobService
    {
        /// <summary>
        /// Generate and send daily CDR report.
        /// </summary>
        [AutomaticRetry(Attempts = 3, DelaysInSeconds = new[] { 300, 600, 1200 })]
        [JobDisplayName("Daily CDR Email Report")]
        Task SendDailyReportAsync();

        /// <summary>
        /// Generate and send weekly CDR report.
        /// </summary>
        [AutomaticRetry(Attempts = 3, DelaysInSeconds = new[] { 300, 600, 1200 })]
        [JobDisplayName("Weekly CDR Email Report")]
        Task SendWeeklyReportAsync();

        /// <summary>
        /// Generate and send monthly CDR report.
        /// </summary>
        [AutomaticRetry(Attempts = 3, DelaysInSeconds = new[] { 300, 600, 1200 })]
        [JobDisplayName("Monthly CDR Email Report")]
        Task SendMonthlyReportAsync();

        /// <summary>
        /// Generate and send on-demand CDR report.
        /// </summary>
        [AutomaticRetry(Attempts = 1)]
        [JobDisplayName("On-Demand CDR Email Report")]
        Task SendOnDemandReportAsync(DateTime startDate, DateTime endDate, string reportType, List<string> recipients, Guid originalExecutionId);

        /// <summary>
        /// Generate and send daily operator report.
        /// </summary>
        [AutomaticRetry(Attempts = 3, DelaysInSeconds = new[] { 300, 600, 1200 })]
        [JobDisplayName("Daily Operator Report")]
        Task SendDailyOperatorReportAsync(string operatorNumber = "80369090");

        /// <summary>
        /// Generate and send weekly operator report.
        /// </summary>
        [AutomaticRetry(Attempts = 3, DelaysInSeconds = new[] { 300, 600, 1200 })]
        [JobDisplayName("Weekly Operator Report")]
        Task SendWeeklyOperatorReportAsync(string operatorNumber = "80369090");

        /// <summary>
        /// Generate and send monthly operator report.
        /// </summary>
        [AutomaticRetry(Attempts = 3, DelaysInSeconds = new[] { 300, 600, 1200 })]
        [JobDisplayName("Monthly Operator Report")]
        Task SendMonthlyOperatorReportAsync(string operatorNumber = "80369090");
    }

    /// <summary>
    /// Implementation of CDR report Hangfire jobs with timeout and error handling.
    /// </summary>
    public class CdrReportJobService : ICdrReportJobService
    {
        private readonly ICdrReportService _reportService;
        private readonly ICdrRecordsService _cdrRecordsService;
        private readonly ICdrReportEmailService _emailService;
        private readonly ILogger<CdrReportJobService> _logger;
        
        // Maximum execution time: 30 minutes (from spec NFR-001)
        private const int MaxExecutionMinutes = 30;

        public CdrReportJobService(
            ICdrReportService reportService,
            ICdrRecordsService cdrRecordsService,
            ICdrReportEmailService emailService,
            ILogger<CdrReportJobService> logger)
        {
            _reportService = reportService;
            _cdrRecordsService = cdrRecordsService;
            _emailService = emailService;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task SendDailyReportAsync()
        {
            _logger.LogInformation("Starting daily CDR report job");
            
            using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(MaxExecutionMinutes));
            
            try
            {
                // Generate report
                var report = await _reportService.GenerateDailyReportAsync();
                
                if (!report.IsSuccess)
                {
                    _logger.LogError("Daily report generation failed: {Error}", report.ErrorMessage);
                    throw new Exception($"Report generation failed: {report.ErrorMessage}");
                }

                // Send emails
                var deliveryResult = await _emailService.SendReportEmailAsync(report, "Scheduled");
                
                if (deliveryResult.FailedDeliveries > 0)
                {
                    _logger.LogWarning(
                        "Daily report sent with {Failed} failed deliveries out of {Total}",
                        deliveryResult.FailedDeliveries, deliveryResult.TotalRecipients);
                }

                _logger.LogInformation(
                    "Daily CDR report job completed. Emails sent: {Success}/{Total}",
                    deliveryResult.SuccessfulDeliveries, deliveryResult.TotalRecipients);
            }
            catch (OperationCanceledException)
            {
                _logger.LogError("Daily report job timed out after {Minutes} minutes", MaxExecutionMinutes);
                throw new TimeoutException($"Job exceeded maximum execution time of {MaxExecutionMinutes} minutes");
            }
        }

        /// <inheritdoc />
        public async Task SendWeeklyReportAsync()
        {
            _logger.LogInformation("Starting weekly CDR report job");
            
            using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(MaxExecutionMinutes));
            
            try
            {
                // Generate report
                var report = await _reportService.GenerateWeeklyReportAsync();
                
                if (!report.IsSuccess)
                {
                    _logger.LogError("Weekly report generation failed: {Error}", report.ErrorMessage);
                    throw new Exception($"Report generation failed: {report.ErrorMessage}");
                }

                // Send emails
                var deliveryResult = await _emailService.SendReportEmailAsync(report, "Scheduled");
                
                if (deliveryResult.FailedDeliveries > 0)
                {
                    _logger.LogWarning(
                        "Weekly report sent with {Failed} failed deliveries out of {Total}",
                        deliveryResult.FailedDeliveries, deliveryResult.TotalRecipients);
                }

                _logger.LogInformation(
                    "Weekly CDR report job completed. Emails sent: {Success}/{Total}",
                    deliveryResult.SuccessfulDeliveries, deliveryResult.TotalRecipients);
            }
            catch (OperationCanceledException)
            {
                _logger.LogError("Weekly report job timed out after {Minutes} minutes", MaxExecutionMinutes);
                throw new TimeoutException($"Job exceeded maximum execution time of {MaxExecutionMinutes} minutes");
            }
        }

        /// <inheritdoc />
        public async Task SendMonthlyReportAsync()
        {
            _logger.LogInformation("Starting monthly CDR report job");
            
            using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(MaxExecutionMinutes));
            
            try
            {
                // Generate report
                var report = await _reportService.GenerateMonthlyReportAsync();
                
                if (!report.IsSuccess)
                {
                    _logger.LogError("Monthly report generation failed: {Error}", report.ErrorMessage);
                    throw new Exception($"Report generation failed: {report.ErrorMessage}");
                }

                // Send emails
                var deliveryResult = await _emailService.SendReportEmailAsync(report, "Scheduled");
                
                if (deliveryResult.FailedDeliveries > 0)
                {
                    _logger.LogWarning(
                        "Monthly report sent with {Failed} failed deliveries out of {Total}",
                        deliveryResult.FailedDeliveries, deliveryResult.TotalRecipients);
                }

                _logger.LogInformation(
                    "Monthly CDR report job completed. Emails sent: {Success}/{Total}",
                    deliveryResult.SuccessfulDeliveries, deliveryResult.TotalRecipients);
            }
            catch (OperationCanceledException)
            {
                _logger.LogError("Monthly report job timed out after {Minutes} minutes", MaxExecutionMinutes);
                throw new TimeoutException($"Job exceeded maximum execution time of {MaxExecutionMinutes} minutes");
            }
        }

        /// <inheritdoc />
        public async Task SendOnDemandReportAsync(
            DateTime startDate,
            DateTime endDate,
            string reportType,
            List<string> recipients,
            Guid originalExecutionId)
        {
            _logger.LogInformation(
                "Starting on-demand {ReportType} report job for period {Start} - {End}, originalExecutionId: {OriginalId}",
                reportType, startDate, endDate, originalExecutionId);

            using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(MaxExecutionMinutes));

            try
            {
                // Parse report type
                var period = reportType.Equals("Monthly", StringComparison.OrdinalIgnoreCase)
                    ? Common.Enums.ReportPeriod.Monthly
                    : Common.Enums.ReportPeriod.Weekly;

                // Generate report
                var report = await _reportService.GenerateReportAsync(startDate, endDate, period);

                if (!report.IsSuccess)
                {
                    _logger.LogError("On-demand report generation failed: {Error}", report.ErrorMessage);
                    throw new Exception($"Report generation failed: {report.ErrorMessage}");
                }

                // Override the execution ID so email delivery stats are written to the ORIGINAL log
                report.ExecutionId = originalExecutionId;

                // Send to specified recipients
                var deliveryResult = await _emailService.SendReportEmailAsync(report, recipients, "OnDemand");

                _logger.LogInformation(
                    "On-demand CDR report job completed. Emails sent: {Success}/{Total}, originalExecutionId: {OriginalId}",
                    deliveryResult.SuccessfulDeliveries, deliveryResult.TotalRecipients, originalExecutionId);
            }
            catch (OperationCanceledException)
            {
                _logger.LogError("On-demand report job timed out after {Minutes} minutes", MaxExecutionMinutes);
                throw new TimeoutException($"Job exceeded maximum execution time of {MaxExecutionMinutes} minutes");
            }
        }

        /// <inheritdoc />
        public async Task SendDailyOperatorReportAsync(string operatorNumber = "80369090")
        {
            _logger.LogInformation("Starting daily operator report job for {OperatorNumber}", operatorNumber);
            
            using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(MaxExecutionMinutes));
            
            try
            {
                // Generate report
                var report = await _reportService.GenerateDailyOperatorReportAsync(operatorNumber);
                
                if (!report.IsSuccess)
                {
                    _logger.LogError("Daily operator report generation failed: {Error}", report.ErrorMessage);
                    throw new Exception($"Report generation failed: {report.ErrorMessage}");
                }

                // Send emails
                var deliveryResult = await _emailService.SendReportEmailAsync(report, "Scheduled");
                
                if (deliveryResult.FailedDeliveries > 0)
                {
                    _logger.LogWarning(
                        "Daily operator report sent with {Failed} failed deliveries out of {Total}",
                        deliveryResult.FailedDeliveries, deliveryResult.TotalRecipients);
                }

                _logger.LogInformation(
                    "Daily operator report job completed. Emails sent: {Success}/{Total}",
                    deliveryResult.SuccessfulDeliveries, deliveryResult.TotalRecipients);
            }
            catch (OperationCanceledException)
            {
                _logger.LogError("Daily operator report job timed out after {Minutes} minutes", MaxExecutionMinutes);
                throw new TimeoutException($"Job exceeded maximum execution time of {MaxExecutionMinutes} minutes");
            }
        }

        /// <inheritdoc />
        public async Task SendWeeklyOperatorReportAsync(string operatorNumber = "80369090")
        {
            _logger.LogInformation("Starting weekly operator report job for {OperatorNumber}", operatorNumber);
            
            using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(MaxExecutionMinutes));
            
            try
            {
                // Generate report
                var report = await _reportService.GenerateWeeklyOperatorReportAsync(operatorNumber);
                
                if (!report.IsSuccess)
                {
                    _logger.LogError("Weekly operator report generation failed: {Error}", report.ErrorMessage);
                    throw new Exception($"Report generation failed: {report.ErrorMessage}");
                }

                // Send emails
                var deliveryResult = await _emailService.SendReportEmailAsync(report, "Scheduled");
                
                if (deliveryResult.FailedDeliveries > 0)
                {
                    _logger.LogWarning(
                        "Weekly operator report sent with {Failed} failed deliveries out of {Total}",
                        deliveryResult.FailedDeliveries, deliveryResult.TotalRecipients);
                }

                _logger.LogInformation(
                    "Weekly operator report job completed. Emails sent: {Success}/{Total}",
                    deliveryResult.SuccessfulDeliveries, deliveryResult.TotalRecipients);
            }
            catch (OperationCanceledException)
            {
                _logger.LogError("Weekly operator report job timed out after {Minutes} minutes", MaxExecutionMinutes);
                throw new TimeoutException($"Job exceeded maximum execution time of {MaxExecutionMinutes} minutes");
            }
        }

        /// <inheritdoc />
        public async Task SendMonthlyOperatorReportAsync(string operatorNumber = "80369090")
        {
            _logger.LogInformation("Starting monthly operator report job for {OperatorNumber}", operatorNumber);
            
            using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(MaxExecutionMinutes));
            
            try
            {
                // Generate report
                var report = await _reportService.GenerateMonthlyOperatorReportAsync(operatorNumber);
                
                if (!report.IsSuccess)
                {
                    _logger.LogError("Monthly operator report generation failed: {Error}", report.ErrorMessage);
                    throw new Exception($"Report generation failed: {report.ErrorMessage}");
                }

                // Send emails
                var deliveryResult = await _emailService.SendReportEmailAsync(report, "Scheduled");
                
                if (deliveryResult.FailedDeliveries > 0)
                {
                    _logger.LogWarning(
                        "Monthly operator report sent with {Failed} failed deliveries out of {Total}",
                        deliveryResult.FailedDeliveries, deliveryResult.TotalRecipients);
                }

                _logger.LogInformation(
                    "Monthly operator report job completed. Emails sent: {Success}/{Total}",
                    deliveryResult.SuccessfulDeliveries, deliveryResult.TotalRecipients);
            }
            catch (OperationCanceledException)
            {
                _logger.LogError("Monthly operator report job timed out after {Minutes} minutes", MaxExecutionMinutes);
                throw new TimeoutException($"Job exceeded maximum execution time of {MaxExecutionMinutes} minutes");
            }
        }
    }
}