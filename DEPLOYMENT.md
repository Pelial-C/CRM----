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
dotnet restore CRM.Lite\CRM.Lite.sln
dotnet build CRM.Lite\CRM.Lite.sln --configuration Release
dotnet publish CRM.Lite\CRM.Web\CRM.Web.csproj --configuration Release --output E:\deploy\crm-lite
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
dotnet E:\deploy\crm-lite\CRM.Web.dll --urls http://0.0.0.0:5268
```

Health endpoint:

```text
/health
```

## Notes

- LocalDB is not required. The default checked-in configuration stores the SQLite database at `E:\crm-lite-data\CrmLite.db`.
- For higher concurrency or remote access, use SQL Server and set `Database__Provider=SqlServer`.
- Build artifacts under `bin/` and `obj/` are ignored and should not be committed.
- If the server is behind IIS, Nginx, or another reverse proxy, configure HTTPS and forwarded headers at the hosting layer.
