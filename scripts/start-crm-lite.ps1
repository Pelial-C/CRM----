param(
    [string]$DeployPath = "E:\deploy\crm-lite-win-x64",
    [string]$Urls = "http://0.0.0.0:5270",
    [string]$DatabasePath = "E:\crm-lite-data\CrmLite.db"
)

$ErrorActionPreference = "Stop"

$exe = Join-Path $DeployPath "CRM.Web.exe"
$dll = Join-Path $DeployPath "CRM.Web.dll"
$logs = Join-Path $DeployPath "logs"
$dataDir = Split-Path -Parent $DatabasePath

New-Item -ItemType Directory -Force -Path $logs, $dataDir | Out-Null

$port = [int]($Urls -replace '.*:(\d+).*', '$1')
$existing = Get-NetTCPConnection -LocalPort $port -ErrorAction SilentlyContinue |
    Select-Object -ExpandProperty OwningProcess -Unique
foreach ($pidValue in $existing) {
    Stop-Process -Id $pidValue -Force -ErrorAction SilentlyContinue
}

$env:ASPNETCORE_ENVIRONMENT = "Production"
$env:ConnectionStrings__DefaultConnection = "Data Source=$DatabasePath"
$env:Database__Provider = "Sqlite"
$env:Database__InitializeOnStartup = "true"

$stdout = Join-Path $logs "stdout.log"
$stderr = Join-Path $logs "stderr.log"

if (Test-Path $exe) {
    $process = Start-Process -FilePath $exe -ArgumentList @("--urls", $Urls) -WorkingDirectory $DeployPath -WindowStyle Hidden -RedirectStandardOutput $stdout -RedirectStandardError $stderr -PassThru
} elseif (Test-Path $dll) {
    $process = Start-Process -FilePath "dotnet" -ArgumentList @($dll, "--urls", $Urls) -WorkingDirectory $DeployPath -WindowStyle Hidden -RedirectStandardOutput $stdout -RedirectStandardError $stderr -PassThru
} else {
    throw "Cannot find CRM.Web.exe or CRM.Web.dll in $DeployPath"
}

Start-Sleep -Seconds 5
Write-Host "CRM Lite started. PID=$($process.Id), URL=$Urls, Database=$DatabasePath"
