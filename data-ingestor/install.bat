@echo off
REM ================================================
REM CDR DataIngestor - Sistem Python Kurulumu
REM Administrator olarak çalıştırın!
REM ================================================

echo CDR DataIngestor Service Kurulumu (Sistem Python)
echo.

REM Python kontrolü
echo Python sürümü kontrol ediliyor...
python --version >nul 2>&1
if %ERRORLEVEL% neq 0 (
    echo HATA: Python bulunamadı! Sistem Python'ı kurulmalı.
    pause && exit /b 1
)

python --version
echo Python yolu: 
where python

REM Sistem geneli paket kurulumu
echo.
echo Gerekli paketler sistem genelinde kuruluyor...
python -m pip install --upgrade pip
python -m pip install pymongo pandas pydantic pyyaml motor pywin32 openpyxl

if %ERRORLEVEL% neq 0 (
    echo HATA: Paket kurulumu başarısız!
    pause && exit /b 1
)

REM pywin32 yapılandır
echo.
echo pywin32 yapılandırılıyor...
python -c "import sys,os; postinstall = os.path.join(sys.prefix, 'Scripts', 'pywin32_postinstall.py'); os.system(f'python {postinstall} -install')" >nul 2>&1

REM Paket kontrolü
echo.
echo Paketler kontrol ediliyor...
python -c "import pymongo; print('✓ pymongo')" || (echo "✗ pymongo hatası" && pause && exit /b 1)
python -c "import pydantic; print('✓ pydantic')" || (echo "✗ pydantic hatası" && pause && exit /b 1)
python -c "import yaml; print('✓ pyyaml')" || (echo "✗ pyyaml hatası" && pause && exit /b 1)
python -c "import win32service; print('✓ pywin32')" || (echo "✗ pywin32 hatası" && pause && exit /b 1)

REM Eski service'i kaldır (varsa)
echo.
echo Eski service kontrol ediliyor...
python service\cdr_service.py remove >nul 2>&1

REM Service'i kur
echo.
echo CDR DataIngestor Service kuruluyor...
python service\cdr_service.py install

if %ERRORLEVEL% equ 0 (
    echo ✓ Service başarıyla kuruldu!
    
    REM Otomatik başlangıç ayarla
    echo Service otomatik başlangıç olarak ayarlanıyor...
    sc config CDRDataIngestor start= auto >nul
    
    REM Service'i başlat
    echo Service başlatılıyor...
    python service\cdr_service.py start
    
    if %ERRORLEVEL% equ 0 (
        echo ✓ Service başarıyla başlatıldı!
    else (
        echo ✗ Service başlatılamadı. Event Viewer'ı kontrol edin.
    )
) else (
    echo ✗ Service kurulumu başarısız!
)

echo.
echo ================================================
echo KURULUM SONUCU
echo ================================================
echo Service Adı: CDRDataIngestor  
echo Kaynak Klasör: E:\CDR
echo İşlenmiş Klasör: E:\CDR_Processed
echo Periyod: Her 2 dakika
echo.
echo Yönetim Komutları:
echo   Başlat:  python service\cdr_service.py start
echo   Durdur:  python service\cdr_service.py stop
echo   Durum:   sc query CDRDataIngestor
echo   Loglar:  eventvwr.msc ^> Application ^> CDRDataIngestor
echo.
pause
