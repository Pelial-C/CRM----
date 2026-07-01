# CRM Lite Deployment

This project is an ASP.NET Core MVC application targeting .NET 8 with SQL Server.

## Requirements

- .NET 8 SDK for build machines
- .NET 8 ASP.NET Core Runtime for servers
- SQL Server or SQL Server Express
- A production connection string supplied by environment variable or production appsettings

## Build

Run from the repository root:

```powershell
dotnet restore CRM.Lite\CRM.Lite.sln
dotnet build CRM.Lite\CRM.Lite.sln --configuration Release
dotnet publish CRM.Lite\CRM.Web\CRM.Web.csproj --configuration Release --output E:\deploy\crm-lite
```

## Configuration

Do not commit production secrets. Prefer environment variables on the server:

```powershell
$env:ASPNETCORE_ENVIRONMENT = "Production"
$env:ConnectionStrings__DefaultConnection = "Server=YOUR_SQL_SERVER;Database=CrmLiteDb;User Id=YOUR_USER;Password=YOUR_PASSWORD;TrustServerCertificate=True;MultipleActiveResultSets=true"
$env:Database__MigrateOnStartup = "true"
```

`Database__MigrateOnStartup=true` applies EF Core migrations when the app starts. Use this only when the deployment account has permission to create/update the database schema.

## Run

```powershell
dotnet E:\deploy\crm-lite\CRM.Web.dll --urls http://0.0.0.0:5268
```

Health endpoint:

```text
/health
```

## Notes

- LocalDB is for local development only. Production needs a real SQL Server connection string.
- Build artifacts under `bin/` and `obj/` are ignored and should not be committed.
- If the server is behind IIS, Nginx, or another reverse proxy, configure HTTPS and forwarded headers at the hosting layer.
