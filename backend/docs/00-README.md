# Cdr.Api Dokümantasyon

## 📚 Dokümantasyon Haritası

Bu dokümantasyon context engineering için optimize edilmiştir. Her dosya bağımsız olarak kullanılabilir.

### Öğrenme Yolu (Sequential)
1. **[01-Overview.md](01-Overview.md)** - Proje amacı, stack, temel kavramlar
2. **[02-ProjectStructure.md](02-ProjectStructure.md)** - Klasör yapısı, namespaces
3. **[03-Architecture.md](03-Architecture.md)** - Design patterns, katmanlı mimari
4. **[04-Authentication.md](04-Authentication.md)** - JWT, Identity, Authorization
5. **[05-DataLayer.md](05-DataLayer.md)** - MongoDB, SQL Server, repositories
6. **[06-Services.md](06-Services.md)** - Business logic, service layer
7. **[07-Hangfire.md](07-Hangfire.md)** - Background jobs, scheduling
8. **[08-APIEndpoints.md](08-APIEndpoints.md)** - Controller endpoints, request/response

### Modüler Erişim (By Topic)
- **Data Access**: 05-DataLayer.md + 03-Architecture.md
- **Authentication**: 04-Authentication.md
- **Background Processing**: 07-Hangfire.md
- **API Development**: 08-APIEndpoints.md + 06-Services.md
- **Configuration**: 03-Architecture.md (Startup section)

### Hızlı Referanslar
- Security & OWASP: Bkz. her dosyanın "Security Considerations" bölümü
- Configuration: appsettings.json + [03-Architecture.md](03-Architecture.md)
- Database Migrations: Migrations/ klasörü

---

## 🎯 Bu Dokümantasyon Neyi Kapsar?

✅ **Kapsanan Konular:**
- .NET 8 Web API mimarisi (DI, Middleware)
- Repository Pattern & Data Access abstraction
- MongoDB integration (Async operations, filtering)
- SQL Server (Identity, EF Core)
- JWT authentication & role-based authorization
- Hangfire background job processing
- AutoMapper profiling
- Error handling & logging

❌ **Kapsamayan Konular:**
- Detaylı Hangfire setup (launchSettings.json referans)
- Endpoint-by-endpoint HTTP documentation (Swagger docs için bakın)
- Entity Framework Core tutorials (EF Core docs referans)

---

## 🔄 Dokümantasyonu Güncel Tutma

Her dosya şu bilgileri barındırır:
- **Last Updated**: Dosya son düzenlenme tarihi
- **Version**: Uygulama versiyonu (şu anda .NET 8, MongoDB 2.29.0)

Kodda yapılan değişiklikler sonra ilgili dokümantasyon bölümünü update edin.

---

## 💡 Context Engineering Tips

Bu dokümantasyon aşağıdaki amaçlarla kullanılabilir:

1. **Kod Generation**: Belirli dosyaları Claude'a besleyerek kod örneği oluşturun
2. **Architecture Understanding**: 03-Architecture.md'yi başında okuyun
3. **Onboarding**: 01-Overview → 02-ProjectStructure → 03-Architecture sırasını izleyin
4. **Bug Fixing**: İlgili katmanın dokümantasyonunu kontekst olarak kullanın
5. **Feature Development**: 06-Services + 08-APIEndpoints kombinasyonu

---

**İçindekiler Tablosu**: Aşağıdaki dosyaların herbiri bağımsız olarak okunabilir.
