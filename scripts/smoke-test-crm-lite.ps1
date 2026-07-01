param(
    [string]$BaseUrl = "http://localhost:5270"
)

$ErrorActionPreference = "Stop"

function Invoke-Page([string]$Path) {
    Invoke-WebRequest -Uri "$BaseUrl$Path" -UseBasicParsing -TimeoutSec 20
}

function Invoke-FormPost([string]$Path, [hashtable]$Body) {
    $request = [System.Net.HttpWebRequest]::Create("$BaseUrl$Path")
    $request.Method = "POST"
    $request.AllowAutoRedirect = $false
    $request.ContentType = "application/x-www-form-urlencoded"

    $encoded = ($Body.GetEnumerator() | ForEach-Object {
        [System.Net.WebUtility]::UrlEncode($_.Key) + "=" + [System.Net.WebUtility]::UrlEncode([string]$_.Value)
    }) -join "&"
    $bytes = [System.Text.Encoding]::UTF8.GetBytes($encoded)
    $request.ContentLength = $bytes.Length

    $stream = $request.GetRequestStream()
    $stream.Write($bytes, 0, $bytes.Length)
    $stream.Close()

    try {
        $response = $request.GetResponse()
        $statusCode = [int]$response.StatusCode
        $response.Close()
        if ($statusCode -lt 200 -or $statusCode -ge 400) {
            throw "POST $Path failed with status $statusCode"
        }
    } catch {
        if ($_.Exception.Response -eq $null) {
            throw
        }

        $statusCode = [int]$_.Exception.Response.StatusCode
        $_.Exception.Response.Close()
        if ($statusCode -lt 300 -or $statusCode -ge 400) {
            throw "POST $Path failed with status $statusCode"
        }
    }
}

$stamp = Get-Date -Format "yyyyMMddHHmmss"
$customerName = "SmokeCustomer-$stamp"
$creditCode = "SMOKE-$stamp"
$contractNo = "SMOKE-HT-$stamp"

$health = Invoke-Page "/health"
if ($health.StatusCode -ne 200 -or $health.Content -notmatch "Healthy") {
    throw "Health check failed: $($health.StatusCode) $($health.Content)"
}

Invoke-FormPost "/Customer/Create" @{
    Name = $customerName
    CreditCode = $creditCode
    Industry = "SmokeTest"
    Province = "Zhejiang"
    City = "Hangzhou"
    District = "Xihu"
    DetailAddress = "E drive online smoke test"
    Remark = "Created by smoke-test-crm-lite.ps1"
}

$customerPage = Invoke-Page "/Customer"
if (!$customerPage.Content.Contains($creditCode)) {
    throw "Created customer was not found on /Customer"
}

Invoke-FormPost "/Contact/Create" @{
    CustomerId = "1"
    Name = "SmokeContact-$stamp"
    Title = "Manager"
    Phone = "13800000000"
    Email = "smoke@example.com"
    IsKeyDecisionMaker = "true"
}

Invoke-FormPost "/Contract/Create" @{
    ContractNo = $contractNo
    ContractName = "Smoke Contract $stamp"
    CustomerId = "1"
    ContactId = "1"
    TotalAmount = "1000"
    CabinetNo = "SMOKE-CAB"
    SignDate = "2026-07-01"
    StartDate = "2026-07-01"
    EndDate = "2026-12-31"
    WarningDays = "30"
    ServiceType = "0"
    ContractType = "0"
    PaymentFrequency = "3"
    RegionalCompany = "East"
    AffiliatedCompany = "Hangzhou"
    Remark = "Smoke test contract"
    "Items[0].ProductName" = "CRM Service"
    "Items[0].Quantity" = "1"
    "Items[0].UnitPrice" = "1000"
    "PaymentPlans[0].PlanDate" = "2026-08-01"
    "PaymentPlans[0].PlanAmount" = "1000"
    "PaymentPlans[0].Description" = "Smoke payment"
}

$contractPage = Invoke-Page "/Contract"
if (!$contractPage.Content.Contains($contractNo)) {
    throw "Created contract was not found on /Contract"
}

$contractIdMatch = [regex]::Match($contractPage.Content, "Detail/(\d+)[^>]*>\s*详情")
if (!$contractIdMatch.Success) {
    $contractIdMatch = [regex]::Match($contractPage.Content, "Detail/(\d+)")
}
if (!$contractIdMatch.Success) {
    throw "Cannot find created contract detail link."
}

$contractId = [int]$contractIdMatch.Groups[1].Value
$detailPage = Invoke-Page "/Contract/Detail/$contractId"
$paymentButtonMatch = [regex]::Match($detailPage.Content, "openPayment\((\d+),(\d+)\)")
if (!$paymentButtonMatch.Success) {
    throw "Cannot find payment plan button on contract detail page."
}

$paymentContractId = [int]$paymentButtonMatch.Groups[1].Value
$paymentPlanId = [int]$paymentButtonMatch.Groups[2].Value
$recordPaymentBody = "{""contractId"":$paymentContractId,""planId"":$paymentPlanId,""actualAmount"":1,""actualDate"":""2026-08-01""}"
$paymentResponse = Invoke-WebRequest `
    -Uri "$BaseUrl/Contract/RecordPayment" `
    -Method Post `
    -Body $recordPaymentBody `
    -ContentType "application/json" `
    -UseBasicParsing `
    -TimeoutSec 20

if ($paymentResponse.Content -notmatch '"code":200') {
    throw "RecordPayment failed: $($paymentResponse.Content)"
}

Write-Host "Smoke test passed."
Write-Host "Health: $($health.Content)"
Write-Host "Customer: $creditCode"
Write-Host "Contract: $contractNo"
Write-Host "ContractId: $contractId"
