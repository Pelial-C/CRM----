param(
    [string]$Configuration = "Release",
    [string]$Runtime = "win-x64",
    [string]$Output = "E:\deploy\crm-lite-win-x64"
)

$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$solution = Join-Path $repoRoot "CRM.Lite\CRM.Lite.sln"
$webProject = Join-Path $repoRoot "CRM.Lite\CRM.Web\CRM.Web.csproj"

$env:NUGET_PACKAGES = "E:\nuget-packages"
$env:DOTNET_CLI_HOME = "E:\dotnet-cli-home"
$env:DOTNET_CLI_TELEMETRY_OPTOUT = "1"

New-Item -ItemType Directory -Force -Path $env:NUGET_PACKAGES, $env:DOTNET_CLI_HOME, $Output | Out-Null

dotnet restore $solution
dotnet build $solution --configuration $Configuration --nologo
dotnet publish $webProject `
    --configuration $Configuration `
    --runtime $Runtime `
    --self-contained true `
    --output $Output `
    --nologo

Write-Host "Published CRM Lite to $Output"
