param(
    [int]$Port = 5270
)

$processIds = Get-NetTCPConnection -LocalPort $Port -ErrorAction SilentlyContinue |
    Select-Object -ExpandProperty OwningProcess -Unique

foreach ($pidValue in $processIds) {
    Stop-Process -Id $pidValue -Force -ErrorAction SilentlyContinue
}

if ($processIds) {
    Write-Host "Stopped CRM Lite processes on port ${Port}: $($processIds -join ', ')"
} else {
    Write-Host "No CRM Lite process found on port $Port"
}
