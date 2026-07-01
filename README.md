# CRM Lite

CRM Lite is an ASP.NET Core MVC system for customer, contact, contract, and payment-plan management.

## Stack

- .NET 8
- ASP.NET Core MVC
- Entity Framework Core
- SQL Server
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

The default development connection string uses SQL Server LocalDB. Install LocalDB or override the connection string:

```powershell
$env:ConnectionStrings__DefaultConnection = "Server=YOUR_SQL_SERVER;Database=CrmLiteDb;User Id=YOUR_USER;Password=YOUR_PASSWORD;TrustServerCertificate=True;MultipleActiveResultSets=true"
```

## Deployment

See [DEPLOYMENT.md](DEPLOYMENT.md).
