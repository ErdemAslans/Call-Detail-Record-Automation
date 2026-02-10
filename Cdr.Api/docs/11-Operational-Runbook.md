# Operational Runbook: CDR Email Reporting

**Version**: 1.0  
**Last Updated**: January 2026  
**Audience**: Operations Team, Support Engineers

---

## Table of Contents

1. [Common Operations](#common-operations)
2. [Manual Report Re-run](#manual-report-re-run)
3. [Email Delivery Troubleshooting](#email-delivery-troubleshooting)
4. [Handling Stuck Jobs](#handling-stuck-jobs)
5. [Monitoring Queries](#monitoring-queries)
6. [Backup & Recovery](#backup--recovery)
7. [Escalation Procedures](#escalation-procedures)

---

## Common Operations

### Check Job Status

1. Navigate to Hangfire Dashboard: `https://api.dogusoto.com/hangfire`
2. Go to **Recurring Jobs** tab
3. Verify both jobs are listed:
   - `weekly-cdr-report` - Next run: Monday 02:00
   - `monthly-cdr-report` - Next run: 1st of month 02:00

### View Recent Executions

1. Hangfire Dashboard → **Succeeded** or **Failed** tabs
2. Or run SQL query:

```sql
SELECT TOP 20 
    Id, ReportType, TriggerType, ExecutionStatus, 
    StartedAt, CompletedAt, ErrorMessage
FROM ReportExecutionLogs
ORDER BY StartedAt DESC;
```

### Check Email Delivery Status

```sql
SELECT TOP 50
    r.ReportType, r.StartedAt,
    e.RecipientEmail, e.DeliveryStatus, e.SentAt, e.ErrorMessage
FROM ReportExecutionLogs r
LEFT JOIN EmailDeliveryAudits e ON r.Id = e.ReportExecutionLogId
ORDER BY r.StartedAt DESC, e.SentAt DESC;
```

---

## Manual Report Re-run

### Scenario: Scheduled job failed and needs to be re-executed

#### Via Hangfire Dashboard (Recommended)

1. Navigate to `https://api.dogusoto.com/hangfire/recurring`
2. Find the failed job (`weekly-cdr-report` or `monthly-cdr-report`)
3. Click **Trigger now** button
4. Monitor progress in **Processing** tab
5. Verify completion in **Succeeded** tab

#### Via API (Admin Token Required)

```bash
# Re-run weekly report
curl -X POST "https://api.dogusoto.com/report/generate-email-report" \
  -H "Authorization: Bearer <admin-token>" \
  -H "Content-Type: application/json" \
  -d '{
    "reportType": "Weekly",
    "sendEmail": true
  }'
```

#### Via SQL (Emergency Only)

If application is down, mark job for re-processing:

```sql
-- Reset failed job for retry
UPDATE Hangfire.Job
SET StateName = 'Enqueued', StateReason = 'Manual retry'
WHERE Id = '<job-id>' AND StateName = 'Failed';
```

**Warning**: Direct database manipulation should only be done as last resort.

---

## Email Delivery Troubleshooting

### Symptom: Emails not received

#### Step 1: Check Delivery Status

```sql
SELECT RecipientEmail, DeliveryStatus, ErrorMessage, AttemptCount
FROM EmailDeliveryAudits
WHERE ReportExecutionLogId = '<execution-id>'
ORDER BY SentAt DESC;
```

#### Step 2: Review Error Messages

| Error | Cause | Resolution |
|-------|-------|------------|
| `SMTP connection failed` | Network/firewall issue | Check SMTP server connectivity |
| `Authentication failed` | Invalid credentials | Verify SMTP password in env vars |
| `Mailbox unavailable` | Recipient address invalid | Remove invalid email from list |
| `Message rejected` | SPF/DKIM failure | Check DNS records |
| `Connection timeout` | SMTP server overloaded | Retry in 15 minutes |

#### Step 3: SMTP Server Test

```bash
# Test SMTP connectivity
telnet smtp.dogusoto.com 587

# Or use OpenSSL
openssl s_client -starttls smtp -connect smtp.dogusoto.com:587
```

#### Step 4: Check Spam Folder

- Verify email not marked as spam
- Check email headers for SPF/DKIM/DMARC pass

#### Step 5: Manual Resend

```bash
curl -X POST "https://api.dogusoto.com/report/send-email-report" \
  -H "Authorization: Bearer <admin-token>" \
  -H "Content-Type: application/json" \
  -d '{
    "reportExecutionId": "<execution-id>",
    "emailRecipients": ["admin@dogusoto.com"]
  }'
```

---

## Handling Stuck Jobs

### Symptom: Job running for >30 minutes

#### Step 1: Identify Stuck Job

```sql
SELECT Id, ReportType, StartedAt, 
    DATEDIFF(MINUTE, StartedAt, GETDATE()) AS RunningMinutes
FROM ReportExecutionLogs
WHERE ExecutionStatus = 'Running'
  AND DATEDIFF(MINUTE, StartedAt, GETDATE()) > 30;
```

#### Step 2: Check Hangfire Processing

1. Hangfire Dashboard → **Processing** tab
2. Look for jobs with long duration
3. Note the Job ID

#### Step 3: Terminate Stuck Job

**Option A: Wait for Timeout** (Recommended)
- Jobs auto-timeout after 30 minutes
- Status will change to `Failed`

**Option B: Manual Cancellation**

```sql
-- Mark job as failed in execution log
UPDATE ReportExecutionLogs
SET ExecutionStatus = 'Failed',
    CompletedAt = GETDATE(),
    ErrorMessage = 'Manually terminated due to timeout'
WHERE Id = '<execution-id>';

-- Delete from Hangfire processing
DELETE FROM Hangfire.Job WHERE Id = '<hangfire-job-id>';
```

#### Step 4: Investigate Root Cause

Common causes:
- MongoDB query timeout (large date range)
- SMTP connection hanging
- Memory pressure on server

Check logs:
```bash
grep -i "timeout\|error\|exception" /var/log/cdr-api/log-*.txt | tail -100
```

#### Step 5: Re-run After Resolution

Follow [Manual Report Re-run](#manual-report-re-run) procedure.

---

## Monitoring Queries

### Daily Health Check

```sql
-- Jobs in last 24 hours
SELECT ReportType, ExecutionStatus, COUNT(*) AS JobCount
FROM ReportExecutionLogs
WHERE StartedAt >= DATEADD(HOUR, -24, GETDATE())
GROUP BY ReportType, ExecutionStatus;

-- Expected: 0-1 weekly, 0-1 monthly, all Completed
```

### Weekly Summary

```sql
-- Weekly report summary
SELECT 
    ReportType,
    COUNT(*) AS TotalJobs,
    SUM(CASE WHEN ExecutionStatus = 'Completed' THEN 1 ELSE 0 END) AS Successful,
    SUM(CASE WHEN ExecutionStatus = 'Failed' THEN 1 ELSE 0 END) AS Failed,
    AVG(DATEDIFF(SECOND, StartedAt, CompletedAt)) AS AvgDurationSec
FROM ReportExecutionLogs
WHERE StartedAt >= DATEADD(DAY, -7, GETDATE())
GROUP BY ReportType;
```

### Email Delivery Rate

```sql
-- Email delivery success rate (last 7 days)
SELECT 
    CAST(SUM(CASE WHEN DeliveryStatus = 'Sent' THEN 1 ELSE 0 END) AS DECIMAL(10,2)) 
        / NULLIF(COUNT(*), 0) * 100 AS SuccessRatePercent,
    COUNT(*) AS TotalEmails,
    SUM(CASE WHEN DeliveryStatus = 'Sent' THEN 1 ELSE 0 END) AS Delivered,
    SUM(CASE WHEN DeliveryStatus = 'Failed' THEN 1 ELSE 0 END) AS Failed
FROM EmailDeliveryAudits
WHERE SentAt >= DATEADD(DAY, -7, GETDATE());

-- Target: >99% success rate
```

### Missed Scheduled Jobs

```sql
-- Check for missed jobs (no execution on expected day)
DECLARE @ExpectedWeeklyRuns TABLE (ExpectedDate DATE);
DECLARE @ExpectedMonthlyRuns TABLE (ExpectedDate DATE);

-- Weekly: Every Monday
INSERT INTO @ExpectedWeeklyRuns
SELECT DATEADD(DAY, -7 * n, CAST(GETDATE() AS DATE))
FROM (SELECT 0 AS n UNION SELECT 1 UNION SELECT 2 UNION SELECT 3) nums
WHERE DATEPART(WEEKDAY, DATEADD(DAY, -7 * n, GETDATE())) = 2; -- Monday

-- Check actual executions
SELECT e.ExpectedDate, 
    CASE WHEN r.Id IS NOT NULL THEN 'Executed' ELSE 'MISSED' END AS Status
FROM @ExpectedWeeklyRuns e
LEFT JOIN ReportExecutionLogs r 
    ON CAST(r.StartedAt AS DATE) = e.ExpectedDate
    AND r.ReportType = 'Weekly'
    AND r.TriggerType = 'Scheduled';
```

### Data Quality Issues

```sql
-- Records with data quality flags
SELECT 
    PeriodStartDate, PeriodEndDate,
    RecordsProcessed, DataQualityWarnings
FROM ReportExecutionLogs
WHERE DataQualityWarnings IS NOT NULL
  AND StartedAt >= DATEADD(DAY, -30, GETDATE())
ORDER BY StartedAt DESC;
```

---

## Backup & Recovery

### Audit Log Backup

Daily backup of audit tables:

```sql
-- Export to CSV
bcp "SELECT * FROM CdrDb.dbo.ReportExecutionLogs WHERE StartedAt >= DATEADD(DAY, -1, GETDATE())" queryout "C:\Backups\ReportLogs_$(date +%Y%m%d).csv" -c -t, -S localhost -d CdrDb -T

bcp "SELECT * FROM CdrDb.dbo.EmailDeliveryAudits WHERE SentAt >= DATEADD(DAY, -1, GETDATE())" queryout "C:\Backups\EmailAudits_$(date +%Y%m%d).csv" -c -t, -S localhost -d CdrDb -T
```

### Retention Policy

| Table | Retention | Archive Strategy |
|-------|-----------|------------------|
| `ReportExecutionLogs` | 2 years | Monthly archive to cold storage |
| `EmailDeliveryAudits` | 1 year | Monthly archive to cold storage |
| `HolidayCalendars` | Indefinite | No archive needed |

### Recovery from Backup

```sql
-- Restore execution logs from backup
BULK INSERT ReportExecutionLogs
FROM 'C:\Backups\ReportLogs_20260101.csv'
WITH (FIELDTERMINATOR = ',', ROWTERMINATOR = '\n', FIRSTROW = 2);
```

---

## Escalation Procedures

### Level 1: Operations Team

**Handles**:
- Routine job re-runs
- Email delivery retries
- Basic troubleshooting

**Escalate if**:
- Issue persists after 2 retry attempts
- Database connectivity issues
- Authentication failures

### Level 2: Backend Engineering

**Contact**: backend@dogusoto.com  
**Response Time**: 2 hours during business hours

**Handles**:
- Application errors
- Query performance issues
- Code-level debugging

### Level 3: Infrastructure/DevOps

**Contact**: devops@dogusoto.com  
**Response Time**: 1 hour for critical

**Handles**:
- Server/network issues
- Database server problems
- SMTP infrastructure

### Critical Incident (P1)

**Trigger**:
- Complete report generation failure for 2+ consecutive weeks
- All email deliveries failing
- Data loss suspected

**Actions**:
1. Page on-call engineer: +90-xxx-xxx-xxxx
2. Create incident ticket in tracking system
3. Begin incident bridge call
4. Notify stakeholders

---

## Appendix: Common Commands

```bash
# View application logs
tail -f /var/log/cdr-api/log-$(date +%Y%m%d).txt

# Check Hangfire worker status
curl -s https://api.dogusoto.com/hangfire/stats | jq .

# Test API health
curl -s https://api.dogusoto.com/health

# Restart application (systemd)
sudo systemctl restart cdr-api

# Check MongoDB connectivity
mongo --eval "db.adminCommand('ping')" mongodb://localhost:27017/cdr
```

---

## Document History

| Version | Date | Author | Changes |
|---------|------|--------|---------|
| 1.0 | Jan 2026 | System | Initial runbook |
