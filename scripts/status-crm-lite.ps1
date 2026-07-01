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

$localAddresses = Get-NetIPAddress -AddressFamily IPv4 -ErrorAction SilentlyContinue |
    Where-Object { $_.IPAddress -notlike "127.*" -and $_.PrefixOrigin -ne "WellKnown" } |
    Select-Object -ExpandProperty IPAddress

if ($localAddresses) {
    Write-Host "LAN URLs:"
    foreach ($address in $localAddresses) {
        Write-Host "  http://$address`:$Port"
    }
}
