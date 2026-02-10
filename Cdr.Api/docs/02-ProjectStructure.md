# Cdr.Api Project Structure

**Last Updated**: January 2026  

---

## 📁 Complete Project Layout

```
Cdr.Api/
├── docs/                    # 📚 DOCUMENTATION
│   ├── 00-README.md         # Navigation guide
│   ├── 01-Overview.md       # Project overview & tech stack
│   ├── 03-Architecture.md   # Design patterns, DI, Repository
│   ├── 04-Authentication.md # JWT, Identity, Authorization
│   ├── 05-DataLayer.md      # MongoDB, SQL Server, contexts
│   └── 07-Hangfire.md       # Background jobs, scheduling
│
├── Common/                  # Shared enums, constants
│   └── Enums/
│
├── Context/                 # Database contexts
│   ├── CdrContext.cs        # EF Core DbContext (SQL Server)
│   └── MongoDbContext.cs    # MongoDB client setup
│
├── Controllers/             # HTTP API endpoints
│   ├── AccountController.cs      # Auth endpoints (/api/account)
│   ├── OperatorController.cs     # Operator endpoints (/api/operators)
│   └── ReportController.cs       # Report endpoints (/api/report)
│
├── Extensions/              # Extension methods
│   └── DepartmentCallStatisticsExtensions.cs
│
├── Helpers/                 # Utility functions
│   ├── CdrDeciderHelper.cs
│   ├── ChartHelper.cs
│   ├── MongoDbSettings.cs
│   └── Mongo/
│
├── Interfaces/              # Service & Repository contracts
│   ├── IBreakRepository.cs
│   ├── ICdrRecordsRepository.cs
│   ├── IDepartmentRepository.cs
│   ├── IJwtConfig.cs
│   ├── IMongoDbSettings.cs
│   ├── IOperatorRepository.cs
│   └── IReadonlyMongoRepository.cs
│
├── Migrations/              # EF Core migrations
│   ├── 20241007214722_InitIdentity.cs
│   └── 20241007230746_Add_RefreshToken.cs
│
├── Models/                  # Data models & DTOs
│   ├── Account/             # Login, auth models
│   │   ├── LoginModel.cs
│   │   └── Responses/
│   ├── Entities/            # Domain entities
│   │   ├── Cdr/             # CDR records
│   │   ├── CdrRecord.cs
│   │   ├── Operator.cs
│   │   └── Department.cs
│   ├── Pagination/          # Pagination models
│   ├── Request/             # Request DTOs
│   ├── Response/            # Response DTOs
│   │   ├── Dashboard/
│   │   └── UserStatistics/
│   └── Notification/        # Email notifications
│
├── Profiles/                # AutoMapper configurations
│   ├── BreakProfile.cs      # Break → BreakDto mapping
│   ├── ChartProfiles.cs     # Entity → ChartData mappings
│   └── Resolvers.cs/        # Custom resolvers
│
├── Properties/              # Project properties
│   └── launchSettings.json  # Development server settings
│
├── Repositories/            # Data access layer
│   ├── BreakRepository.cs
│   ├── CdrRecordsRepository.cs   # ⭐ CDR data access + global filter
│   ├── DepartmentRepository.cs
│   ├── OperatorRepository.cs
│   └── ReadonlyMongoRepository.cs # Generic MongoDB base
│
├── Services/                # Business logic layer
│   ├── AccountService.cs         # Login, auth logic
│   ├── CdrRecordsService.cs      # CDR reports & aggregations
│   ├── HangfireJobs.cs           # Background job scheduling
│   ├── OperatorService.cs        # Operator management
│   ├── TokenService.cs           # JWT token generation
│   ├── Interfaces/               # Service contracts
│   └── Notification/             # Email sending
│
├── .gitignore               # Git ignore patterns
├── appsettings.json         # Production config
├── appsettings.Development.json
├── Cdr.Api.csproj          # Project file
├── Cdr.Api.sln             # Solution file
└── Program.cs              # Startup & DI configuration
```

---

## 🎯 Where to Find Things

### I need to...

#### **Understand the project**
- Start: [01-Overview.md](docs/01-Overview.md)
- Then: [03-Architecture.md](docs/03-Architecture.md)

#### **Work with authentication**
- Go to: [04-Authentication.md](docs/04-Authentication.md)
- Files: Controllers/AccountController.cs, Services/AccountService.cs

#### **Query CDR data**
- Go to: [05-DataLayer.md](docs/05-DataLayer.md)
- Files: Repositories/CdrRecordsRepository.cs, Models/Entities/CdrRecord.cs
- **Remember**: Always use `ApplyGlobalFilter()`!

#### **Create a report**
- Go to: [07-Hangfire.md](docs/07-Hangfire.md)
- Files: Services/CdrRecordsService.cs, Services/HangfireJobs.cs

#### **Add a new API endpoint**
1. Create request/response DTOs in Models/
2. Add repository method if data access needed (Repositories/)
3. Add service method (Services/)
4. Add controller action (Controllers/)
5. Add AutoMapper profile if DTOs created (Profiles/)
6. Register dependencies in Program.cs

#### **Debug a query**
- Check: Repositories/CdrRecordsRepository.cs for ApplyGlobalFilter()
- Check: Context/MongoDbContext.cs for connection
- Check: appsettings.Development.json for connection strings

---

## 🔑 Key Files by Responsibility

### Dependency Injection & Configuration
- `Program.cs` - DI setup, middleware, Hangfire
- `appsettings.json` - Configuration

### API Endpoints
- `Controllers/*.cs` - HTTP handlers
- Routes defined in controller action attributes

### Business Logic
- `Services/*.cs` - Domain logic, orchestration
- `Repositories/*.cs` - Data access abstraction

### Data Models
- `Models/Entities/*.cs` - Domain entities
- `Models/Request/*.cs` - Input DTOs
- `Models/Response/*.cs` - Output DTOs
- `Models/Account/*.cs` - Auth models

### Data Access
- `Context/MongoDbContext.cs` - MongoDB setup
- `Context/CdrContext.cs` - SQL Server setup (Identity)
- `Repositories/*.cs` - Queries

### Mappings
- `Profiles/*.cs` - Entity to DTO conversions

### Background Jobs
- `Services/HangfireJobs.cs` - Job scheduling
- `Services/CdrRecordsService.cs` - Report generation logic

---

## 🔄 Request Flow Example

### GET /api/report/operator-stats

```
1. HTTP Request arrives at Controller
   └─ ReportController.GetOperatorStatsAsync()

2. Controller calls Service
   └─ CdrRecordsService.GetOperatorStatsAsync()

3. Service uses Repository
   └─ CdrRecordsRepository.GetOperatorStatsAsync()

4. Repository queries MongoDB
   ├─ ApplyGlobalFilter() [⭐ CRITICAL]
   ├─ Apply date range filter
   ├─ Aggregation pipeline
   └─ Return raw data

5. Service processes data
   ├─ Calculate derived metrics
   ├─ Sort/format results
   └─ Return business objects

6. Controller maps to DTO
   ├─ AutoMapper.Map<OperatorStatsDto>()
   └─ JSON serialize

7. Response returned to client
   └─ { ... operator stats ... }
```

---

## 📦 Dependencies Overview

### NuGet Packages
```xml
<!-- ORM & Databases -->
MongoDB.Driver          v2.29.0   ← CDR data (NoSQL)
Microsoft.EntityFrameworkCore.SqlServer  ← Identity (SQL)

<!-- Authentication -->
Microsoft.AspNetCore.Authentication.JwtBearer ← JWT tokens
Microsoft.AspNetCore.Identity.EntityFrameworkCore ← User mgmt

<!-- DI & Mapping -->
AutoMapper                      ← DTO mapping
AutoMapper.Extensions.Microsoft.DependencyInjection

<!-- Background Jobs -->
Hangfire                        ← Scheduled reports
Hangfire.AspNetCore
Hangfire.SqlServer

<!-- Utilities -->
EPPlus                          ← Excel export
Swashbuckle.AspNetCore          ← Swagger docs
```

---

## 🔐 Security Checklist

- ✅ Password complexity enforced (Program.cs)
- ✅ JWT tokens validated (AddJwtBearer)
- ✅ Global filter on CDR queries (ApplyGlobalFilter)
- ✅ HTTPS redirected (app.UseHttpsRedirection)
- ✅ CORS restricted (AddCors - single origin)
- ⚠️ Review: API key rotation, token refresh, audit logging

---

## 🧪 Testing the API

### Swagger Documentation
```
https://localhost:5001/swagger
```

### Hangfire Dashboard
```
https://localhost:5001/hangfire
```

### Common Endpoints
```
POST   /api/account/login
GET    /api/report/operator-stats
GET    /api/report/daily
GET    /api/report/weekly
GET    /api/operators
GET    /api/operators/{id}
```

---

## 📝 Adding a New Feature

1. **Define data model** → Models/Entities/
2. **Create repository method** → Repositories/
3. **Add service logic** → Services/
4. **Create controller endpoint** → Controllers/
5. **Add mapping profiles** → Profiles/ (if DTO needed)
6. **Register in DI** → Program.cs
7. **Document in docs/** → Add reference here
8. **Test with Swagger** → https://localhost:5001/swagger

---

## 🛠️ Common Tasks

### Modify a CDR Query
- File: `Repositories/CdrRecordsRepository.cs`
- Remember: Chain `ApplyGlobalFilter()` in filters

### Add a Scheduled Report
- File: `Services/HangfireJobs.cs`
- Pattern: `RecurringJob.AddOrUpdate<IService>(...)`

### Change Authentication Logic
- File: `Services/AccountService.cs`
- Config: `Program.cs` (AddIdentity options)

### Add a New DTO
- Create: `Models/Response/MyNewDto.cs`
- Map: `Profiles/MyNewProfile.cs`
- Register: `Program.cs` (AddAutoMapper)

---

## 📚 Full Documentation Index

| Topic | File |
|-------|------|
| Overview | [01-Overview.md](docs/01-Overview.md) |
| Architecture | [03-Architecture.md](docs/03-Architecture.md) |
| Authentication | [04-Authentication.md](docs/04-Authentication.md) |
| Data Layer | [05-DataLayer.md](docs/05-DataLayer.md) |
| Background Jobs | [07-Hangfire.md](docs/07-Hangfire.md) |

---

## 🚀 Quick Start Commands

```bash
# Run locally
dotnet run

# Run migrations
dotnet ef database update

# Create migration
dotnet ef migrations add MigrationName

# View API
https://localhost:5001/swagger

# View Hangfire
https://localhost:5001/hangfire
```

