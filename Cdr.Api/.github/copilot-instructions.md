# Cdr.Api - Call Detail Records Processing System

## Project Summary

A .NET 8.0 REST API for processing, storing, and reporting on Call Detail Records (CDR) from telephony systems. Tracks call metadata across distributed operators, departments, and generates analytics for call center operations.

## Critical Architecture Patterns

### Dual-Database Design
- **SQL Server**: User identity, authentication, and role management via Entity Framework Core
- **MongoDB**: CDR records (high-volume operational data) with read-heavy analytics patterns
- **Key Decision**: MongoDB hosts raw CDR records because they're imported from telephony systems and queried for aggregations; SQL Server manages identity for authorization

### Global Filtering Pattern
All CDR queries apply `ApplyGlobalFilter()` in `CdrRecordsRepository` - only includes records where phone numbers start with "8036" (domain-specific business rule). This filter chains with other predicates using MongoDB `Builders<T>.Filter.And()`.

### Multi-Tenant Report Generation
Reports are period-based (Weekly/Monthly/Yearly). Service methods route through `CdrRecordsService` → repository aggregation methods → extension methods for chart conversion. See `GetWeeklyAnsweredCallsAsync()` pattern.

## Data Flow Architecture

1. **CDR Import**: External telephony system → MongoDB collection `incoming_calls`
2. **Query Layer**: `ReadonlyMongoRepository<T>` base class provides LINQ-expression predicates; `CdrRecordsRepository` extends with domain aggregations
3. **Transformation**: AutoMapper profiles (`BreakProfile.cs`, `ChartProfiles.cs`) convert mongo documents to DTOs
4. **Async Reporting**: Hangfire jobs scheduled in `HangfireJobs.ExecuteJobs()` run department statistics daily/weekly/monthly

## Key Integration Points

### Hangfire Scheduled Jobs
Located in `Program.cs` and `HangfireJobs.cs`:
- Uses SQL Server for job persistence
- Jobs run at **2:00 AM local time** for daily/weekly/monthly statistics
- Always passes `DateTime.Now.AddDays(-X)` for relative date ranges; respect timezone via `RecurringJobOptions`

### JWT Authentication
- Configured in `Program.cs` with `JwtConfigModel` from appsettings
- Role-based authorization: `[Authorize(Roles = "Admin")]` on controllers
- `ITokenService` generates access + refresh tokens; `AccountService` validates credentials

### Email Notifications
- Injected as `INotification<EmailMessage>` singleton
- Used in `ReportController.SendTestEmail()` and can extend to async report delivery

## Essential Conventions

### Repository Methods Pattern
All async, return `Task<T>`. MongoDB methods use `Builders<T>.Filter` not LINQ. Example:
```csharp
public async Task<IEnumerable<CdrRecord>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
{
    var filter = Builders<CdrRecord>.Filter.And(
        ApplyGlobalFilter(),
        Builders<CdrRecord>.Filter.Gte(x => x.DateTime!.Connect, startDate),
        Builders<CdrRecord>.Filter.Lte(x => x.DateTime!.Disconnect, endDate)
    );
    return await _collection.Find(filter).ToListAsync();
}
```

### Service Layer Delegation
Services orchestrate repositories and mappers. Keep business logic minimal - most complexity lives in repository aggregations. Services handle enums (e.g., `ReportPeriod` switch statements) and orchestration.

### DTO Transformation
Use AutoMapper profiles in `Profiles/` folder. Chart data uses extension methods (`DepartmentCallStatisticsExtensions.ConvertToBarChartResponse<TTarget, TSource>()`) for type-safe conversions.

### CDR Entity Structure
`Models/Entities/CDR/CdrRecord.cs` contains nested objects:
- `CallingParty`, `OriginalCalledParty`, `FinalCalledParty` (phone number details)
- `DateTime.Connect`, `DateTime.Disconnect` (call timing)
- `GlobalCall`, `Incoming`, `Outgoing`, `Destination` (call routing info)
- Refer to `Helpers/CdrFields_Descriptions.md` for field meanings

## Configuration & Secrets

**appsettings.json** defines:
- `ConnectionStrings.DefaultConnection`: SQL Server (identity database)
- `ConnectionStrings.MongoDbConnection`: MongoDB cluster
- `MongoDb.DatabaseName`: "cdr" (database), `CollectionName`: "incoming_calls" (collection)
- `JwtConfig`: Token issuer/audience/expiry (secrets rotate per environment)

Development uses hardcoded values; production requires environment variables (not committed).

## Common Implementation Workflows

### Adding a Report Endpoint
1. Define aggregation in `CdrRecordsRepository` (returns domain DTOs)
2. Implement service method in `CdrRecordsService` that calls repository + applies `_mapper`
3. Add `[HttpGet(...)]` to `ReportController`
4. For Hangfire jobs, add `RecurringJob.AddOrUpdate()` in `HangfireJobs.ExecuteJobs()`

### Extending Pagination/Filtering
`CdrFilter`, `PagedRequest`, `PagedRequestOrder` support filtering. Repository methods accept these objects and build MongoDB filters dynamically. Always include `ApplyGlobalFilter()` in the filter chain.

### Adding Authentication to New Endpoints
- Default: inherit `[Authorize]` from controller class
- Role-based: `[Authorize(Roles = "Admin")]` (check User table for role assignments)
- Anonymous: explicitly add `[AllowAnonymous]` attribute

## Development Practices

- **Async-first**: All data access is async (MongoDB, EF Core)
- **No blocking calls**: Avoid `.Result`, `.Wait()` in services
- **Filter composition**: Build MongoDB filters incrementally, combine with `Filter.And()`
- **Logging**: Injected via `ILogger<T>` (configured in Startup)
- **Error handling**: Controllers handle `UnauthorizedAccessException` for auth; repositories bubble data errors
- **Comments**: Complex aggregations merit explanatory comments (see `ApplyGlobalFilter()` example)
