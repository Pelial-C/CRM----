param(
    [string]$Url = "http://localhost:5270/health",
    [int]$Port = 5270
)

$connections = Get-NetTCPConnection -LocalPort $Port -ErrorAction SilentlyContinue |
    Select-Object LocalAddress, LocalPort, State, OwningProcess

if ($connections) {
    $connections | Format-Table
} else {
    Write-Host "No process is listening on port $Port"
}

try {
    $response = Invoke-WebRequest -Uri $Url -UseBasicParsing -TimeoutSec 10
    Write-Host "Health: $($response.StatusCode) $($response.Content)"
} catch {
    Write-Host "Health check failed: $($_.Exception.Message)"
}
