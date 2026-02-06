@echo off
REM ================================================  
REM CDR DataIngestor - SİSTEM PYTHON KURULUMU
REM Administrator olarak çalıştırın!
REM ================================================

echo CDR DataIngestor - Sistem Python Kurulumu
echo.

REM Python kontrolü
python --version >nul 2>&1
if %ERRORLEVEL% neq 0 (
    echo HATA: Python bulunamadı!
    pause && exit /b 1
)

echo Python bulundu: 
python --version

REM Paketleri sistem Python'ına kur
echo.
echo Paketleri sistem Python'ina kuruluyor...
python -m pip install --upgrade pip
python -m pip install pymongo==4.6.0 pydantic==2.5.0 pyyaml==6.0.1 pywin32==306

REM pywin32 yapılandır
echo.
echo pywin32 yapılandırılıyor...
python -c "import sys,os; postinstall=sys.prefix+'/Scripts/pywin32_postinstall.py'; os.system(f'python {postinstall} -install')" >nul 2>&1

REM Service'i kur
echo.
echo Service kuruluyor...
python service\cdr_service.py install

if %ERRORLEVEL% equ 0 (
    echo ✓ Service kuruldu
    
    REM Otomatik başlangıç yap
    sc config CDRDataIngestor start= auto >nul
    echo ✓ Otomatik başlangıç ayarlandı
    
    REM Service'i başlat
    echo.
    echo Service başlatılıyor...
    python service\cdr_service.py start
    
    if %ERRORLEVEL% equ 0 (
        echo ✓ Service başlatıldı!
        echo.
        echo ===========================================
        echo KURULUM BAŞARILI!
        echo ===========================================
        echo Service her 2 dakikada çalışacak
        echo Kaynak: E:\CDR  
        echo İşlenmiş: E:\CDR_Processed
        echo.
    else (
        echo ✗ Service başlatılamadı
        echo Event Viewer'dan logları kontrol edin
    )
) else (
    echo ✗ Service kurulamadı
    echo Administrator yetkisi var mı kontrol edin
)

echo.
echo Yönetim Komutları:
echo   Başlat:  python service\cdr_service.py start
echo   Durdur:  python service\cdr_service.py stop  
echo   Durum:   sc query CDRDataIngestor
echo   Kaldır:  python service\cdr_service.py remove

pause
