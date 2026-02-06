# Configuration Management

**Last Updated**: January 2026  
**File**: config.yaml  

---

## 📋 Configuration Overview

CDR.DataIngestor uses **config.yaml** for centralized configuration.

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
  password: "Sa-252Wer"
```

---

## 🔧 MongoDB Configuration

### URI Format
```yaml
uri: "mongodb://[username:password@]host[:port][/database]"
```

### Examples
```yaml
# Local development
uri: "mongodb://localhost:27017"

# With authentication
uri: "mongodb://user:password@mongodb.example.com:27017"

# Replica set
uri: "mongodb://host1,host2,host3/?replicaSet=rs0"

# Atlas cloud
uri: "mongodb+srv://user:password@cluster.mongodb.net/database"
```

### Collections
| Collection | Purpose |
|-----------|---------|
| `incoming_calls` | CDR records |
| `users` | Operator information |
| `departments` | Department mappings |
| `logs` | Error/info logging |
| `breaks` | Break periods |

---

## 🗄️ MSSQL Configuration

### Connection String
```yaml
mssql:
  server: "localhost,1433"        # Host,Port
  database: "CDR"                 # Database name
  user: "sa"                      # Username
  password: "Sa-252Wer"           # Password
```

### Format Examples
```yaml
# Local development
server: "localhost,1433"

# Named instance
server: "SERVERNAME\INSTANCENAME"

# Remote server
server: "192.168.1.100,1433"

# Azure SQL
server: "servername.database.windows.net,1433"
```

---

## 📂 Loading Configuration

### Code: helpers/config.py

```python
import yaml
from pathlib import Path

def load_config():
    """Load configuration from config.yaml"""
    config_path = Path(__file__).parent.parent / 'config.yaml'
    
    with open(config_path, 'r') as f:
        config = yaml.safe_load(f)
    
    return config
```

### Usage

```python
from helpers.config import load_config

config = load_config()

# Access MongoDB config
mongo_uri = config['mongo']['uri']
mongo_db = config['mongo']['database']

# Access MSSQL config
mssql_server = config['mssql']['server']
mssql_password = config['mssql']['password']
```

---

## ⚠️ Security Best Practices

### ❌ DON'T (Current Implementation)
```yaml
# config.yaml (version control)
mssql:
  password: "Sa-252Wer"  # 🔴 EXPOSED!
```

### ✅ DO (Production)

#### Option 1: Environment Variables
```yaml
# config.yaml
mssql:
  server: "localhost,1433"
  database: "CDR"
  user: ${MSSQL_USER}        # Load from env
  password: ${MSSQL_PASSWORD}
```

#### Option 2: Separate secrets file (not in git)
```bash
# .gitignore
secrets.yaml

# secrets.yaml (local only)
mssql:
  user: "sa"
  password: "Sa-252Wer"
```

#### Option 3: Environment variables (recommended)
```bash
# Export before running
export MSSQL_USER=sa
export MSSQL_PASSWORD=Sa-252Wer
export MONGO_URI=mongodb://localhost:27017

python src/main.py
```

Then in code:
```python
import os
from helpers.config import load_config

config = load_config()
config['mssql']['password'] = os.getenv('MSSQL_PASSWORD')
config['mongo']['uri'] = os.getenv('MONGO_URI', config['mongo']['uri'])
```

---

## 🔄 Configuration at Runtime

### Modifying Config Programmatically

```python
from helpers.config import load_config

config = load_config()

# Override from environment
if os.getenv('MONGO_URI'):
    config['mongo']['uri'] = os.getenv('MONGO_URI')

# Use modified config
client = MongoClient(config['mongo']['uri'])
```

### Multiple Environments

```
config/
├── config.local.yaml      # Development
├── config.staging.yaml    # Staging
└── config.production.yaml # Production
```

Then load based on environment:
```python
import os

env = os.getenv('APP_ENV', 'local')
config_path = f'config/config.{env}.yaml'

with open(config_path) as f:
    config = yaml.safe_load(f)
```

---

## 📝 Configuration Validation

### Schema Validation with Pydantic

```python
from pydantic import BaseModel
from typing import Optional

class MongoConfig(BaseModel):
    uri: str
    database: str
    collection: str
    log_collection: str
    user_collection: str

class MSSQLConfig(BaseModel):
    server: str
    database: str
    user: str
    password: str

class AppConfig(BaseModel):
    mongo: MongoConfig
    mssql: MSSQLConfig

# Load and validate
config_dict = load_config()
config = AppConfig(**config_dict)  # Raises ValueError if invalid
```

---

## 🚀 Running with Custom Config

```bash
# Use environment variable
export CONFIG_PATH=/path/to/config.yaml
python src/main.py

# Or programmatically
import os
os.environ['CONFIG_PATH'] = '/etc/cdr/config.yaml'
```

---

## 🔍 Troubleshooting Configuration

### Error: "config.yaml not found"
```
Fix: Ensure config.yaml is in src/ directory
src/
├── main.py
├── config.yaml  ← Should be here
└── helpers/
```

### Error: "Invalid YAML syntax"
```bash
# Validate YAML
python -m yaml config.yaml

# Or use online validator
# https://www.yamllint.com/
```

### Error: "Connection refused (MongoDB)"
```yaml
# Check URI format
uri: "mongodb://localhost:27017"  # ✓ Correct
uri: "mongo://localhost:27017"    # ✗ Wrong scheme

# Verify MongoDB is running
mongosh  # Should connect if running
```

---

## 📊 Configuration Reference

| Key | Type | Required | Default | Example |
|-----|------|----------|---------|---------|
| `mongo.uri` | String | Yes | - | `mongodb://localhost:27017` |
| `mongo.database` | String | Yes | - | `cdr` |
| `mongo.collection` | String | Yes | - | `incoming_calls` |
| `mssql.server` | String | No | - | `localhost,1433` |
| `mssql.database` | String | No | - | `CDR` |
| `mssql.user` | String | No | - | `sa` |
| `mssql.password` | String | No | - | `SecurePass123` |

