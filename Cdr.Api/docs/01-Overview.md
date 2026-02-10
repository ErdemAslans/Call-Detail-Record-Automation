# Cdr.Api - Genel Bakış

**Last Updated**: January 2026  
**Framework**: .NET 8  
**Pattern**: REST API with Repository Pattern  

---

## 📌 Proje Amacı

**Cdr.Api**, Call Detail Record (CDR) verilerini analiz ve raporlama için sağlayan REST API'sidir. Telefon merkezi çağrı kayıtlarını MongoDB'den okuyarak, operatör ve departman bazlı raporlar oluşturur.

**Ana Sorumluluklar:**
- CDR verilerini filtreleme ve sorgulama
- Operatör & departman performans raporlaması
- Tekrarlayan raporları otomatik olarak oluşturma (Hangfire)
- Kullanıcı kimlik doğrulama & yetkilendirme
- Web frontend'e API endpoints sağlama

---

## 🔧 Tech Stack

| Katman | Teknoloji | Amaç |
|--------|-----------|------|
| **Runtime** | .NET 8 | Web API framework |
| **ORM (SQL)** | Entity Framework Core 8.0.8 | SQL Server veritabanı |
| **ORM (NoSQL)** | MongoDB.Driver 2.29.0 | MongoDB bağlantısı |
| **Authentication** | ASP.NET Identity + JWT Bearer | Kullanıcı auth |
| **Mapping** | AutoMapper 12.0.1 | DTO ↔ Entity dönüşümü |
| **Background Jobs** | Hangfire 1.8.17 | Scheduled reporting |
| **Documentation** | Swagger/OpenAPI | API docs |
| **Excel** | EPPlus 7.5.2 | Report generation |

---

## 💾 Veri Kaynakları

### MongoDB
```
┌─────────────────────────────────────────┐
│  MongoDB (Operational Data)             │
├─────────────────────────────────────────┤
│ • incoming_calls   (CDR records)        │
│ • users           (Operators)           │
│ • departments     (Department info)     │
│ • breaks          (Break records)       │
└─────────────────────────────────────────┘
```

**İçerik**: Yüksek hacimli CDR verisi (INSERT-heavy)  
**Kısıt**: Sadece read işlemleri API tarafından yapılır

### SQL Server
```
┌─────────────────────────────────────────┐
│  SQL Server (Identity & Config)         │
├─────────────────────────────────────────┤
│ • AspNetUsers          (User accounts)  │
│ • AspNetRoles          (Role definitions)|
│ • AspNetUserRoles      (Role mapping)   │
│ • AspNetRefreshTokens  (Token storage)  │
└─────────────────────────────────────────┘
```

**İçerik**: Kimlik yönetimi (User, Role)  
**Erişim**: Entity Framework Core via `CdrContext`

---

## 🏗️ Mimari Katmanlar

```
┌─────────────────────────────────────┐
│      Controllers (API Endpoints)     │
│  AccountController, ReportController │
└──────────────────┬──────────────────┘
                   │
┌──────────────────▼──────────────────┐
│      Services (Business Logic)       │
│ AccountService, CdrRecordsService    │
└──────────────────┬──────────────────┘
                   │
┌──────────────────▼──────────────────┐
│   Repositories (Data Access)         │
│ CdrRecordsRepository, etc.           │
└──────────────────┬──────────────────┘
                   │
┌──────────────────▼──────────────────┐
│    MongoDB/SQL Server Contexts       │
│ MongoDbContext, CdrContext           │
└─────────────────────────────────────┘
```

**Akış**: Request → Controller → Service → Repository → Context → Database

---

## 🔐 Güvenlik Modeli

### Authentication Flow
```
1. User → POST /api/account/login (email, password)
   ↓
2. AccountService: Password verification (via Identity)
   ↓
3. TokenService: Generate JWT token
   ↓
4. Response: { token, refreshToken }
```

### Authorization
- **JWT Bearer Token**: Her request header'da `Authorization: Bearer <token>`
- **Role-Based Access**: Controller actions'lar `[Authorize(Roles = "...")]` ile korunur
- **Global Filter**: Tüm CDR queries "8036" ile başlayan numaraları filtreler

---

## 🔄 İş Akışları

### 1. Raporlama Akışı
```
Hangfire Scheduled Job
    ↓ (Daily/Weekly/Monthly)
HangfireJobs.cs (CdrRecordsRepository.GetWeeklyAnsweredCalls() vs)
    ↓ (Aggregation)
Report DTO objects
    ↓ (AutoMapper)
Excel/JSON Response
```

### 2. Operatör Performans Sorgusu
```
ReportController.GetOperatorStats()
    ↓
CdrRecordsService
    ↓
CdrRecordsRepository.ApplyGlobalFilter() + Custom aggregation
    ↓
MongoDB Aggregation Pipeline
    ↓
Response DTO
```

---

## 🚀 Başlangıç

### Ön Koşullar
- .NET 8 SDK
- MongoDB instance (bağlantı string: appsettings.Development.json)
- SQL Server (Identity için)

### Çalıştırma
```bash
cd Cdr.Api
dotnet run
# API: https://localhost:5001
# Swagger: https://localhost:5001/swagger
# Hangfire Dashboard: https://localhost:5001/hangfire
```

### Konfigürasyon (appsettings.json)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=...;Database=Cdr;...",
    "MongoDbConnection": "mongodb://..."
  },
  "MongoDb": {
    "DatabaseName": "cdr_db",
    "CollectionName": "incoming_calls"
  },
  "JwtConfig": {
    "Issuer": "...",
    "Audience": "...",
    "Key": "...",
    "ExpiresInMinutes": 60
  }
}
```

---

## 📚 Dokümantasyon Haritası

Derinlemesine öğrenme için:
- **Architecture**: [03-Architecture.md](03-Architecture.md)
- **Authentication**: [04-Authentication.md](04-Authentication.md)
- **Data Layer**: [05-DataLayer.md](05-DataLayer.md)
- **Services**: [06-Services.md](06-Services.md)
- **Background Jobs**: [07-Hangfire.md](07-Hangfire.md)
- **API Endpoints**: [08-APIEndpoints.md](08-APIEndpoints.md)

---

## 💡 Key Concepts

| Konsept | Açıklama |
|---------|----------|
| **Repository Pattern** | Data access logic'i encapsulate eder |
| **Global Filter** | CDR queries'e otomatik "8036" filtresi uygular |
| **AutoMapper Profiles** | Entity-to-DTO conversions tanımlar |
| **Hangfire Jobs** | Scheduled background processing |
| **JWT Tokens** | Stateless authentication |

---

## ⚠️ Security Considerations

- ✅ **Password Hashing**: Identity framework (PBKDF2)
- ✅ **JWT Validation**: Token signature, expiration checked
- ✅ **HTTPS Enforced**: `app.UseHttpsRedirection()`
- ✅ **CORS Restricted**: Specific origin whitelist (production domain)
- ✅ **SQL Injection Protected**: Parameterized queries via EF Core & MongoDB.Driver
- ⚠️ **Note**: Bkz. secure-coding-owasp.instructions.md for OWASP compliance details

