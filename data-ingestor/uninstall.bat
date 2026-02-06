@echo off
REM CDR DataIngestor Service Kaldırma

echo CDR Service kaldırılıyor...

python service\cdr_service.py stop 2>nul
python service\cdr_service.py remove 2>nul

echo Service kaldırıldı!
pause
