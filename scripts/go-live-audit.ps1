param(
    [string]$RepoRoot = (Split-Path -Parent $PSScriptRoot),
    [string]$DeployPath = "E:\deploy\crm-lite-win-x64",
    [string]$DatabasePath = "E:\crm-lite-data\CrmLite.db",
    [string]$BackupDir = "E:\crm-lite-backups",
    [string]$HealthUrl = "http://localhost:5270/health",
    [int]$Port = 5270,
    [switch]$RunSmokeTest
)

$ErrorActionPreference = "Stop"
$failures = New-Object System.Collections.Generic.List[string]

function Add-Failure([string]$Message) {
    $script:failures.Add($Message) | Out-Null
}

function Test-EPath([string]$Path, [string]$Label) {
    if (!$Path.StartsWith("E:\", [StringComparison]::OrdinalIgnoreCase)) {
        Add-Failure "$Label is not on E drive: $Path"
    }
}

Push-Location $RepoRoot
try {
    Write-Host "=== CRM Lite Go-Live Audit ==="
    Write-Host "Repo: $RepoRoot"
    Write-Host "Deploy: $DeployPath"
    Write-Host "Database: $DatabasePath"
    Write-Host "BackupDir: $BackupDir"
    Write-Host ""

    Test-EPath $RepoRoot "Repository"
    Test-EPath $DeployPath "Deployment path"
    Test-EPath $DatabasePath "Database path"
    Test-EPath $BackupDir "Backup path"

    $trackedBuildOutputCount = [int]((git ls-files "*/bin/*" "*/obj/*" | Measure-Object).Count)
    if ($trackedBuildOutputCount -ne 0) {
        Add-Failure "Git still tracks $trackedBuildOutputCount bin/obj files."
    }

    $status = git status --short --branch
    $status | ForEach-Object { Write-Host $_ }
    if (($status | Where-Object { $_ -match "ahead|behind" }).Count -gt 0) {
        Add-Failure "Local branch is not synchronized with remote."
    }

    $dirty = git status --short | Where-Object { $_ -notmatch "^\?\? " }
    if ($dirty.Count -gt 0) {
        Add-Failure "Working tree has tracked uncommitted changes."
    }

    if (!(Test-Path -LiteralPath $DeployPath)) {
        Add-Failure "Deployment path does not exist."
    }

    $selfContainedExe = Join-Path $DeployPath "CRM.Web.exe"
    if (!(Test-Path -LiteralPath $selfContainedExe)) {
        Add-Failure "Self-contained executable missing: $selfContainedExe"
    }

    if (!(Test-Path -LiteralPath $DatabasePath)) {
        Add-Failure "Database file missing: $DatabasePath"
    }

    if (!(Test-Path -LiteralPath $BackupDir)) {
        Add-Failure "Backup directory missing: $BackupDir"
    } else {
        $backupCount = (Get-ChildItem -LiteralPath $BackupDir -Filter "*.db" -ErrorAction SilentlyContinue | Measure-Object).Count
        if ($backupCount -lt 1) {
            Add-Failure "No database backups found in $BackupDir"
        }
    }

    $connections = Get-NetTCPConnection -LocalPort $Port -ErrorAction SilentlyContinue |
        Where-Object { $_.State -eq "Listen" }
    if (!$connections) {
        Add-Failure "No process is listening on port $Port."
    }

    try {
        $health = Invoke-WebRequest -Uri $HealthUrl -UseBasicParsing -TimeoutSec 10
        Write-Host "Health: $($health.StatusCode) $($health.Content)"
        if ($health.StatusCode -ne 200 -or $health.Content -notmatch "Healthy") {
            Add-Failure "Health endpoint did not return Healthy."
        }
    } catch {
        Add-Failure "Health endpoint failed: $($_.Exception.Message)"
    }

    if ($RunSmokeTest) {
        Write-Host ""
        Write-Host "Running smoke test..."
        & (Join-Path $PSScriptRoot "smoke-test-crm-lite.ps1")
        if ($LASTEXITCODE -ne 0) {
            Add-Failure "Smoke test failed."
        }
    }

    Write-Host ""
    if ($failures.Count -eq 0) {
        Write-Host "GO-LIVE AUDIT PASSED"
        exit 0
    }

    Write-Host "GO-LIVE AUDIT FAILED"
    foreach ($failure in $failures) {
        Write-Host " - $failure"
    }
    exit 1
}
finally {
    Pop-Location
}
