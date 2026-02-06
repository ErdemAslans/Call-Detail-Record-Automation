# Code Review Checklist: CDR Email Reporting

**Feature**: Automated CDR Email Reports  
**Reviewer**: _________________  
**Date**: _________________  
**PR/Branch**: `002-automated-cdr-email-reports`

---

## 1. Security (OWASP Compliance)

### A. Injection Prevention

- [ ] **SQL Injection**: All database queries use EF Core parameterized queries or Dapper with parameters
- [ ] **NoSQL Injection**: MongoDB queries use `Builders<T>` fluent API, not string concatenation
- [ ] **Command Injection**: No user input passed to system commands
- [ ] **LDAP Injection**: N/A (no LDAP queries in this feature)

**Verification**:
```csharp
// ✅ CORRECT: Parameterized query
var filter = Builders<CdrRecord>.Filter.Eq(x => x.Id, userId);

// ❌ WRONG: String concatenation
var filter = BsonDocument.Parse($"{{ _id: '{userId}' }}");
```

### B. Authentication & Authorization

- [ ] All report endpoints have `[Authorize(Roles = "Admin")]` attribute
- [ ] JWT token validation configured in `Program.cs`
- [ ] No hardcoded credentials in source code
- [ ] Sensitive settings in environment variables or Azure Key Vault

**Verification**:
```csharp
[ApiController]
[Route("[controller]")]
[Authorize(Roles = "Admin")]  // ✅ Required
public class ReportController : ControllerBase
```

### C. Input Validation

- [ ] Date parameters validated (ISO 8601, reasonable range)
- [ ] Email addresses validated (RFC 5322 format)
- [ ] Report type enum validated
- [ ] File upload size limits enforced (if applicable)
- [ ] Rate limiting implemented (10 requests/min)

**Verification**:
```csharp
// ✅ Email validation
if (!CdrReportHelper.IsValidEmail(email))
    return BadRequest("Invalid email format");

// ✅ Date range validation
if (startDate > endDate)
    return BadRequest("Start date must be before end date");
```

### D. Sensitive Data Handling

- [ ] SMTP credentials stored in environment variables
- [ ] No PII logged in plain text
- [ ] Audit logs do not contain sensitive data
- [ ] Excel reports do not expose internal system IDs

### E. Error Handling

- [ ] Generic error messages returned to clients (no stack traces)
- [ ] Detailed errors logged server-side only
- [ ] Exception handling in all public methods

---

## 2. Global Filter Compliance

**CRITICAL**: Every CDR query MUST chain `ApplyGlobalFilter()`

### Verification Checklist

- [ ] `CdrReportService.GenerateWeeklyReportAsync()` - Uses `ApplyGlobalFilter()`
- [ ] `CdrReportService.GenerateMonthlyReportAsync()` - Uses `ApplyGlobalFilter()`
- [ ] `CdrReportService.GenerateReportAsync()` - Uses `ApplyGlobalFilter()`
- [ ] `CdrRecordsRepository.GetByDateRangeAsync()` - Uses `ApplyGlobalFilter()`
- [ ] `CdrRecordsRepository.GetCallCountAsync()` - Uses `ApplyGlobalFilter()`
- [ ] `CdrRecordsRepository.GetAnsweredCallsAsync()` - Uses `ApplyGlobalFilter()`

**Search for violations**:
```bash
# Search for MongoDB queries that might miss global filter
grep -rn "\.Find(" --include="*.cs" | grep -v "ApplyGlobalFilter"
grep -rn "\.Aggregate(" --include="*.cs" | grep -v "ApplyGlobalFilter"
```

---

## 3. Hangfire Jobs

### A. Job Registration

- [ ] Weekly job registered with correct CRON: `0 2 * * 1`
- [ ] Monthly job registered with correct CRON: `0 2 1 * *`
- [ ] TimeZone set to Turkey Standard Time
- [ ] Jobs registered to "reports" queue
- [ ] Retry policy configured (max 3 retries)

### B. Job Execution

- [ ] Jobs are idempotent (safe to re-run)
- [ ] Timeout handling (30 minutes max)
- [ ] Execution logged to `ReportExecutionLogs`
- [ ] Error details captured on failure

### C. Concurrency

- [ ] No duplicate job execution (via `DisableConcurrentExecution`)
- [ ] Queue priority respects real-time operations

---

## 4. Email Delivery

### A. Configuration

- [ ] SMTP settings externalized (not hardcoded)
- [ ] Sender email: noreply@dogusoto.com
- [ ] Attachment size limit: 25MB
- [ ] Retry configuration: 3 retries, 5-min intervals

### B. Recipient Management

- [ ] Admin users discovered from AspNetUsers
- [ ] Switchboard staff excluded (SSaroglu@dogusotomotiv.com.tr)
- [ ] Invalid email addresses handled gracefully
- [ ] Batch sending with 2-second delay between recipients

### C. Audit Trail

- [ ] All delivery attempts logged to `EmailDeliveryAudits`
- [ ] Success/failure status recorded
- [ ] Error messages captured for failures

---

## 5. Data Quality

### A. Metrics Accuracy

- [ ] All 15 required metrics calculated
- [ ] Metric values validated against test data
- [ ] Zero-value periods handled (no division by zero)
- [ ] Outliers flagged (calls >8 hours)

### B. Date/Time Handling

- [ ] All dates in Turkey timezone (UTC+3)
- [ ] DST transitions handled correctly
- [ ] Week boundaries: Monday 00:00 - Sunday 23:59
- [ ] Month boundaries: 1st 00:00 - last day 23:59

### C. Excel Generation

- [ ] All 10 sheets generated correctly
- [ ] Column headers match specification
- [ ] Data formatting (dates, numbers, percentages)
- [ ] File size reasonable (<25MB)

---

## 6. Code Quality

### A. Naming Conventions

- [ ] Classes: PascalCase
- [ ] Methods: PascalCase
- [ ] Variables: camelCase
- [ ] Constants: UPPER_SNAKE_CASE or PascalCase
- [ ] Interfaces: IPrefixed (e.g., `ICdrReportService`)

### B. Documentation

- [ ] Public methods have XML documentation
- [ ] Complex logic has inline comments
- [ ] README files updated
- [ ] API documentation complete

### C. Async/Await

- [ ] All async methods return `Task` or `Task<T>`
- [ ] No `.Result` or `.Wait()` calls (blocking)
- [ ] `CancellationToken` propagated where appropriate

### D. Dependency Injection

- [ ] Services registered with correct lifetime (Scoped for repositories)
- [ ] No `new` instantiation of services in controllers
- [ ] Interfaces used for dependencies

---

## 7. Testing

### A. Unit Tests

- [ ] `CdrReportService` methods tested
- [ ] `CdrReportEmailService` methods tested
- [ ] `CdrReportHelper` methods tested
- [ ] Edge cases covered (empty data, null values)

### B. Integration Tests

- [ ] Hangfire job execution tested
- [ ] Email delivery tested (with test SMTP)
- [ ] Database operations tested

### C. Test Coverage

- [ ] Core services: >80% coverage
- [ ] Helpers: >90% coverage
- [ ] Controllers: >70% coverage

---

## 8. Performance

### A. Database Queries

- [ ] Queries use appropriate indexes
- [ ] No N+1 query patterns
- [ ] Projections used (select only needed fields)
- [ ] Date range filters applied before aggregation

### B. Memory Usage

- [ ] Large collections streamed, not loaded entirely
- [ ] Excel files generated incrementally
- [ ] No memory leaks in long-running jobs

### C. Benchmarks

- [ ] Weekly report generation: <10 seconds
- [ ] Monthly report generation: <15 seconds
- [ ] Email delivery: <5 minutes total

---

## 9. Logging & Monitoring

### A. Structured Logging

- [ ] `ILogger<T>` used throughout
- [ ] Log levels appropriate (Info, Warning, Error)
- [ ] Correlation IDs for request tracing
- [ ] No sensitive data in logs

### B. Audit Logging

- [ ] Report generation logged
- [ ] Email delivery logged
- [ ] User actions logged (who triggered what)

---

## 10. Frontend (Vue.js)

### A. Component Structure

- [ ] Composition API used (`<script setup>`)
- [ ] Props and emits properly typed
- [ ] Pinia store for state management

### B. API Integration

- [ ] ApiService used for HTTP calls
- [ ] Error handling with ElMessage
- [ ] Loading states managed

### C. Internationalization

- [ ] All user-facing text in i18n files
- [ ] Both English and Turkish translations

### D. Authorization

- [ ] Route guards for Admin-only pages
- [ ] UI elements hidden for non-Admins

---

## Sign-off

| Role | Name | Date | Status |
|------|------|------|--------|
| Author | | | |
| Reviewer 1 | | | ☐ Approved / ☐ Changes Requested |
| Reviewer 2 | | | ☐ Approved / ☐ Changes Requested |
| Tech Lead | | | ☐ Approved |

---

## Notes

_Add any additional notes, concerns, or follow-up items here._
