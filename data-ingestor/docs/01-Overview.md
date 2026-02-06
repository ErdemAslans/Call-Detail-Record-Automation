# CDR.DataIngestor - Genel Bakış

**Last Updated**: January 2026  
**Language**: Python 3.9+  
**Pattern**: ETL (Extract-Transform-Load)  

---

## 📌 Proje Amacı

**CDR.DataIngestor**, telefon merkezi CSVlerini parse ederek MongoDB ve SQL Server'a yükleyen **ETL servisidir**.

**Ana Sorumluluklar:**
- CSV dosyalarını rekursif tarama (data/ klasörü)
- Veri validasyonu (Pydantic models)
- Phone number normalization & operator/department mapping
- Asynchronous MongoDB insertion
- SQL Server'a veri yükleme (future: şu anda comment'li)
- Hata logging ve tracking

---

## 🔄 Veri Akışı

```
CSV Files (data/)
    │
    ▼
CSV Reader (csv.DictReader)
    │
    ▼
parse_csv_to_model() [converters.py]
    ├─ Row validation (Pydantic)
    ├─ Phone number parsing
    ├─ Operator/Department mapping
    └─ DateTime normalization
    │
    ▼
Pydantic Model (CdrModel)
    │
    ▼
insert_to_mongo() [utils.py]
    │
    ├─ MongoDB collection.insert_one()
    │
    └─ Error logging (ValidationError, Exception)
    │
    ▼
MongoDB incoming_calls collection
    │
    └─ Indexed by date, operator, caller numbers
```

### Parallel Execution
```
main.py (periodic_task)
    │
    ├─ Process File 1 ─────┐
    ├─ Process File 2 ─────┼─── asyncio.gather() ─── All inserted simultaneously
    ├─ Process File 3 ─────┤
    └─ Process File N ─────┘
    │
    ▼
Sleep 3600 seconds (1 hour)
    │
    └─ Repeat
```

---

## 🏗️ Mimari Katmanlar

```
┌─────────────────────────────────┐
│   main.py (Main Loop)           │
│ • periodic_task (1 hour cycle)  │
│ • process_files_in_directory()  │
└─────────────┬───────────────────┘
              │
┌─────────────▼───────────────────┐
│   Data Processing               │
│ • utils.py (insert_to_mongo)   │
│ • helpers/converters.py (parsing) │
└─────────────┬───────────────────┘
              │
┌─────────────▼───────────────────┐
│   Data Validation               │
│ • models/ (Pydantic classes)    │
│ • CdrModel, CdrSubModels        │
└─────────────┬───────────────────┘
              │
┌─────────────▼───────────────────┐
│   External Services             │
│ • MongoDB (incoming_calls)      │
│ • SQL Server (future)           │
│ • Logger                        │
└─────────────────────────────────┘
```

---

## 🔧 Tech Stack

| Component | Teknoloji | Amaç |
|-----------|-----------|------|
| **Runtime** | Python 3.9+ | Script language |
| **Validation** | Pydantic | Type checking, data validation |
| **MongoDB** | PyMongo | Sync MongoDB client |
| **SQL Server** | PyODBC + SQLAlchemy | MSSQL async operations |
| **Configuration** | PyYAML | config.yaml parsing |
| **Async** | asyncio | Async task processing |
| **Logging** | Python logging | Error & info tracking |

---

## 💾 Veri Kaynakları

### Input: CSV Files
```
data/
├── calls_2024_01.csv
├── calls_2024_02.csv
└── calls_2024_03.csv

Columns (örnek):
| DateTime | CallingParty | OriginalCalledParty | Duration | ... |
|----------|--------------|-------------------|----------|-----|
| 2024-01-01T10:30:45 | 80361234567 | 80365555555 | 120 | ... |
```

### Output: MongoDB
```
Database: cdr
Collections:
  ├─ incoming_calls   (CDR records)
  ├─ users           (Operators)
  ├─ departments     (Department info)
  ├─ logs            (Error logs)
  └─ breaks          (Break records)
```

### Optional: SQL Server
```
Database: CDR
Tables:
  ├─ IncomingCalls    (CDR records)
  ├─ Calls            (Call details)
  └─ Users           (Operator mapping)
```

---

## 🚀 Başlangıç

### Ön Koşullar
- Python 3.9+
- MongoDB running (default: localhost:27017)
- SQL Server (optional, for future integration)

### Kurulum
```bash
cd CDR.DataIngestor

# Create virtual environment
python -m venv venv
source venv/bin/activate  # macOS/Linux
# or: venv\Scripts\activate  # Windows

# Install dependencies
pip install -r requirements.txt
```

### Çalıştırma
```bash
python src/main.py
```

**Output:**
```
INFO:root:All files have been successfully saved to MongoDB.
All files have been successfully saved to MongoDB.
[Sleeps 3600 seconds, then repeats]
```

### Konfigürasyon (config.yaml)
```yaml
mongo:
  uri: "mongodb://localhost:27017"
  database: "cdr"
  collection: "incoming_calls"
  log_collection: "logs"
  user_collection: "users"

mssql:
  server: "localhost,1433"
  database: "CDR"
  user: "sa"
  password: "Sa-252Wer"  # ⚠️ Externalize this for production!
```

---

## 🔐 Güvenlik Modeli

### Input Validation
- ✅ Pydantic schemas enforce type checking
- ✅ CSV parsing with error handling
- ✅ Invalid rows logged, not inserted

### Data Protection
- ✅ MongoDB authentication (if enabled)
- ✅ SQL Server credentials in config.yaml (⚠️ should use env vars)
- ⚠️ No encryption for data at rest (MongoDB default)

### Error Handling
- ✅ ValidationError caught and logged
- ✅ Insert failures don't stop process
- ✅ All errors written to mongo.logs collection

---

## 📚 Dokümantasyon Haritası

Derinlemesine öğrenme için:
- **Configuration**: [03-Configuration.md](03-Configuration.md)
- **Data Models**: [04-DataModels.md](04-DataModels.md)
- **Helpers & Utilities**: [05-HelpersFunctions.md](05-HelpersFunctions.md)
- **ETL Pipeline**: [06-ETLPipeline.md](06-ETLPipeline.md)
- **MongoDB Integration**: [07-MongoDB.md](07-MongoDB.md)
- **SQL Server Integration**: [08-MSSQL.md](08-MSSQL.md)

---

## 💡 Key Concepts

| Konsept | Açıklama |
|---------|----------|
| **Pydantic Validation** | Runtime type checking + schema validation |
| **Async Processing** | asyncio.gather() parallelizes inserts |
| **Periodic Task** | 1-hour loop for continuous processing |
| **ETL** | Extract (CSV) → Transform (validate) → Load (MongoDB) |
| **Logging** | All errors/info written to mongo.logs |

---

## ⚠️ Security Considerations

- 🔴 **CRITICAL**: Credentials in config.yaml (use environment variables in production)
- 🟡 **WARNING**: No data encryption at rest (MongoDB)
- 🟢 **GOOD**: Input validation (Pydantic)
- 🟢 **GOOD**: Error handling (try-except blocks)

---

## 🔄 Tipik İş Akışı

```
1. Startup: python src/main.py
   ↓
2. Create collection_if_not_exists() [MongoDB setup]
   ↓
3. periodic_task() loop:
   │
   ├─ Walk directory data/
   ├─ For each CSV file:
   │  ├─ Read csv.DictReader
   │  ├─ For each row:
   │  │  ├─ parse_csv_to_model() [validation]
   │  │  ├─ insert_to_mongo() [async insert]
   │  │  └─ Log any errors
   │  └─ (All inserts run in parallel via asyncio.gather)
   ├─ Log "All files saved successfully"
   └─ Sleep 3600 seconds (1 hour)
   │
4. Repeat from step 3
```

