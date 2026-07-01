# CRM Lite

CRM Lite is an ASP.NET Core MVC system for customer, contact, contract, and payment-plan management.

## Stack

- .NET 8
- ASP.NET Core MVC
- Entity Framework Core
- SQLite by default, SQL Server supported for server deployment
- Bootstrap and Razor views

## Local Run

```powershell
dotnet restore CRM.Lite\CRM.Lite.sln
dotnet build CRM.Lite\CRM.Lite.sln
dotnet run --project CRM.Lite\CRM.Web\CRM.Web.csproj --urls http://localhost:5268
```

Open:

```text
http://localhost:5268
```

The default connection string uses an SQLite database file on the E drive:

```powershell
$env:ConnectionStrings__DefaultConnection = "Data Source=E:\crm-lite-data\CrmLite.db"
$env:Database__Provider = "Sqlite"
$env:Database__InitializeOnStartup = "true"
```

For SQL Server deployment:

```powershell
$env:ConnectionStrings__DefaultConnection = "Server=YOUR_SQL_SERVER;Database=CrmLiteDb;User Id=YOUR_USER;Password=YOUR_PASSWORD;TrustServerCertificate=True;MultipleActiveResultSets=true"
$env:Database__Provider = "SqlServer"
$env:Database__MigrateOnStartup = "true"
```

## Deployment

See [DEPLOYMENT.md](DEPLOYMENT.md).

Quick single-machine deployment on the E drive:

```powershell
.\scripts\publish-crm-lite.ps1
.\scripts\start-crm-lite.ps1
.\scripts\status-crm-lite.ps1
.\scripts\smoke-test-crm-lite.ps1
```
