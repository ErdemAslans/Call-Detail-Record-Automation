# CDR.DataIngestor Project Structure

**Last Updated**: January 2026  

---

## 📁 Complete Project Layout

```
CDR.DataIngestor/
├── docs/                    # 📚 DOCUMENTATION
│   ├── 00-README.md         # Navigation guide
│   ├── 01-Overview.md       # Project overview
│   └── 03-Configuration.md  # Configuration management
│
├── data/                    # 📥 INPUT DATA (CSV FILES)
│   ├── calls_2024_01.csv
│   ├── calls_2024_02.csv
│   └── ...
│
├── mongo/                   # 🗄️ MONGODB SETUP
│   ├── incoming_calls_schema.js  # Collection schema
│   └── users/
│
├── src/                     # 🐍 PYTHON SOURCE CODE
│   ├── __init__.py
│   ├── config.yaml          # Configuration file ⭐
│   ├── create_collection.py # MongoDB collection initialization
│   ├── departments.py       # Department data handling
│   ├── main.py              # Main entry point
│   ├── mssql_handler.py     # SQL Server integration
│   ├── test.py              # Testing/development script
│   ├── users.py             # User/operator mapping
│   ├── utils.py             # Utility functions
│   │
│   ├── helpers/             # Helper modules
│   │   ├── __init__.py
│   │   ├── config.py        # Configuration loader
│   │   ├── converters.py    # CSV → Pydantic parsing ⭐
│   │   └── logger.py        # Logging setup
│   │
│   └── models/              # Pydantic data models
│       ├── __init__.py
│       ├── cdrModel.py      # Main CDR model
│       ├── cdrSubModels.py  # Sub-models (DateTime, Party, etc.)
│       ├── incomingCalls.py # Incoming call specific model
│       └── sql/             # SQL Server models
│
├── requirements.txt         # Python dependencies
└── README.md               # Project readme
```

---

## 🎯 Where to Find Things

### I need to...

#### **Understand the data flow**
- Start: [01-Overview.md](docs/01-Overview.md)
- Diagram: Shows CSV → Validation → MongoDB flow

#### **Configure MongoDB or MSSQL**
- Go to: [03-Configuration.md](docs/03-Configuration.md)
- File: `src/config.yaml`

#### **Add a new CSV field**
- Go to: `src/models/` (define new field in Pydantic model)
- Go to: `src/helpers/converters.py` (add parsing logic)
- Return: Field will auto-validate and insert

#### **Fix validation errors**
- Check: `src/models/cdrModel.py` and `cdrSubModels.py`
- Check: `src/helpers/converters.py` for parsing logic
- View: Logs in MongoDB `logs` collection

#### **Debug data issues**
- Check: `src/helpers/logger.py` configuration
- Check: MongoDB `logs` collection for error details
- Run: `python src/test.py` for manual testing

#### **Run the ingestor**
```bash
python src/main.py
```
- Processes data/ folder every 1 hour
- Inserts into MongoDB `incoming_calls` collection

---

## 🔑 Key Files by Responsibility

### Entry Point
- `src/main.py` - Application entry, periodic task loop

### Configuration
- `src/config.yaml` - MongoDB & MSSQL connection settings
- `src/helpers/config.py` - YAML loader

### Data Models
- `src/models/cdrModel.py` - Main CDR record model
- `src/models/cdrSubModels.py` - Sub-models (DateTime, Party details)
- `src/models/incomingCalls.py` - Incoming call specific

### Data Processing (ETL)
- `src/utils.py` - MongoDB operations, insert logic
- `src/helpers/converters.py` - CSV row → Pydantic model parsing ⭐

### Database
- `src/create_collection.py` - MongoDB collection setup
- `src/mssql_handler.py` - SQL Server integration (future)

### Support
- `src/helpers/logger.py` - Logging to MongoDB `logs`
- `src/users.py` - Operator mapping & phone number extraction
- `src/departments.py` - Department data handling

---

## 🔄 Data Processing Flow

### Step 1: File Discovery (main.py)
```python
process_files_in_directory(directory_path)
```
- Walks `data/` folder recursively
- Finds all CSV files (excluding .DS_Store)

### Step 2: CSV Reading (main.py)
```python
for filename in files:
    with open(file_path) as file:
        csv_reader = csv.DictReader(file)
```
- Reads CSV with header row
- Creates dict for each row

### Step 3: Validation & Parsing (utils.py → converters.py)
```python
record = parse_csv_to_model(row, users_collection)
```
- **converters.py**: Transforms CSV dict → Pydantic model
- Validates data types, formats
- Maps phone numbers to operators
- Normalizes dates

### Step 4: MongoDB Insert (utils.py)
```python
collection.insert_one(record.model_dump())
```
- Inserts validated data to MongoDB
- Errors logged but don't stop processing

### Step 5: Logging (helpers/logger.py)
```python
logger.error(f"Validation error for row {row}: {e}")
```
- All errors written to MongoDB `logs` collection

### Step 6: Sleep & Repeat (main.py)
```python
await asyncio.sleep(3600)  # 1 hour
```
- Waits 1 hour, then repeats

---

## 📦 Dependencies Overview

### Python Packages
```
pymongo          ← MongoDB client
pydantic         ← Data validation
pyyaml          ← YAML configuration
motor           ← Async MongoDB (imported but not used)
pyodbc          ← ODBC driver for MSSQL
sqlalchemy      ← SQL abstraction
```

**Install:**
```bash
pip install -r requirements.txt
```

---

## 🔐 Security Considerations

### ⚠️ Current Issues
- 🔴 Credentials in config.yaml (visible in git)
- 🔴 No data encryption at rest

### ✅ Good Practices
- ✅ Input validation (Pydantic models)
- ✅ Error handling (try-except blocks)
- ✅ Logging of all failures

### 🔒 Recommendations
1. Move credentials to environment variables
2. Use `.env` file (add to .gitignore)
3. Use MongoDB authentication
4. Enable MongoDB encryption

---

## 🧪 Testing & Debugging

### Manual Testing
```bash
python src/test.py
```
- Test individual CSV parsing
- Verify Pydantic validation
- Debug field mapping

### Check Logs
```python
# MongoDB logs collection
db['logs'].find().sort('_id', -1).limit(10)
```
- View last 10 errors
- Check validation failures

### Verify Data
```python
# Check inserted records
db['incoming_calls'].count_documents({})  # Total count
db['incoming_calls'].find_one()  # Sample record
```

---

## 📝 Adding a New Field

### If you have a new CSV column:

1. **Update Pydantic Model**
   ```python
   # src/models/cdrSubModels.py or cdrModel.py
   class CdrRecord(BaseModel):
       newField: str | None = None  # Optional[str]
   ```

2. **Add Parsing Logic**
   ```python
   # src/helpers/converters.py
   def parse_csv_to_model(row, users_collection):
       new_field = row.get('NewColumnName', '')  # Extract from CSV
       # ... parsing logic ...
       return CdrModel(
           # ... other fields ...
           newField=new_field  # Add field
       )
   ```

3. **Test**
   ```bash
   python src/test.py
   ```

4. **Run**
   ```bash
   python src/main.py
   ```

---

## 🗄️ MongoDB Collections

### incoming_calls (CDR Records)
```javascript
{
  "_id": ObjectId,
  "dateTime": { origination, connect, disconnect },
  "duration": NumberLong,
  "callingParty": { number, displayName },
  "originalCalledParty": { number, displayName },
  // ... more fields
}
```

### logs (Error Logging)
```javascript
{
  "_id": ObjectId,
  "message": String,
  "timestamp": ISODate,
  "level": "error|warning|info"
}
```

### users (Operators)
```javascript
{
  "_id": ObjectId,
  "name": String,
  "extension": String
}
```

---

## 💡 Key Concepts

| Concept | Explanation |
|---------|------------|
| **Pydantic** | Validates data types & formats at runtime |
| **ETL** | Extract (CSV) → Transform (validate) → Load (MongoDB) |
| **Async** | asyncio.gather() runs inserts in parallel |
| **Optional** | Fields can be None if missing from CSV |
| **Converters** | All parsing logic in converters.py |

---

## 🛠️ Common Tasks

### View all errors
```bash
# List all logs
db['logs'].find().pretty()

# Count errors
db['logs'].count_documents({"level": "error"})
```

### Reprocess failed records
```bash
# Delete and re-ingest
db['incoming_calls'].delete_many({})
python src/main.py  # Run again
```

### Check data quality
```bash
# Find records with null fields
db['incoming_calls'].find({"duration": null})

# Find invalid phone numbers
db['incoming_calls'].find({"callingParty.number": {$not: /^8036/}})
```

---

## 📚 Full Documentation Index

| Topic | File |
|-------|------|
| Overview | [01-Overview.md](docs/01-Overview.md) |
| Configuration | [03-Configuration.md](docs/03-Configuration.md) |

---

## 🚀 Quick Start

```bash
# Setup
cd CDR.DataIngestor
pip install -r requirements.txt

# Configure
# Edit src/config.yaml with MongoDB connection

# Run
python src/main.py

# Monitor
# Check MongoDB logs collection for errors
```

