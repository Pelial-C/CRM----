# CRM Lite Deployment

This project is an ASP.NET Core MVC application targeting .NET 8. It runs with SQLite by default and also supports SQL Server.

## Requirements

- .NET 8 SDK for build machines
- .NET 8 ASP.NET Core Runtime for servers
- SQLite database file on the E drive for single-machine deployment
- SQL Server or SQL Server Express for multi-user/server deployment
- A production connection string supplied by environment variable or production appsettings

## Build

Run from the repository root:

```powershell
.\scripts\publish-crm-lite.ps1
```

The default script creates a self-contained Windows x64 deployment at:

```text
E:\deploy\crm-lite-win-x64
```

NuGet packages and .NET CLI home are also kept on the E drive:

```text
E:\nuget-packages
E:\dotnet-cli-home
```

## Configuration

Do not commit production secrets. For single-machine deployment without installing SQL Server, use SQLite on the E drive:

```powershell
$env:ASPNETCORE_ENVIRONMENT = "Production"
$env:ConnectionStrings__DefaultConnection = "Data Source=E:\crm-lite-data\CrmLite.db"
$env:Database__Provider = "Sqlite"
$env:Database__InitializeOnStartup = "true"
```

For SQL Server deployment:

```powershell
$env:ASPNETCORE_ENVIRONMENT = "Production"
$env:ConnectionStrings__DefaultConnection = "Server=YOUR_SQL_SERVER;Database=CrmLiteDb;User Id=YOUR_USER;Password=YOUR_PASSWORD;TrustServerCertificate=True;MultipleActiveResultSets=true"
$env:Database__Provider = "SqlServer"
$env:Database__MigrateOnStartup = "true"
```

`Database__InitializeOnStartup=true` creates the SQLite schema when the app starts.
`Database__MigrateOnStartup=true` applies EF Core migrations for SQL Server when the app starts. Use this only when the deployment account has permission to create/update the database schema.

## Run

```powershell
.\scripts\start-crm-lite.ps1
```

Health endpoint:

```text
http://localhost:5270/health
```

Stop and status:

```powershell
.\scripts\status-crm-lite.ps1
.\scripts\stop-crm-lite.ps1
```

Smoke test after deployment:

```powershell
.\scripts\smoke-test-crm-lite.ps1
```

Go-live audit:

```powershell
.\scripts\go-live-audit.ps1 -RunSmokeTest
```

Database backup:

```powershell
.\scripts\backup-crm-lite-data.ps1
```

## Notes

- LocalDB is not required. The default checked-in configuration stores the SQLite database at `E:\crm-lite-data\CrmLite.db`.
- For higher concurrency or remote access, use SQL Server and set `Database__Provider=SqlServer`.
- Build artifacts under `bin/` and `obj/` are ignored and should not be committed.
- If the server is behind IIS, Nginx, or another reverse proxy, configure HTTPS and forwarded headers at the hosting layer.
