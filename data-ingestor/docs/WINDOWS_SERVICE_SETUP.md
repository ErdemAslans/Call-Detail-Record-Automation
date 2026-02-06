# CDR DataIngestor - Windows Service Kurulum Rehberi

Bu rehber, CDR DataIngestor uygulamasını Windows Server'da otomatik başlayan bir Windows Service olarak kurmanızı sağlar.

## Ön Gereksinimler

1. **Python 3.10+** kurulu olmalı
2. **Administrator** yetkisi
3. **MongoDB** çalışır durumda (`mongodb://localhost:27017`)
4. Uygulama dizini: `C:\CDR.DataIngestor`

## Hızlı Kurulum

### 1. Uygulamayı Sunucuya Kopyala

```powershell
# Projeyi C:\CDR.DataIngestor dizinine kopyalayın
xcopy /E /I "\\network\share\CDR.DataIngestor" "C:\CDR.DataIngestor"
```

### 2. Python Bağımlılıklarını Kur

```powershell
cd C:\CDR.DataIngestor
pip install -r requirements.txt
```

### 3. Service Kurulum Script'ini Çalıştır

**PowerShell'i Administrator olarak açın:**

```powershell
# Default ayarlarla kurulum
.\scripts\install_service.ps1

# Veya özel parametrelerle
.\scripts\install_service.ps1 `
    -PythonPath "C:\Python310\python.exe" `
    -AppPath "C:\CDR.DataIngestor\src\main.py" `
    -SourceDir "E:\CDR" `
    -ProcessedDir "E:\CDR_Processed" `
    -LogDir "E:\CDR_Logs"
```

## Manuel Kurulum (Script Kullanmadan)

### 1. NSSM İndir

```powershell
# https://nssm.cc/download adresinden indir
# C:\tools\nssm dizinine çıkar
```

### 2. Service Kaydet

```powershell
C:\tools\nssm\nssm.exe install CDRIngestor "C:\Python310\python.exe" "C:\CDR.DataIngestor\src\main.py"
C:\tools\nssm\nssm.exe set CDRIngestor AppDirectory "C:\CDR.DataIngestor\src"
C:\tools\nssm\nssm.exe set CDRIngestor Start SERVICE_AUTO_START
C:\tools\nssm\nssm.exe set CDRIngestor AppRestartDelay 5000
C:\tools\nssm\nssm.exe set CDRIngestor AppStdout "E:\CDR_Logs\stdout.log"
C:\tools\nssm\nssm.exe set CDRIngestor AppStderr "E:\CDR_Logs\stderr.log"
```

### 3. Service Başlat

```powershell
C:\tools\nssm\nssm.exe start CDRIngestor
```

## Service Yönetimi

### Durum Kontrolü

```powershell
Get-Service CDRIngestor
# veya
C:\tools\nssm\nssm.exe status CDRIngestor
```

### Başlat / Durdur

```powershell
# PowerShell
Start-Service CDRIngestor
Stop-Service CDRIngestor

# NSSM
C:\tools\nssm\nssm.exe start CDRIngestor
C:\tools\nssm\nssm.exe stop CDRIngestor
```

### GUI ile Düzenleme

```powershell
C:\tools\nssm\nssm.exe edit CDRIngestor
```

### Service Kaldırma

```powershell
.\scripts\uninstall_service.ps1
# veya
C:\tools\nssm\nssm.exe remove CDRIngestor confirm
```

## Log Dosyaları

| Log | Konum | Açıklama |
|-----|-------|----------|
| stdout | `E:\CDR_Logs\stdout.log` | NSSM stdout çıktısı |
| stderr | `E:\CDR_Logs\stderr.log` | NSSM stderr çıktısı |
| Application | `E:\CDR_Logs\cdr_ingestor.log` | Uygulama logları |
| MongoDB | `cdr.logs` collection | Veritabanı logları |

## Sorun Giderme

### Service Başlamıyor

1. **Log dosyalarını kontrol edin:**
   ```powershell
   Get-Content E:\CDR_Logs\stderr.log -Tail 50
   ```

2. **Python path doğru mu?**
   ```powershell
   C:\Python310\python.exe --version
   ```

3. **MongoDB çalışıyor mu?**
   ```powershell
   Test-NetConnection -ComputerName localhost -Port 27017
   ```

### Restart Döngüsü

Service sürekli yeniden başlıyorsa:

1. NSSM Event Log kontrol:
   ```powershell
   Get-EventLog -LogName Application -Source nssm -Newest 20
   ```

2. Uygulama loglarını incele:
   ```powershell
   Get-Content E:\CDR_Logs\cdr_ingestor.log -Tail 100
   ```

### Dizin İzin Hatası

```powershell
# SYSTEM kullanıcısına tam yetki ver
icacls "E:\CDR" /grant "SYSTEM:F" /T
icacls "E:\CDR_Processed" /grant "SYSTEM:F" /T
icacls "E:\CDR_Logs" /grant "SYSTEM:F" /T
```

## Konfigürasyon

### Ortam Değişkenleri (Opsiyonel)

NSSM ile environment variable set etmek için:

```powershell
C:\tools\nssm\nssm.exe set CDRIngestor AppEnvironmentExtra "MONGO_URI=mongodb://remote-server:27017"
```

### Config Dosyası

`C:\CDR.DataIngestor\src\config.yaml` dosyasını düzenleyin:

```yaml
mongo:
  uri: "mongodb://localhost:27017"
  database: "cdr"
  collection: "incoming_calls"
  log_collection: "logs"
  user_collection: "users"
```

## Yedekleme

Service ayarlarını yedeklemek için:

```powershell
# Export
reg export "HKLM\SYSTEM\CurrentControlSet\Services\CDRIngestor" "C:\backup\CDRIngestor_service.reg"

# Import
reg import "C:\backup\CDRIngestor_service.reg"
```
