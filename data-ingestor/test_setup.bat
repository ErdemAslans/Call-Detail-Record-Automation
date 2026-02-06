@echo off
echo ================================================
echo CDR DataIngestor - Sistem Testi
echo ================================================

REM Python kontrolü
echo Python Sürümü:
python --version || (echo "HATA: Python yok!" && pause && exit)
echo Python Yolu:
where python

REM Paket kontrolü
echo.
echo Paket Kontrolü:
python -c "import pymongo; print('✓ pymongo - Sürüm:', pymongo.version)" 2>nul || echo "✗ pymongo YOK"
python -c "import pydantic; print('✓ pydantic - Sürüm:', pydantic.version.VERSION)" 2>nul || echo "✗ pydantic YOK" 
python -c "import yaml; print('✓ pyyaml - Sürüm: OK')" 2>nul || echo "✗ pyyaml YOK"
python -c "import win32service; print('✓ pywin32 - Service modülü OK')" 2>nul || echo "✗ pywin32 YOK"

REM CDR Processor testi
echo.
echo CDR Sistem Testi:
cd src 2>nul && (
    python -c "from processors.cdr_processor import CDRProcessor; print('✓ CDRProcessor import OK')" 2>nul || echo "✗ CDRProcessor HATALI"
    cd ..
) || echo "✗ src klasörü bulunamadı"

REM MongoDB testi
echo.
echo Veritabanı Testi:
python -c "import sys; sys.path.append('src'); exec('try:\n from utils import get_mongo_collection\n collection = get_mongo_collection()\n result = collection.database.command(\"ping\")\n print(\"✓ MongoDB bağlantısı BAŞARILI\")\nexcept Exception as e:\n print(\"✗ MongoDB bağlantısı BAŞARISIZ:\", str(e)[:80])')" 2>nul || echo "✗ MongoDB testi çalıştırılamadı"

REM Klasör kontrolü
echo.
echo Klasör Kontrolü:
if not exist "E:\CDR" (
    echo ✗ E:\CDR klasörü yok, oluşturuluyor...
    mkdir "E:\CDR" 2>nul
)
if exist "E:\CDR" (echo ✓ E:\CDR klasörü hazır) else (echo ✗ E:\CDR oluşturulamadı)

if not exist "E:\CDR_Processed" (
    echo ✗ E:\CDR_Processed klasörü yok, oluşturuluyor...
    mkdir "E:\CDR_Processed" 2>nul
)
if exist "E:\CDR_Processed" (echo ✓ E:\CDR_Processed klasörü hazır) else (echo ✗ E:\CDR_Processed oluşturulamadı)

REM Service dosyası kontrolü
echo.
echo Service Kontrolü:
if exist "service\cdr_service.py" (
    echo ✓ Service dosyası mevcut
) else (
    echo ✗ Service dosyası bulunamadı!
)

echo.
echo ================================================
if exist "service\cdr_service.py" (
    echo ✅ SİSTEM HAZIR! 
    echo.
    echo Kurulum için çalıştırın:
    echo   install.bat ^(Administrator olarak^)
) else (
    echo ❌ SİSTEM HAZIR DEĞİL!
    echo Eksiklikleri giderin ve tekrar test edin.
)
echo ================================================

pause
