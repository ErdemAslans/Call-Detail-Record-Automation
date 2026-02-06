# CDR DataIngestor - Windows Service

## Hızlı Kurulum

### 1. Test Et
```bat
test_setup.bat
```

### 2. Kur (Administrator)
```bat
install.bat
```

### 3. Yönet
```bat
# Başlat
python service\cdr_service.py start

# Durdur  
python service\cdr_service.py stop

# Durum
sc query CDRDataIngestor

# Kaldır
uninstall.bat
```

## Özellikler
- ✅ Her 2 dakikada çalışır
- ✅ E:\CDR → E:\CDR_Processed taşıma
- ✅ Otomatik başlangıç
- ✅ Windows Event Log

## Loglar
- Event Viewer: `eventvwr.msc` → Application → CDRDataIngestor
- Service Manager: `services.msc`
