#Requires -RunAsAdministrator
<#
.SYNOPSIS
    CDR DataIngestor Windows Service Kurulum Script'i (NSSM)

.DESCRIPTION
    Bu script:
    1. NSSM'i indirir (yoksa)
    2. CDRService kullanıcısı oluşturur
    3. Gerekli dizinlere izin verir
    4. Windows Service olarak CDR DataIngestor'u kaydeder
    5. Service'i başlatır

.NOTES
    Administrator olarak çalıştırılmalı!
    Tarih: 2026-01-10
#>

param(
    [string]$ServiceName = "CDRIngestor",
    [string]$ServiceDisplayName = "CDR Data Ingestor Service",
    [string]$ServiceDescription = "CDR dosyalarini MongoDB'ye aktaran servis",
    [string]$PythonPath = "C:\Projects\CDR.DataIngestor\venv\Scripts\python.exe",
    [string]$AppPath = "C:\Projects\CDR.DataIngestor\src\main.py",
    [string]$AppDirectory = "C:\Projects\CDR.DataIngestor\src",
    [string]$SourceDir = "E:\CDR",
    [string]$ProcessedDir = "E:\CDR_Processed",
    [string]$LogDir = "E:\CDR_Logs",
    [string]$NssmPath = "C:\tools\nssm",
    [string]$ServiceUser = "CDRService",
    [int]$RestartDelayMs = 5000
)

$ErrorActionPreference = "Stop"

Write-Host "=" * 60 -ForegroundColor Cyan
Write-Host "CDR DataIngestor Windows Service Kurulum Script'i" -ForegroundColor Cyan
Write-Host "=" * 60 -ForegroundColor Cyan

# ============================================
# 1. NSSM Kontrolü ve İndirme
# ============================================
Write-Host "`n[1/6] NSSM kontrolü..." -ForegroundColor Yellow

$nssmExe = Join-Path $NssmPath "nssm.exe"

if (-not (Test-Path $nssmExe)) {
    Write-Host "NSSM bulunamadı. İndiriliyor..." -ForegroundColor Yellow
    
    # NSSM dizinini oluştur
    if (-not (Test-Path $NssmPath)) {
        New-Item -ItemType Directory -Path $NssmPath -Force | Out-Null
    }
    
    # NSSM indir
    $nssmUrl = "https://nssm.cc/release/nssm-2.24.zip"
    $nssmZip = Join-Path $env:TEMP "nssm.zip"
    
    try {
        Invoke-WebRequest -Uri $nssmUrl -OutFile $nssmZip -UseBasicParsing
        
        # Zip'i çıkar
        Expand-Archive -Path $nssmZip -DestinationPath $env:TEMP -Force
        
        # 64-bit exe'yi kopyala
        $extractedExe = Join-Path $env:TEMP "nssm-2.24\win64\nssm.exe"
        Copy-Item -Path $extractedExe -Destination $nssmExe -Force
        
        # Temizlik
        Remove-Item $nssmZip -Force
        Remove-Item (Join-Path $env:TEMP "nssm-2.24") -Recurse -Force
        
        Write-Host "NSSM başarıyla indirildi: $nssmExe" -ForegroundColor Green
    }
    catch {
        Write-Host "NSSM indirilemedi! Manuel olarak https://nssm.cc/download adresinden indirin." -ForegroundColor Red
        Write-Host "Hata: $_" -ForegroundColor Red
        exit 1
    }
}
else {
    Write-Host "NSSM mevcut: $nssmExe" -ForegroundColor Green
}

# ============================================
# 2. Dizinleri Oluştur
# ============================================
Write-Host "`n[2/6] Dizinler oluşturuluyor..." -ForegroundColor Yellow

$directories = @($SourceDir, $ProcessedDir, $LogDir)

foreach ($dir in $directories) {
    if (-not (Test-Path $dir)) {
        New-Item -ItemType Directory -Path $dir -Force | Out-Null
        Write-Host "  Oluşturuldu: $dir" -ForegroundColor Green
    }
    else {
        Write-Host "  Mevcut: $dir" -ForegroundColor Gray
    }
}

# ============================================
# 3. Service User Oluştur (Opsiyonel)
# ============================================
Write-Host "`n[3/6] Service kullanıcısı kontrolü..." -ForegroundColor Yellow

# NOT: Local System hesabı kullanılacak (en basit yöntem)
# Özel kullanıcı istenirse aşağıdaki kodu aktif edin
<#
$userExists = Get-LocalUser -Name $ServiceUser -ErrorAction SilentlyContinue
if (-not $userExists) {
    $password = Read-Host "CDRService kullanıcısı için şifre girin" -AsSecureString
    New-LocalUser -Name $ServiceUser -Password $password -Description "CDR DataIngestor Service Account" -PasswordNeverExpires
    Write-Host "  Kullanıcı oluşturuldu: $ServiceUser" -ForegroundColor Green
}
#>
Write-Host "  Local System hesabı kullanılacak" -ForegroundColor Green

# ============================================
# 4. Dizin İzinleri
# ============================================
Write-Host "`n[4/6] Dizin izinleri ayarlanıyor..." -ForegroundColor Yellow

foreach ($dir in $directories) {
    try {
        # Everyone için Full Control (Local System zaten erişebilir)
        $acl = Get-Acl $dir
        $rule = New-Object System.Security.AccessControl.FileSystemAccessRule(
            "SYSTEM", "FullControl", "ContainerInherit,ObjectInherit", "None", "Allow"
        )
        $acl.SetAccessRule($rule)
        Set-Acl -Path $dir -AclObject $acl
        Write-Host "  İzin verildi: $dir" -ForegroundColor Green
    }
    catch {
        Write-Host "  İzin ayarlanamadı: $dir - $_" -ForegroundColor Yellow
    }
}

# ============================================
# 5. Mevcut Service Kontrolü ve Kaldırma
# ============================================
Write-Host "`n[5/6] Mevcut service kontrolü..." -ForegroundColor Yellow

$existingService = Get-Service -Name $ServiceName -ErrorAction SilentlyContinue
if ($existingService) {
    Write-Host "  Mevcut service durduruluyor..." -ForegroundColor Yellow
    & $nssmExe stop $ServiceName 2>$null
    Start-Sleep -Seconds 2
    & $nssmExe remove $ServiceName confirm 2>$null
    Write-Host "  Mevcut service kaldırıldı" -ForegroundColor Green
}
else {
    Write-Host "  Mevcut service yok" -ForegroundColor Gray
}

# ============================================
# 6. NSSM Service Kurulumu
# ============================================
Write-Host "`n[6/6] Windows Service kuruluyor..." -ForegroundColor Yellow

# Python path kontrolü
if (-not (Test-Path $PythonPath)) {
    Write-Host "Python bulunamadı: $PythonPath" -ForegroundColor Red
    Write-Host "Lütfen doğru Python yolunu -PythonPath parametresi ile belirtin" -ForegroundColor Yellow
    
    # Otomatik bulma denemesi
    $pythonAuto = (Get-Command python -ErrorAction SilentlyContinue).Source
    if ($pythonAuto) {
        Write-Host "Bulunan Python: $pythonAuto" -ForegroundColor Cyan
        $PythonPath = $pythonAuto
    }
    else {
        exit 1
    }
}

# App path kontrolü
if (-not (Test-Path $AppPath)) {
    Write-Host "Uygulama bulunamadı: $AppPath" -ForegroundColor Red
    Write-Host "Lütfen doğru uygulama yolunu -AppPath parametresi ile belirtin" -ForegroundColor Yellow
    exit 1
}

# Service kurulumu
Write-Host "  Service kaydediliyor..." -ForegroundColor Cyan

& $nssmExe install $ServiceName $PythonPath $AppPath
& $nssmExe set $ServiceName DisplayName $ServiceDisplayName
& $nssmExe set $ServiceName Description $ServiceDescription
& $nssmExe set $ServiceName AppDirectory $AppDirectory

# Otomatik başlatma
& $nssmExe set $ServiceName Start SERVICE_AUTO_START

# Yeniden başlatma politikası (5 saniye delay)
& $nssmExe set $ServiceName AppRestartDelay $RestartDelayMs
& $nssmExe set $ServiceName AppThrottle 5000

# Stdout/Stderr logları
& $nssmExe set $ServiceName AppStdout (Join-Path $LogDir "stdout.log")
& $nssmExe set $ServiceName AppStderr (Join-Path $LogDir "stderr.log")

# Log rotation (her 10MB'da yeni dosya)
& $nssmExe set $ServiceName AppStdoutCreationDisposition 4
& $nssmExe set $ServiceName AppStderrCreationDisposition 4
& $nssmExe set $ServiceName AppRotateFiles 1
& $nssmExe set $ServiceName AppRotateBytes 10485760

# Environment variables
& $nssmExe set $ServiceName AppEnvironmentExtra "CDR_LOG_DIR=$LogDir"

# Exit action: Restart
& $nssmExe set $ServiceName AppExit Default Restart

Write-Host "  Service başarıyla kuruldu!" -ForegroundColor Green

# ============================================
# Service Başlatma
# ============================================
Write-Host "`nService başlatılıyor..." -ForegroundColor Yellow

& $nssmExe start $ServiceName
Start-Sleep -Seconds 3

$serviceStatus = (Get-Service -Name $ServiceName).Status
if ($serviceStatus -eq "Running") {
    Write-Host "Service başarıyla çalışıyor!" -ForegroundColor Green
}
else {
    Write-Host "Service başlatılamadı. Durum: $serviceStatus" -ForegroundColor Red
    Write-Host "Logları kontrol edin: $LogDir\stdout.log" -ForegroundColor Yellow
}

# ============================================
# Özet
# ============================================
Write-Host "`n" + "=" * 60 -ForegroundColor Cyan
Write-Host "KURULUM TAMAMLANDI" -ForegroundColor Green
Write-Host "=" * 60 -ForegroundColor Cyan

Write-Host @"

Service Bilgileri:
  Ad           : $ServiceName
  Görünen Ad   : $ServiceDisplayName
  Durum        : $serviceStatus
  Başlatma     : Otomatik (Windows ile birlikte)
  Restart      : Hata durumunda ${RestartDelayMs}ms sonra

Dizinler:
  Kaynak       : $SourceDir
  İşlenmiş     : $ProcessedDir
  Loglar       : $LogDir

Faydalı Komutlar:
  Service durumu  : Get-Service $ServiceName
  Service başlat  : $nssmExe start $ServiceName
  Service durdur  : $nssmExe stop $ServiceName
  Service kaldır  : $nssmExe remove $ServiceName confirm
  NSSM GUI        : $nssmExe edit $ServiceName

Log Dosyaları:
  Stdout         : $LogDir\stdout.log
  Stderr         : $LogDir\stderr.log
  Uygulama       : $LogDir\cdr_ingestor.log

"@ -ForegroundColor White
