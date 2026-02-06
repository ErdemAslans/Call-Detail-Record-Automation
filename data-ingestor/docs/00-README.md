# CDR.DataIngestor Dokümantasyon

## 📚 Dokümantasyon Haritası

Bu dokümantasyon context engineering için optimize edilmiştir. Her dosya bağımsız olarak kullanılabilir.

### Öğrenme Yolu (Sequential)
1. **[01-Overview.md](01-Overview.md)** - Proje amacı, veri akışı, mimarilerin
2. **[02-ProjectStructure.md](02-ProjectStructure.md)** - Klasör yapısı, modüller
3. **[03-Configuration.md](03-Configuration.md)** - config.yaml, environment setup
4. **[04-DataModels.md](04-DataModels.md)** - Pydantic models, validation
5. **[05-HelpersFunctions.md](05-HelpersFunctions.md)** - Logger, converters, utilities
6. **[06-ETLPipeline.md](06-ETLPipeline.md)** - CSV parsing, data flow, async processing
7. **[07-MongoDB.md](07-MongoDB.md)** - Collection schemas, indexing
8. **[08-MSSQL.md](08-MSSQL.md)** - SQL Server integration, stored procedures

### Modüler Erişim (By Topic)
- **CSV Processing**: 06-ETLPipeline.md + 04-DataModels.md
- **Validation**: 04-DataModels.md + 05-HelpersFunctions.md
- **Data Storage**: 07-MongoDB.md + 08-MSSQL.md
- **Configuration**: 03-Configuration.md
- **Debugging**: 05-HelpersFunctions.md (logging section)

### Hızlı Referanslar
- Data Models: 04-DataModels.md
- Logger Setup: 05-HelpersFunctions.md#logging
- MongoDB Schema: 07-MongoDB.md
- Running Ingestor: 06-ETLPipeline.md#running

---

## 🎯 Bu Dokümantasyon Neyi Kapsar?

✅ **Kapsanan Konular:**
- CSV-to-MongoDB ETL pipeline
- Pydantic data validation
- Async task processing
- Configuration management
- MongoDB schema & indexing
- SQL Server integration
- Logging & error handling
- Data quality checks

❌ **Kapsamayan Konular:**
- Detailed Pandas tutorials (kütüphane kullanılmıyor)
- Motor async library specifics (PyMongo blocking client kullanılıyor)
- Advanced MongoDB aggregation pipelines

---

## 💡 Context Engineering Tips

Bu dokümantasyon aşağıdaki amaçlarla kullanılabilir:

1. **Data Validation**: 04-DataModels.md + 05-HelpersFunctions.md'yi beraber kullanın
2. **Pipeline Understanding**: 06-ETLPipeline.md'nin full workflow'unu okuyun
3. **Schema Design**: 07-MongoDB.md'deki collection definitions'ı referans alın
4. **Debugging**: 05-HelpersFunctions.md (logging section) → console output'u kontrol edin
5. **Feature Addition**: İlgili modülün dokümantasyonunu, koda ekleme yapmanız gerekiyorsa başında okuyun

---

**İçindekiler Tablosu**: Aşağıdaki dosyaların herbiri bağımsız olarak okunabilir.
