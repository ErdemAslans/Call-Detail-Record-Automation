# Hangfire Background Jobs

**Last Updated**: January 2026  
**Focus**: Scheduled jobs, automated reporting  

---

## 🔄 Hangfire Overview

Hangfire automatically runs scheduled **reporting jobs** at specified times (daily, weekly, monthly).

```
┌─────────────────────────────────┐
│   Hangfire Server (ASP.NET)     │
├─────────────────────────────────┤
│ • Monitors SQL Server queue      │
│ • Executes scheduled jobs        │
│ • Handles retries & failures     │
│ • Web dashboard for monitoring   │
└─────────────────┬───────────────┘
                  │
              ┌───▼────────────────────┐
              │   SQL Server Queue     │
              │ (Job definitions)      │
              └───────────────────────┘
```

---

## ⚙️ Hangfire Setup (Program.cs)

```csharp
// Storage configuration
builder.Services.AddHangfire(configuration => configuration
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(
        builder.Configuration.GetConnectionString("DefaultConnection"), 
        new SqlServerStorageOptions
        {
            CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
            SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
            QueuePollInterval = TimeSpan.Zero,
            UseRecommendedIsolationLevel = true,
            DisableGlobalLocks = true
        }));

// Start Hangfire server
builder.Services.AddHangfireServer();

// At startup
HangfireJobs.ExecuteJobs();
```

### Configuration Options
| Setting | Value | Purpose |
|---------|-------|---------|
| **CommandBatchMaxTimeout** | 5 minutes | Max time for batch operations |
| **QueuePollInterval** | 0ms | Poll queue immediately (aggressive) |
| **DisableGlobalLocks** | true | Better performance, requires SQL 2012+ |

---

## 📅 Scheduled Jobs (HangfireJobs.cs)

```csharp
public static class HangfireJobs
{
    public static void ExecuteJobs()
    {
        // Daily job - runs at 2 AM
        RecurringJob.AddOrUpdate<ICdrRecordsService>(
            "daily-report",
            x => x.GenerateDailyReportAsync(),
            Cron.Daily(2));

        // Weekly job - runs every Monday at 3 AM
        RecurringJob.AddOrUpdate<ICdrRecordsService>(
            "weekly-report",
            x => x.GenerateWeeklyReportAsync(),
            Cron.Weekly(DayOfWeek.Monday, 3));

        // Monthly job - runs on 1st of month at 4 AM
        RecurringJob.AddOrUpdate<ICdrRecordsService>(
            "monthly-report",
            x => x.GenerateMonthlyReportAsync(),
            Cron.Monthly(1, 4));
    }
}
```

### Cron Expressions
```csharp
Cron.Hourly()                    // Every hour
Cron.Daily(2)                    // Every day at 2 AM
Cron.Weekly(DayOfWeek.Monday)    // Every Monday at midnight
Cron.Monthly(1)                  // 1st of month at midnight
Cron.Never()                     // Disabled
```

---

## 🎯 Job Implementation Pattern

```csharp
public class CdrRecordsService : ICdrRecordsService
{
    private readonly ICdrRecordsRepository _repository;
    private readonly ILogger<CdrRecordsService> _logger;

    public async Task GenerateDailyReportAsync()
    {
        try
        {
            _logger.LogInformation("Daily report generation started");
            
            var yesterday = DateTime.Now.AddDays(-1);
            var startDate = yesterday.Date;
            var endDate = startDate.AddDays(1);

            // Fetch data
            var records = await _repository.GetByDateRangeAsync(startDate, endDate);

            // Aggregate/Process
            var dailyStats = records
                .GroupBy(x => x.Operator.Id)
                .Select(g => new DailyOperatorStats
                {
                    OperatorId = g.Key,
                    TotalCalls = g.Count(),
                    AnsweredCalls = g.Count(x => x.DateTime.Connect.HasValue),
                    TotalDuration = g.Sum(x => x.Duration)
                })
                .ToList();

            // Save report
            await _repository.SaveReportAsync(dailyStats, ReportType.Daily);

            _logger.LogInformation("Daily report generated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Daily report generation failed");
            throw;  // Hangfire will retry
        }
    }
}
```

---

## 🎛️ Hangfire Dashboard

### Access
```
https://localhost:5001/hangfire
```

### What You Can See
- ✅ Recurring jobs schedule
- ✅ Job history (past 10 jobs)
- ✅ Failed jobs with error details
- ✅ Queue status
- ✅ Job statistics

### Security
⚠️ **Dashboard is currently unprotected!**

Secure with authorization:
```csharp
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { new HangfireAuthorizationFilter() }
});

public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();
        return httpContext.User.IsInRole("Admin");
    }
}
```

---

## 📊 Job Execution Flow

```
1. Hangfire polls SQL Server queue (every 0ms)
   ↓
2. Scheduled time reached for job
   ↓
3. Job marked as "Running"
   ↓
4. Service method invoked (ICdrRecordsService.GenerateDailyReportAsync())
   ↓
5. Repository queries MongoDB (with ApplyGlobalFilter())
   ↓
6. Data aggregated/processed
   ↓
7. Report saved to database
   ↓
8. Job marked as "Completed"
   ↓
9. Next scheduled time, repeat from step 2
```

---

## 🔄 Retry Policy

### Default Behavior
- Failed jobs are retried automatically
- Retry delay increases exponentially (1 min, 5 min, 10 min, ...)
- After max retries, job moved to "Failed" queue

### Custom Retry Policy
```csharp
RecurringJob.AddOrUpdate<IReportService>(
    "weekly-report",
    x => x.GenerateWeeklyReportAsync(),
    Cron.Weekly(DayOfWeek.Monday, 3),
    options: new RecurringJobOptions
    {
        TimeZone = TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time")
    });
```

---

## 🐛 Monitoring & Debugging

### Logging Integration
```csharp
// Hangfire logs to ILogger automatically
public class CdrRecordsService
{
    private readonly ILogger<CdrRecordsService> _logger;

    public async Task GenerateDailyReportAsync()
    {
        _logger.LogInformation("Job started at {Time}", DateTime.Now);
        try
        {
            // Job logic...
            _logger.LogInformation("Job completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Job failed");
            throw;  // Important: rethrow so Hangfire can retry
        }
    }
}
```

### Common Issues

| Problem | Cause | Solution |
|---------|-------|----------|
| Jobs not running | Server not started | Check `AddHangfireServer()` in Program.cs |
| Job keeps failing | Exception in code | Check Hangfire dashboard Failed jobs |
| Wrong schedule | Cron expression error | Verify with `crontab.guru` |
| Timezone issues | Default UTC | Set `TimeZoneInfo` in job options |

---

## ⚠️ Security & Production Considerations

### Don't Do This
```csharp
// ❌ Unprotected dashboard
app.UseHangfireDashboard();

// ❌ Sensitive logic without error handling
RecurringJob.AddOrUpdate<ServiceWithoutLogging>(...)

// ❌ Jobs that don't retry on failure
// (Use throw to trigger Hangfire retry)
```

### Do This
```csharp
// ✅ Protect dashboard
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { new AdminAuthorizationFilter() }
});

// ✅ Log everything
_logger.LogInformation("Job started");
_logger.LogError(ex, "Job failed");

// ✅ Let exceptions propagate for retries
try
{
    // Job logic
}
catch (Exception ex)
{
    _logger.LogError(ex, "Unrecoverable error");
    throw;  // Hangfire will retry
}
```

### Database Maintenance
- Monitor `HangFire_Job` table growth
- Purge old completed jobs periodically
- Consider archiving old reports

