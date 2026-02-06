# Call Detail Record Automation

Enterprise-grade CDR (Call Detail Record) management platform. Automates the ingestion, processing, analysis, and reporting of telecom call records from Cisco CUCM systems.

## Architecture

```
Cisco CUCM ──FTP──> CSV Files ──> Data Ingestor ──> MongoDB ──> REST API ──> Web Dashboard
```

| Component | Tech Stack | Description |
|-----------|-----------|-------------|
| **backend/** | .NET 8, MongoDB, SQL Server | REST API - Authentication, reporting, Excel export, email |
| **frontend/** | Vue.js 3, TypeScript, Vite | Web Dashboard - Real-time analytics and call reports |
| **data-ingestor/** | Python 3.11, Pydantic, Motor | ETL Service - CSV parsing, validation, batch MongoDB insert |

## Quick Start

### Backend (.NET API)
```bash
cd backend
dotnet restore
dotnet run
```

### Frontend (Vue.js)
```bash
cd frontend
npm install
npm run dev
```

### Data Ingestor (Python)
```bash
cd data-ingestor
pip install -r requirements.txt
python src/main.py
```

## CI/CD

Automated via GitHub Actions with self-hosted runner:

- **api.yml** - Backend build, test, deploy to IIS
- **web.yml** - Frontend lint, build, deploy static files
- **ingestor.yml** - Python validation, service restart

Each workflow triggers only when its respective directory changes.

## Author

**Erdem Aslan** - [GitHub](https://github.com/ErdemAslans)
