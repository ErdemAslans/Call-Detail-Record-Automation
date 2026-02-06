# Data Layer - MongoDB & SQL Server

**Last Updated**: January 2026  
**Focus**: Data access patterns, contexts, queries  

---

## 🏛️ Data Storage Architecture

```
┌──────────────────────────────────────┐
│      Cdr.Api Application             │
└──────────────┬───────────────────────┘
               │
      ┌────────┴────────┐
      │                 │
┌─────▼────────┐   ┌───▼──────────┐
│ MongoDbContext   │  CdrContext  │
│ (NoSQL Access)  │ (SQL Access) │
└─────┬────────┘   └───┬──────────┘
      │                 │
┌─────▼─────────────────▼──────────┐
│                                   │
│ MongoDB (Operational Data)        │
│ • incoming_calls (CDR records)    │
│ • users (Operators)               │
│ • departments                     │
│ • breaks                          │
│                                   │
└─────────────────────────────────┘

┌──────────────────────────────────┐
│                                  │
│ SQL Server (Identity & Config)   │
│ • AspNetUsers                    │
│ • AspNetRoles                    │
│ • AspNetUserRoles                │
│ • RefreshTokens                  │
│                                  │
└──────────────────────────────────┘
```

---

## 🔗 MongoDbContext

### Initialization

```csharp
public class MongoDbContext
{
    private readonly IConfiguration _configuration;
    private readonly IMongoDatabase _database;

    public MongoDbContext(IConfiguration configuration)
    {
        _configuration = configuration;
        var client = new MongoClient(
            configuration.GetConnectionString("MongoDbConnection"));
        _database = client.GetDatabase(
            configuration["MongoDb:DatabaseName"]);
    }

    public IMongoCollection<T> GetCollection<T>(string name)
    {
        return _database.GetCollection<T>(name);
    }
}
```

### Configuration (appsettings.json)
```json
{
  "ConnectionStrings": {
    "MongoDbConnection": "mongodb://localhost:27017"
  },
  "MongoDb": {
    "DatabaseName": "cdr_db",
    "CollectionName": "incoming_calls"
  }
}
```

### Why Singleton?
- MongoDB.Driver client is thread-safe
- Connection pooling is internal
- Expensive initialization (DNS resolution, authentication)
- Single instance per app lifetime

---

## 📊 MongoDB Collections

### 1. incoming_calls (CDR Records)

```javascript
// Schema: mongo/incoming_calls_schema.js
{
  "_id": ObjectId,
  "dateTime": {
    "origination": ISODate,
    "connect": ISODate,
    "disconnect": ISODate
  },
  "duration": NumberLong,
  "callingParty": {
    "number": String,      // e.g., "80361234567"
    "displayName": String
  },
  "originalCalledParty": {
    "number": String,      // e.g., "80365555555"
    "displayName": String
  },
  "finalCalledParty": {
    "number": String
  },
  "operator": {
    "_id": ObjectId,
    "name": String,
    "extension": String
  },
  "department": {
    "_id": ObjectId,
    "name": String
  },
  "callResult": String,  // "answered", "missed", etc.
  // ... additional fields
}
```

**Constraints:**
- Global Filter: Only records with `callingParty.number` OR `originalCalledParty.number` OR `finalCalledParty.number` starting with "8036"

### 2. users (Operators)

```javascript
{
  "_id": ObjectId,
  "name": String,
  "extension": String,
  "email": String,
  "department": ObjectId  // Reference to departments
}
```

### 3. departments

```javascript
{
  "_id": ObjectId,
  "name": String,
  "manager": String
}
```

### 4. breaks

```javascript
{
  "_id": ObjectId,
  "operator": ObjectId,
  "startTime": ISODate,
  "endTime": ISODate,
  "type": String  // "lunch", "meeting", etc.
}
```

---

## 🔍 Query Patterns

### Global Filter Implementation

```csharp
// CdrRecordsRepository.cs
private FilterDefinition<CdrRecord> ApplyGlobalFilter()
{
    return Builders<CdrRecord>.Filter.Or(
        Builders<CdrRecord>.Filter.And(
            Builders<CdrRecord>.Filter.Ne(x => x.OriginalCalledParty, null),
            Builders<CdrRecord>.Filter.Regex(x => x.OriginalCalledParty!.Number, 
                new BsonRegularExpression("^8036.*"))
        ),
        Builders<CdrRecord>.Filter.And(
            Builders<CdrRecord>.Filter.Ne(x => x.CallingParty, null),
            Builders<CdrRecord>.Filter.Regex(x => x.CallingParty!.Number, 
                new BsonRegularExpression("^8036.*"))
        ),
        Builders<CdrRecord>.Filter.And(
            Builders<CdrRecord>.Filter.Ne(x => x.FinalCalledParty, null),
            Builders<CdrRecord>.Filter.Regex(x => x.FinalCalledParty!.Number, 
                new BsonRegularExpression("^8036.*"))
        )
    );
}
```

**⭐ CRITICAL**: This filter MUST be chained in every query!

### Simple Query Example

```csharp
public async Task<IEnumerable<CdrRecord>> GetByDateRangeAsync(
    DateTime startDate, DateTime endDate)
{
    var filter = Builders<CdrRecord>.Filter.And(
        ApplyGlobalFilter(),  // ⭐ Always include!
        Builders<CdrRecord>.Filter.Gte(x => x.DateTime!.Connect, startDate),
        Builders<CdrRecord>.Filter.Lte(x => x.DateTime!.Disconnect, endDate)
    );
    
    return await _collection.Find(filter).ToListAsync();
}
```

### Aggregation Pipeline Example

```csharp
public async Task<IEnumerable<WeeklyAnsweredCallRate>> GetWeeklyAnsweredCallsAsync(
    DateTime startDate, DateTime endDate)
{
    var filter = Builders<CdrRecord>.Filter.And(
        ApplyGlobalFilter(),
        Builders<CdrRecord>.Filter.Gte(x => x.DateTime!.Origination, startDate),
        Builders<CdrRecord>.Filter.Lte(x => x.DateTime!.Origination, endDate)
    );

    var aggregate = await _collection.Aggregate()
        .Match(filter)
        .Project(new BsonDocument
        {
            { "year", new BsonDocument("$year", "$dateTime.origination") },
            { "month", new BsonDocument("$month", "$dateTime.origination") },
            { "dayOfWeek", new BsonDocument("$dayOfWeek", "$dateTime.origination") },
            { "connectAndDuration", new BsonDocument("$cond", new BsonDocument
            {
                { "if", new BsonDocument("$and", new BsonArray
                {
                    new BsonDocument("$ne", new BsonArray { "$dateTime.connect", BsonNull.Value }),
                    new BsonDocument("$gt", new BsonArray { "$duration", 0 })
                }) },
                { "then", 1 },
                { "else", 0 }
            }))
        })
        .Group(new BsonDocument
        {
            { "_id", new BsonDocument
            {
                { "year", "$year" },
                { "month", "$month" },
                { "dayOfWeek", "$dayOfWeek" }
            }},
            { "totalRecords", new BsonDocument("$sum", 1) },
            { "answeredCalls", new BsonDocument("$sum", "$connectAndDuration") }
        })
        .ToListAsync();

    return _mapper.Map<IEnumerable<WeeklyAnsweredCallRate>>(aggregate);
}
```

---

## 🗄️ CdrContext (SQL Server)

### DbSets Definition

```csharp
public class CdrContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    public CdrContext(DbContextOptions<CdrContext> options) 
        : base(options) { }

    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<Report> Reports { get; set; }
    
    // Identity tables are inherited:
    // DbSet<User> (from IdentityDbContext)
    // DbSet<IdentityRole<Guid>>
    // DbSet<IdentityUserRole<Guid>>
}
```

### Migrations

```bash
# Add migration
dotnet ef migrations add InitialCreate

# Apply migration
dotnet ef database update

# Current migrations folder:
Migrations/
├── 20241007214722_InitIdentity.cs
├── 20241007214722_InitIdentity.Designer.cs
└── 20241007230746_Add_RefreshToken.cs
```

---

## 🔐 Security in Data Layer

### SQL Injection Prevention
- ✅ EF Core uses parameterized queries
- ✅ MongoDB.Driver uses BSON serialization (no string concatenation)

### Authorization at Repository Level
- ✅ Global Filter in CdrRecordsRepository
- ✅ Repository methods should not expose unfiltered data

### Sensitive Data Handling
- ❌ Passwords: Never loaded via repository (Identity handles this)
- ✅ Audit Logging: Consider logging data access in production

---

## 📈 Performance Considerations

### Indexing in MongoDB

```javascript
// Create indexes for frequently queried fields
db.incoming_calls.createIndex({ "dateTime.origination": 1 })
db.incoming_calls.createIndex({ "operator._id": 1 })
db.incoming_calls.createIndex({ "callingParty.number": 1 })
db.incoming_calls.createIndex({ "originalCalledParty.number": 1 })
```

### Query Optimization
- ✅ Pagination: Use `.Skip().Take()` for large result sets
- ✅ Projection: Select only needed fields if possible
- ✅ Async/Await: All database calls are asynchronous

### SQL Server Indexes
EF Core automatically creates indexes on:
- Primary keys
- Foreign keys
- Properties marked with `[Index]` attribute

---

## 🔄 Async Patterns

### All Database Operations Are Async

```csharp
// Correct
var records = await _repository.GetByDateRangeAsync(start, end);

// Incorrect - Blocks thread
var records = _repository.GetByDateRangeAsync(start, end).Result;
```

### Why Async?
- Non-blocking I/O
- Better resource utilization under load
- Recommended for web APIs

