# Email Reporting Service

## Overview

The Email Reporting Service provides automated and on-demand CDR (Call Detail Record) report generation and delivery via email. Reports are generated in Excel format and sent to admin users on configurable schedules.

## Architecture

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                              Hangfire Scheduler                              │
│  ┌─────────────────────┐  ┌─────────────────────┐                           │
│  │ SendWeeklyReportJob │  │ SendMonthlyReportJob│                           │
│  │  Mon 02:00 Turkey   │  │ 1st 02:00 Turkey    │                           │
│  └──────────┬──────────┘  └──────────┬──────────┘                           │
└─────────────┼────────────────────────┼──────────────────────────────────────┘
              │                        │
              ▼                        ▼
┌─────────────────────────────────────────────────────────────────────────────┐
│                          CdrReportJobService                                 │
│  ┌─────────────────────────────────────────────────────────────────────────┐│
│  │ ExecuteWeeklyReportJobAsync() / ExecuteMonthlyReportJobAsync()          ││
│  │ - 30-minute timeout                                                      ││
│  │ - Logs to ReportExecutionLog                                            ││
│  └────────────────────────────────┬────────────────────────────────────────┘│
└───────────────────────────────────┼─────────────────────────────────────────┘
                                    │
              ┌─────────────────────┼─────────────────────┐
              ▼                     ▼                     ▼
┌──────────────────────┐ ┌──────────────────────┐ ┌──────────────────────┐
│   CdrReportService   │ │CdrReportEmailService │ │ ReportController API │
│ ┌──────────────────┐ │ │ ┌──────────────────┐ │ │ ┌──────────────────┐ │
│ │GenerateWeekly()  │ │ │ │ComposeReportEmail│ │ │ │POST /generate    │ │
│ │GenerateMonthly() │ │ │ │SendReportEmail() │ │ │ │POST /send        │ │
│ │GenerateReport()  │ │ │ │GetAdminRecipients│ │ │ │GET /history      │ │
│ └────────┬─────────┘ │ │ │HandleRetry()     │ │ │ │GET /download     │ │
└──────────┼───────────┘ │ └────────┬─────────┘ │ └──────────┬─────────┘
           │             └──────────┼───────────┘            │
           ▼                        ▼                        │
┌──────────────────────┐ ┌──────────────────────┐            │
│  CdrRecordsRepository│ │   IEmailService      │            │
│ ┌──────────────────┐ │ │ ┌──────────────────┐ │            │
│ │GetDepartmentCall │ │ │ │SendAsync()       │ │            │
│ │StatisticsAsync() │ │ │ │(SmtpClient)      │ │            │
│ │ApplyGlobalFilter │ │ │ └──────────────────┘ │            │
│ └────────┬─────────┘ │ └──────────────────────┘            │
└──────────┼───────────┘                                     │
           ▼                                                 │
┌──────────────────────┐                                     │
│       MongoDB        │                                     │
│  incoming_calls      │◄────────────────────────────────────┘
└──────────────────────┘
```

## Components

### 1. CdrReportService (`Services/CdrReportService.cs`)

Responsible for generating CDR reports with metrics.

**Interface:**
```csharp
public interface ICdrReportService
{
    Task<CdrReportResult> GenerateWeeklyReportAsync(DateTime? referenceDate = null);
    Task<CdrReportResult> GenerateMonthlyReportAsync(DateTime? referenceDate = null);
    Task<CdrReportResult> GenerateReportAsync(DateTime startDate, DateTime endDate, ReportPeriod reportType);
}
```

**Key Features:**
- Generates weekly reports (Monday 00:00 - Sunday 23:59 Turkey time)
- Generates monthly reports (1st 00:00 - last day 23:59)
- Creates Excel files using existing `DepartmentCallStatisticsExtensions.ToExcelFile()`
- Logs all executions to `ReportExecutionLog` table
- Applies global filter via `ApplyGlobalFilter()` for all CDR queries

### 2. CdrReportEmailService (`Services/CdrReportEmailService.cs`)

Handles email composition and delivery with retry logic.

**Interface:**
```csharp
public interface ICdrReportEmailService
{
    Task<List<EmailDeliveryResult>> SendReportToAdminsAsync(byte[] excelData, string fileName, string reportType, DateTime periodStart, DateTime periodEnd);
    Task<List<EmailDeliveryResult>> SendReportAsync(byte[] excelData, string fileName, string reportType, DateTime periodStart, DateTime periodEnd, List<string> recipients);
    List<string> GetAdminRecipients();
}
```

**Key Features:**
- Discovers admin recipients from AspNetUsers (excludes switchboard staff)
- 3 retry attempts with 5-minute intervals
- 2-second delay between recipients (serial sending)
- Logs all delivery attempts to `EmailDeliveryAudit` table
- Plain text fallback for HTML email

### 3. CdrReportJobService (`Services/HangfireJobs.cs`)

Hangfire job implementations for scheduled execution.

**Interface:**
```csharp
public interface ICdrReportJobService
{
    Task ExecuteWeeklyReportJobAsync();
    Task ExecuteMonthlyReportJobAsync();
}
```

**Schedule Configuration:**
- Weekly: Monday 02:00 AM Turkey time (CRON: `0 2 * * 1`)
- Monthly: 1st of month 02:00 AM Turkey time (CRON: `0 2 1 * *`)
- Queue: "reports" (lower priority than real-time operations)
- Timeout: 30 minutes maximum

### 4. ReportController API (`Controllers/ReportController.cs`)

REST API endpoints for on-demand report generation.

**Endpoints:**

| Method | Path | Description | Auth |
|--------|------|-------------|------|
| POST | `/api/report/generate-email-report` | Generate a report | Admin |
| POST | `/api/report/send-email-report` | Send report via email | Admin |
| GET | `/api/report/execution-history` | Get past executions | Admin |
| GET | `/api/report/download/{executionId}` | Download report Excel | Admin |

## Data Model

### ReportExecutionLog

Tracks all report generation executions:

| Column | Type | Description |
|--------|------|-------------|
| Id | Guid | Primary key |
| ReportType | string | "Weekly" or "Monthly" |
| TriggerType | string | "Scheduled", "Manual", or "API" |
| PeriodStartDate | DateTime | Report period start |
| PeriodEndDate | DateTime | Report period end |
| ExecutionStatus | string | "Running", "Completed", "Failed" |
| StartedAt | DateTime | Job start time |
| CompletedAt | DateTime? | Job completion time |
| GenerationDurationMs | long? | Excel generation time |
| RecordsProcessed | int? | CDR records in report |
| FileSizeBytes | long? | Excel file size |
| TotalEmailRecipients | int | Recipients count |
| SuccessfulEmailDeliveries | int | Successful sends |
| FailedEmailDeliveries | int | Failed sends |
| ErrorMessage | string? | Error if failed |

### EmailDeliveryAudit

Tracks individual email delivery attempts:

| Column | Type | Description |
|--------|------|-------------|
| Id | Guid | Primary key |
| ExecutionLogId | Guid | FK to ReportExecutionLog |
| RecipientEmail | string | Target email address |
| DeliveryStatus | string | "Pending", "Sent", "Failed", "Retrying" |
| AttemptCount | int | Number of attempts |
| LastAttemptAt | DateTime? | Last send attempt |
| DeliveredAt | DateTime? | Successful delivery time |
| ErrorMessage | string? | SMTP error if failed |

### HolidayCalendar

Turkey public holidays for after-hours classification:

| Column | Type | Description |
|--------|------|-------------|
| Id | int | Primary key |
| HolidayDate | DateOnly | Holiday date |
| HolidayName | string | Holiday name |
| HolidayType | string | "Public", "Religious", "National" |
| IsRecurring | bool | Annual recurrence |
| Year | int | Year (for non-recurring) |
| IsActive | bool | Active flag |

## Configuration

### appsettings.json

```json
{
  "Email": {
    "SmtpHost": "smtp.example.com",
    "SmtpPort": 587,
    "SmtpUsername": "noreply@dogusoto.com",
    "SmtpPassword": "***",
    "SenderEmail": "noreply@dogusoto.com",
    "SenderName": "Doğuş Oto CDR System",
    "EnableSsl": true
  },
  "CdrReporting": {
    "WeeklyCron": "0 2 * * 1",
    "MonthlyCron": "0 2 1 * *",
    "TimeZone": "Turkey Standard Time",
    "MaxRetryAttempts": 3,
    "RetryDelayMinutes": 5,
    "JobTimeoutMinutes": 30,
    "MaxFileSizeMB": 25,
    "EmailDelaySeconds": 2,
    "DefaultRecipients": [
      "VeGuler@dogusotomotiv.com.tr",
      "MTurun@dogusotomotiv.com.tr",
      "MKanberoglu@dogusotomotiv.com.tr"
    ],
    "ExcludedRecipients": [
      "SSaroglu@dogusotomotiv.com.tr"
    ]
  }
}
```

## Error Handling

### Retry Strategy

1. **Email Delivery**: 3 attempts, 5-minute intervals
2. **Job Execution**: Hangfire automatic retry (configurable)
3. **Report Generation**: Single attempt, logged as "Failed" with error message

### Error Codes

| Code | HTTP | Description |
|------|------|-------------|
| ValidationError | 400 | Invalid request parameters |
| Unauthorized | 401 | Missing or invalid JWT |
| Forbidden | 403 | Non-admin user |
| NotFound | 404 | Report execution not found |
| Timeout | 408 | Generation exceeded timeout |
| InternalError | 500 | Server-side error |

## Monitoring

### Hangfire Dashboard

Access: `/hangfire` (Admin only)

Features:
- View scheduled jobs
- Monitor job execution history
- Manually trigger jobs
- Retry failed jobs

### Logging

All components use `ILogger` with structured logging:

```csharp
_logger.LogInformation(
    "Starting {ReportType} report generation for period {StartDate} - {EndDate}",
    reportType, startDate, endDate);
```

### Health Checks

Monitor via SQL queries:

```sql
-- Missed jobs in last 7 days
SELECT * FROM ReportExecutionLogs 
WHERE ExecutionStatus = 'Failed' 
  AND StartedAt > DATEADD(day, -7, GETUTCDATE());

-- Email delivery failures
SELECT * FROM EmailDeliveryAudits 
WHERE DeliveryStatus = 'Failed' 
  AND LastAttemptAt > DATEADD(day, -7, GETUTCDATE());
```

## Performance Considerations

- Weekly reports: <10 seconds generation time target
- Monthly reports: <15 seconds generation time target
- Email delivery: All emails sent within 5 minutes
- Memory: <512MB per report generation
- Concurrent requests: Maximum 2 queued, others rejected

## Security

- All endpoints require JWT authentication
- Admin role required for all report operations
- Switchboard staff (SSaroglu) excluded from recipients
- SMTP credentials stored in environment variables
- Audit logging for all API access
- Input validation for dates, email addresses

## Related Files

| File | Purpose |
|------|---------|
| `Services/CdrReportService.cs` | Report generation logic |
| `Services/CdrReportEmailService.cs` | Email delivery logic |
| `Services/HangfireJobs.cs` | Scheduled job implementations |
| `Controllers/ReportController.cs` | API endpoints |
| `Helpers/CdrReportHelper.cs` | Date boundary calculations |
| `Models/Entities/ReportExecutionLog.cs` | Execution tracking entity |
| `Models/Entities/EmailDeliveryAudit.cs` | Delivery audit entity |
| `Models/Entities/HolidayCalendar.cs` | Holiday configuration entity |
