#Requires -RunAsAdministrator
<#
.SYNOPSIS
    CDR DataIngestor Service Kaldırma Script'i

.DESCRIPTION
    Bu script:
    1. Service'i durdurur
    2. NSSM'den kaldırır

.NOTES
    Administrator olarak çalıştırılmalı!
#>

param(
    [string]$ServiceName = "CDRIngestor",
    [string]$NssmPath = "C:\tools\nssm\nssm.exe"
)

$ErrorActionPreference = "Stop"

Write-Host "CDR DataIngestor Service Kaldırma" -ForegroundColor Cyan
Write-Host "=" * 40 -ForegroundColor Cyan

# Service kontrolü
$existingService = Get-Service -Name $ServiceName -ErrorAction SilentlyContinue

if (-not $existingService) {
    Write-Host "Service bulunamadı: $ServiceName" -ForegroundColor Yellow
    exit 0
}

# Durdur
Write-Host "Service durduruluyor..." -ForegroundColor Yellow
& $NssmPath stop $ServiceName 2>$null
Start-Sleep -Seconds 2

# Kaldır
Write-Host "Service kaldırılıyor..." -ForegroundColor Yellow
& $NssmPath remove $ServiceName confirm

Write-Host "`nService başarıyla kaldırıldı!" -ForegroundColor Green
