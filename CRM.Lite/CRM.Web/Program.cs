using CRM.Application.Contracts.Contacts;
using CRM.Application.Contracts.Customers;
using CRM.Application.Contracts.Contracts;
using CRM.Application.Contacts;
using CRM.Application.Customers;
using CRM.Domain.Repositories;
using CRM.Infrastructure.Persistence;
using CRM.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using ContractAppService = CRM.Application.Contracts.ContractAppService;

namespace CRM.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("ConnectionStrings:DefaultConnection is required.");
            }

            var databaseProvider = builder.Configuration.GetValue<string>("Database:Provider") ?? "SqlServer";

            builder.Services.AddDbContext<CrmDbContext>(options =>
            {
                if (databaseProvider.Equals("Sqlite", StringComparison.OrdinalIgnoreCase))
                {
                    options.UseSqlite(connectionString);
                }
                else if (databaseProvider.Equals("SqlServer", StringComparison.OrdinalIgnoreCase))
                {
                    options.UseSqlServer(connectionString);
                }
                else
                {
                    throw new InvalidOperationException($"Unsupported database provider: {databaseProvider}");
                }
            });

            builder.Services.AddHealthChecks()
                .AddCheck<HealthChecks.DatabaseHealthCheck>("database");

            builder.Services.AddScoped(typeof(IRepository<,>), typeof(EfRepository<,>));
            builder.Services.AddScoped<ICustomerAppService, CustomerAppService>();
            builder.Services.AddScoped<IContactAppService, ContactAppService>();
            builder.Services.AddScoped<IContractAppService, ContractAppService>();

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapHealthChecks("/health");

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            if (builder.Configuration.GetValue<bool>("Database:InitializeOnStartup") ||
                builder.Configuration.GetValue<bool>("Database:MigrateOnStartup"))
            {
                using var scope = app.Services.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<CrmDbContext>();
                if (databaseProvider.Equals("Sqlite", StringComparison.OrdinalIgnoreCase))
                {
                    var dataSource = new Microsoft.Data.Sqlite.SqliteConnectionStringBuilder(connectionString).DataSource;
                    var directory = Path.GetDirectoryName(Path.GetFullPath(dataSource));
                    if (!string.IsNullOrWhiteSpace(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    dbContext.Database.EnsureCreated();
                }
                else
                {
                    dbContext.Database.Migrate();
                }
            }

            app.Run();
        }
    }
}
