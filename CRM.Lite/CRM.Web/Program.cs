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

            builder.Services.AddDbContext<CrmDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

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

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
