# Architecture & Design Patterns

**Last Updated**: January 2026  
**Focus**: Repository Pattern, Dependency Injection, Global Filtering  

---

## 📐 Mimari Genel Bakış

Cdr.Api **Layered Architecture** + **Repository Pattern** kullanır:

```
┌─────────────────────────────────────────────┐
│         API Layer (Controllers)             │
│   - AccountController (Auth endpoints)      │
│   - ReportController (CDR reporting)        │
│   - OperatorController (Operator endpoints) │
└─────────────────┬───────────────────────────┘
                  │ Dependency Injection
┌─────────────────▼───────────────────────────┐
│      Service Layer (Business Logic)         │
│   - AccountService                          │
│   - CdrRecordsService                       │
│   - OperatorService                         │
└─────────────────┬───────────────────────────┘
                  │ Interface-based
┌─────────────────▼───────────────────────────┐
│    Repository Layer (Data Access)           │
│   - ICdrRecordsRepository                   │
│   - IOperatorRepository                     │
│   - IBreakRepository                        │
│   - IReadonlyMongoRepository<T>             │
└─────────────────┬───────────────────────────┘
                  │ Abstraction
┌─────────────────▼───────────────────────────┐
│         Data Access Layer                   │
│   - MongoDbContext                          │
│   - CdrContext (Entity Framework)           │
│   - MongoDB Collections                     │
│   - SQL Server Tables                       │
└─────────────────────────────────────────────┘
```

---

## 🔌 Dependency Injection (Program.cs)

### DI Container Setup

```csharp
// Contexts
builder.Services.AddDbContext<CdrContext>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSingleton<MongoDbContext>();

// Services
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<ICdrRecordsService, CdrRecordsService>();

// Repositories
builder.Services.AddScoped<ICdrRecordsRepository, CdrRecordsRepository>();
builder.Services.AddScoped<IOperatorRepository, OperatorRepository>();
builder.Services.AddScoped<IReadonlyMongoRepository<CdrRecord>>(sp =>
{
    var context = sp.GetRequiredService<MongoDbContext>();
    var mongoDbSettings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
    return new ReadonlyMongoRepository<CdrRecord>(context, mongoDbSettings.CollectionName);
});

// AutoMapper
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
```

### Lifetime Policies
| Scope | Kullanım | Örnek |
|-------|----------|-------|
| **Singleton** | App lifetime boyunca tek instance | MongoDbContext |
| **Scoped** | Per HTTP request | Services, Repositories |
| **Transient** | Her istekte yeni instance | (Bu projede kullanılmıyor) |

**Neden MongoDbContext Singleton?** MongoDB client thread-safe ve expensive initialization'dan kaçınmak için

---

## 🎯 Repository Pattern

### Amaç
- Data access logic'i encapsulate
- Business logic'ten database details'i izole
- Testability'yi artırma (mock repository)

### Generic Base: ReadonlyMongoRepository<T>

```csharp
public class ReadonlyMongoRepository<T> where T : class
{
    protected readonly MongoDbContext _context;
    protected readonly IMongoCollection<T> _collection;

    public ReadonlyMongoRepository(MongoDbContext context, string collectionName)
    {
        _context = context;
        _collection = _context.GetCollection<T>(collectionName);
    }

    public async Task<T?> GetByIdAsync(ObjectId id)
    {
        var filter = Builders<T>.Filter.Eq("_id", id);
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _collection.Find(Builders<T>.Filter.Empty).ToListAsync();
    }
}
```

### Specialized: CdrRecordsRepository

```csharp
public class CdrRecordsRepository : ReadonlyMongoRepository<CdrRecord>, ICdrRecordsRepository
{
    private readonly IMongoCollection<Operator> _userCollection;
    private readonly IMongoCollection<Department> _departmentCollection;

    // ⭐ CRITICAL: Global Filter Implementation
    private FilterDefinition<CdrRecord> ApplyGlobalFilter()
    {
        return Builders<CdrRecord>.Filter.Or(
            Builders<CdrRecord>.Filter.And(
                Builders<CdrRecord>.Filter.Ne(x => x.OriginalCalledParty, null),
                Builders<CdrRecord>.Filter.Regex(x => x.OriginalCalledParty!.Number, 
                    new BsonRegularExpression("^8036.*"))
            ),
            // ... (other phone number fields)
        );
    }

    public async Task<IEnumerable<CdrRecord>> GetByDateRangeAsync(
        DateTime startDate, DateTime endDate)
    {
        var filter = Builders<CdrRecord>.Filter.And(
            ApplyGlobalFilter(),  // ⭐ ALWAYS CHAIN THIS
            Builders<CdrRecord>.Filter.Gte(x => x.DateTime!.Connect, startDate),
            Builders<CdrRecord>.Filter.Lte(x => x.DateTime!.Disconnect, endDate)
        );
        return await _collection.Find(filter).ToListAsync();
    }
}
```

### ⚠️ CRITICAL RULE: Global Filter

**Her CDR query'sinde `ApplyGlobalFilter()` kullanılMALIDIR!**

```csharp
// ✅ CORRECT
var filter = Builders<CdrRecord>.Filter.And(
    ApplyGlobalFilter(),
    otherConditions
);

// ❌ WRONG (Data leakage!)
var filter = Builders<CdrRecord>.Filter.And(
    otherConditions
);
```

**Neden?** "8036" ile başlamayan numaralar filtrelenmeli (business rule)

---

## 📊 AutoMapper Profiles

### Mapping Tanımları (Profiles/)

```csharp
// ChartProfiles.cs
public class ChartProfile : Profile
{
    public ChartProfile()
    {
        CreateMap<CdrRecord, OperatorChartData>()
            .ForMember(dest => dest.OperatorName, 
                opt => opt.MapFrom(src => src.Operator.Name));
    }
}
```

### Entity → DTO Dönüşümü

```csharp
// Service'te
var cdrRecords = await _repository.GetByDateRangeAsync(start, end);
var dtos = _mapper.Map<IEnumerable<CdrRecordDto>>(cdrRecords);
```

**Benefits:**
- DTO'lar sensitive fields'ı hide edebilir
- Entity changes API'yi break etmez
- Boilerplate code azalır

---

## 🔐 Authentication Architecture

### JWT Token Flow

```
1. Login Request (email, password)
   ↓
2. AccountService.LoginAsync()
   ├─ UserManager.FindByEmailAsync() [SQL Server]
   ├─ UserManager.CheckPasswordAsync() [Identity]
   └─ TokenService.GenerateAccessToken() [JWT creation]
   ↓
3. Response: { token, refreshToken }
```

### Token Validation

```csharp
// Program.cs
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtConfig.Issuer,
            ValidAudience = jwtConfig.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtConfig.Key))
        };
    });
```

### Authorization Attributes

```csharp
[Authorize]  // Authenticated users only
public class ReportController : ControllerBase { }

[Authorize(Roles = "Admin")]  // Admin only
public async Task DeleteReportAsync(int id) { }
```

---

## 🗂️ Folder Structure & Responsibilities

```
Cdr.Api/
├── Common/           # Shared enums, constants
├── Context/          # DbContexts (SQL, MongoDB)
├── Controllers/      # HTTP endpoint handlers
├── Extensions/       # Extension methods
├── Helpers/          # Utilities, helpers
├── Interfaces/       # Service & Repository contracts
├── Migrations/       # EF Core migrations
├── Models/
│   ├── Account/      # Auth models
│   ├── Entities/     # Domain entities
│   ├── Request/      # Request DTOs
│   ├── Response/     # Response DTOs
│   └── ...
├── Profiles/         # AutoMapper mappings
├── Repositories/     # Data access implementations
├── Services/         # Business logic implementations
└── Program.cs        # DI & Middleware configuration
```

---

## ⚡ Key Patterns

### 1. Repository Abstraction
**Pattern**: Generic repository base + specialized implementations

```csharp
// Generic
public interface IReadonlyMongoRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByIdAsync(ObjectId id);
}

// Specialized
public interface ICdrRecordsRepository : IReadonlyMongoRepository<CdrRecord>
{
    Task<IEnumerable<CdrRecord>> GetByDateRangeAsync(DateTime start, DateTime end);
    Task<IEnumerable<WeeklyAnsweredCallRate>> GetWeeklyAnsweredCallsAsync(...);
}
```

### 2. Service → Repository Delegation
```csharp
// Service
public class CdrRecordsService : ICdrRecordsService
{
    private readonly ICdrRecordsRepository _repository;
    
    public async Task<IEnumerable<CdrRecordDto>> GetRecordsAsync(...)
    {
        var records = await _repository.GetByDateRangeAsync(...);
        return _mapper.Map<IEnumerable<CdrRecordDto>>(records);
    }
}
```

### 3. Global Filtering (EF Core Query Filters analogy)
```csharp
// MongoDB doesn't have automatic QueryFilters like EF Core
// So ApplyGlobalFilter() must be called manually in every query
```

---

## 🔄 Request Lifecycle

```
1. HTTP Request arrives
   ↓
2. Middleware pipeline (CORS, Auth, etc.)
   ↓
3. Routing → Controller action selected
   ↓
4. DI Container: Inject dependencies
   ↓
5. Controller calls Service method
   ↓
6. Service orchestrates:
   - Business logic
   - Repository calls
   - Mapping
   ↓
7. Repository executes data query
   ↓
8. MongoDB/SQL Server returns data
   ↓
9. Response DTO serialized to JSON
   ↓
10. HTTP Response sent
```

---

## 📧 Email Reporting Service Layer

### Architecture Extension for Automated Reports

The email reporting feature adds a new service layer for scheduled report generation and delivery:

```
┌─────────────────────────────────────────────────────────────────────┐
│                       Scheduling Layer                               │
│                                                                      │
│   ┌─────────────────────┐  CRON Triggers  ┌─────────────────────┐   │
│   │ CdrReportJobService │◄───────────────►│    Hangfire Jobs    │   │
│   │                     │                  │ Weekly:  0 2 * * 1  │   │
│   │ - TriggerWeekly()   │                  │ Monthly: 0 2 1 * *  │   │
│   │ - TriggerMonthly()  │                  └─────────────────────┘   │
│   └──────────┬──────────┘                                            │
└──────────────│───────────────────────────────────────────────────────┘
               │
               │ Orchestrates
               ▼
┌──────────────────────────────────────────────────────────────────────┐
│                      Report Generation Layer                          │
│                                                                       │
│   ┌──────────────────────┐              ┌───────────────────────┐    │
│   │   CdrReportService   │              │ CdrReportEmailService │    │
│   │                      │              │                       │    │
│   │ - GenerateReport()   │──generates──►│ - SendReportEmail()   │    │
│   │ - BuildMetrics()     │              │ - BuildEmailBody()    │    │
│   │ - CreateExcelFile()  │              │ - AttachReport()      │    │
│   └──────────┬───────────┘              └───────────┬───────────┘    │
│              │                                      │                 │
└──────────────│──────────────────────────────────────│─────────────────┘
               │                                      │
               │ Queries with                         │ Sends via
               │ ApplyGlobalFilter()                  │
               ▼                                      ▼
┌──────────────────────────┐              ┌───────────────────────┐
│  CdrRecordsRepository    │              │     SMTP Service      │
│                          │              │                       │
│  MongoDB Collections     │              │  smtp.dogusoto.com    │
└──────────────────────────┘              └───────────────────────┘
```

### Service Registration (Program.cs)

```csharp
// Email Reporting Services
builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<ICdrReportService, CdrReportService>();
builder.Services.AddScoped<ICdrReportEmailService, CdrReportEmailService>();
builder.Services.AddScoped<ICdrReportJobService, CdrReportJobService>();
```

### Global Filter in Report Queries

**CRITICAL**: All report aggregation queries MUST use `ApplyGlobalFilter()`:

```csharp
public class CdrReportService : ICdrReportService
{
    public async Task<CdrEmailReportResponse> GenerateReport(ReportType type)
    {
        // ⭐ Global filter applied to all metrics
        var baseFilter = Builders<CdrRecord>.Filter.And(
            ApplyGlobalFilter(),
            GetDateRangeFilter(type)
        );
        
        // Aggregate metrics from filtered data only
        var totalCalls = await CountCallsAsync(baseFilter);
        var answeredCalls = await CountAnsweredAsync(baseFilter);
        // ...
    }
}
```

### Job Scheduling via Hangfire

```csharp
// HangfireJobs.cs
RecurringJob.AddOrUpdate<ICdrReportJobService>(
    "weekly-cdr-report",
    job => job.TriggerWeeklyReportAsync(),
    "0 2 * * 1",  // Every Monday at 02:00 Turkey time
    new RecurringJobOptions { TimeZone = turkeyTimeZone }
);

RecurringJob.AddOrUpdate<ICdrReportJobService>(
    "monthly-cdr-report", 
    job => job.TriggerMonthlyReportAsync(),
    "0 2 1 * *",  // 1st of month at 02:00 Turkey time
    new RecurringJobOptions { TimeZone = turkeyTimeZone }
);
```

### Audit Trail (SQL Server)

Report executions are logged for audit and troubleshooting:

```sql
-- ReportExecutionLogs table
SELECT ReportType, Status, StartTime, EndTime, RecipientCount, ErrorMessage
FROM ReportExecutionLogs
WHERE StartTime >= DATEADD(DAY, -7, GETDATE())
ORDER BY StartTime DESC
```

For detailed documentation, see [08-Email-Reporting.md](08-Email-Reporting.md).

---

## ⚠️ Security Considerations

- **Dependency Scope**: Scoped services ensure request isolation
- **Repository Pattern**: Centralizes security checks (e.g., ApplyGlobalFilter)
- **AutoMapper**: Can hide sensitive fields in mapping profiles
- **Authentication**: Decorator pattern via `[Authorize]` attributes
- **Email Reports**: Admin-only endpoints, audit logging for all report generations

