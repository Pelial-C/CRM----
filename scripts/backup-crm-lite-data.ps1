param(
    [string]$DatabasePath = "E:\crm-lite-data\CrmLite.db",
    [string]$BackupDir = "E:\crm-lite-backups"
)

$ErrorActionPreference = "Stop"

if (!(Test-Path -LiteralPath $DatabasePath)) {
    throw "Database file not found: $DatabasePath"
}

New-Item -ItemType Directory -Force -Path $BackupDir | Out-Null
$timestamp = Get-Date -Format "yyyyMMdd_HHmmss"
$backupPath = Join-Path $BackupDir "CrmLite_$timestamp.db"
Copy-Item -LiteralPath $DatabasePath -Destination $backupPath -Force

Write-Host "Backup created: $backupPath"
