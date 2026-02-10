# Deployment Guide: Automated CDR Email Reporting

**Version**: 1.0  
**Last Updated**: January 2026  
**Target Environment**: Production

---

## Prerequisites Checklist

Before deployment, ensure the following prerequisites are met:

### Infrastructure

- [ ] SQL Server 2019+ with sufficient storage (minimum 10GB for audit tables)
- [ ] MongoDB 6.0+ for CDR data
- [ ] SMTP server access (smtp.dogusoto.com or equivalent)
- [ ] .NET 8 Runtime installed on server
- [ ] Node.js 18+ for frontend build

### Access & Permissions

- [ ] SQL Server credentials with CREATE TABLE, INSERT, UPDATE, DELETE
- [ ] MongoDB read-only credentials for CDR collections
- [ ] SMTP credentials (noreply@dogusoto.com)
- [ ] Admin access for Hangfire dashboard

---

## Environment Configuration

### 1. appsettings.{Environment}.json

Create environment-specific configuration files:

**Production (appsettings.Production.json)**:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=prod-sql-server;Database=CdrDb;User Id=cdr_app;Password=${SQL_PASSWORD};TrustServerCertificate=true",
    "HangfireConnection": "Server=prod-sql-server;Database=HangfireDb;User Id=hangfire_app;Password=${HANGFIRE_PASSWORD};TrustServerCertificate=true"
  },
  "MongoDbSettings": {
    "ConnectionString": "mongodb://readonly:${MONGO_PASSWORD}@prod-mongo:27017/cdr?authSource=admin",
    "DatabaseName": "cdr",
    "CollectionName": "incoming_calls"
  },
  "EmailSettings": {
    "SmtpServer": "smtp.dogusoto.com",
    "SmtpPort": 587,
    "EnableSsl": true,
    "SenderEmail": "noreply@dogusoto.com",
    "SenderName": "CDR Report System",
    "Username": "noreply@dogusoto.com",
    "Password": "${SMTP_PASSWORD}",
    "MaxAttachmentSizeMb": 25,
    "RetryCount": 3,
    "RetryDelayMinutes": 5
  },
  "Hangfire": {
    "DashboardPath": "/hangfire",
    "SqlServerStorage": true,
    "WorkerCount": 2,
    "Queues": ["default", "reports"]
  }
}
```

### 2. Environment Variables

Set the following environment variables (never commit to source control):

```bash
# SQL Server
export SQL_PASSWORD="<production-sql-password>"
export HANGFIRE_PASSWORD="<production-hangfire-password>"

# MongoDB
export MONGO_PASSWORD="<production-mongo-password>"

# SMTP
export SMTP_PASSWORD="<production-smtp-password>"

# JWT (if separate from existing)
export JWT_KEY="<production-jwt-signing-key>"
```

For Windows Server, use Environment Variables in System Properties or IIS Application Pool settings.

---

## Database Migration Steps

### 1. Run EF Core Migrations

```bash
cd Cdr.Api

# List pending migrations
dotnet ef migrations list --project Cdr.Api.csproj

# Apply migrations to production
dotnet ef database update --project Cdr.Api.csproj --connection "Server=prod-sql-server;Database=CdrDb;..."
```

### 2. Verify Tables Created

```sql
-- Verify new tables exist
SELECT TABLE_NAME 
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_NAME IN ('ReportExecutionLogs', 'EmailDeliveryAudits', 'HolidayCalendars');

-- Expected: 3 rows
```

### 3. Seed Holiday Calendar

```bash
# Run seed script
sqlcmd -S prod-sql-server -d CdrDb -i "Database/Seeds/Turkey2026Holidays.sql"
```

Verify:
```sql
SELECT COUNT(*) FROM HolidayCalendars WHERE Year = 2026;
-- Expected: ~15 holidays
```

---

## Hangfire Setup

### 1. Create Hangfire Database

```sql
-- Create database
CREATE DATABASE HangfireDb;
GO

-- Hangfire will auto-create tables on first run
```

### 2. Dashboard Access Control

Add to `Program.cs` (already configured):

```csharp
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { new HangfireAuthorizationFilter() },
    DashboardTitle = "CDR Report Jobs"
});
```

Ensure `HangfireAuthorizationFilter` restricts access to Admin users only.

### 3. Verify Job Registration

After deployment, navigate to `/hangfire/recurring` and verify:

| Job ID | CRON | Next Execution |
|--------|------|----------------|
| `weekly-cdr-report` | `0 2 * * 1` | Next Monday 02:00 |
| `monthly-cdr-report` | `0 2 1 * *` | 1st of next month 02:00 |

---

## SMTP Configuration

### 1. SPF Record

Ensure DNS includes SPF record for dogusoto.com:
```
v=spf1 include:_spf.dogusoto.com ~all
```

### 2. DKIM Setup

Configure DKIM signing with 2048-bit RSA key:
```
selector._domainkey.dogusoto.com TXT "v=DKIM1; k=rsa; p=..."
```

### 3. DMARC Policy

Add DMARC record:
```
_dmarc.dogusoto.com TXT "v=DMARC1; p=quarantine; rua=mailto:dmarc@dogusoto.com"
```

### 4. Test Email Delivery

```bash
# Run test endpoint (Admin only)
curl -X POST "https://api.dogusoto.com/report/send-test-email" \
  -H "Authorization: Bearer <admin-token>"
```

Verify:
- Email received without spam marking
- SPF/DKIM/DMARC checks pass (check email headers)

---

## Frontend Deployment

### 1. Build Production Bundle

```bash
cd CDR.Web

# Install dependencies
npm ci

# Set environment
export VITE_APP_API_URL=https://api.dogusoto.com

# Build
npm run build
```

### 2. Deploy Static Files

Copy `dist/` contents to web server (IIS, Nginx, etc.):

```bash
# Example: rsync to production server
rsync -avz dist/ webserver:/var/www/cdr-web/
```

### 3. Verify Route Access

Navigate to `https://cdr.dogusoto.com/email-reports` and verify:
- Page loads without errors
- Only Admin users can access (others see 403 or redirect)

---

## Post-Deployment Verification

### 1. Health Checks

| Check | Command/URL | Expected |
|-------|-------------|----------|
| API Health | `GET /health` | 200 OK |
| Hangfire Dashboard | `GET /hangfire` | Dashboard loads |
| MongoDB Connection | Review startup logs | "MongoDB connected" |
| SQL Server Connection | Review startup logs | "EF Core initialized" |

### 2. Manual Report Test

```bash
# Generate on-demand report
curl -X POST "https://api.dogusoto.com/report/generate-email-report" \
  -H "Authorization: Bearer <admin-token>" \
  -H "Content-Type: application/json" \
  -d '{"reportType": "Weekly", "sendEmail": false}'
```

Expected: 200 OK with report data

### 3. Email Delivery Test

```bash
# Send report to test recipient
curl -X POST "https://api.dogusoto.com/report/generate-email-report" \
  -H "Authorization: Bearer <admin-token>" \
  -H "Content-Type: application/json" \
  -d '{"reportType": "Weekly", "sendEmail": true, "emailRecipients": ["test@dogusoto.com"]}'
```

Expected: Email received within 5 minutes

---

## Rollback Procedure

If deployment fails:

### 1. Application Rollback

```bash
# Stop current deployment
systemctl stop cdr-api

# Restore previous version
cp -r /backups/cdr-api-prev/* /var/www/cdr-api/

# Restart
systemctl start cdr-api
```

### 2. Database Rollback (if needed)

```bash
# Rollback last migration
dotnet ef database update <PreviousMigrationName> --project Cdr.Api.csproj
```

**Warning**: Only rollback if absolutely necessary. Data in new tables will be lost.

---

## Monitoring Setup

### 1. Log Aggregation

Configure Serilog to send logs to centralized logging:

```json
{
  "Serilog": {
    "WriteTo": [
      { "Name": "Console" },
      { "Name": "File", "Args": { "path": "/var/log/cdr-api/log-.txt", "rollingInterval": "Day" } },
      { "Name": "Seq", "Args": { "serverUrl": "https://seq.dogusoto.com" } }
    ]
  }
}
```

### 2. Alerts

Configure alerts for:
- Hangfire job failures (>2 consecutive failures)
- Email delivery failures (>5% failure rate)
- Report generation timeout (>30 minutes)

### 3. Dashboard Queries

```sql
-- Jobs executed in last 7 days
SELECT ReportType, ExecutionStatus, COUNT(*) 
FROM ReportExecutionLogs 
WHERE StartedAt >= DATEADD(DAY, -7, GETDATE())
GROUP BY ReportType, ExecutionStatus;

-- Email delivery rate
SELECT 
    CAST(SUM(CASE WHEN DeliveryStatus = 'Sent' THEN 1 ELSE 0 END) AS FLOAT) / COUNT(*) * 100 AS SuccessRate
FROM EmailDeliveryAudits
WHERE SentAt >= DATEADD(DAY, -7, GETDATE());
```

---

## Support Contacts

| Role | Contact | Responsibility |
|------|---------|----------------|
| DevOps | devops@dogusoto.com | Infrastructure, deployment |
| Backend Lead | backend@dogusoto.com | API issues, Hangfire |
| DBA | dba@dogusoto.com | Database migrations |
| IT Support | it@dogusoto.com | SMTP, DNS records |

---

## Document History

| Version | Date | Author | Changes |
|---------|------|--------|---------|
| 1.0 | Jan 2026 | System | Initial deployment guide |
